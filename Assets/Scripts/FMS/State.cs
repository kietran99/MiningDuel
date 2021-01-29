namespace MD.AI
{
    public class FSMState
    {
        public enum STATE {FINDPLAYERTOTHROW, FINDPLAYER, IDLE, THROWBOMBAWAY, RUN, THROWBOMB, DIG, FINDDIGGABLE, WANDER}
        public enum EVENT {ENTER, UPDATE, EXIT}

        public STATE name;
        public EVENT stage;
        public FSMState nextState;
        public PlayerBot bot;

        public FSMState(PlayerBot bot)
        {
            this.bot = bot;
            stage = EVENT.ENTER;
            nextState = null;
        }

        public virtual void Enter() {stage = EVENT.UPDATE;}
        public virtual void Update() {}
        public virtual void Exit() {stage = EVENT.EXIT;}

        public FSMState Process()
        {
            if (stage == EVENT.ENTER) Enter();
            if (stage == EVENT.UPDATE) Update();
            if (stage == EVENT.EXIT)
            {
                Exit();
                return nextState;
            }
            return this;
        }
    }
}