using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Audio/Room Audio Profile")]
public class RoomAudioProfile : ScriptableObject
{
    [Header("Ambient")]
    public SFXBank ambientLoop;

    [Header("Random sounds")]
    public SFXBank[] randomSounds;

    public Vector2 randomDelay = new Vector2(5f, 20f);
}
