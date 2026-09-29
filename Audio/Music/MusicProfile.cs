using UnityEngine;
public enum MusicAge
{
    Basic,
    Old1,
    Old2,
    Old3,
    Oldest
}
[CreateAssetMenu(menuName = "Audio/Music")]
public class MusicProfile : ScriptableObject
{
    public AudioClip[] versions;
    public AudioClip GetAgedVersion(MusicAge age)
    {
        return versions[(int)age];
    }
}
