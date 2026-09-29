using UnityEngine;
public class FindCornerPositionState : IEntityState
{
    private Entity entity;
    private float checkTimer = 0.25f;
    float lastChecked;
    public void Enter(Entity entity)
    {
        this.entity = entity;
    }
    public void Update() 
    {
        if (lastChecked + checkTimer < Time.time)
        {
            Vector3 pos = PositionFinder.FindBestCorner(EntitiesManager.Instance.corners, entity.player, entity.transform);
            if (pos != Vector3.zero)
            {
                entity.SetTargetPosition(pos);
                entity.SetState(new MoveToPositionState());
            }
        }
    }
    public void Exit() { }
}
