namespace Altinoren.ActiveWriter.GeneratedCodeExtensions
{
    using System;

    public static class InheritedRelationTypeExtensions
    {
        public static RelationType GetMatchingRelationType(this InheritedRelationType type)
        {
            switch (type)
            {
                case InheritedRelationType.Guess:
                    return RelationType.Guess;
                case InheritedRelationType.Bag:
                    return RelationType.Bag;
                case InheritedRelationType.Set:
                    return RelationType.Set;
                case InheritedRelationType.IdBag:
                    return RelationType.IdBag;
                case InheritedRelationType.Map:
                    return RelationType.Map;
                case InheritedRelationType.List:
                    return RelationType.List;
                default:
                    throw new ArgumentOutOfRangeException("type", "InheritedRelationType can only be used as a hint to determine the derived relation's RelationType relative to the Model.");
            }
        }
    }
}