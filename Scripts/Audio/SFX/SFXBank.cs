using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(menuName = "Audio/SFX Bank")]
public class SFXBank : ScriptableObject
{
    public AudioClip[] clips;
    public AudioMixerGroup group;

    [Header("Volume")]
    public Vector2 volumeRange = new Vector2(0.9f, 1f);

    [Header("Pitch")]
    public Vector2 pitchRange = new Vector2(0.95f, 1.05f);
    public AudioClip GetClip()
    {
        return clips[Random.Range(0, clips.Length)];
    }
    public float GetVolume()
    {
        return Random.Range(volumeRange.x, volumeRange.y);
    }
    public float GetPitch()
    {
        return Random.Range(pitchRange.x, pitchRange.y);
    }
}
