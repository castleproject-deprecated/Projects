namespace Altinoren.ActiveWriter
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using DslModeling = global::Microsoft.VisualStudio.Modeling;

    public partial class Model : DslModeling::ModelElement
    {
        List<Import> additionalImportsPropertyStorage = new List<Import>();

        private List<Import> GetAdditionalImportsValue()
        {
            return this.additionalImportsPropertyStorage;
        }

        private void SetAdditionalImportsValue(List<Import> value)
        {
            this.additionalImportsPropertyStorage = value;
        }
    }

    [Serializable]
    [DefaultProperty("Name")]
    public class Import
    {
        private string _name;

        [CategoryAttribute("Import")]
        [Description("Enter the namespace to be imported in the generated code.")]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
    }
}
