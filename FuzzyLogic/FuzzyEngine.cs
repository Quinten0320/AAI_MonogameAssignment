using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Project.FuzzyLogic
{
    public class FuzzyEngine
    {
        private readonly Dictionary<string, FuzzyVariable> _variables = new Dictionary<string, FuzzyVariable>();
        private readonly List<FuzzyRule> _rules = new List<FuzzyRule>();

        public void AddVariable(FuzzyVariable variable)
        {
            _variables[variable.Name] = variable;
        }

        public void AddRule(FuzzyRule rule)
        {
            _rules.Add(rule);
        }

        public Dictionary<string, float> Evaluate(Dictionary<string, float> inputs)
        {
            var fuzzified = new Dictionary<string, Dictionary<string, float>>();
            foreach (var input in inputs)
            {
                if (_variables.TryGetValue(input.Key, out var variable))
                    fuzzified[input.Key] = variable.Fuzzify(input.Value);
            }

            var consequentStrengths = new Dictionary<string, Dictionary<string, float>>();

            foreach (var rule in _rules)
            {
                float firingStrength = float.MaxValue;
                bool valid = true;

                foreach (var antecedent in rule.Antecedents)
                {
                    if (!fuzzified.TryGetValue(antecedent.Key, out var varDoms) ||
                        !varDoms.TryGetValue(antecedent.Value, out float dom))
                    {
                        valid = false;
                        break;
                    }
                    firingStrength = Math.Min(firingStrength, dom);
                }

                if (!valid || firingStrength <= 0f)
                    continue;

                if (!consequentStrengths.ContainsKey(rule.ConsequentVariable))
                    consequentStrengths[rule.ConsequentVariable] = new Dictionary<string, float>();

                var setStrengths = consequentStrengths[rule.ConsequentVariable];
                if (!setStrengths.ContainsKey(rule.ConsequentSet) || firingStrength > setStrengths[rule.ConsequentSet])
                    setStrengths[rule.ConsequentSet] = firingStrength;
            }

            var results = new Dictionary<string, float>();
            foreach (var consequent in consequentStrengths)
            {
                if (!_variables.TryGetValue(consequent.Key, out var variable))
                    continue;

                float weightedSum = 0f;
                float totalWeight = 0f;

                foreach (var setStrength in consequent.Value)
                {
                    var set = variable.GetSet(setStrength.Key);
                    if (set == null) continue;

                    float weight = setStrength.Value;
                    weightedSum += set.RepresentativeValue * weight;
                    totalWeight += weight;
                }

                results[consequent.Key] = totalWeight > 0 ? weightedSum / totalWeight : 0f;
            }

            return results;
        }

        public static FuzzyEngine LoadFromJson(string path)
        {
            var engine = new FuzzyEngine();
            string json = File.ReadAllText(path);
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            foreach (var varProp in root.GetProperty("variables").EnumerateObject())
            {
                var variable = new FuzzyVariable(varProp.Name);

                foreach (var setProp in varProp.Value.GetProperty("sets").EnumerateObject())
                {
                    string type = setProp.Value.GetProperty("type").GetString();
                    var points = setProp.Value.GetProperty("points");

                    FuzzySet set;
                    if (type == "Triangle")
                    {
                        set = new TriangleFuzzySet(
                            setProp.Name,
                            points[0].GetSingle(),
                            points[1].GetSingle(),
                            points[2].GetSingle()
                        );
                    }
                    else
                    {
                        set = new TrapezoidFuzzySet(
                            setProp.Name,
                            points[0].GetSingle(),
                            points[1].GetSingle(),
                            points[2].GetSingle(),
                            points[3].GetSingle()
                        );
                    }

                    variable.AddSet(set);
                }

                engine.AddVariable(variable);
            }

            foreach (var ruleElem in root.GetProperty("rules").EnumerateArray())
            {
                var antecedents = new Dictionary<string, string>();
                foreach (var ant in ruleElem.GetProperty("antecedents").EnumerateObject())
                    antecedents[ant.Name] = ant.Value.GetString();

                var consequent = ruleElem.GetProperty("consequent");
                foreach (var con in consequent.EnumerateObject())
                {
                    engine.AddRule(new FuzzyRule(antecedents, con.Name, con.Value.GetString()));
                }
            }

            return engine;
        }
    }
}
