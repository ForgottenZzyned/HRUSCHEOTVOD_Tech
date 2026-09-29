using UnityEngine;
public class MoveInfrontState : IEntityState
{
    private Entity entity;
    public void Enter(Entity entity)
    {
        this.entity = entity;
        entity.SetPosition(PositionFinder.GetPosInfrontPlayer
                           (CameraMovement.Instance.gameObject.transform, 2f));
        entity.SetState(new DetectedState());
    }
    public void Update()
    {
        
    }
    public void Exit()
    {

    }
}
