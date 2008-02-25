using Castle.Core.Resource;
namespace Castle.MonoRail.Framework.View.Xslt
{
	using System;
	using System.IO;
	using System.Xml;
	using System.Xml.Xsl;
	using System.Collections.Generic;
	using System.Xml.Serialization;
	using System.Reflection;
	using System.Collections;
	using System.Diagnostics;
	using System.Text;
	using System.Xml.XPath;


	public class XmlNavigableBuilder
	{
		private MemoryStream _dataStream;
		private XmlTextWriter _textWriter;
		private bool _completed = false;
		private bool _notEmpty = false;
		private XslCompiledTransform _hierachyFixTransform;


		public bool IsCompleted
		{
			get
			{
				return _completed;
			}
		}

		private void EnsureNotCompleted()
		{
			if (IsCompleted)
				throw new InvalidOperationException("The builder has allready delivered it's product");
		}
		public XmlNavigableBuilder()
		{
			AssemblyResource resource = new AssemblyResource("assembly://Castle.MonoRail.View.Xslt/Castle.MonoRail.View.Xslt/InheritanceHelper.xslt");
			
			_dataStream = new MemoryStream();
			_textWriter = new XmlTextWriter(_dataStream, Encoding.Default);
			_textWriter.WriteStartDocument();
			_hierachyFixTransform = new XslCompiledTransform();
			using (XmlTextReader reader = new XmlTextReader(resource.GetStreamReader()))
            {
				_hierachyFixTransform.Load(reader);	
            }
	
		}

		public void AddGroup(string name)
		{
			EnsureNotCompleted();
			_textWriter.WriteStartElement(name);
			_notEmpty = true;
		}

		public void AddObject(object o)
		{
			EnsureNotCompleted();
			//See docs: when using this constructor, the XmlSerializer (and the generated assemblies) is automatically reused.
			XmlSerializer serializer = new XmlSerializer(o.GetType());
			using (MemoryStream temporaryStream = new MemoryStream())
			{
				using (XmlTextWriter tempWriter = new XmlTextWriter(temporaryStream, Encoding.Default))
				{
					serializer.Serialize(tempWriter, o);

					temporaryStream.Position = 0;
					//This transformation transforms xsi:type attribute values to node names
					_hierachyFixTransform.Transform(new XmlTextReader(temporaryStream), _textWriter);
				}
			};

			_notEmpty = true;
		}

		public void EndGroup()
		{
			EnsureNotCompleted();
			_textWriter.WriteEndElement();
		}

		public IXPathNavigable GetNavigable()
		{
			if (!_notEmpty) return null;
			EnsureNotCompleted();
			_textWriter.WriteEndDocument();
			_completed = true;
			_textWriter.Flush();
			_dataStream.Seek(0, SeekOrigin.Begin);

			Debug.Write(Encoding.Default.GetString(_dataStream.ToArray()));

			using (XmlTextReader reader = new XmlTextReader(_dataStream))
			{
				XmlDocument doc = new XmlDocument();
				doc.Load(reader);
				_textWriter.Close();
				return doc;
			}
		}
	}
}
