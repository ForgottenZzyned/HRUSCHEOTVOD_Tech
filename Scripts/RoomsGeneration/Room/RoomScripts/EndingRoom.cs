using UnityEngine;

public class EndingRoom : Room
{
    public Transform endingWindow;
    public AudioClip endingMusic;
    public override void ActivateRoom()
    {
        base.ActivateRoom();
    }
    public override void SetLightsOff()
    {
        return;
    }
    public void StartCutscene()
    {
        PauseMenu.canPause = false;
        RoomChainManager.Instance.CloseAllRooms();
        MusicManager.Instance.ForceStopMusic();
        MusicManager.Instance.ForcePlayMusic(endingMusic, 70f, EndingManager.Instance.audioSource);
        EndingManager.Instance.window = endingWindow;
    }
}
