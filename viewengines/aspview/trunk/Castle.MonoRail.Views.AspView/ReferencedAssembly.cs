using System;
using System.Collections.Generic;
using System.Text;

namespace Castle.MonoRail.Views.AspView
{
	public class ReferencedAssembly
	{
		private string _name;
		private AssemblySource _source;

		public ReferencedAssembly(string name, AssemblySource source)
		{
			_name = name;
			_source = source;
		}

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}


		public AssemblySource Source
		{
			get { return _source; }
			set { _source = value; }
		}

		public enum AssemblySource
		{
			GlobalAssemblyCache,
			BinDirectory
		}
		
	}
}
