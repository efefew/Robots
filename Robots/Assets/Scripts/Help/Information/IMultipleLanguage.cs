public static class Language
{
    public const int countLanguage = 2;
    public enum LanguageType
    {
        english,
        russian
    }
    private static LanguageType languageValue;
    public static LanguageType language
    {
        get
        {
            return languageValue;    // возвращаем значение свойства
        }
        set
        {
            OnChangeLanguage(value);
            languageValue = value;   // устанавливаем новое значение свойства
        }
    }
    public delegate void DelegateChangeLanguage(LanguageType language);
    public static event DelegateChangeLanguage eventChangeLanguage;

    /// <summary>
    /// При изменении языка
    /// </summary>
    private static void OnChangeLanguage(LanguageType newLanguage)
    {
        if (newLanguage == language)
            return;
        eventChangeLanguage?.Invoke(newLanguage);
    }
}
/// <summary>
/// у меня несколько языков
/// </summary>
public interface IMultipleLanguage
{
    /// <summary>
    /// При изменении языка
    /// </summary>
    /// <param name="language">выбранный язык</param>
    void OnChangeLanguage(Language.LanguageType language);
}