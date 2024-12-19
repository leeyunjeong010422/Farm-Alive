using UnityEngine;

public abstract class BaseState
{
    public abstract void StateEnter();
    public abstract void StateUpdate();
    public abstract void StateExit();
}
