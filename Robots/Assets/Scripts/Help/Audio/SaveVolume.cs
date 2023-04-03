using UnityEngine;
using UnityEngine.Audio;

public class SaveVolume : MonoBehaviour
{
    public float minVolume = -20;
    private AudioMixer audioMixer;
    private void Start()
    {
        audioMixer = (AudioMixer)Resources.Load("Audio/AudioMixer");
        SetSettingVolume();
    }

    /// <summary>
    /// Конвертация еденицы измерения громкости
    /// </summary>
    /// <param name="volume">громкость</param>
    void ConvertVolume(ref float volume)
    {
        volume = volume * 2.5f - 20;
        if (volume <= minVolume)
            volume = -80;
    }
    /// <summary>
    /// Функция для управления мастер-громкостью
    /// </summary>
    /// <param name="volume">громкость</param>
    public void SetMasterVolume(float volume)
    {
        Setting.mainVolume = volume;
        ConvertVolume(ref volume);
        audioMixer.SetFloat("masterVolume", volume);
    }
    /// <summary>
    /// Функция управления громкостью звуковых эффектов
    /// </summary>
    /// <param name="volume">громкость</param>
    public void SetEffectsVolume(float volume)
    {
        Setting.effectsVolume = volume;
        ConvertVolume(ref volume);
        audioMixer.SetFloat("effectsVolume", volume);
    }
    /// <summary>
    /// Функция управления громкостью фоновой музыки
    /// </summary>
    /// <param name="volume">громкость</param>
    public void SetMusicVolume(float volume)
    {
        Setting.musicVolume = volume;
        ConvertVolume(ref volume);
        audioMixer.SetFloat("musicVolume", volume);
    }
    /// <summary>
    /// Функция управления громкостью звуков интерфейса
    /// </summary>
    /// <param name="volume">громкость</param>
    public void SetInterfaceVolume(float volume)
    {
        Setting.interfaceVolume = volume;
        ConvertVolume(ref volume);
        audioMixer.SetFloat("interfaceVolume", volume);
    }
    public void SetSettingVolume()
    {
        SetMasterVolume(Setting.mainVolume);
        SetEffectsVolume(Setting.effectsVolume);
        SetMusicVolume(Setting.musicVolume);
        SetInterfaceVolume(Setting.interfaceVolume);
    }
}
