using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;

public class SFXManager : Singleton<SFXManager>
{
    [Header("Pool")]
    public int poolSize = 20;

    private AudioSource[] sources;
    private int currentSource;

    private void OnEnable()
    {
        sources = new AudioSource[poolSize];

        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = new GameObject("Audio_" + i);
            obj.transform.parent = transform;
            AudioSource source = obj.AddComponent<AudioSource>();
            obj.AddComponent<AudioLowPassFilter>();
            obj.GetComponent<AudioLowPassFilter>().cutoffFrequency = 22000;
            source.spatialBlend = 1f;

            sources[i] = source;
        }
    }
    public void Play(SFXBank bank, Vector3 position, float spatialBlend = 1f, float minDist = 50f, bool ignoreScreamer = false)
    {
        if (!PlayerManager.Instance.IsPlayerAlive()) return;
        if (!ignoreScreamer && PlayerEffectsManager.isScreamer) return;
        if (PlayerManager.Instance.GetDist(position) > minDist) return;
        AudioSource source = sources[currentSource];
        source.transform.position = position;
        source.clip = bank.GetClip();
        source.spatialBlend = spatialBlend;
        source.pitch = bank.GetPitch();
        source.volume = bank.GetVolume();
        source.outputAudioMixerGroup = bank.group;
        source.Play();
        currentSource = (currentSource + 1) % poolSize;
    }
    public void PlayScream(SFXBank bank, Vector3 position, float spatialBlend = 1f)
    {
        AudioSource source = sources[currentSource];
        ApplyLowPass(source, PlayerManager.Instance.playerObject.transform);
        source.transform.position = position;
        source.clip = bank.GetClip();
        source.spatialBlend = spatialBlend;
        source.pitch = bank.GetPitch();
        source.volume = bank.GetVolume();
        source.outputAudioMixerGroup = bank.group;
        source.Play();
        currentSource = (currentSource + 1) % poolSize;
    }
    private static void ApplyLowPass(AudioSource source, Transform listener)
    {
        Vector3 dir = (source.transform.position -listener.position).normalized;
        float distance = Vector3.Distance(listener.position,source.transform.position);
        int mask = ~LayerMask.GetMask("IgnoreRaycast");
        if (Physics.Raycast(listener.position, dir, out RaycastHit hit, distance, mask, QueryTriggerInteraction.Ignore))
        {
            source.volume *= 1f;
            source.GetComponent<AudioLowPassFilter>().cutoffFrequency = 22000;
        }
        else
        {
            source.volume *= 1f;
            source.GetComponent<AudioLowPassFilter>().cutoffFrequency = 22000;
        }
    }

    public void InitializeLoop(SFXBank bank, AudioSource source, float spatialBlend = 1f)
    {
        source.clip = bank.GetClip();
        source.spatialBlend = spatialBlend;
        source.pitch = bank.GetPitch();
        source.volume = bank.GetVolume();
        source.outputAudioMixerGroup = bank.group;
        source.loop = true;
        source.Play();
    }
}
