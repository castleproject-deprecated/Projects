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
namespace Castle.ActiveWriter
{
	using System.ComponentModel;
	using System.Runtime.InteropServices;
	using Microsoft.VisualStudio.Shell;

	[Guid("25A08036-5C86-456e-A1FC-E91FD94484D2")]
    public class ActiveWriterOptions : DialogPage
    {
        [Description("Controls the sorting of Property items in an entity. If set to true, primary keys will be on top and properties will be sorted by Name.")]
        [Category("Server Explorer Integration")]
        [DisplayName("Sort Properties")]
        public bool SortProperties { get; set; }
    }
}
