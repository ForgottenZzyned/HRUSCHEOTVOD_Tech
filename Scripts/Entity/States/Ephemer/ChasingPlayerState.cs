using DG.Tweening;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Unity.VisualScripting.FullSerializer;
public class ChasingPlayerState : IEntityState
{
    private Entity entity;
    private float speed = 3f;
    private bool soundActivated = false;
    private AudioSource source;
    private string[] killStrings =
    {
        "Another Shekastiy?\nTry to run from him in next room as soon as possible.",
        "Shekastiy.\nHe's lonely and trying to find friends, you just need to leave him in previous room.",
        "Sudden Shekastiy, isn't it?\nJust run forward to next room."
    };
    public ChasingPlayerState(float speed = 3f)
    {
        this.speed = speed;
    }
    public void Enter(Entity entity)
    {
        this.entity = entity;
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(1f)
            .OnComplete(() =>
            {
                RoomChainManager.Instance.BreakAllAvaibleLights();
            });
        Debug.Log($"[ENTITY] OnDoorOpened SUBSCRIBE | {entity.gameObject}");
        GameEvents.OnDoorOpened += HandleDoorOpened;
        EnergyManager.Instance.SetEnergyDrain(12f);
        Debug.Log(
                $"[ENTITY] Creating AudioSource | " +
                $"entity={entity} | " +
                $"entityGO={entity?.gameObject} | " +
                $"instanceID={entity?.gameObject?.GetInstanceID()}"
            );
        source = entity.gameObject.AddComponent<AudioSource>();
        Debug.Log(
            $"[ENTITY] AudioSource result | " +
            $"source={source} | " +
            $"entityGO={entity?.gameObject}"
        );
        //if (source == null) return;
        SFXManager.Instance.InitializeLoop(Resources.Load<SFXBank>("Audio/Entities/Ephemer/Chasing/EphemerChasing"), source, 0.8f);
        source.volume = 0;
        source.DOFade(source.volume + 0.2f,0.5f).SetEase(Ease.OutQuad);
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
            PlayerEffectsManager.Instance.TriggerEphemerScream();
            entity.SetState(new DespawnState());
        }
        if (!soundActivated && Vector3.Distance(CameraMovement.Instance.gameObject.transform.position, entity.transform.position) < 20f)
        {
            if (source == null) return;
            DOTween.To(
                () => source.spatialBlend,
                x => source.spatialBlend = x,
                0f,
                5f
            ).SetEase(Ease.Linear);
            source.DOFade(source.volume+0.6f,6f);
        }
    }
    public void HandleDoorOpened(Door door)
    {
        Debug.Log(
            $"[ENTITY] HandleDoorOpened | " +
            $"entityName={entity.data.name} | " +
            $"entity={entity} | " +
            $"door={door}"
        );
        PlayerEffectsManager.Instance.TriggerGlitch(0, 0.05f, 1, 0.1f);
        entity.SetState(new DespawnState());
    }
    public void Exit()
    {
        Debug.Log($"[ENTITY] Exit START | entity={entity} | gameObject={entity?.gameObject} | source={source}");
        /*if (source != null)*/ source.enabled = false;
        GameEvents.OnDoorOpened -= HandleDoorOpened;
        EnergyManager.Instance.SetEnergyDrain();
        Debug.Log("[ENTITY] Exit END");
    }
}
