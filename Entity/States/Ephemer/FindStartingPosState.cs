using UnityEngine;

public class FindStartingPosState : IEntityState
{
    private Entity entity;
    public void Enter(Entity entity)
    {
        this.entity = entity;
        entity.SetPosition(PositionFinder.FindHiddenSpotDummy(CameraMovement.Instance.gameObject.transform,-0.4f,35,45));
        entity.SetState(new ChasingPlayerState(5f));
    }
    public void Update()
    {

    }
    public void Exit()
    {

    }
}
