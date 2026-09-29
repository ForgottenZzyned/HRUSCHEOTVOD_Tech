using UnityEngine;
public class GlitchAttackingState : IEntityState
{
    private Entity entity;
    private float distDegrStep = 4f;
    private float tpInterval = 2f;
    private float lastTpTime;
    private float maxDist = 20;
    private float timeNeededToKill = 1f;

    private SFXBank replaceSFX;
    private float delay;
    public void Enter(Entity entity)
    {
        this.entity = entity;
        delay = Random.Range(entity.data.spawnDelay.x, entity.data.spawnDelay.y);
        replaceSFX = Resources.Load<SFXBank>("Audio/Entities/Glitch/Replace/GlitchReplace");
    }
    public void Update()
    {
        if (entity.timeAlive < delay) return;
        if (lastTpTime + tpInterval < Time.time)
        {
            lastTpTime = Time.time;
            maxDist -= Random.Range(distDegrStep-1,distDegrStep+1);
            if(maxDist <= 0) entity.SetState(new GlitchChaseState(10f));
            entity.SetPosition(PositionFinder.FindHiddenSpotDummy(PlayerManager.Instance.playerObject.transform, 1f, maxDist,maxDist));
            SFXManager.Instance.Play(replaceSFX, entity.gameObject.transform.position);
        }
        if (PositionFinder.IsVisibleToPlayerDot(entity.player, entity.transform) && Flashlight.Instance.UsingFlashLight())
        {
            timeNeededToKill -= Time.deltaTime;
            if (timeNeededToKill <= 0) entity.SetState(new GlitchDespawnState());        
        }
        if (Vector3.Distance(entity.player.position, entity.transform.position) < 3f)
        {
            entity.SetState(new GlitchChaseState(10f));
        }
    }
    public void Exit()
    {

    }
}
