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
	using System.IO;
	using System.Xml;
	using System.Collections.Generic;
	using System.Xml.Serialization;
	using System.Reflection;
	using System.Collections;
	using System.Diagnostics;
	using System.Text;
	using System.Xml.XPath;

	/// <summary>
	/// Base interface for all single XsltTransformations
	/// </summary>
	public interface IXsltTransform
	{
		/// <summary>
		/// Gets the IXsltEngine that was used to created this IXsltTransform.
		/// </summary>
		/// <value>The engine.</value>
		IXsltEngine Engine { get; }

		/// <summary>
		/// Transforms the specified input to the specified output using the specified arguments.
		/// </summary>
		/// <param name="input">The input document.</param>
		/// <param name="output">The output document to write to.</param>
		/// <param name="arguments">The arguments of the transformation.</param>
		void Transform(XmlReader input, XmlWriter output, XsltTransformationArguments arguments);
	}
}
