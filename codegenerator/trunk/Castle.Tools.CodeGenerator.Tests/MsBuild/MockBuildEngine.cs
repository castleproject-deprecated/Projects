// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

namespace Castle.Tools.CodeGenerator.MsBuild
{
	using System;
	using System.Collections;
	using Microsoft.Build.BuildEngine;
	using Microsoft.Build.Framework;

	public class MockBuildEngine : IBuildEngine
	{
		protected Project project;

		public MockBuildEngine(Project project)
		{
			this.project = project;
		}

		public virtual bool BuildProjectFile(string projectFileName, string[] targetNames, IDictionary globalProperties, IDictionary targetOutputs)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public virtual int ColumnNumberOfTaskNode
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		public virtual bool ContinueOnError
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		public virtual int LineNumberOfTaskNode
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		public virtual void LogCustomEvent(CustomBuildEventArgs e)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public virtual void LogErrorEvent(BuildErrorEventArgs e)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public virtual void LogMessageEvent(BuildMessageEventArgs e)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public virtual void LogWarningEvent(BuildWarningEventArgs e)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public virtual string ProjectFileOfTaskNode
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}
	}
}