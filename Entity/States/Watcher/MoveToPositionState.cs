using UnityEngine;
public class MoveToPositionState : IEntityState
{
    private Entity entity;
    public void Enter(Entity entity)
    {
        this.entity = entity;
    }
    public void Update()
    {
        entity.transform.position = entity.targetPosition;

        if (Vector3.Distance(entity.transform.position, entity.targetPosition) < 0.2f)
        {
            entity.SetState(new WaitForLookState());
        }
    }
    public void Exit() { }
}
