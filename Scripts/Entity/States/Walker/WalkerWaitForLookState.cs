using UnityEngine;
public class WalkerWaitForLookState : IEntityState
{
    private Entity entity;
    private float despawnTime = 1.5f;
    public void Enter(Entity entity)
    {
        this.entity = entity;
    }
    public void Update()
    {
        if (PositionFinder.IsVisibleToPlayerDot(entity.player, entity.transform, 0.55f)
            && Vector3.Distance(entity.player.position, entity.transform.position) <= 100f
            && !PlayerEffectsManager.isScreamer)
        {
            entity.SetState(new WalkerAttackState());
            return;
        }
        if (entity.timeAlive > despawnTime)
            entity.SetState(new DespawnState());
    }
    public void Exit() { }
}
