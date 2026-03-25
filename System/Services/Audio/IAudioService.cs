using UnityEngine;

public interface IAudioService : IService
{
    void PlaySound(AudioClip clip);
    void PlayMusic(AudioClip clip, bool loop = true);
    void StopMusic();
    float GetMusicVolume();
    float GetSoundVolume();
    void SetMusicVolume(float volume);
    void SetSoundVolume(float volume);
}
