using System;
using System.Collections.Generic;

using UnityEngine;
public static class ResourcesStat
{
    public const int countResource = 3;
    private static float[] massResource = new float[] { 0.1f, 0.5f, 0.6f };
    private static string[] nameResource = new string[] { "alpha", "beta", "gamma" };
    public static float GetMass(int id)
    {
        if (id < 0 || id >= countResource)
            return 0;
        return massResource[id];
    }
    public static string GetName(int id)
    {
        if (id < 0 || id >= countResource)
            return "";
        return nameResource[id];
    }
}
public static class Info
{
    public static string TextCodeStart, TextInterfaceStart, TextCodeEnd, TextInterfaceEnd, MyText;
    public static bool Variable, LogicCompare, IfOrFor, BeLeftBrace, ReadVariable;
}
public static class InfoCon
{
    public static bool BeConstructor;
    public static bool BeDelete;
    public static bool BeController;
    public delegate void CheckConteiners();
    public static CheckConteiners checkConteiners;
}
public static class Setting
{
    public const int countLanguage = 2;
    public static string sceneLoad;
    public static float mainVolume, effectsVolume, musicVolume, interfaceVolume;
    public static bool particles, effectsUI;
    public static bool showDamage, visualCode;
    /// <summary>
    /// тема уровней
    /// </summary>
    public static string themeLevelNow;
    public static int idLevelNow;
}
public class SaveArea : MonoBehaviour
{

    public ScriptSave SaverScript = new ScriptSave();
    public RobotSave SaverRobot = new RobotSave();
    public SaveAll all = new SaveAll();
    public SaveSetting setting = new SaveSetting();
    public SaveThemes themeLevels = new SaveThemes();
    public SaveTheme levels = new SaveTheme();
    public SaveLevel currentLevel = new SaveLevel();
    /// <summary>
    /// все существующие детали
    /// </summary>
    public DetailObject[] MyDetails;

    private void Awake()
    {
        if (PlayerPrefs.HasKey("✪MainCode✪"))
            all = JsonUtility.FromJson<SaveAll>(PlayerPrefs.GetString("✪MainCode✪"));
        CheckSaveAll();
        LoadSetting();
    }
    [ContextMenu("Delete all save")]
    public void DeleteAllSave() => PlayerPrefs.DeleteAll();

    private void CheckSaveAll()
    {
        if (all.RobotNames == null)
            all.RobotNames = new List<string>();
        if (all.ScriptsNames == null)
            all.ScriptsNames = new List<string>();
        if (all.needResourcesForRobots == null)
            all.needResourcesForRobots = new List<SaveNeedResources>();
        if (all.LevelsOpenTheme.Count == 0 || all.LevelsOpenId.Count == 0 || all.LevelsMax.Count == 0 || !all.LevelsOpenTheme.Contains("✪DevelopTheme✪"))
        {
            all.LevelsOpenTheme = new List<string>();
            all.LevelsOpenId = new List<int>();
            all.LevelsMax = new List<int>();
            all.LevelsOpenTheme.Add("✪DevelopTheme✪");
            all.LevelsOpenId.Add(0);
            all.LevelsMax.Add(2);
        }
        Save();
    }
    public void Save() => PlayerPrefs.SetString("✪MainCode✪", JsonUtility.ToJson(all));
    /// <summary>
    /// находит индекс последнего открытого уровня в выбранной теме уровня
    /// </summary>
    /// <param name="theme">выбранная тема уровня</param>
    /// <returns>индекс последнего открытого уровня в выбранной теме уровня</returns>
    public int LevelOpen(string theme) => all.LevelsOpenId[all.LevelsOpenTheme.IndexOf(theme)];

    public void SaveRobot(string NameRobot, List<GameObject> AllDetails, GameObject[] AllParentDetails, int[] AllIdDetails, bool[] Orientations, float[] needResources, SaveScriptInComputers[] saveScripts, string[] Keys, string typeKeys, string saveLine)
    {
        DeleteRobot(NameRobot);
        SaverRobot.orientations = Orientations;
        SaverRobot.Keys = Keys;
        SaverRobot.typeKeys = typeKeys;
        SaverRobot.saveLine = saveLine;

        int count = AllDetails.Count - 1;
        SaverRobot.IdDetails = AllIdDetails;
        SaverRobot.saveScripts = saveScripts;
        SaverRobot.AngleDetails = new float[count];
        SaverRobot.PositionDetails = new Vector2[count];
        SaverRobot.ParentDetails = new int[count];
        SaverRobot.IndexDetails = new int[count];
        for (int i = 0; i < AllDetails.Count - 1; i++)
        {
            SaverRobot.AngleDetails[i] = AllDetails[i + 1].transform.eulerAngles.z;
            SaverRobot.PositionDetails[i] = AllDetails[i + 1].transform.localPosition;
            SaverRobot.ParentDetails[i] = AllDetails.IndexOf(AllParentDetails[i]);
            SaverRobot.IndexDetails[i] = AllDetails[i + 1].GetComponent<DetailObject>().IndexDetail;
        }

        all.needResourcesForRobots.Add(new SaveNeedResources(needResources));
        all.RobotNames.Add(NameRobot);
        PlayerPrefs.SetString(NameRobot, JsonUtility.ToJson(SaverRobot));
        Save();
    }
    public string[][][][] LoadRobot(string NameRobot)
    {
        string[][][][] propertyParts;
        SaverRobot = JsonUtility.FromJson<RobotSave>(PlayerPrefs.GetString(NameRobot));
        propertyParts = LoadConstructor(SaverRobot.saveLine);
        return propertyParts;
    }
    public void DeleteRobot(string NameRobot)
    {
        if (!all.RobotNames.Contains(NameRobot))
            return;
        PlayerPrefs.DeleteKey(NameRobot);
        int id = all.RobotNames.IndexOf(NameRobot);
        all.RobotNames.RemoveAt(id);
        all.needResourcesForRobots.RemoveAt(id);
        Save();
    }

    public void SaveScript(string NameCode, string[] RealCode, string[] InterfaceCode)
    {
        DeleteScript(NameCode);
        SaverScript.InterfaceCode = InterfaceCode;
        SaverScript.RealCode = RealCode;
        all.ScriptsNames.Add(NameCode);
        PlayerPrefs.SetString(NameCode, JsonUtility.ToJson(SaverScript));
        Save();
    }
    public void LoadScript(string NameCode, ref List<string> RealCode, ref List<string> InterfaceCode)
    {
        if (!PlayerPrefs.HasKey(NameCode))
            return;
        SaverScript = JsonUtility.FromJson<ScriptSave>(PlayerPrefs.GetString(NameCode));
        RealCode.Clear();
        InterfaceCode.Clear();
        RealCode.AddRange(SaverScript.RealCode);
        InterfaceCode.AddRange(SaverScript.InterfaceCode);
    }
    public void LoadScript(string NameCode, ref List<string> RealCode)
    {
        if (!PlayerPrefs.HasKey(NameCode))
            return;
        SaverScript = JsonUtility.FromJson<ScriptSave>(PlayerPrefs.GetString(NameCode));
        RealCode.Clear();
        RealCode.AddRange(SaverScript.RealCode);
    }
    public void DeleteScript(string NameCode)
    {
        if (!all.ScriptsNames.Contains(NameCode))
            return;
        PlayerPrefs.DeleteKey(NameCode);
        all.ScriptsNames.Remove(NameCode);
        Save();
    }

    public string[][][][] LoadConstructor(string saveLine)
    {
        string[] keyParts = saveLine.Split('|');

        string[][] dNameParts = new string[keyParts.Length][];
        for (int key = 0; key < keyParts.Length; key++)
            dNameParts[key] = keyParts[key].Split('$');

        string[][][] dIdParts = new string[keyParts.Length][][];
        for (int key = 0; key < keyParts.Length; key++)
        {
            dIdParts[key] = new string[dNameParts[key].Length][];
            for (int NameD = 0; NameD < dNameParts[key].Length; NameD++)
                dIdParts[key][NameD] = dNameParts[key][NameD].Split(' ');
        }

        string[][][][] rez = new string[keyParts.Length][][][];
        for (int key = 0; key < keyParts.Length; key++)
        {
            rez[key] = new string[dNameParts[key].Length][][];
            for (int NameD = 0; NameD < dNameParts[key].Length; NameD++)
            {
                rez[key][NameD] = new string[dIdParts[key][NameD].Length][];
                for (int idD = 0; idD < dIdParts[key][NameD].Length; idD++)
                    rez[key][NameD][idD] = dIdParts[key][NameD][idD].Split('%');
            }
        }
        return rez;
    }

    public void LoadSetting(MyMainMenu menu)
    {
        if (PlayerPrefs.HasKey("setting"))
        {
            setting = JsonUtility.FromJson<SaveSetting>(PlayerPrefs.GetString("setting"));
            SetStaticSetting();
            menu.SetSettingMenu();
        }
        else
            ResetSetting(menu);
    }
    public void LoadSetting()
    {
        if (PlayerPrefs.HasKey("setting"))
        {
            setting = JsonUtility.FromJson<SaveSetting>(PlayerPrefs.GetString("setting"));
            SetStaticSetting();
        }
    }
    public void SaveSetting()
    {
        setting.language = (int)Language.language;
        setting.mainVolume = Setting.mainVolume;
        setting.effectsVolume = Setting.effectsVolume;
        setting.musicVolume = Setting.musicVolume;
        setting.interfaceVolume = Setting.interfaceVolume;
        setting.particles = Setting.particles;
        setting.effectsUI = Setting.effectsUI;
        setting.showDamage = Setting.showDamage;
        setting.visualCode = Setting.visualCode;
        PlayerPrefs.SetString("setting", JsonUtility.ToJson(setting));
    }
    public void ResetSetting(MyMainMenu menu)
    {
        setting.language = 0;
        setting.mainVolume = 20;
        setting.effectsVolume = 20;
        setting.musicVolume = 20;
        setting.interfaceVolume = 20;
        setting.particles = true;
        setting.effectsUI = true;
        setting.showDamage = true;
        setting.visualCode = true;
        SetStaticSetting();
        menu.SetSettingMenu();
    }

    private void SetStaticSetting()
    {
        Language.language = (Language.LanguageType)setting.language;
        Setting.mainVolume = setting.mainVolume;
        Setting.effectsVolume = setting.effectsVolume;
        Setting.musicVolume = setting.musicVolume;
        Setting.interfaceVolume = setting.interfaceVolume;
        Setting.particles = setting.particles;
        Setting.effectsUI = setting.effectsUI;
        Setting.showDamage = setting.showDamage;
        Setting.visualCode = setting.visualCode;
    }

    public void SaveLevel(string themeLevel, string nameLevel, SaveTask[] tasks, SaveBase[] bases, SaveGameplayObject[] gameplayObjects, SaveRobot[] robots, SaveDecoration[] decorations)
    {
        DeleteLevel(themeLevel, nameLevel);
        if (themeLevels.themeLevels.Contains(themeLevel))
            levels = JsonUtility.FromJson<SaveTheme>(PlayerPrefs.GetString(themeLevel));
        else
        {
            themeLevels.themeLevels.Add(themeLevel);
            levels = new SaveTheme();
        }
        levels.levels.Add(themeLevel + nameLevel);
        currentLevel = new SaveLevel();
        currentLevel.tasks.AddRange(tasks);
        currentLevel.bases.AddRange(bases);
        currentLevel.gameplayObjects.AddRange(gameplayObjects);
        currentLevel.robots.AddRange(robots);
        currentLevel.decorations.AddRange(decorations);
        PlayerPrefs.SetString(themeLevel + nameLevel, JsonUtility.ToJson(currentLevel));
        PlayerPrefs.SetString(themeLevel, JsonUtility.ToJson(levels));
        PlayerPrefs.SetString("✪ThemeLevels✪", JsonUtility.ToJson(themeLevels));
    }
    public void DeleteLevel(string themeLevel, string nameLevel)
    {
        if (!themeLevels.themeLevels.Contains(themeLevel))
            return;
        levels = JsonUtility.FromJson<SaveTheme>(PlayerPrefs.GetString(themeLevel));
        if (!levels.levels.Contains(nameLevel))
            return;
        PlayerPrefs.DeleteKey(themeLevel + nameLevel);
        levels.levels.Remove(themeLevel + nameLevel);
        if (levels.levels.Count == 0)
        {
            themeLevels.themeLevels.Remove(themeLevel);
            PlayerPrefs.DeleteKey(themeLevel);
        }
        currentLevel = null;
        PlayerPrefs.SetString("✪ThemeLevels✪", JsonUtility.ToJson(themeLevels));
    }
    public void LoadLevel(string themeLevel, string nameLevel, ref SaveTask[] tasks, ref SaveBase[] bases, ref SaveGameplayObject[] gameplayObjects, ref SaveRobot[] robots, ref SaveDecoration[] decorations)
    {
        if (!PlayerPrefs.HasKey(themeLevel + nameLevel))
            return;
        currentLevel = JsonUtility.FromJson<SaveLevel>(PlayerPrefs.GetString(themeLevel + nameLevel));
        tasks = currentLevel.tasks.ToArray();
        bases = currentLevel.bases.ToArray();
        gameplayObjects = currentLevel.gameplayObjects.ToArray();
        robots = currentLevel.robots.ToArray();
        decorations = currentLevel.decorations.ToArray();
    }
}
[Serializable]
public class ScriptSave
{
    public string[] RealCode, InterfaceCode;
}
[Serializable]
public class RobotSave
{
    public Vector2[] PositionDetails;
    public int[] IdDetails, ParentDetails, IndexDetails;
    public string typeKeys, saveLine, scripts;
    public string[] Keys;
    public bool[] orientations;
    public float[] AngleDetails;
    /// <summary>
    /// Массив имён скриптов для каждой клавиши
    /// </summary>
    public SaveScriptInComputers[] saveScripts;
}
[Serializable]
public class SaveAll
{
    public List<string> ScriptsNames, RobotNames;
    public List<SaveNeedResources> needResourcesForRobots;
    /// <summary>
    /// тема уровней
    /// </summary>
    public List<string> LevelsOpenTheme;
    /// <summary>
    /// индекс последнего открытого уровня в темах
    /// </summary>
    public List<int> LevelsOpenId;
    /// <summary>
    /// индекс максимального уровня в темах
    /// </summary>
    public List<int> LevelsMax;
}
[Serializable]
public class SaveSetting
{
    public int language;
    public float mainVolume, effectsVolume, musicVolume, interfaceVolume;
    public bool particles, effectsUI;
    public bool showDamage, visualCode;
}
[Serializable]
public class SaveNeedResources
{
    public float[] needResources;
    public SaveNeedResources(float[] needResources)
    {
        this.needResources = needResources;
    }
}
[Serializable]
public class SaveScriptInComputers
{
    public List<string> Scripts;
    public List<int> idComputers;
    public int typeKey;
    public string key;
    public SaveScriptInComputers(List<string> Scripts, List<int> idComputers, int typeKey, string key)
    {
        this.Scripts = Scripts;
        this.idComputers = idComputers;
        this.typeKey = typeKey;
        this.key = key;
    }
}

[Serializable]
public class SaveThemes
{
    public List<string> themeLevels;
}
[Serializable]
public class SaveTheme
{
    public int openLevel = 0;
    public List<string> levels;
}
[Serializable]
public class SaveLevel
{
    public List<SaveTask> tasks;
    public List<SaveBase> bases;
    public List<SaveGameplayObject> gameplayObjects;
    public List<SaveRobot> robots;
    public List<SaveDecoration> decorations;
}
[Serializable]
public class SaveTask
{
    public List<float> values;
    public int idTypeTask;
}
[Serializable]
public class SaveBase
{
    public ResourceDetail resources;
    public int idPlayer;
    public Vector2 position;
    public float angle;
}
[Serializable]
public class SaveGameplayObject
{
    public List<float> values;
    public int idTypeGameplayObject;
    public Vector2 position;
    public float angle;
}
[Serializable]
public class SaveRobot
{
    public string name;
    public Vector2 position;
    public float angle;
}
[Serializable]
public class SaveDecoration
{
    public int idTypeDecoration;
    public Vector2 position;
    public float angle;
}