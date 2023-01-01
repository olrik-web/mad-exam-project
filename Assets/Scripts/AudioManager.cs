using UnityEngine;
using UnityEngine.Audio;
using System;
using System.Collections;

public class AudioManager : Singleton<AudioManager>
{
    public Sound[] sounds; // Array of sounds. 
    public static AudioManager instance; // Instance of the AudioManager.
    private Sound currentMusic; // The current background music.

    // Start is called before the first frame update
    void Start()
    {
        // Loop through all sounds and add AudioSource component to each
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    // Play random background music.
    public void PlayRandomBackgroundMusic()
    {
        // Get all sounds which have the backgroundMusic bool set to true.
        Sound[] backgroundMusicSounds = Array.FindAll(sounds, sound => sound.backgroundMusic == true);
        // Select a random sound from the backgroundMusicSounds array.
        Sound randomBackgroundMusicSound = backgroundMusicSounds[UnityEngine.Random.Range(0, backgroundMusicSounds.Length)];
        // Play the random sound.
        FadeIn(randomBackgroundMusicSound.name);
        // Set the currentMusic variable to the random sound.
        currentMusic = randomBackgroundMusicSound;
    }

    public void StopBackgroundMusic()
    {
        Stop(currentMusic.name);
    }

    public void StopBackgroundMusicFadeOut()
    {
        FadeOut(currentMusic.name);
    }

    // Play a sound by name.
    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        s.source.Play();
    }

    // Stop a sound by name.
    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        s.source.Stop();
    }

    public void FadeOut(string name, float FadeTime = 1f)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        StartCoroutine(FadeOut(s.source, FadeTime));
    }

    public void FadeIn(string name, float FadeTime = 1f)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        StartCoroutine(FadeIn(s.source, FadeTime));
    }

    // Coroutine to fade out a sound. 
    public IEnumerator FadeOut(AudioSource audioSource, float FadeTime)
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.fixedDeltaTime / FadeTime;
            yield return null;
        }
        audioSource.Stop();
        audioSource.volume = startVolume;
    }

    // Coroutine to fade in a sound.
    public IEnumerator FadeIn(AudioSource audioSource, float FadeTime)
    {
        float startVolume = audioSource.volume;

        audioSource.Play();
        audioSource.volume = 0f;
        // Fade in the sound.
        while (audioSource.volume < startVolume)
        {
            audioSource.volume += startVolume * Time.fixedDeltaTime / FadeTime;

            yield return null;
        }
    }
}
