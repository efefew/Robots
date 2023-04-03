using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeScriptAndRobot : MonoBehaviour {

    public Dropdown RobotList, ScriptList;
    public SaveArea saveArea;

    public List<string> RobotNameList, ScriptNameList;

    public GameObject CodeMenu, ConstructorMenu, GameMenu, RobotMenu, GameInterface;
    ConstructorManager constructorRobot;
    CodeConstructor stringCreater;
    public Button NewButScript, ChangeButScript, DeleteButScript, NewButRobot, ChangeButRobot, DeleteButRobot, GameBut;
    public InputField NewRobotText, NewScriptText;
    public DescriptionManager exceptionRobot, exceptionScript;
    public LevelManager[] levels;
    public Text labelTaskDescription;
    void Awake () {
        stringCreater = CodeMenu.GetComponent<CodeConstructor>();
        constructorRobot = ConstructorMenu.GetComponent<ConstructorManager>();
        ChangeButRobot.interactable = false;
        DeleteButRobot.interactable = false;
        GameBut.interactable = false;
        ChangeButScript.interactable = false;
        DeleteButScript.interactable = false;
    }
    void OnEnable()
    {
        CodeMenu.SetActive(false);
        ConstructorMenu.SetActive(false);
        RobotMenu.SetActive(false);
        GameMenu.SetActive(false);
        GameInterface.SetActive(false);

        NewRobotText.text = "";
        NewScriptText.text = "";
        NewButRobot.interactable = false;
        NewButScript.interactable = false;
        StartCoroutine(UpdateListRobot());
        StartCoroutine(UpdateListScript());
        if (Setting.themeLevelNow == "✪DevelopTheme✪")//пока для моих уровней так
            labelTaskDescription.text = levels[Setting.idLevelNow].taskDescription;
    }
    
    
    void OnWriteName(List<string> nameList, Button but, string text, string add, DescriptionManager exception)
    {
        if (text == "")
        {
            but.interactable = false;

            //exception.TargetActivator("Имя должно быть не пустым");
            exception.TargetDeactivator();
            return;
        }
        if (nameList.Contains(add + text))
        {
            but.interactable = false;
            switch (Language.language)
            {
                case Language.LanguageType.english:
                    exception.TargetActivator("Name must be unique");
                    break;
                case Language.LanguageType.russian:
                    exception.TargetActivator("Имя должно быть уникальным");
                    break;
            }
            return;
        }
        exception.TargetDeactivator();
        but.interactable = true;
    }
    IEnumerator UpdateListRobot()
    {
        RobotNameList.Clear();
        RobotList.ClearOptions();

        yield return new WaitForEndOfFrame();

        if (saveArea.all.RobotNames.Count > 0)
        {
            RobotNameList.AddRange(saveArea.all.RobotNames);
            RobotList.captionText.text = RobotNameList[0].Substring(5);

            for (int i = 0; i < RobotNameList.Count; i++)
                RobotList.options.Add(new Dropdown.OptionData(RobotNameList[i].Substring(5)));
            RobotList.value = RobotList.options.Count - 1;

            ChangeButRobot.interactable = true;
            DeleteButRobot.interactable = true;
            GameBut.interactable = true;
        }
        else
        {
            GameBut.interactable = false;
            ChangeButRobot.interactable = false;
            DeleteButRobot.interactable = false;
        }
    }
    public void NewRobot()
    {
        InfoCon.BeConstructor = true;
        constructorRobot.BeChangeRobot = false;
        constructorRobot.NameRobot = "Robot" + NewRobotText.text;
        ConstructorMenu.SetActive(true);
    }
    public void OnWriteRobotName(string text)
    {
        OnWriteName(RobotNameList, NewButRobot, text, "Robot", exceptionRobot);
    }
    public void ChangeRobot()
    {
        if (RobotList.options.Count == 0)
            return;
        InfoCon.BeConstructor = true;
        constructorRobot.BeChangeRobot = true;
        constructorRobot.NameRobot = "Robot" + RobotList.options[RobotList.value].text;
        ConstructorMenu.SetActive(true);
    }
    public void DeleteRobot()
    {
        string nameRobot;
        if(RobotList.options.Count > 0)
            nameRobot = RobotList.options[RobotList.value].text;
        else
            return;

        saveArea.DeleteRobot("Robot" + nameRobot);
        StartCoroutine(UpdateListRobot());
    }

    IEnumerator UpdateListScript()
    {
        ScriptNameList.Clear();
        ScriptList.ClearOptions();

        yield return new WaitForEndOfFrame();

        if (saveArea.all.ScriptsNames.Count > 0)
        {
            ScriptNameList.AddRange(saveArea.all.ScriptsNames);
            ScriptList.captionText.text = ScriptNameList[0].Substring(6);

            for (int i = 0; i < ScriptNameList.Count; i++)
                ScriptList.options.Add(new Dropdown.OptionData(ScriptNameList[i].Substring(6)));
            ScriptList.value = ScriptList.options.Count - 1;
            ChangeButScript.interactable = true;
            DeleteButScript.interactable = true;
        }
        else
        {
            ChangeButScript.interactable = false;
            DeleteButScript.interactable = false;
        }
    }
    public void NewScript()
    {
        stringCreater.BeChangeScript = false;
        stringCreater.NameCode = "Script" + NewScriptText.text;
        CodeMenu.SetActive(true);
    }
    public void OnWriteScriptName(string text)
    {
        OnWriteName(ScriptNameList, NewButScript, text, "Script", exceptionScript);
    }
    public void ChangeScript()
    {
        if (ScriptList.options.Count == 0)
            return;
        stringCreater.BeChangeScript = true;
        stringCreater.NameCode = "Script" + ScriptList.options[ScriptList.value].text;
        CodeMenu.SetActive(true);
    }
    public void DeleteScript()
    {
        string nameScript;
        if (ScriptList.options.Count > 0)
            nameScript = ScriptList.options[ScriptList.value].text;
        else
            return;

        saveArea.DeleteScript("Script" + nameScript);
        StartCoroutine(UpdateListScript());
    }

    public void OnGame()
    {
        InfoCon.BeConstructor = false;
        GameMenu.SetActive(true);
        GameInterface.SetActive(true);
    }
    public void OnDropdownRobot()
    {
        if (RobotList.options.Count > 1 && RobotList.options.Count > 0)
        {
            DeleteButRobot.interactable = true;
            ChangeButRobot.interactable = true;
            GameBut.interactable = true;//|| ScriptList.options[ScriptList.value].text == NULL)
        }
        else
        {
            ChangeButRobot.interactable = false;
            DeleteButRobot.interactable = false;
            GameBut.interactable = false;
        }
    }
    public void OnDropdownScript()
    {
        if (ScriptList.options.Count > 1 && ScriptList.options.Count > 0)
        {
            DeleteButScript.interactable = true;
            ChangeButScript.interactable = true;
        }
        else
        {
            ChangeButScript.interactable = false;
            DeleteButScript.interactable = false;
        }
    }
}
