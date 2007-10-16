using TestModel;

namespace AspViewTestSite.Interfaces.UsingDictionaryAdapter
{
	public interface IWithTypedPropertiesView
	{
		int Id { get; set; }
		string Name { get; set; }
		Post Post { get; set; }
	}
}