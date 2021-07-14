namespace MD.AI
{
    public class FSMState
    {
        public enum STATE 
        {
            FIND_PLAYER_TO_THROW, 
            FIND_PLAYER, 
            IDLE, 
            THROW_PROJECTILE_AWAY, 
            RUN, 
            THROW_PROJECTILE, 
            DIG, 
            FINDING_DIGGABLE, 
            WANDER,
            CHASEPLAYER,
            ATTACKPLAYER
        }

        public enum EVENT { ENTER, UPDATE, EXIT }

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