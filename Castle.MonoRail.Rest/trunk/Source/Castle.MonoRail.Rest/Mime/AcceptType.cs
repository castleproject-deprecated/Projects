// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Rest.Mime
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	public class AcceptType
	{
		public string Name { get; set; }
		public int Order { get; set; }
		public float Q { get; set; }

		
		public AcceptType()
		{

		}

		public static MimeType[] Parse(string acceptHeader, MimeTypes mimes)
		{
			
			var splitHeaders = acceptHeader.Split(',');
			var acceptTypes = new List<AcceptType>(splitHeaders.Length);

			for (int i = 0; i < splitHeaders.Length; i++)
			{
				var parms = splitHeaders[i].Split(';');
				AcceptType at = new AcceptType();
				at.Name = parms[0].Trim();
				at.Order = i;

				at.Q = parms.Length == 2 ? Convert.ToSingle(parms[1].Substring(2)) : 1;
				acceptTypes.Add(at);
			}

			var appXml = acceptTypes.Find(at => at.Name == "application/xml");
			if (appXml != null)
			{
				var regEx = new System.Text.RegularExpressions.Regex(@"\+xml$");

				int appXmlIndex;
				int idx = appXmlIndex = acceptTypes.IndexOf(appXml);

				while (idx < acceptTypes.Count)
				{
					var at = acceptTypes[idx];
					if (at.Q < appXml.Q)
					{
						break;
					}
					
					if(regEx.IsMatch(at.Name)) {
						acceptTypes.Remove(at);
						acceptTypes.Insert(appXmlIndex,at);
						appXmlIndex++;
					}
					idx++;
				}                
			}
																										
			var returnTypes = new List<MimeType>();
			foreach (var type in acceptTypes.OrderByDescending(at => at.Q))
			{
				returnTypes.AddRange(mimes.Where(m => m.MimeString == type.Name || m.Synonyms.Contains(type.Name)));                
			}


			return returnTypes.Distinct().ToArray();
		}
	}
}
