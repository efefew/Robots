using UnityEngine;
using UnityEngine.UI;

public class ChooseLevel : MonoBehaviour
{
    public SaveArea saveArea;
    public MyMainMenu menu;
    /// <summary>
    /// тема уровней
    /// </summary>
    public string themeLevel;
    /// <summary>
    /// номер уровня
    /// </summary>
    public int idLevel;

    private void Start()
    {
        if (!saveArea.all.LevelsOpenTheme.Contains(themeLevel))
            Debug.LogError("нет темы уровней для того, чтобы узнать сколько открыто");
        else
            GetComponent<Toggle>().interactable = saveArea.LevelOpen(themeLevel) >= idLevel;
    }
    /// <summary>
    /// при выборе уровня
    /// </summary>
    /// <param name="on">выбран?</param>
    public void OnChooseLevel(bool on)
    {
        if (on)
        {
            Setting.themeLevelNow = themeLevel;
            Setting.idLevelNow = idLevel;
        }
        else
            if(Setting.themeLevelNow == themeLevel && Setting.idLevelNow == idLevel)
                Setting.themeLevelNow = "";
        menu.OnChooseLevel();
    }
}
