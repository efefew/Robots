using UnityEngine;
using UnityEngine.UI;

public class MyMainMenu : MonoBehaviour
{
    [SerializeField]
    private Dropdown languageDropdown;
    [SerializeField]
    private Slider mainVolumeSlider, effectsVolumeSlider, musicVolumeSlider, interfaceVolumeSlider;
    [SerializeField]
    private Toggle particlesToggle, effectsUIToggle, showDamageToggle, visualCodeToggle;
    [SerializeField]
    private SaveArea save;
    [SerializeField]
    private SaveVolume volume;
    /// <summary>
    /// кнопка, чтобы играть выбраный уровень
    /// </summary>
    [SerializeField]
    private Button playChooseLevel;
    [SerializeField]
    private Button playChooseLearnLevel;

    private void Start() => save.LoadSetting(this);
    #region play
    /// <summary>
    /// при выборе уровня
    /// </summary>
    public void OnChooseLevel()
    {
        playChooseLevel.interactable = Setting.themeLevelNow != "";
        playChooseLearnLevel.interactable = Setting.themeLevelNow == "✪DevelopTheme✪";
    }
    /// <summary>
    /// при загрузке уровня
    /// </summary>
    public void OnLoadChooseLevel()
    {
        Setting.sceneLoad = "Game";
    }
    #endregion
    #region settiing
    public void OnChangeLanguage()
    {
        Language.language = (Language.LanguageType)languageDropdown.value;
    }
    public void OnChangeParticles(bool on)
    {
        Setting.particles = on;
    }
    public void OnChangeEffectsUI(bool on)
    {
        Setting.effectsUI = on;
    }
    public void OnChangeShowDamage(bool on)
    {
        Setting.showDamage = on;
    }
    public void OnChangeVisualCode(bool on)
    {
        Setting.visualCode = on;
    }
    /// <summary>
    /// установка сохранёных значений на интерфейс настроек
    /// </summary>
    public void SetSettingMenu()
    {
        languageDropdown.value = (int)Language.language;
        mainVolumeSlider.value = Setting.mainVolume;
        effectsVolumeSlider.value = Setting.effectsVolume;
        musicVolumeSlider.value = Setting.musicVolume;
        interfaceVolumeSlider.value = Setting.interfaceVolume;
        particlesToggle.isOn = Setting.particles;
        effectsUIToggle.isOn = Setting.effectsUI;
        showDamageToggle.isOn = Setting.showDamage;
        visualCodeToggle.isOn = Setting.visualCode;
        volume.SetSettingVolume();
    }
    /// <summary>
    /// Выход из игры
    /// </summary>
    #endregion
    #region quit
    public void Quit()
    {
        Application.Quit();
    }
    #endregion
}
