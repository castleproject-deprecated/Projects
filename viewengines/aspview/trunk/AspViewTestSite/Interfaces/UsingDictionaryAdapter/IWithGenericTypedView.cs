namespace AspViewTestSite.Interfaces.UsingDictionaryAdapter {
  public interface IWithGenericTypedView<TItem> {
    TItem Item { get; set; }
  }
}
