using System;

namespace AspViewTestSite.Interfaces.UsingDictionaryAdapter
{
	public interface IStupidView
	{
		Guid Id { get; set; }
		string Name { get; set; }
		string Message { get; set; }
	}
}