using System.Collections.Generic;

namespace Project.FuzzyLogic
{
    public class FuzzyRule
    {
        public Dictionary<string, string> Antecedents { get; }

        public string ConsequentVariable { get; }
        public string ConsequentSet { get; }

        public FuzzyRule(Dictionary<string, string> antecedents, string consequentVariable, string consequentSet)
        {
            Antecedents = antecedents;
            ConsequentVariable = consequentVariable;
            ConsequentSet = consequentSet;
        }
    }
}
