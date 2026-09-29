using UnityEngine;

public class GlitchChaseState : IEntityState
{
    private Entity entity;
    private float speed = 3f;
    private SFXBank attackSound;
    private string[] killStrings =
    {
        "This is Glitch.\nTry shining some light on him.",
        "Glitch? Again?\nYou can use your flashlight to scare him.",
    };
    public GlitchChaseState(float speed)
    {
        this.speed = speed;
    }
    public void Enter(Entity entity)
    {
        this.entity = entity;
        attackSound = Resources.Load<SFXBank>("Audio/Entities/Glitch/Attack/GlitchAttack");
        SFXManager.Instance.Play(attackSound,entity.transform.position);
        entity.SetPosition(PositionFinder.GetPosInfrontPlayer(CameraMovement.Instance.gameObject.transform, 3f));
    }
    public void Update()
    {
        entity.transform.position = Vector3.MoveTowards(
            entity.transform.position,
            CameraMovement.Instance.gameObject.transform.position,
            speed * Time.deltaTime
            );
        if (Vector3.Distance(entity.player.position, entity.transform.position) < 1f)
        {
            GameOverController.Instance.killedString = killStrings[Random.Range(0, killStrings.Length)];
            entity.SetState(new TriggeredState());
        }
    }
    public void Exit()
    {

    }
}
