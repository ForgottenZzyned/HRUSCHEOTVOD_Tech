using Cinemachine;
using UnityEngine;
using TMPro;
using JetBrains.Annotations;
public class PlayerManager : Singleton<PlayerManager>
{
    public GameObject playerPrefab;
    public GameObject playerObject;
    public SFXBank hitBank;
    private bool isDead;
    public bool isGovnoOtvod = false;
    public Transform govnoOtvodSpawnPos;
    private void Start()
    {
        isDead = false;
        if (isGovnoOtvod)
        {
            SpawnPlayer(govnoOtvodSpawnPos.position, Quaternion.identity);
        }
    }
    public float GetDist(Vector3 pos)
    {
        return Vector3.Distance(playerObject.transform.position, pos);
    }
    public void SpawnPlayer(Vector3 pos,Quaternion rot, Transform parent = null)
    {
        playerObject = Instantiate(playerPrefab,pos, rot, parent);
        DistanceCullingManager.Instance.RegisterPlayer(playerObject.transform);
        EndingManager.Instance.mainCamera = playerObject.
                                            GetComponentInChildren<CinemachineVirtualCamera>();
    }
    public void HitPlayer(float damageAmount, string cause = "null")
    {
        if (isDead) return;
        if (PlayerEffectsManager.Instance.isRessurection) return;
        HealthManager.Instance.DecreaseHealth(damageAmount,cause);
        if (PlayerEffectsManager.Instance.isRessurection) return;
        PlayerEffectsManager.Instance.TriggerVignette(0.2f,1f, Color.red, 0.7f);
        AudioEffects.Instance.PlayPanicEffect(1f);
        SFXManager.Instance.Play(hitBank,Vector3.zero,0,int.MaxValue);
    }
    public void DamagePlayerTo(float value)
    {
        float damageAmount = HealthManager.Instance.GetCurrHealth() - value;
        HitPlayer(damageAmount, "DamagedTo");
    }
    public void KillPlayer(string cause)
    {
        RessurectionSlot resurrSlot = SlotManager.Instance.IsContainingRessurection();
        if(resurrSlot != null)
        {
            resurrSlot.SlotUse();
            PlayerEffectsManager.Instance.TriggerRessurect();
            EntitiesManager.Instance.safeRoomCount += 1;
            HealthManager.Instance.SetCurrHealth(50);
            
            return;
        }
        isDead = true;
        PauseMenu.canPause = false;
        playerObject.GetComponent<PlayerMovement>().enabled = false;
        playerObject.GetComponentInChildren<CameraMovement>().enabled = false;
        AnalyticsManager.PlayerDied(RoomChainManager.Instance.currentRoom,cause);
        GameOverController.Instance.GameOver();
    }
    public bool IsPlayerAlive()
    {
        return !isDead;
    }
}
