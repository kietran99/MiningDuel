﻿namespace MD.Character
{
    public struct ScoreChangeData : EventSystems.IEventData
    {
        public int newScore;

        public ScoreChangeData(int newScore)
        {
            this.newScore = newScore;
        }
    }
}
