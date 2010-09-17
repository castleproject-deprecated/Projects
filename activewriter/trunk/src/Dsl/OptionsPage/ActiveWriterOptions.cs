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
