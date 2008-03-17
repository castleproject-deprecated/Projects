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
	using Castle.MonoRail.Rest.Mime;
	using System.IO;
	
	public interface IControllerBridge
	{
		void SetResponseType(MimeType mime);
		void SendRenderView(string view);
		void SendCancelLayoutAndView();
		void UseResponseWriter(Action<TextWriter> writerAction);
		void SetResponseCode(int code);
		void AppendResponseHeader(string headerName, string value);
		void SendRenderText(string text);
		string ControllerAction { get; }
		bool IsFormatDefined { get; }
		string GetFormat();
	}
}
