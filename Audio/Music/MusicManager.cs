using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Audio;
[System.Serializable]
public class MusicAgeStage
{
    public int roomThreshold;
    public MusicAge age;
}
public class MusicManager : Singleton<MusicManager>
{
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioMixerGroup musicMixerGroup;
    public List<MusicProfile> musicList = new();
    public MusicAgeStage[] stages;
    public float musicVolume = 0.2f;
    [Range(0, 1)]
    public float musicChance;
    public float minWaitTime;
    public float maxWaitTime;
    private Coroutine musicCoroutine;
    [SerializeField] private float lastMusicPlayedTime = 0f;
    private void Start()
    {
        StartCoroutine(RandomMusicLoop());
    }
    private IEnumerator RandomMusicLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            if (!PlayerManager.Instance.IsPlayerAlive())
                break;
            if (CheckChance())
            {
                PlayRandomMusic();
            }
        }
    }
    public void PlayRandomMusic()
    {
        ForceStopMusic(musicSource);
        lastMusicPlayedTime = Time.time;
        AudioClip clip = GetRandomMusicClip();
        musicCoroutine = StartCoroutine(PlayMusic(clip,musicVolume,musicSource));
        Debug.Log($"Playing {clip.name}");
    }
    public void ForcePlayMusic(AudioClip music,float timing = 0f, AudioSource customSource = null)
    {
        if(musicCoroutine != null) StopCoroutine(musicCoroutine);
        ForceStopMusic(/*customSource == null ?*/ musicSource /*: customSource*/);
        lastMusicPlayedTime = Time.time;
        musicCoroutine = StartCoroutine(PlayMusic(music, musicVolume, customSource == null ? musicSource : customSource,timing));
        Debug.Log($"Playing {music.name}");
    }
    private AudioClip GetRandomMusicClip()
    {
        var music = musicList[Random.Range(0, musicList.Count)];
        return music.GetAgedVersion(GetAge());
    }
    private MusicAge GetAge()
    {
        int curr = RoomChainManager.Instance.currentRoom;

        for (int i = stages.Length - 1; i >= 0; i--)
        {
            if (curr >= stages[i].roomThreshold)
                return stages[i].age;
        }

        return MusicAge.Basic;
    }
    private bool CheckChance()
    {
        float normalized = 0f;
        if (Time.time > minWaitTime + lastMusicPlayedTime)
        {
            normalized = Mathf.InverseLerp(minWaitTime + lastMusicPlayedTime, maxWaitTime + lastMusicPlayedTime, Time.time);
            if ((musicChance * normalized) > Random.value) return true;
        }
        return false;
    }
    private IEnumerator PlayMusic(AudioClip clip, float targetVolume, AudioSource source, float timing = 0f)
    {
        source.spatialBlend = 0f;
        source.clip = clip;
        source.time = timing;
        source.volume = 0;
        source.Play();
        source.outputAudioMixerGroup = musicMixerGroup;
        source.DOFade(targetVolume, 10f);
        yield return new WaitForSeconds(clip.length);
        source.DOFade(0f, 2f)
            .OnComplete(() =>
            {
                source.Stop();
            })
            .OnKill(()=>
            {
                source.Stop();
            });
        source.Stop();
    }
    public void ForceStopMusic(AudioSource source = null)
    {
        if (source == null) source = musicSource;
        source.volume = 0;
        source.Stop();
    }
}
