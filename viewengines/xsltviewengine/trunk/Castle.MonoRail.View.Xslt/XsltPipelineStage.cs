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

	/// <summary>
	/// Represents a single stage in an XsltPipeline. One stage can do
	/// one XsltTransform a single time.
	/// </summary>
	/// <remark>
	/// A XsltPipelineStage can only be executed once
	/// </remark>
	public class XsltPipelineStage
	{
		public event EventHandler<XsltPipelineStageEventArgs> BeforeExecute;
		public event EventHandler<XsltPipelineStageEventArgs> AfterExecute;

		private bool _executed = false;

		private IXsltTransform _Transformer;

		public IXsltTransform Transformer
		{
			get { return _Transformer; }
			private set { _Transformer = value; }
		}

		public XsltPipelineStage(IXsltTransform tranformer)
		{
			_Transformer = tranformer;
		}

		public void Execute(XmlReader reader, XmlWriter writer, XsltTransformationArguments arguments)
		{
			if (_executed)
				throw new InvalidOperationException("XsltPipelineStage has already been executed!");

			if (BeforeExecute != null)
				BeforeExecute.Invoke(this, new XsltPipelineStageEventArgs(reader, writer, arguments));

			_Transformer.Transform(reader, writer, arguments);
			_executed = true;

			if (AfterExecute != null)
				AfterExecute.Invoke(this, new XsltPipelineStageEventArgs(reader, writer, arguments));
		}
	}
}
