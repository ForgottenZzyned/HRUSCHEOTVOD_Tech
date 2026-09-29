using UnityEngine;
public class Entity : MonoBehaviour
{
    [SerializeField]private IEntityState currentState;
    public Transform player;
    public Vector3 targetPosition;
    public EntityData data;
    public SFXBank entitySFXBank;
    public float timeAlive;
    public Material alwaysVisibleMat;
    public Material nonVisibleMat;
    public bool isVisible = false;

    private SpriteRenderer spriteRend;
    public void Initialize(EntityData data, Transform player)
    {
        this.player = player;
        this.data = data;
        data.behaviour.Initialize(this);
        EntitiesManager.Instance.currEnemies++;
        GetComponent<SpriteRenderer>().sprite = data.entityIcon;

        spriteRend = GetComponent<SpriteRenderer>();
        alwaysVisibleMat = new Material(alwaysVisibleMat);
        nonVisibleMat = new Material(nonVisibleMat);
    }
    public void SetState(IEntityState newState)
    {
        if (newState == null) return;
        currentState?.Exit();
        currentState = newState;
        if (!PlayerManager.Instance.IsPlayerAlive())     
            currentState = new DespawnState();  
        currentState.Enter(this);
    }
    public void SetTargetPosition(Vector3 pos)
    {
        targetPosition = pos;
    }
    public void SetPosition(Vector3 pos)
    {
        transform.position = pos;
    }
    public void SwitchVisibility()
    {
        isVisible = !isVisible;
        switch (isVisible)
        {
            case true:
                spriteRend.material = alwaysVisibleMat;
                break;
            case false:
                spriteRend.material = nonVisibleMat;
                break;
        }
        Debug.Log(spriteRend.material);
    }
    private void Update()
    {
        if (EndingManager.Instance.IsEnding() || PlayerEffectsManager.Instance.isRessurection || player == null)
        {
            SetState(new DespawnState());
            return;
        }
        currentState?.Update();
        Vector3 targetPos = player.position;
        targetPos.y = transform.position.y;
        transform.LookAt(targetPos);
    }
    private void FixedUpdate()
    {
        timeAlive += Time.deltaTime;
    }
    public void DoDamage()
    {
        PlayerManager.Instance.HitPlayer(data.damage,data.damageCause);
    }
}
