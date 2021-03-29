namespace MD.AI.BehaviourTree
{
    public static class BTLogger
    {
        public static bool active;

        public static void Log(string msg)
        {
            if (active) UnityEngine.Debug.Log("BT: " + msg);
        } 
    }
}
