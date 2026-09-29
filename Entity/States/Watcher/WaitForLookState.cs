using UnityEngine;
public class WaitForLookState : IEntityState
{
    private Entity entity;
    private float checkTimer;
    private float checkInterval = 3f;
    private float forceDetect = 10f;
    private SFXBank waitingSound;
    public void Enter(Entity entity)
    {
        this.entity = entity;
        waitingSound = Resources.Load<SFXBank>("Audio/Entities/Watcher/Waiting/WatcherWaiting");
        PlayerEffectsManager.Instance.TriggerGlitch(1f, 1f, -0.2f, 3f, 0.025f);
        SFXManager.Instance.Play(waitingSound, entity.gameObject.transform.position,1, int.MaxValue);
        checkTimer = 0f;
    }

    public void Update()
    {
        checkTimer += Time.deltaTime;
        if (PositionFinder.IsVisibleToPlayerDot(entity.player, entity.transform, 0.4f)
            && Vector3.Distance(entity.player.position, entity.transform.position) <= 20f)
        {
            PlayerEffectsManager.Instance.TriggerGlitch(0.2f,1f,-0.5f,0.5f,0.1f);
            entity.SetState(new DetectedState());
            return;
        }
        if (checkTimer < checkInterval)
            return;
        checkTimer = 0f;
        if (!PositionFinder.IsGoodSpot(entity.player, entity.transform))
            entity.SetState(new FindCornerPositionState());
        if(entity.timeAlive > forceDetect)    
            entity.SetState(new MoveInfrontState());
    }
    public void Exit() { }
}
