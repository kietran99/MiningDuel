using UnityEngine;

namespace MD.AI.BehaviourTree
{
    public static class BTLogger
    {
        public static bool active;

        public const string SEPARATOR = "--------------------------------------------------";

        public const string SUCCESS_STR = " - SUCCESS";
        public const string FAILURE_STR = " - FAILURE";
        public const string RUNNING_STR = " - RUNNING";

        public static void LogSeparator()
        {
            Log(SEPARATOR);
        }

        public static void LogResult(GameObject node, BTNodeState result)
        {
            if (!active)
            {
                return;
            }

            var resStr = result == BTNodeState.SUCCESS ? SUCCESS_STR : (result == BTNodeState.FAILURE ? FAILURE_STR : RUNNING_STR);
            Log(node.name + resStr);
        }

        public static void Log(string msg)
        {
            if (active) Debug.Log("BT: " + msg);
        } 
    }
}
