using System.Collections.Generic;
using Functional.Type;

namespace MD.AI.BehaviourTree
{
    public static class OptionExtension
    {
        public static BTNodeState Map<T>(this Option<T> option, System.Func<T, BTNodeState> fn)
        {
            return option.Match(fn, () => BTNodeState.FAILURE);
        }
    }

    public class BTBlackboard
    {
        private IDictionary<string, object> dict;

        public BTBlackboard(int capacity = 1)
        {
            dict = new Dictionary<string, object>(capacity);
        }

        public void Set<T>(string key, T val)
        {
            dict[key] = val;
        }  

        public Option<T> Get<T>(string key, bool ignoreKeyNotFound = false)
        {
            if (dict.TryGetValue(key, out var val))
            {
                if (val is T)
                {
                    return (T) val;                
                }

                UnityEngine.Debug.LogError("Behaviour Tree: Value type mismatch");
                return Option<T>.None;
            }

            if (!ignoreKeyNotFound) UnityEngine.Debug.LogError("Behaviour Tree: Key " + key + " was not found");
            return Option<T>.None;
        }   

        public void Log()
        {
            foreach (var pair in dict)
            {
                UnityEngine.Debug.Log(pair.Key + ": " + pair.Value);
            }
        }   
    }
}
