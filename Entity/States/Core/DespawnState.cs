using UnityEngine;

public class DespawnState : IEntityState
{
    private Entity entity;
    public void Enter(Entity entity)
    {
        this.entity = entity;
        EntitiesManager.Instance.currEnemies--;
        GameObject.Destroy(entity.gameObject);
    }
    public void Update()
    {

    }
    public void Exit()
    {
        
    }
}
