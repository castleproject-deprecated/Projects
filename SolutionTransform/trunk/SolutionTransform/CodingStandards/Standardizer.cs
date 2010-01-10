// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace SolutionTransform.CodingStandards {
	using System;
	using System.Collections.Generic;
	using System.IO;

    class Standardizer : IStandardizer {
		private readonly string licenseHeader;
		private readonly int tabSize;

		public Standardizer(string licenseHeader, int tabSize)
		{
			this.licenseHeader = licenseHeader;
			this.tabSize = tabSize;
		}

		public string Standardize(FilePath path, string code) {
			var result = InternalStandardize(code);
			if (result != InternalStandardize(result)) {
				throw new Exception("Standardization code was not idempotent.");
			}
			return result;
		}

		string InternalStandardize(string code) {
			var reader = new StringReader(code);
			var licenseReader = new StringReader(licenseHeader);
			var usingStatements = new List<string>();
			var writer = new StringWriter();
			string codeLine = null;
			bool isLicensePresent = true;
			foreach (var licenseLine in licenseReader.AsLines()) {
				if (isLicensePresent) {
					codeLine = reader.ReadLine();
				}
				writer.WriteLine(licenseLine);
				isLicensePresent = codeLine == licenseLine;
			}
			// If the license wasn't present, we have a line we need to push back onto the reader
			var lines = isLicensePresent
				? reader.AsLines()
				: Prepend(codeLine, reader.AsLines());
			bool hasEncounteredNamespace = false;
			foreach (var sourceLine in lines) {
				if (usingStatements != null) {
					usingStatements = ProcessUsingStatements(writer, usingStatements, ref hasEncounteredNamespace, sourceLine);
				} else {
					writer.WriteLine(Tabify(sourceLine));
				}
			}
			if (usingStatements != null) {
				throw new Exception("Using statements were never dealt with.");
			}
			return writer.ToString();
		}

		private List<string> ProcessUsingStatements(TextWriter writer, List<string> usingStatements, ref bool hasEncounteredNamespace, string sourceLine) {
            if (!hasEncounteredNamespace)
            {
                if (sourceLine.StartsWith("using")) {
				    usingStatements.Add(sourceLine);
			    } else if (sourceLine.StartsWith("namespace"))
			    {
			        hasEncounteredNamespace = true;
			        if (!sourceLine.Contains("{"))
			        {
			            writer.WriteLine(Tabify(sourceLine));
			        }
			    } else
			    {
			        writer.WriteLine(sourceLine);  // Don't mess with any other line, including compile directives
			    }
            }
			if (hasEncounteredNamespace && sourceLine.Contains("{")) {
				writer.WriteLine(sourceLine);
				if (usingStatements.Count > 0) {
					foreach (var usingLine in usingStatements) {
						writer.WriteLine("\t" + usingLine);
					}
					writer.WriteLine();
				}
				usingStatements = null;
			}
			return usingStatements;
		}

		IEnumerable<T> Prepend<T>(T value, IEnumerable<T> list) {
			yield return value;
			foreach (var t in list) {
				yield return t;
			}
		}

		string Tabify(string line) {
			int spaces = 0;
			int chars = 0;
			foreach (char c in line) {
				if (c == '\t') {
					chars++;
					spaces += tabSize;
				} else if (c == ' ') {
					chars++;
					spaces++;
				} else {
					break;
				}
			}
			return string.Concat(
				new string('\t', spaces / tabSize),
				new string(' ', spaces % tabSize),
				line.Substring(chars)
				);
		}
	}
}
