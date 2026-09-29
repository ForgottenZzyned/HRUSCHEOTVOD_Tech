using UnityEngine;
public class GlitchDespawnState : IEntityState
{
    private Entity entity;
    private SFXBank despawnSound;
    public void Enter(Entity entity)
    {
        this.entity = entity;
        EntitiesManager.Instance.currEnemies--;
        despawnSound = Resources.Load<SFXBank>("Audio/Entities/Glitch/Despawn/GlitchDespawn");
        PlayerEffectsManager.Instance.TriggerGlitch(0.05f, 0.05f, -0.1f, 1f, 0.2f);
        SFXManager.Instance.Play(despawnSound,entity.transform.position);
        EnergyManager.Instance.AddToCurrEnergy(-(EnergyManager.Instance.GetEnergy()/5f));
        Flashlight.Instance.ForceFlickerFlashlight();
        GameObject.Destroy(entity.gameObject);
    }
    public void Update() { }
    public void Exit() { }
}
