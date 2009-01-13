using System;
using Castle.MonoRail.Rest;

namespace Castle.Tools.CodeGenerator.RestfulRouting
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class RestRoutesAttribute : Attribute
	{
		public RestRoutesAttribute(string name, string collection, string identifier)
		{
			Name = name;
			Collection = collection.Trim('/');
			Identifier = identifier.Trim('/');
		}

		public RestRoutesAttribute(string name, string collection, string identifier, Type resolverType)
		{
			Name = name;
			Collection = collection.Trim('/');
			Identifier = identifier.Trim('/');
			
			if (!typeof(IRestVerbResolver).IsAssignableFrom(resolverType) || !resolverType.IsClass)
				throw new ArgumentException("The resolverType must be a class that implements IRestVerbResolver");

			RestResolver = resolverType;
		}

		public string Name { get; private set; }
		public string Collection { get; private set; }
		public string Identifier { get; private set; }
		public Type RestResolver { get; private set; }
	}
}