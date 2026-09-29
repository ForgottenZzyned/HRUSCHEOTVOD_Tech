using UnityEngine;
public enum SpawnType
{
    OnDoorOpen,
    Timer,
    Both
}
[CreateAssetMenu(menuName = "Game/Entity Data")]
public class EntityData : ScriptableObject
{
    public Sprite entityIcon;
    public string id;
    public EntityBehaviour behaviour;
    public GameObject prefab;
    public float damage;
    public string damageCause;
    public float spawnWeight;
    public int spawnThreshold;
    public Vector2 spawnDelay;
    public SpawnType spawnType;
}
