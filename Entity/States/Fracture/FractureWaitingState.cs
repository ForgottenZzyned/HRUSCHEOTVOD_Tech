using UnityEngine;

public class FractureWaitingState : IEntityState
{
    private Entity entity;
    private float maxWaitTime = 10f;
    private float waitTimer;
    private bool isWarned = false;
    private string[] killStrings =
    {
        "This is Fracture.\n He's very shy of his eyes, just look at him at minimal distance.",
        "You've run out of time.\n Fracture is looking in your soul from outside, look back at it.",
        "Frecture.\n Try to scare him by looking in his eyes."
    };
    private SFXBank sfxKnock;
    public void Enter(Entity entity)
    {
        this.entity = entity;
        waitTimer = 0f;
        sfxKnock = Resources.Load<SFXBank>("Audio/Entities/Fracture/WindowKnock/WindowKnock");
        SFXManager.Instance.Play(sfxKnock,entity.transform.position);
    }
    public void Update()
    {
        waitTimer += Time.deltaTime;
        if (PositionFinder.IsVisibleToPlayerDot(entity.player, entity.transform,0.4f)
            && Vector3.Distance(entity.player.position, entity.transform.position) <= 6f)
        {
            entity.SetState(new FractureDetectedState());
            return;
        }
        if (maxWaitTime-1.5f < waitTimer && !isWarned)
        {
            isWarned = true;
            sfxKnock = Resources.Load<SFXBank>("Audio/Entities/Fracture/WindowKnock/WindowKnock2");
            SFXManager.Instance.Play(sfxKnock, entity.transform.position,0.3f);
            PlayerEffectsManager.Instance.TriggerGlitch(0.05f, 0.05f, -0.4f, 1.5f, 0.2f);
        }
        if (maxWaitTime > waitTimer || PauseMenu.IsPaused)
            return;
        GameOverController.Instance.killedString = killStrings[Random.Range(0, killStrings.Length)];
        PlayerEffectsManager.Instance.TriggerFractureScream();
        entity.SetState(new DespawnState());
    }
    public void Exit()
    {

    }
}