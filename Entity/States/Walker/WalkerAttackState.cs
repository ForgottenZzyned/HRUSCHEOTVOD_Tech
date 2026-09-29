using UnityEngine;
public class WalkerAttackState : IEntityState
{
    private Entity entity;
    private float speed = 100f;
    private string[] killStrings =
    {
        "null1.\n NULLIFIED",
        "null2.\n NULLIFIED",
        "null3.\n NULLIFIED",
        "null4.\n NULLIFIED"
    };
    public void Enter(Entity entity)
    {
        this.entity = entity;
        SFXBank sfx = Resources.Load<SFXBank>("Audio/Entities/Walker/Attacking/Attacking");
        SFXManager.Instance.Play(sfx,entity.transform.position,0.2f,int.MaxValue);
    }
    public void Update()
    {
        entity.transform.position = Vector3.MoveTowards(
            entity.transform.position,
            CameraMovement.Instance.gameObject.transform.position,
            speed * Time.deltaTime
            );
        if (Vector3.Distance(CameraMovement.Instance.gameObject.transform.position, entity.transform.position) < 1f)
        {
            GameOverController.Instance.killedString = killStrings[Random.Range(0, killStrings.Length)];
            PlayerManager.Instance.DamagePlayerTo(1f);
            PlayerEffectsManager.Instance.TriggerWalkerGlitch(0, 0, 1, 0.5f);
            entity.SetState(new DespawnState());
        }
    }
    public void Exit() { }
}
