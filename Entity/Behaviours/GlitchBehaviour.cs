using UnityEngine;

[CreateAssetMenu(menuName = "Game/Behaviours/Glitch")]

public class GlitchBehaviour : EntityBehaviour
{
    public override void Initialize(Entity entity)
    {
        entity.SetState(new GlitchIdleState());
    }
}
