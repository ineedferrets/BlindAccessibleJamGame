using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD;

    public class MusicManager : MonoBehaviour
{
    private FMOD.Studio.EventInstance eventInstance;
    public FMODUnity.EventReference fmodEvent;
    public static MusicManager Instance;

    // Start is called before the first frame update
    void Start()
    {
        eventInstance = FMODUnity.RuntimeManager.CreateInstance(fmodEvent);
        eventInstance.start();
    }

    public void SetNewMusicTrack(string newMusicTrack)
    {
        eventInstance.setParameterByNameWithLabel("music_track", newMusicTrack);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
}
