using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlerManager : MonoBehaviour
{
    public enum keyEvent
    {
        create,
        change,
        delete
    }

    public Toggle toggle;
    public bool beChangeKey { get; set; }
    public DetailObject detailTarget { get; set; }
    public GameObject InfoDetailConteiner, InfoScript, AddKeyConteiner;
    public Parameter parameter;
    public Dropdown dropdown;
    private Dropdown dropdownInfoScript;

    public List<InputField> infoText;
    public List<bool> infoChanged;

    public DescriptionManager descriptionManager;
    public Transform Content;
    public Button buildNewKey;
    public Text showKey, titleText;
    public Toggle SetValueOnClick, SwitchValueToSetOrDefaultOnClick, SetValueAndReturnDefaultValueOnClick, KeyArea;
    public List<string> Keys;
    public List<SaveScriptInComputers> saveScripts = new List<SaveScriptInComputers>();
    public string nameDetail, idDetail, typeKeys, saveLine, key;
    public int typeKey;
    public List<string> keyParts = new List<string>();
    public SaveArea save;
    private readonly Color32
         white = new Color32(216, 228, 225, 255),
         red = new Color32(233, 50, 51, 255);

    private void OnEnable()
    {
        InfoDetailConteiner.SetActive(false);
        AddKeyConteiner.SetActive(false);
        toggle.isOn = false;
        InfoCon.BeController = false;
        Keys.Clear();
        typeKeys = "";


        saveLine = "";

        typeKey = 0;
        key = "OnStartGame";

        if (keyParts != null)
            keyParts.Clear();
        dropdown.ClearOptions();
        dropdown.options.Add(new Dropdown.OptionData("\nOnStartGame"));
        dropdown.value = 0;
        dropdown.captionText.text = dropdown.options[0].text;
    }
    /// <summary>
    /// Создание панели изменения характеристики и проверка на возможность подключения скрипта (к макросу компьютера)
    /// </summary>
    /// <param name="detail">деталь (для валидации компьютера)</param>
    public void CreateInfoDetailControler(DetailObject detail)
    {
        if (!detail.isActiveAndEnabled)
            return;
        if (detailTarget != detail)
        {
            detailTarget = detail;
            nameDetail = detail.NameDetail;
            idDetail = detail.IndexDetail.ToString();
            detail.ValidateOnDescription();
            ClearInfoDetailConteiner();
            titleText.text = detail.labelDetail.text[(int)Language.language] + " " + detail.IndexDetail;
            titleText.GetComponent<Description>().SetDescription(detail.descriptionDetail.text, detail.visualisationDetail);

            string characteristics = "";
            string[] oldValueProperties, valueProperty = null;

            bool oldProperty = FindCharacteristics(key, typeKey, nameDetail, idDetail, ref characteristics);
            if (oldProperty)
            {
                oldValueProperties = characteristics.Split('%');
                valueProperty = new string[detail.NameProperties.Length];
                for (int i = 1; i < oldValueProperties.Length; i += 2)
                    valueProperty[oldValueProperties[i].ToInt()] = oldValueProperties[i + 1];
            }

            for (int idProperty = 0; idProperty < detail.NameProperties.Length; idProperty++)
            {
                if (detail.NameProperties[idProperty] == "NULL")
                    break;
                Parameter tempParameter = Instantiate(parameter, Content);
                tempParameter.managerInConstructor = this;
                tempParameter.id = idProperty;
                InputField input = tempParameter.inputValue;
                tempParameter.title.GetComponent<Description>().SetDescription(descriptionManager, detail.descriptionsProperty[idProperty].text, detail.visualisationProperty[idProperty]);
                tempParameter.title.text = detail.labelProperty[idProperty].text[(int)Language.language];
                infoText.Add(input);

                bool valueChanged = oldProperty && valueProperty != null && valueProperty[idProperty] != null;
                infoChanged.Add(valueChanged);
                input.text = valueChanged ? valueProperty[idProperty] : detail.ValProperties[idProperty].ToString();
                input.textComponent.color = valueChanged ? red : white;
                tempParameter.toggle.SetIsOnWithoutNotify(valueChanged);
            }
            if (detail.GetType() == typeof(DetailComputer))
                SetComputerScript((DetailComputer)detail);
            else
                dropdownInfoScript = null;
            InfoDetailConteiner.SetActive(true);
        }
        else
        {
            detailTarget = null;
            InfoDetailConteiner.SetActive(false);
        }
    }

    /// <summary>
    /// Очистка окна с редактированием макроса детали
    /// </summary>
    private void ClearInfoDetailConteiner()
    {
        for (int i = 0; i < Content.childCount; i++)
            Destroy(Content.GetChild(i).gameObject);
        infoText.Clear();
        infoChanged.Clear();
    }
    /// <summary>
    /// присваивание макросов клавиш робота, которого изменяют
    /// </summary>
    /// <param name="Keys">набор клавиш</param>
    /// <param name="typeKeys">набор типов</param>
    /// <param name="saveLine">код макроса клавиш</param>
    public void OnEnableKeysWhenRobotChange(string[] Keys, string typeKeys, string saveLine)
    {
        this.Keys = new List<string>(Keys);
        this.saveLine = saveLine;
        this.typeKeys = typeKeys;
        keyParts = new List<string>(this.saveLine.Split('|'));
        if (keyParts[0] == "")
            keyParts.Clear();

        if (Keys.Length > 0)
            for (int i = 0; i < this.Keys.Count; i++)
            {
                string keyName = this.Keys[i];
                string stringTypeKey = CharKeyTypeToString(this.typeKeys[i]);
                if (keyName != "OnStartGame")
                    dropdown.options.Add(new Dropdown.OptionData(stringTypeKey + '\n' + keyName));
            }
        dropdown.value = 0;
        dropdown.captionText.text = dropdown.options[0].text;
        if (Keys.Length == 0)
            return;
        // сделать клавишу поумолчанию
        //key = this.Keys[0];
        //typeKey = this.typeKeys[0].ToInt();
    }
    /// <summary>
    /// Валидация при создании клавиши
    /// </summary>
    public void ValidateKey()
    {

        char typeKey = '0';
        string keyName = showKey.text;
        if (keyName == "none")//ключа нет
            return;
        bool keyExist = false;
        if (SetValueOnClick.isOn)
            typeKey = '0';
        if (SwitchValueToSetOrDefaultOnClick.isOn)
            typeKey = '1';
        if (SetValueAndReturnDefaultValueOnClick.isOn)
            typeKey = '2';
        if (Keys.Count > 0)
            for (int i = 0; i < Keys.Count; i++)
                if (typeKey == typeKeys[i] && keyName == Keys[i])
                    keyExist = true;
        if (keyExist)//ключ повторяется
            return;

        if (beChangeKey)
        {
            beChangeKey = false;
            ChangeKey(keyName, typeKey);
        }
        else
            CreateKey(keyName, typeKey);
    }

    /// <summary>
    /// Создание клавиши
    /// </summary>
    /// <param name="key">имя</param>
    /// <param name="typeKey">тип</param>
    private void CreateKey(string key, char typeKey)
    {
        string nameLabel = CharKeyTypeToString(typeKey) + '\n' + key;
        dropdown.options.Add(new Dropdown.OptionData(nameLabel));
        dropdown.value = dropdown.options.Count - 1;
        dropdown.captionText.text = nameLabel;
    }

    /// <summary>
    /// Изменение имени и типа клавиши
    /// </summary>
    /// <param name="key">имя</param>
    /// <param name="typeKey">тип</param>
    private void ChangeKey(string key, char typeKey)
    {
        string oldKey = dropdown.options[dropdown.value].text.Split('\n')[1];
        int oldType = StringKeyTypeToInt(dropdown.options[dropdown.value].text.Split('\n')[0]);

        int id = FindKey(oldKey, oldType);
        if (id == -1)
            return;

        Keys[id] = key;
        typeKeys = typeKeys.Remove(id, 1);
        typeKeys = typeKeys.Insert(id, typeKey.ToString());
        dropdown.options[dropdown.value].text = CharKeyTypeToString(typeKey) + '\n' + key;
        dropdown.captionText.text = typeKey + '\n' + key;
    }
    /// <summary>
    /// Удаление клавиши
    /// </summary>
    public void DeleteKey()
    {
        string text = dropdown.options[dropdown.value].text;
        string oldKey = text.Split('\n')[1];
        int oldType = StringKeyTypeToInt(text.Split('\n')[0]);

        int id = FindKey(oldKey, oldType);
        if (id == -1)
            return;

        keyParts.RemoveAt(id);
        saveLine = keyParts.ToArray().ToOneString("|").Replace("  ", " ");
        saveScripts.Remove(SearchSaveScriptInComputers(Keys[id], typeKeys[id].ToInt()));
        Keys.RemoveAt(id);
        typeKeys = typeKeys.Remove(id, 1);

        dropdown.options.RemoveAt(dropdown.value);
        dropdown.value = 0;
        dropdown.captionText.text = dropdown.options[0].text;
    }
    public SaveScriptInComputers SearchSaveScriptInComputers(string key, int typeKey)
    {
        for (int i = 0; i < saveScripts.Count; i++)
            if(saveScripts[i].key == key && saveScripts[i].typeKey == typeKey)
                return saveScripts[i];
        return null;
    }
    /// <summary>
    /// Находит id необходимой клавиши.
    /// <code>
    /// Например 1 key: OnStartGame typeKey:0 return: 2
    /// Keys: W, S, OnStartGame, A, D, W, S, A, D
    /// typeKeys: 000002222
    /// </code>
    /// </summary>
    /// <param name="key">клавиша</param>
    /// <param name="typeKey">тип нажатия</param>
    /// <returns>id клавиши</returns>
    private int FindKey(string key, int typeKey)
    {
        for (int i = 0; i < Keys.Count; i++)
            if (Keys[i] == key && typeKeys[i].ToInt() == typeKey)
                return i;
        return -1;
    }

    /// <summary>
    /// Находит id набора с необходимым типом деталей.
    /// <code>
    /// Например idKey: 2 nameDetail: Miner return: 2
    /// ClassDetails: Whell 0%0%0 1%0%0, Gun 0%0%1%1%1%2%0, Miner 0%0%25%1%0,5%2%12 
    /// </code>
    /// </summary>
    /// <param name="idKey">id клавиши</param>
    /// <param name="nameDetail">необходимый тип</param>
    /// <param name="ClassDetails">набор разных типов деталей из клавиши с id</param>
    /// <returns>id набора с необходимым типом деталей</returns>
    private int FindClassDetail(int idKey, string nameDetail, out string[] ClassDetails)
    {
        ClassDetails = keyParts[idKey].Split('$');
        for (int i = 0; i < ClassDetails.Length; i++)
            if (ClassDetails[i].StartsWith(nameDetail))
                return i;
        return -1;
    }

    /// <summary>
    /// Находит id свойств детали с необходимым индексом
    /// <code>
    /// Например idNameDetail: 1 idDetail:1 return: 2
    /// ClassDetails: Piston 0%0%1%1%0, Whell 0%0%-1000 1%0%1000 2%0%-1000 3%0%1000
    /// targetProperties: Whell, 0%0%-1000, 1%0%1000, 2%0%-1000, 3%0%1000
    /// </code>
    /// </summary>
    /// <param name="idNameDetail">id набора с необходимым типом деталей</param>
    /// <param name="idDetail">индекс детали</param>
    /// <param name="ClassDetails">набор разных типов деталей</param>
    /// <param name="targetProperties">набор с одним типом деталей, но разным индексом</param>
    /// <returns>id свойств детали с необходимым индексом</returns>
    private int FindIdDetail(int idNameDetail, string idDetail, string[] ClassDetails, out string[] targetProperties)
    {
        targetProperties = ClassDetails[idNameDetail].Split(' ');
        for (int i = 1; i < targetProperties.Length; i++)
            if (targetProperties[i].StartsWith(idDetail))
                return i;
        return -1;
    }

    /// <summary>
    /// Пытается найти характеристики необходимой детали
    /// </summary>
    /// <param name="key">имя клавиши, где находятся характеристики необходимой детали</param>
    /// <param name="typeKey">тип клавиши, где находятся характеристики необходимой детали</param>
    /// <param name="nameDetail">имя типа детали, где находятся характеристики необходимой детали</param>
    /// <param name="idDetail">индекс необходимой детали</param>
    /// <param name="characteristics">характеристики</param>
    /// <returns>нашёл ли характеристики?</returns>
    private bool FindCharacteristics(string key, int typeKey, string nameDetail, string idDetail, ref string characteristics)
    {
        int idKey = FindKey(key, typeKey);
        if (idKey < 0)
            return false;

        string[] nameParts;
        int idNameDetail = FindClassDetail(idKey, nameDetail, out nameParts);
        if (idNameDetail < 0)
            return false;

        string[] idDetails;
        int idIdDetail = FindIdDetail(idNameDetail, idDetail, nameParts, out idDetails);
        if (idIdDetail < 0)
            return false;
        characteristics = idDetails[idIdDetail];
        return true;
    }
    /// <summary>
    /// Редактирует набор макросов
    /// </summary>
    public void AddInfoItems()
    {
        string strProperty = "";//набор команд
        #region запись значений
        if (infoText.Count > 0)
        {
            strProperty = idDetail;
            for (int i = 0; i < infoText.Count; i++)
                if (infoChanged[i])
                {
                    if (infoText[i].text == "")
                        strProperty += "%" + i + "%0";
                    else
                        strProperty += "%" + i + "%" + infoText[i].text;
                }
        }
        if (strProperty == idDetail)
            strProperty = "";
        #endregion
        #region дробление
        int indexKey = FindKey(key, typeKey);//id необходимой клавиши
        if (indexKey == -1)
        {
            if (strProperty == "")
                return;
            #region упаковка
            Keys.Add(key);
            typeKeys += typeKey.ToChar();
            keyParts.Add(nameDetail + " " + strProperty);
            saveLine = keyParts.ToArray().ToOneString("|").Replace("  ", " ");
            #endregion
            return;
        }//ключа нет
        string[] ClassDetails;//массив деталей с именами
        int idNameDetail = FindClassDetail(indexKey, nameDetail, out ClassDetails);//id деталей с необходимым именем

        if (idNameDetail == -1)
        {
            if (strProperty == "")
                return;
            #region упаковка
            keyParts[indexKey] += "$" + nameDetail + " " + strProperty;
            saveLine = keyParts.ToArray().ToOneString("|").Replace("  ", " ");
            #endregion
            return;
        }//класса детали нет

        string[] idDetails;//массив деталей с необходимым именем
        int idIdDetail = FindIdDetail(idNameDetail, idDetail, ClassDetails, out idDetails);//id детали с необходимым именем и индексом
        #region упаковка
        ClassDetails[idNameDetail] = idDetails.ToOneString(" ");
        if (idIdDetail > -1)
        {
            idDetails[idIdDetail] = strProperty;
            if (strProperty == "" && idDetails.Length == 2)
            {
                ClassDetails[idNameDetail] = "";
                if (ClassDetails.Length == 1)
                {
                    keyParts.RemoveAt(indexKey);
                    typeKeys = typeKeys.Remove(indexKey, 1);
                    Keys.RemoveAt(indexKey);
                    ClassDetails = null;
                }
            }
            else
            {
                ClassDetails[idNameDetail] = idDetails.ToOneString(" ");
            }
        }//индекс детали найден
        else
        {
            if (strProperty == "")
                return;
            ClassDetails[idNameDetail] += " " + strProperty;
        }

        if (ClassDetails != null || strProperty != "")
            keyParts[indexKey] = ClassDetails.ToOneString("$");
        saveLine = keyParts.ToArray().ToOneString("|").Replace("  ", " ");
        #endregion
        #endregion
    }

    public void SetComputerScript(DetailComputer computer)
    {
        dropdownInfoScript = Instantiate(InfoScript, Content).GetComponent<Dropdown>();
        dropdownInfoScript.GetComponent<Parameter>().managerInConstructor = this;
        dropdownInfoScript.options.Add(new Dropdown.OptionData(""));
        for (int i = 0; i < save.all.ScriptsNames.Count; i++)
            dropdownInfoScript.options.Add(new Dropdown.OptionData(save.all.ScriptsNames[i].Substring(6)));


        int indexKey = FindKeyComputerScript(key, typeKey);//id необходимой клавиши скриптов компьютера
        if (indexKey == -1 || save.SaverRobot.saveScripts.Length == 0)
            return;
        int countScripts = saveScripts[indexKey].idComputers.Count;
        if (countScripts > 0)
            for (int i = 0; i < countScripts; i++)
            {
                if(saveScripts[indexKey].idComputers[i] == computer.IndexDetail)
                {
                    string script = saveScripts[indexKey].Scripts[i];
                    int idOption = FindDropdownOption(dropdownInfoScript, script);
                    dropdownInfoScript.value = idOption;
                }
            }
    }
    int FindDropdownOption(Dropdown dropdown, string str)
    {
        for (int i = 0; i < dropdown.options.Count; i++)
            if(dropdown.options[i].text == str)
                return i;
        return -1;
    }
    /// <summary>
    /// Подключение скрипта к макросу компьютера
    /// </summary>
    public void AddComputerScript()
    {
        if (!dropdownInfoScript || detailTarget.GetType() != typeof(DetailComputer))
            return;
        string script = dropdownInfoScript.options[dropdownInfoScript.value].text;
        int idComputer = idDetail.ToInt();
        int idKeyFind, idComputerFind;
        (idKeyFind, idComputerFind) = FindComputerScript(key, typeKey, idComputer);
        if (idKeyFind == -1)
        {
            saveScripts.Add(new SaveScriptInComputers(new List<string>() { script }, new List<int>() { idComputer }, typeKey, key));
        }
        else
        {
            if (idComputerFind == -1)
            {
                saveScripts[idKeyFind].Scripts.Add(script);
                saveScripts[idKeyFind].idComputers.Add(idComputer);
            }
            else
            {
                saveScripts[idKeyFind].Scripts[idComputerFind] = script;
            }
        }
    }
    /// <summary>
    /// Ищет массив скриптов компьютера определёной клавиши
    /// </summary>
    /// <param name="typeKey">тип нажатия клавиши</param>
    /// <param name="key">имя клавиши</param>
    /// <returns>индекс массива скриптов компьютера</returns>
    int FindKeyComputerScript(string key, int typeKey)
    {
        if(saveScripts.Count > 0)
        for (int id = 0; id < saveScripts.Count; id++)
        {
                if(saveScripts[id].typeKey == typeKey && saveScripts[id].key == key)
                    return id;
        }
        return -1;
    }
    /// <summary>
    /// скрипт компьютера определёной клавиши
    /// </summary>
    /// <param name="typeKey">тип нажатия клавиши</param>
    /// <param name="key">имя клавиши</param>
    /// <param name="idComputer">индекс скрипта компьютера</param>
    /// <returns>индекс скрипта компьютера определёной клавиши</returns>
    (int, int) FindComputerScript(string key, int typeKey, int idComputer)
    {
        int idKey = FindKeyComputerScript(key, typeKey);
        if(saveScripts.Count > 0 && saveScripts[idKey].idComputers.Count > 0)
        for (int id = 0; id < saveScripts[idKey].idComputers.Count; id++)
            if (saveScripts[idKey].idComputers[id] == idComputer)
                return (idKey, id);
        return (idKey, -1);
    }
    /// <summary>
    /// Замена скрипта в макросе компьютера
    /// </summary>
    private void ChangeComputerScript(int indexKey)
    {
        if (dropdownInfoScript && nameDetail == "Computer")
        {
            int idD = idDetail.ToInt();
            if (saveScripts[indexKey].idComputers.Contains(idD))
            {
                int id = saveScripts[indexKey].idComputers.IndexOf(idD);
                saveScripts[indexKey].Scripts[id] = dropdownInfoScript.options[dropdownInfoScript.value].text;
            }
            else
            {
                saveScripts[indexKey].Scripts.Add(dropdownInfoScript.options[dropdownInfoScript.value].text);
                saveScripts[indexKey].idComputers.Add(idDetail.ToInt());
            }

        }
    }

    /// <summary>
    /// Для создания макроса клавиш
    /// </summary>
    private void OnGUI()
    {
        if (KeyArea.isOn && Input.anyKeyDown)
        {
            Event e = Event.current;
            if (e.isKey)
            {
                KeyArea.isOn = false;
                showKey.text = e.keyCode.ToString();
                buildNewKey.interactable = showKey.text != "none";
            }
        }
    }

    /// <summary>
    /// Преобразует в наименование типа клавиши
    /// </summary>
    /// <param name="intkeyType">тип</param>
    /// <returns>наименование типа клавиши</returns>
    private string CharKeyTypeToString(char intkeyType)
    {
        switch (intkeyType)
        {
            case '0':
                return "при нажатии клавиши";
            case '1':
                return "при удержании клавиши";
            case '2':
                return "при отпускании клавиши";
            default:
                return "";
        }
    }
    /// <summary>
    /// Преобразует в тип
    /// </summary>
    /// <param name="stringkeyType">наименование типа клавиши</param>
    /// <returns>тип</returns>
    public int StringKeyTypeToInt(string stringkeyType)
    {
        switch (stringkeyType)
        {
            case "при нажатии клавиши":
                return 0;
            case "при удержании клавиши":
                return 1;
            case "при отпускании клавиши":
                return 2;
            default:
                return 0;
        }
    }
    public void OnChangedInfo(bool on, int id)
    {
        if (infoChanged.Count <= id)
            return;
        infoChanged[id] = on;
        infoText[id].textComponent.color = on ? red : white;
        if (!on)
            infoText[id].text = detailTarget.ValProperties[id].ToString();
    }
}