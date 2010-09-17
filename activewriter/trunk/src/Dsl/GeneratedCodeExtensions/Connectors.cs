namespace Castle.ActiveWriter
{
    using Microsoft.VisualStudio.Modeling.Diagrams;

    public partial class ManyToOneConnector
    {
        protected override int ModifyLuminosity(int currentLuminosity, Microsoft.VisualStudio.Modeling.Diagrams.DiagramClientView view)
        {
            if (!view.HighlightedShapes.Contains(new DiagramItem(this)))
                return currentLuminosity;

            return 130;

        }

    }

    public partial class ManyToManyConnector
    {
        protected override int ModifyLuminosity(int currentLuminosity, Microsoft.VisualStudio.Modeling.Diagrams.DiagramClientView view)
        {
            if (!view.HighlightedShapes.Contains(new DiagramItem(this)))
                return currentLuminosity;

            return 130;

        }

    }

    public partial class OneToOneConnector
    {
        protected override int ModifyLuminosity(int currentLuminosity, Microsoft.VisualStudio.Modeling.Diagrams.DiagramClientView view)
        {
            if (!view.HighlightedShapes.Contains(new DiagramItem(this)))
                return currentLuminosity;

            return 130;

        }

    }

    public partial class NestedConnector
    {
        protected override int ModifyLuminosity(int currentLuminosity, Microsoft.VisualStudio.Modeling.Diagrams.DiagramClientView view)
        {
            if (!view.HighlightedShapes.Contains(new DiagramItem(this)))
                return currentLuminosity;

            return 130;

        }

    }
}