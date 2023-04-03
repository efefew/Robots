using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// ???
/// </summary>
public class LevelNameCreator : MonoBehaviour
{
    public string Name;
    public int Id;
    string txt;
    public Text text;
    public GameObject WaitScene;
    public SaveArea saveArea;
    void Start()
    {
        if(PlayerPrefs.HasKey("✪MainCode✪"))
            saveArea.all = JsonUtility.FromJson<SaveAll>(PlayerPrefs.GetString("✪MainCode✪"));
        Id = transform.GetSiblingIndex();
        if (!saveArea.all.LevelsOpenTheme.Contains(Setting.themeLevelNow))
            Debug.LogError("нет темы уровней для того, чтобы узнать сколько открыто");
        if (saveArea.LevelOpen(Setting.themeLevelNow) < Id)
            GetComponent<Button>().interactable = false;
        txt = Id + "\n" + Name;
        transform.name = txt;
        text.text = txt;
    }
    //!!!
    public void SaveLoadLevel()
    {
        Setting.idLevelNow = Id;
        Setting.sceneLoad = "Game";
        WaitScene.SetActive(true);
    }
}
