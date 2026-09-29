using UnityEngine;

[CreateAssetMenu(menuName = "Game/Behaviours/Ephemer")]
public class EphemerBehaviour : EntityBehaviour
{
    public override void Initialize(Entity entity)
    {
        entity.SetState(new FindStartingPosState());
    }
}
