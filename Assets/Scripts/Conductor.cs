using UnityEngine;

public class Conductor : MonoBehaviour
{
    public AudioSource musicSource;
    public AudioClip song;
    public float bpm = 140f;
    public float offset = 0.1f; 

    public float currentSongPosition { get; set; }
    public float crotchet { get; set; } //how long each beat is?

    public float dspSongStartTime; //unity internal timer for its audio system called dspTime not tied to your frame rate or Time.time for precise audio sync

    public void Setup(AudioClip song, float bpm, float startTime = 0f)
    {
        musicSource.Stop();
        musicSource.clip = song;
        crotchet = 60f / bpm;

        // Seek to specific time
        musicSource.time = startTime;

        dspSongStartTime = (float)AudioSettings.dspTime - startTime;

        Debug.Log($"Song started at DSP: {dspSongStartTime}, Start time: {startTime}");
        musicSource.Play();
    }

    void Update()
    {
        if (musicSource.isPlaying)
        {
            float raw = ((float)AudioSettings.dspTime - dspSongStartTime) * musicSource.pitch - offset;

            currentSongPosition = Mathf.Clamp(raw, 0f, musicSource.clip.length); //clamp raw to [0, clip.length] so songPosition never goes below 0 or beyond the track’s end

            //Debug.Log($"Song position: {songPosition}");
        }
    }
}