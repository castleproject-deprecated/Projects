#region License
//  Copyright 2004-2010 Castle Project - http:www.castleproject.org/
//  
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  
//      http:www.apache.org/licenses/LICENSE-2.0
//  
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
// 
#endregion

namespace ICSharpCode.NRefactory.Parser
{
	using System;
	using System.Text;

	public delegate void ErrorCodeProc(int line, int col, int n);
	public delegate void ErrorMsgProc(int line, int col, string msg);
	
	public class Errors
	{
		int count = 0;  // number of errors detected
		public ErrorCodeProc SynErr;
		public ErrorCodeProc SemErr;
		public ErrorMsgProc  Error;
		StringBuilder errorText = new StringBuilder();
		
		public string ErrorOutput {
			get {
				return errorText.ToString();
			}
		}
		
		public Errors()
		{
			SynErr = new ErrorCodeProc(DefaultCodeError);  // syntactic errors
			SemErr = new ErrorCodeProc(DefaultCodeError);  // semantic errors
			Error  = new ErrorMsgProc(DefaultMsgError);    // user defined string based errors
		}
		
		public int Count {
			get {
				return count;
			}
		}
		
		void DefaultCodeError(int line, int col, int n)
		{
			errorText.AppendLine(String.Format("-- line {0} col {1}: error {2}", line, col, n));
			count++;
		}
	
		void DefaultMsgError(int line, int col, string s) {
			errorText.AppendLine(String.Format("-- line {0} col {1}: {2}", line, col, s));
			count++;
		}
	} // Errors
}
