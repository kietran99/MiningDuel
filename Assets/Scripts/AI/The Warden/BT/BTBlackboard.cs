using System.Collections.Generic;
using Functional.Type;

namespace MD.AI.BehaviourTree
{
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

        public Option<T> Get<T>(string key)
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

            UnityEngine.Debug.LogError("Behaviour Tree: Key " + key + " was not found");
            return Option<T>.None;
        }      
    }
}
