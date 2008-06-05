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

namespace Castle.MonoRail.Rest
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Linq.Expressions;
	using Castle.MonoRail.Rest.Mime;
	using System.Xml.Serialization;
	using System.Collections.Specialized;

	public static class RepsonderExtensions 
	{
		public static void Serialize(this Responder responder, Object obj)
		{
			XmlSerializer serial = new XmlSerializer(obj.GetType());
			responder.SerializeWith(serial.Serialize, obj);
		}
	}

	public class Responder
	{
		private IControllerBridge _controllerBridge;
		private string _controllerAction;

		public String Format { get; set; }

		public Responder(IControllerBridge controllerBridge, string controllerAction)
		{
			_controllerBridge = controllerBridge;
			_controllerAction = controllerAction;
		}

		/// <summary>
		/// Renders the according view such as actionname_format
		/// <example>
		/// <list type="table">
		/// <listheader><term>context item</term><description>sample</description></listheader>
		/// <item><term>action</term><description>ControllerType.ActionName</description></item>
		/// <item><term>format</term><description>xml</description></item>
		/// </list>
		///		will pick /Views/ControllerType/ActionName_xml template
		/// </example>
		/// </summary>
		public void DefaultResponse()
		{
			_controllerBridge.SendRenderView(_controllerAction + "_" + Format);
		}

		/// <summary>
		/// Serialize given object with provided serialization action adapter
		/// </summary>
		/// <typeparam name="T">object type to be serialized</typeparam>
		/// <param name="serializationActionAdapter">delegate accepting TextWriter and object that will perform serialization</param>
		/// <param name="objectToSerialize">object to be serialized</param>
		public void SerializeWith<T>(Action<System.IO.TextWriter, T> serializationActionAdapter, T objectToSerialize) 
		{
			_controllerBridge.SendCancelLayoutAndView();
			_controllerBridge.UseResponseWriter(writer => serializationActionAdapter(writer, objectToSerialize));
		}
    
		/// <summary>
		/// Sends an empty HTTP response appending given headers to the HTTP Response
		/// </summary>
		/// <param name="statusCode">HTTP response status code</param>
		/// <param name="addHeaders">Additional HTTP headers population delegate</param>
		public void Empty(int statusCode, Action<IDictionary<string, string>> addHeaders)
		{
			Dictionary<string, string> headers = new Dictionary<string, string>();
			
			if (addHeaders != null)
			{
				addHeaders(headers);
			}
			_controllerBridge.SetResponseCode(statusCode);

			foreach (KeyValuePair<string, string> pair in headers)
			{
				_controllerBridge.AppendResponseHeader(pair.Key, pair.Value);

			}
			_controllerBridge.SendCancelLayoutAndView();
		}

		/// <summary>
		/// Sends an empty HTTP response
		/// </summary>
		/// <param name="statusCode">HTTP response status code</param>
		/// <param name="addHeaders">Additional HTTP headers population delegate</param>
		public void Empty(int statusCode)
		{
			Empty(statusCode, null);
		}

		/// <summary>
		/// Sends given text as HTTP response content
		/// </summary>
		/// <param name="text">content</param>
		public void Text(string text)
		{
			_controllerBridge.SendRenderText(text);
		}
	}
}
