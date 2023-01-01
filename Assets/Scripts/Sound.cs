using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound 
{
    public string name; // The name of the sound.
    public AudioClip clip; // The clip of the sound.
    [Range(0f, 1f)]
    public float volume; // The volume of the sound (0-1).
    [Range(.1f, 3f)]
    public float pitch = 1f; // The pitch of the sound (.1-3).
    public bool loop; // Should the sound loop?
    public bool backgroundMusic; // Is the sound background music?

    [HideInInspector]
    public AudioSource source; // The AudioSource component of the sound. (This is set in the AudioManager script)
}