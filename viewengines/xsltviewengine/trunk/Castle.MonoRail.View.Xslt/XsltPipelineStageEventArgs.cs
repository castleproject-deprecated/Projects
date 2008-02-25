namespace Castle.MonoRail.Framework.View.Xslt
{
	using System;
	using System.Xml.Xsl;
	using System.Xml;
	using System.Collections.Generic;


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