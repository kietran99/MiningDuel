using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FMSState
{
    public enum STATE {FINDPLAYERTOTHROW, FINDPLAYER, IDLE, THROWBOMBAWAY, RUN, THROWBOMB, DIG, FINDDIGGABLE, WANDER}
    public enum EVENT {ENTER, UPDATE, EXIT}

    public STATE name;
    public EVENT stage;
    public FMSState nextState;
    public PlayerBot bot;

    public FMSState(PlayerBot bot)
    {
        this.bot = bot;
        stage = EVENT.ENTER;
        nextState = null;
    }
    public virtual void Enter() {stage = EVENT.UPDATE;}
    public virtual void Update() {}
    public virtual void Exit() {stage = EVENT.EXIT;}

    public FMSState Process()
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
