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

namespace Castle.MonoRail.Framework.View.Xslt
{
	using System;
	using System.Xml;
	
	public class XsltPipelineStageEventArgs : EventArgs
	{

		/// <summary>
		/// Initializes a new instance of the XsltPipelineStageEventArgs class.
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="writer"></param>
		/// <param name="arguments"></param>
		public XsltPipelineStageEventArgs(XmlReader reader, XmlWriter writer, XsltTransformationArguments arguments)
		{
			_Reader = reader;
			_Writer = writer;
			_Arguments = arguments;
		}

		private XmlReader _Reader;
		public XmlReader Reader
		{
			get { return _Reader; }
		}

		private XmlWriter _Writer;
		public XmlWriter Writer
		{
			get { return _Writer; }
		}

		private XsltTransformationArguments _Arguments;
		public XsltTransformationArguments Arguments
		{
			get { return _Arguments; }
		}
	}
}