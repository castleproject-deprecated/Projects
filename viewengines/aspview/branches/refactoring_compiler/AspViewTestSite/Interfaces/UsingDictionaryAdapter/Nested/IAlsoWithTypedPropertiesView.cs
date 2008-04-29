namespace AspViewTestSite.Interfaces.UsingDictionaryAdapter.Nested
{
	public interface IAlsoWithTypedPropertiesView : IWithTypedPropertiesView
	{
		bool? IsImportant { get; set; }
	}
}