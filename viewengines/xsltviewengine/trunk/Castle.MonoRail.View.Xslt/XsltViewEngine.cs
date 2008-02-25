// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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
	using System.IO;
	using System.Xml;
	using System.Collections.Generic;
	using System.Xml.Serialization;
	using System.Reflection;
	using System.Collections;
	using System.Diagnostics;
	using System.Text;
	using System.Xml.XPath;
	using Castle.MonoRail.Framework.Helpers;
	using sdf.XPath;

	/// <summary>
	/// 
	/// </summary>
	public class XsltViewEngine : ViewEngineBase
	{
		private XsltTemplateStore _tempStore;
		/// <summary>
		/// The IXsltEngine to do all transformations with
		/// </summary>
		private IXsltEngine _xsltEngine;

		/// <summary>
		/// Gets the template store.
		/// </summary>
		/// <value>The template store.</value>
		public XsltTemplateStore TemplateStore
		{
			get
			{
				if (_tempStore == null)
					_tempStore = new XsltTemplateStore(base.ViewSourceLoader, _xsltEngine);
				return _tempStore;
			}
		}

		/// <summary>
		/// Initializes a new instance of the XsltViewEngine class.
		/// </summary>
		public XsltViewEngine()
		{
			//Instantiate the IXsltEngine
			//TODO: put in configuration somewhere
			//Type xsltEngineType = Type.GetType("Castle.MonoRail.View.Xslt.Saxon.XsltEngine, Castle.MonoRail.View.Xslt.Saxon");
			Type xsltEngineType = Type.GetType("Castle.MonoRail.View.Xslt.NativeEngine.XsltEngine, Castle.MonoRail.View.Xslt.NativeEngine");
			_xsltEngine = Activator.CreateInstance(xsltEngineType) as IXsltEngine;
		}

		/// <summary>
		/// Evaluates whether the specified template exists.
		/// </summary>
		/// <returns><c>true</c> if it exists</returns>
		public override bool HasTemplate(string templateName)
		{
			return TemplateStore.HasTemplate(templateName); ;
		}


		/// <summary>
		/// Processes the view - using the templateName to obtain the correct template,
		/// and using the context to output the result.
		/// </summary>
		public override void Process(IRailsEngineContext context, IController controller, String templateName)
		{
			try
			{
				AdjustContentType(context);

				//Todo: make configurable so this can be turned off
				//Dump all arguments in XML to the ResponseStream
				if (context.Request.Params["debugxml"] != null)
				{

					XsltTransformationArguments arguments = CreateArguments(context, controller);

					XmlDocument doc = new XmlDocument();
					foreach (XsltTransformationParameter thing in arguments.Parameters)
					{
						XmlElement elem = doc.CreateElement(thing.Name, thing.NameSpace);

						if (thing.Value is IXPathNavigable)
							elem.InnerXml = (thing.Value as IXPathNavigable).CreateNavigator().OuterXml;
						else
							elem.Value = thing.Value.ToString();
						
					}
					context.Response.ContentType = "text/xml";
					doc.WriteTo(new XmlTextWriter(context.Response.Output));
				}
				else
				{
					//Run the transformations.
					RunStylesheets(context, controller, templateName);
				}
			}
			catch (Exception ex)
			{
				if (context.Request.IsLocal)
				{
					SendErrorDetails(ex, context.Response.Output);
				}
				else
				{
					//TODO: possibly incorrect message
					throw new RailsException("Could not obtain view: " + templateName, ex);
				}
			}
		}


		/// <summary>
		/// Runs the necessary stylesheet transformations.
		/// </summary>
		/// <param name="context">The IRailsEngineContext containing the necessary transformation parameters.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="templateName">Name of the template to run.</param>
		private void RunStylesheets(IRailsEngineContext context, IController controller, String templateName)
		{
			//Collect arguments
			XsltTransformationArguments arguments = CreateArguments(context, controller);
			//Start new pipeline
			XsltPipeline pipeline = new XsltPipeline(arguments);
			//First stage
			pipeline.AddStage(new XsltPipelineStage(TemplateStore.LoadTemplate(templateName, arguments)));

			//Layout stage
			if (controller.LayoutName != null)
			{
				pipeline.AddStage(new XsltPipelineStage(TemplateStore.LoadTemplate(@"layouts\" + controller.LayoutName, arguments)));
			}

			//Execute pipeline
			pipeline.Execute(InputDocument, new XmlTextWriter(context.Response.Output));
		}

		/// <summary>
		/// Wraps the specified content in the layout using the context to output the result.
		/// </summary>
		public override void ProcessContents(IRailsEngineContext context, IController controller, String contents)
		{
			//TODO: implement me!
			throw new NotImplementedException();
		}


		/// <summary>
		/// Returns an Empty Xml document that is used as input to 
		/// the XsltPipeline
		/// </summary>
		private XmlTextReader InputDocument
		{
			get
			{
				return CreateInputXmlDocument();
			}
		}

		private static XmlTextReader CreateInputXmlDocument()
		{
			return new XmlTextReader(new StringReader("<?xml version=\"1.0\" encoding=\"utf-8\" ?><root></root>"));
		}

		/// <summary>
		/// Creates an XsltTransformationArguments instance using the information
		/// from the controller and IRailsEngineContext
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="controller">The controller.</param>
		/// <returns></returns>
		private XsltTransformationArguments CreateArguments(IRailsEngineContext context, IController controller)
		{
			XsltTransformationArguments arguments = new XsltTransformationArguments();
			
			//Flash
			AddFlash(context, arguments);
			//P-bag
			AddPropertyBag(context, controller, arguments);
			//Helpers
			AddExtensionObjects(context, controller, arguments);

			return arguments;
		}

		/// <summary>
		/// Adds the necessary helper objects to the specified
		/// XsltTransformationArguments instance.
		/// </summary>
		/// <param name="context">The context</param>
		/// <param name="controller">The controller to get the Helpers from</param>
		/// <param name="arguments">The XsltTransformationArguments instance to add the Helpers to.</param>
		private static void AddExtensionObjects(IRailsEngineContext context, IController controller, XsltTransformationArguments arguments)
		{
			List<string> extensions = new List<string>();
			foreach (object helper in controller.Helpers.Values)
			{
				//TODO: get_Name is an expensive reflection operation
				string name = helper.GetType().Name;

				//Make sure every helper is only added once
				if (extensions.Contains(name)) continue;

				arguments.AddExtensionObject(helper);
				extensions.Add(name);
			}
			
			
		}

		/// <summary>
		/// Adds XPathNavigable representations of the items in the propertybag
		/// of the controller.
		/// </summary>
		/// <param name="controller">The controller.</param>
		/// <param name="arguments">The arguments.</param>
		private static void AddPropertyBag(IRailsEngineContext railscontext, IController controller, XsltTransformationArguments arguments)
		{
			ObjectXPathContext context = new ObjectXPathContext();
			//For each object in the property bag build an ObjectXPathNavigator to
			//create an XPath-able represenation of that object
			foreach (String key in controller.PropertyBag.Keys)
			{
				object value = controller.PropertyBag[key];

				if (value != null)
				{
					arguments.AddParam(key, string.Empty, context.CreateNavigator(value));
				}
			}

			arguments.AddParam("context", string.Empty, context.CreateNavigator(railscontext));
			//arguments.AddExtensionObject("urn:request", context.Request);
			//arguments.AddExtensionObject("urn:response", context.Response);
			//arguments.AddExtensionObject("urn:server", context.Server);
			//arguments.AddExtensionObject("urn:session", context.Session);
		}

		private static void AddFlash(IRailsEngineContext context, XsltTransformationArguments arguments)
		{
			if (context.Flash.Keys.Count == 0) return;

				ObjectXPathContext ocontext = new ObjectXPathContext();

			arguments.AddParam(Flash.FlashKey, string.Empty, ocontext.CreateNavigator(context.Flash));
		}

		/// <summary>
		/// Sends the error details.
		/// </summary>
		/// <param name="ex">The ex.</param>
		/// <param name="writer">The writer to send the error details to.</param>
		private void SendErrorDetails(Exception ex, TextWriter writer)
		{
			writer.WriteLine(ex.ToString());
		}

		public override object CreateJSGenerator(IRailsEngineContext context)
		{
			throw new NotImplementedException();
		}

		public override void GenerateJS(TextWriter output, IRailsEngineContext context, IController controller, string templateName)
		{
			//TODO: implement me
			throw new NotImplementedException();
		}

		public override string JSGeneratorFileExtension
		{
			//TODO: implement me
			get { throw new NotImplementedException(); }
		}

		public override void Process(TextWriter output, IRailsEngineContext context, IController controller, string templateName)
		{
			//TODO: implement me
			throw new NotImplementedException();
		}

		public override void ProcessPartial(TextWriter output, IRailsEngineContext context, IController controller, string partialName)
		{
			//TODO: implement me
			throw new NotImplementedException();
		}

		public override bool SupportsJSGeneration
		{
			//TODO: implement me
			get { return false; }
		}

		/// <summary>
		/// Gets the view file extension.
		/// </summary>
		/// <value>.xslt</value>
		public override string ViewFileExtension
		{
			get { return ".xslt"; }
		}
	}
}

