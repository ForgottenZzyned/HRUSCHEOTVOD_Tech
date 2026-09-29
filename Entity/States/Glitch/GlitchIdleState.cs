using UnityEngine;

public class GlitchIdleState : IEntityState
{
    private Entity entity;
    public void Enter(Entity entity)
    {
        this.entity = entity;
        entity.SetPosition(PositionFinder.FindHiddenSpotDummy(PlayerManager.Instance.playerObject.transform,1f,20f));
        entity.SetState(new GlitchAttackingState());
    }
    public void Update()
    {

    }
    public void Exit()
    {

    }
}
