using UnityEngine;

[CreateAssetMenu(menuName = "Game/Behaviours/Fracture")]
public class FractureBehaviour : EntityBehaviour
{
    public override void Initialize(Entity entity)
    {
        entity.SetState(new FindSuitableWindowState());
    }
}