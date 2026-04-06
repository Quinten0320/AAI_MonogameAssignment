using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Project.Behaviour
{
    public class StateConfig
    {
        public string Steering { get; set; }
        public float Speed { get; set; }
    }

    public class TransitionConfig
    {
        public string From { get; set; }
        public string To { get; set; }
        public List<string> Conditions { get; set; }
    }

    public class ScriptedStateMachine
    {
        private readonly Dictionary<string, StateConfig> _states;
        private readonly List<TransitionConfig> _transitions;
        private readonly float _detectionRange;

        public string CurrentStateName { get; private set; }
        public StateConfig CurrentStateConfig => _states[CurrentStateName];

        private ScriptedStateMachine(Dictionary<string, StateConfig> states, List<TransitionConfig> transitions,
            string initialState, float detectionRange)
        {
            _states = states;
            _transitions = transitions;
            _detectionRange = detectionRange;
            CurrentStateName = initialState;
        }

        public bool Update(float aggression, float playerDistance)
        {
            bool playerInRange = playerDistance <= _detectionRange;

            foreach (var transition in _transitions)
            {
                if (transition.From != CurrentStateName)
                    continue;

                if (EvaluateConditions(transition.Conditions, aggression, playerInRange))
                {
                    CurrentStateName = transition.To;
                    return true;
                }
            }

            return false;
        }

        private static bool EvaluateConditions(List<string> conditions, float aggression, bool playerInRange)
        {
            foreach (var condition in conditions)
            {
                if (condition == "playerInRange" && !playerInRange)
                    return false;
                if (condition == "playerOutOfRange" && playerInRange)
                    return false;
                if (condition.StartsWith("aggressionAbove:"))
                {
                    float threshold = float.Parse(condition.Substring("aggressionAbove:".Length));
                    if (aggression <= threshold)
                        return false;
                }
                if (condition.StartsWith("aggressionBelow:"))
                {
                    float threshold = float.Parse(condition.Substring("aggressionBelow:".Length));
                    if (aggression >= threshold)
                        return false;
                }
            }
            return true;
        }

        public static ScriptedStateMachine LoadFromJson(string path)
        {
            string json = File.ReadAllText(path);
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            string initialState = root.GetProperty("initialState").GetString();
            float detectionRange = root.GetProperty("detectionRange").GetSingle();

            var states = new Dictionary<string, StateConfig>();
            foreach (var stateProp in root.GetProperty("states").EnumerateObject())
            {
                states[stateProp.Name] = new StateConfig
                {
                    Steering = stateProp.Value.GetProperty("steering").GetString(),
                    Speed = stateProp.Value.GetProperty("speed").GetSingle()
                };
            }

            var transitions = new List<TransitionConfig>();
            foreach (var transElem in root.GetProperty("transitions").EnumerateArray())
            {
                var conditions = new List<string>();
                foreach (var cond in transElem.GetProperty("conditions").EnumerateArray())
                    conditions.Add(cond.GetString());

                transitions.Add(new TransitionConfig
                {
                    From = transElem.GetProperty("from").GetString(),
                    To = transElem.GetProperty("to").GetString(),
                    Conditions = conditions
                });
            }

            return new ScriptedStateMachine(states, transitions, initialState, detectionRange);
        }
    }
}
