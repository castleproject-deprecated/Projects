namespace Altinoren.ActiveWriter.GeneratedCodeExtensions
{
    using System;

    public static class InheritedRelationTypeExtensions
    {
        public static RelationType GetMatchingRelationType(this InheritedRelationType type)
        {
            try
            {
                return (RelationType)Enum.Parse(typeof(RelationType), type.ToString());
            }
            catch (ArgumentException)
            {
                throw new ArgumentOutOfRangeException("type", "InheritedRelationType can only be used as a hint to determine the derived relation's RelationType relative to the Model.");
            }
        }
    }
}
