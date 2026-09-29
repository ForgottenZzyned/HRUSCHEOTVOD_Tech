using UnityEngine;

[CreateAssetMenu(menuName = "Game/Behaviours/Walker")]
public class WalkerBehaviour : EntityBehaviour
{
    public override void Initialize(Entity entity)
    {
        entity.SetState(new GetPosInBackState());
    }
}
