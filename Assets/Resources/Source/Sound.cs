using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using static ProgramSettings;

public static class Sound
{
    //All sound effects loaded up into the memory
    public static Dictionary<string, AudioClip> sounds;

    //Plays a singular sound effect
    public static void PlaySound(string path, float volume = 0.7f)
    {
        if (soundsPlayedThisFrame.Contains(path)) return;
        if (!settings.soundEffects.Value()) return;
        if (!sounds.ContainsKey(path)) return;
        soundEffects.PlayOneShot(sounds[path], volume);
        soundsPlayedThisFrame.Add(path);
    }

    //Sound effect controller that plays sound effects
    public static AudioSource soundEffects;

    //Amount of falling element sounds played this frame
    //It is used to ensure that user's headphones are not
    //destroyed with a stacked sound of 47 falling sounds played at once
    public static List<string> soundsPlayedThisFrame;

    //Queued ambience to be played by the ambience controller
    public static (AudioClip, float, bool) queuedAmbience;
}
