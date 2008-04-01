namespace Altinoren.ActiveWriter
{
    using Microsoft.VisualStudio.Modeling.Diagrams;

    public interface IActiveWriterDiagramExtensibilityObject
    {
        Diagram ActiveWriterDiagram { get; }
    }
}