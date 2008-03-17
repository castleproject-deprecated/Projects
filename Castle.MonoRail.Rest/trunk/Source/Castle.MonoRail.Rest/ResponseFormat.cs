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
	using Castle.MonoRail.Rest.Mime;
	using System.Linq;

	public interface ResponseFormatInternal
	{
		void AddRenderer(string formatSymbol, ResponderDelegate renderer);
		bool RespondWith(string format, IControllerBridge bridge);
	}

	public class ResponseFormat : ResponseFormatInternal
	{
		private Dictionary<string, ResponderDelegate> _renderers;
		private List<String> _order;

		public ResponseFormat()
		{
			_renderers = new Dictionary<string, ResponderDelegate>();
			_order = new List<string>();
		}

		void ResponseFormatInternal.AddRenderer(string formatSymbol, ResponderDelegate renderer)
		{        
			_renderers[formatSymbol] = renderer;
			_order.Add(formatSymbol);
		}

		bool ResponseFormatInternal.RespondWith(string format, IControllerBridge bridge)
		{
			if (_renderers.ContainsKey(format))
			{
				DoResponse(format, bridge);               
				return true;
			}
			else
			{
				if (String.Equals("all", format, StringComparison.InvariantCultureIgnoreCase))
				{
					DoResponse(_order[0], bridge);
					return true;
				}
				else
				{
					return false;
				}
			}

		}

		private void DoResponse(string format, IControllerBridge bridge)
		{
			Responder hander = new Responder(bridge, bridge.ControllerAction);
			hander.Format = format;
			_renderers[format](hander);

			MimeTypes types = new MimeTypes();
			types.RegisterBuiltinTypes();

			MimeType usedType = types.Where(mime => mime.Symbol == format).First();
			bridge.SetResponseType(usedType);
		}
	   
	   
	}
}
