using UnityEngine;

[CreateAssetMenu(menuName = "Game/Behaviours/Watcher")]
public class WatcherBehaviour : EntityBehaviour
{
    public override void Initialize(Entity entity)
    {
        entity.SetState(new FindCornerPositionState());
    }
}
