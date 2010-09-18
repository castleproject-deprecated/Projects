#region License
//  Copyright 2004-2010 Castle Project - http:www.castleproject.org/
//  
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  
//      http:www.apache.org/licenses/LICENSE-2.0
//  
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
// 
#endregion
using DslModeling = global::Microsoft.VisualStudio.Modeling;
using DslDesign = global::Microsoft.VisualStudio.Modeling.Design;
using DslDiagrams = global::Microsoft.VisualStudio.Modeling.Diagrams;

namespace Castle.ActiveWriter
{
	/// <summary>
	/// Helper class used to map shapes and model elements to help keywords.
	/// </summary>
	/// <remarks>
	/// Double-derived class to allow easier code customization.
	/// </remarks>
	public partial class ActiveWriterHelpKeywordHelper : ActiveWriterHelpKeywordHelperBase 
	{
		
		/// <summary>
		/// Constructs a new ActiveWriterHelpKeywordHelper.
		/// </summary>
		protected ActiveWriterHelpKeywordHelper()
			: base() { }
			
		/// <summary>
		/// Single instance of the ActiveWriterHelpKeywordHelper.
		/// </summary>
		public static ActiveWriterHelpKeywordHelper Instance
		{
			get
			{
				return null; // This DSL does not define any help keywords.
			}
		}
	}
	
	/// <summary>
	/// Helper class used to map shapes and model elements to F1 help keywords.
	/// </summary>
	public abstract class ActiveWriterHelpKeywordHelperBase
	{
		private global::System.Collections.Generic.Dictionary<string, string> helpKeywords;
		
		/// <summary>
		/// Constructs a new ActiveWriterHelpKeywordHelperBase.
		/// </summary>
		protected ActiveWriterHelpKeywordHelperBase()
			: base() { }
			
		/// <summary>
		/// Called to initialize the HelpKeywords dictionary.  Derived classes may override this to add custom keywords to the collection.
		/// </summary>
		protected virtual void Initialize()
		{
			helpKeywords = new global::System.Collections.Generic.Dictionary<string, string>(0);
		}
		
		/// <summary>
		/// Collection of key/value pairs describing help keywords for this DSL.
		/// Keys are strings such as domain class names that describe elements of the DSL.
		/// Values are corresponding help keywords.
		/// </summary>
		protected global::System.Collections.Generic.IDictionary<string, string> HelpKeywords
		{
			get
			{
				if (helpKeywords == null)
				{
					helpKeywords = new global::System.Collections.Generic.Dictionary<string, string>(10);
				}

				return helpKeywords;
			}
		}
		
		/// <summary>
		/// Gets the help keyword associated with the given shape or model element instance.
		/// </summary>
		/// <returns>Help keyword, or empty string if there is no associated help keyword.</returns>
		public virtual string GetHelpKeyword(DslModeling::ModelElement modelElement)
		{
			if(modelElement == null) throw new global::System.ArgumentNullException("modelElement");
			
			if(helpKeywords == null)
			{
				Initialize();
			}
			
			string helpKeyword;
			if(helpKeywords.TryGetValue(modelElement.GetType().FullName, out helpKeyword))
			{
				return helpKeyword;
			}
			
			return string.Empty;
		}
		
		/// <summary>
		/// Gets the help keyword associated with the given domain role.
		/// </summary>
		/// <returns>Help keyword, or empty string if there is no associated help keyword.</returns>
		public virtual string GetDomainRoleHelpKeyword(DslModeling::DomainRoleInfo domainRole)
		{
			if(domainRole == null) throw new global::System.ArgumentNullException("domainRole");
			
			if(helpKeywords == null)
			{
				Initialize();
			}
			
			string helpKeyword;
			if(helpKeywords.TryGetValue(domainRole.DomainRelationship.ImplementationClass.FullName + "/" + domainRole.Name, out helpKeyword))
			{
				return helpKeyword;
			}
			
			return string.Empty;
		}
		
		/// <summary>
		/// Gets the help keyword associated with the given domain property.
		/// </summary>
		/// <returns>Help keyword, or empty string if there is no associated help keyword.</returns>
		public virtual string GetDomainPropertyHelpKeyword(DslModeling::DomainPropertyInfo domainProperty)
		{
			if(domainProperty == null) throw new global::System.ArgumentNullException("domainProperty");
			
			if(helpKeywords == null)
			{
				Initialize();
			}
			
			string helpKeyword;
			if(helpKeywords.TryGetValue(domainProperty.DomainClass.ImplementationClass.FullName + "/" + domainProperty.Name, out helpKeyword))
			{
				return helpKeyword;
			}
			
			return string.Empty;
		}
	}
}
