using UnityEngine;
public class GetPosInBackState : IEntityState
{
    private Entity entity;
    public void Enter(Entity entity)
    {
        this.entity = entity;
        entity.SetPosition(PositionFinder.GetPosInbackPlayer(CameraMovement.Instance.gameObject.transform, 60f));
        SFXBank sfx = Resources.Load<SFXBank>("Audio/Entities/Walker/Running/Running");
        SFXManager.Instance.Play(sfx, entity.transform.position,0.5f,int.MaxValue);
        entity.SetState(new WalkerWaitForLookState());
    }
    public void Update()
    {

    }
    public void Exit() { }
}
