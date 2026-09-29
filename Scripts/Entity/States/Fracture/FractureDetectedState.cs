using UnityEngine;

public class FractureDetectedState : IEntityState
{
    private Entity entity;
    private float waitTime = 2f;
    private float alwaysVisibleTimer = 1f;
    private bool isSwitched = false;
    private float timer;
    public void Enter(Entity entity)
    {
        this.entity = entity;
        timer = 0f;
    }
    public void Update()
    {
        
        timer += Time.deltaTime;
        if (timer > alwaysVisibleTimer && !isSwitched)
        {
            isSwitched = true;
            entity.SwitchVisibility();
        }
        if (timer > waitTime)
        {
            PlayerEffectsManager.Instance.TriggerGlitch(0.05f, 0.05f, 1f, 0.5f, 0.5f);
            entity.SetState(new DespawnState());
        }
    }
    public void Exit()
    {

    }
}