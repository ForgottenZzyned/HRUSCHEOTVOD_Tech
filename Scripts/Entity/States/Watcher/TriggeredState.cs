using UnityEngine;

public class TriggeredState : IEntityState
{
    private Entity entity;
    public void Enter(Entity entity)
    {
        this.entity = entity;
        entity.DoDamage();
        entity.SetState(new DespawnState());
    }
    public void Update() { }
    public void Exit() 
    {

    }
}
