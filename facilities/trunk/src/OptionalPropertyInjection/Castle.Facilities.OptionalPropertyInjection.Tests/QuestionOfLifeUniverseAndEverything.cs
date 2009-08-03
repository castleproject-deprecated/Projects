using System;

namespace Castle.Facilities.OptionalPropertyInjection.Tests {
    public class AnswerToLifeUniverseAndEverything {
        public readonly int Value = 42;
    }
    public class QuestionOfLifeUniverseAndEverything {
        public AnswerToLifeUniverseAndEverything TheAnswer { get; set; }
        public AnswerToLifeUniverseAndEverything OtherAnswer { get; set; }
    }
    public class AnswerToBeOrNotToBe {
    }
    public class QuestionToBeOrNotToBe {
        public AnswerToBeOrNotToBe TheAnswer { get; set; }
    }
}