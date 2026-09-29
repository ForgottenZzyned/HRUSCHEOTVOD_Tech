using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomAudioController : MonoBehaviour
{
    public RoomAudioProfile profile;
    private Coroutine randomLoop;
    private Room roomScript;
    private void Awake()
    {
        roomScript = GetComponent<Room>();
    }
    private void Start()
    {
        ActivateRoom();
    }
    public void ActivateRoom()
    {
        if (profile.ambientLoop != null)
        {
            //SFXManager.Instance.InitializeLoop(profile.ambientLoop, gameObject.AddComponent<AudioSource>());
        }

        if (profile.randomSounds.Length > 0)
        {
            randomLoop = StartCoroutine(SetRandomSounds());
        }
    }

    private IEnumerator SetRandomSounds()
    {
        while (true)
        {
            float delay = Random.Range(profile.randomDelay.x, profile.randomDelay.y);
            yield return new WaitForSeconds(delay);
            if (roomScript.playerInside)
            {
                SFXBank sound = profile.randomSounds[Random.Range(0, profile.randomSounds.Length)];
                Vector3 pos = roomScript.GetRandomPointInRoom();
                SFXManager.Instance.Play(sound, pos);
            }
            else
            {
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
    
}
