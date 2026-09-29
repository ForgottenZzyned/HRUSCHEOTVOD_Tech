using UnityEngine;

public class FindSuitableWindowState : IEntityState
{
    private Entity entity;
    private float checkTimer = 0.25f;
    float lastChecked;
    public void Enter(Entity entity)
    {
        this.entity = entity;
        entity.SetPosition(Vector3.zero + (Vector3.up*100));
    }
    public void Update()
    {
        if(entity.timeAlive > 30f) entity.SetState(new DespawnState());
        if (lastChecked + checkTimer < Time.time)
        {
            Vector3 pos = Vector3.zero;
            WindowPoint window = null;
            if (EntitiesManager.Instance.windowPoints.Count > 0)
            {
                window = PositionFinder.FindBestWindowPoint(EntitiesManager.Instance.windowPoints, entity.player, 10f);
                if (window == null) return;
                pos = window.transform.position;
            }
                
            if (pos != Vector3.zero && window.GetComponentInParent<Room>().playerInside)
            {
                window.isOccupied = true;
                entity.SetPosition(pos);
                entity.SetState(new FractureWaitingState());
            }
        }
    }
    public void Exit()
    {

    }
}
