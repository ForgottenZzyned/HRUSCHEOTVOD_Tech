using UnityEngine;
public class DetectedState : IEntityState
{
    private Entity entity;
    private float timer = 0f;
    private float minWaitTime = 1f;
    private float maxWaitTime = 4f;

    private string[] killStrings =
    {
        "Dried Watcher.\nYou need to look at him and do not do anything after this.",
        "Dried Watcher dried you out!\nTry to not move after you saw him.",
        "You died from Dried Watcher.\nHe's searching for alive khrusches, so you need to fake that you are dead khrusch"
    };
    public void Enter(Entity entity)
    {
        this.entity = entity;
        SFXBank sfx = Resources.Load<SFXBank>("Audio/Entities/Watcher/Detected/WatcherDetected");
        SFXManager.Instance.Play(sfx, entity.transform.position, 0);
        PlayerEffectsManager.Instance.TriggerVignette(0.4f,5f, Color.black,0.3f);
        PlayerEffectsManager.Instance.TriggerNotMoveImage();
    }
    private bool IsAnyInput()
    {
        if (Input.anyKey)
            return true;
        if (Input.GetAxis("Mouse X") != 0f || Input.GetAxis("Mouse Y") != 0)
            return true;
        return false;
    }
    public void Update()
    {
        timer += Time.deltaTime;
        if (timer < minWaitTime) return;
        if (IsAnyInput()) 
        {
            GameOverController.Instance.killedString = killStrings[Random.Range(0, killStrings.Length)];
            entity.SetState(new TriggeredState());
        }
        if (timer > maxWaitTime)
        {
            PlayerEffectsManager.Instance.TriggerGlitch(0,0.05f,1,0.1f);
            entity.SetState(new DespawnState());
        }  
    }
    public void Exit() { }
}
