using UnityEngine;

public class AudioService : MonoBehaviour, IAudioService, IInitializable
{
    [Header("Audio Sources")]
    public AudioSource effectSource;
    public AudioSource musicSource;

    [Header("Audio Clips")]
    public AudioClip dropClip;
    public AudioClip mergeClip;
    public AudioClip openingClip;
    public AudioClip inGameClip;
    public AudioClip gameOverClip;

    public void Initialize()
    {
        if (effectSource == null) effectSource = gameObject.AddComponent<AudioSource>();
        if (musicSource == null)  musicSource  = gameObject.AddComponent<AudioSource>();
        effectSource.volume = 0.5f;
        musicSource.volume  = 0.5f;
    }

    // ── IAudioService (generic) ───────────────────────────────────────────
    public void PlaySound(AudioClip clip) { if (clip && effectSource) effectSource.PlayOneShot(clip); }
    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (!clip || !musicSource) return;
        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.Play();
    }
    public void StopMusic() => musicSource.Stop();

    public float GetMusicVolume() => musicSource != null ? musicSource.volume : 0f;
    public float GetSoundVolume() => effectSource != null ? effectSource.volume : 0f;

    public void SetMusicVolume(float volume) { if (musicSource)  musicSource.volume  = volume; }
    public void SetSoundVolume(float volume) { if (effectSource) effectSource.volume = volume; }

    // ── Game-specific wrappers ────────────────────────────────────────────
    public void PlayDropSound()     => PlaySound(dropClip);
    public void PlayMergeSound()    => PlaySound(mergeClip);
    public void PlayOpeningMusic()  => PlayMusic(openingClip);
    public void PlayInGameMusic()   => PlayMusic(inGameClip);
    public void PlayGameOverMusic() => PlaySound(gameOverClip);
    public void StopGameMusic()     => StopMusic();
}
