using System.Collections.Generic;

namespace Project.FuzzyLogic
{
    public class FuzzyVariable
    {
        public string Name { get; }
        private readonly List<FuzzySet> _sets = new List<FuzzySet>();

        public FuzzyVariable(string name)
        {
            Name = name;
        }

        public IReadOnlyList<FuzzySet> Sets => _sets;

        public void AddSet(FuzzySet set)
        {
            _sets.Add(set);
        }

        public FuzzySet GetSet(string name)
        {
            foreach (var set in _sets)
                if (set.Name == name)
                    return set;
            return null;
        }

        public Dictionary<string, float> Fuzzify(float crispValue)
        {
            var result = new Dictionary<string, float>();
            foreach (var set in _sets)
                result[set.Name] = set.GetDOM(crispValue);
            return result;
        }
    }
}
