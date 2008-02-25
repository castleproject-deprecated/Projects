using System;
using System.Xml.Xsl;
using System.Xml;
using System.Collections;
using System.Collections.Generic;

namespace Castle.MonoRail.Framework.View.Xslt
{
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
