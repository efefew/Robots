using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DetailComputer : DetailObject
{
    enum TypeValueProperty
    {
        NewValue,
        OldValue,
        OldOrNewValue
    }
    private Transform GameMenu;
    public RobotManager Robot { get; private set; }
    [HideInInspector]
    public SaveArea save;
    [HideInInspector]
    public SaveScriptInComputers[] SaveScripts;
    [Header("все детали робота")]
    public List<DetailObject> Details;

    public float WaitTime = 0.1f;
    private float result;
    private float A, B, C, D, E, F, G, TimeDelay, MinTimeDelay;
    [HideInInspector]
    public float ElectricityGenerate;
    private int CountNeedScipBrace, CountNeedRightBrace, IfRank, WhileRank, CountIf, CountWhile;

    public List<string> mainLineCode = new List<string>();
    public string[][][][] propertyParts;
    //[HideInInspector]
    public string[] Keys;
    //[HideInInspector]
    public string typeKeys;
    private string ChangeStr, OperatorStr, mainNameCode;
    private bool ItNumber, BoolResult, Inverce;
    public bool autopilot, CodeOn;

    private void Awake()
    {
        ElectricityGenerate = ValueReadProperties[5];
        MinTimeDelay = ValueReadProperties[6];
    }
    protected override void Start()
    {
        Robot = transform.parent.GetComponent<RobotManager>();
        GameMenu = transform.parent.parent;
        if (!InfoCon.BeConstructor)
        {
            ////имяДетали idДетали%idСвойства%зн%idСвойства%зн idДетали%idСвойства%зн$имяДетали idДетали%idСвойства%зн|имяДетали idДетали%idСвойства%зн
            if (MainBrain)
            {
                for (int key = 0; key < Keys.Length; key++)
                    if (Keys[key] == "OnStartGame")
                    {
                        SetKeyPropertyes(key);
                        break;
                    }
                int id = FindKeyScriptComputers("OnStartGame", 0);
                if (id != -1)
                    SetKeyScripts(id);
            }

            CodeOn = true;
            OnChangeScript("Script" + mainNameCode, ref mainLineCode);
        }
        base.Start();
    }

    /// <summary>
    /// поиск привязок скриптов к клавишам
    /// </summary>
    /// <param name="key">ключ</param>
    /// <param name="typeKey">тип ключа</param>
    /// <returns></returns>
    private int FindKeyScriptComputers(string key, int typeKey)
    {
        for (int i = 0; i < SaveScripts.Length; i++)
            if (SaveScripts[i].key == key && SaveScripts[i].typeKey == typeKey)
                return i;
        return -1;
    }
    protected override void Update()
    {
        base.Update();
        if (beBreak || !electricity || InfoCon.BeConstructor)
            return;
        CheckKeys();
        if (CodeOn && mainLineCode.Count > 0 && mainNameCode != "")
        {
            CodeOn = false;
            StartCoroutine(Compiler("Script" + mainNameCode, true, mainLineCode));
        }
    }

    protected override void CalculationOfProperties(float[] newValProperties)
    {
        if (beBreak)
            return;
        TimeDelay = newValProperties[0] < MinTimeDelay ? MinTimeDelay : newValProperties[0];
        if (electricity)
        {
            A = newValProperties[1];
            B = newValProperties[2];
            C = newValProperties[3];
            D = newValProperties[4];
            E = newValProperties[5];
            F = newValProperties[6];
            G = newValProperties[7];

            ValueProperties[1] = A;
            ValueProperties[2] = B;
            ValueProperties[3] = C;
            ValueProperties[4] = D;
            ValueProperties[5] = E;
            ValueProperties[6] = F;
            ValueProperties[7] = G;
            ValueProperties[0] = TimeDelay;
        }
        ValueReadProperties[3] = MaxElectricityConsumption * (MinTimeDelay / TimeDelay);
        ElectricityConsumption = ValueReadProperties[3];
    }
    public override void ValidateOnDescription()
    {
        base.ValidateOnDescription();
        labelProperty[0] = new TextLanguage("Temporary delay", "Временная задержка");
        labelProperty[1] = new TextLanguage("A", "А");
        labelProperty[2] = new TextLanguage("B", "Б");
        labelProperty[3] = new TextLanguage("C", "В");
        labelProperty[4] = new TextLanguage("D", "Г");
        labelProperty[5] = new TextLanguage("E", "Д");
        labelProperty[6] = new TextLanguage("F", "Е");
        labelProperty[7] = new TextLanguage("G", "Ж");

        labelReadProperty[5] = new TextLanguage("Power generation", "Производство электроэнергии");
        labelReadProperty[6] = new TextLanguage("Minimum time delay", "Минимальная временная задержка");

        descriptionDetail = new TextLanguage(

            "The computer is running scripts," +
            " also without the main computer, control over the robot will be lost.",

            "Компьютер запускает скрипты," +
            " также без главного компьютера потеряется контроль над роботом.");

        descriptionsReadProperty[5] = new TextLanguage(

            "A parameter that determines the value of electricity," +
            " which is generated by the computer.",

            "Параметр определяющий значение электричества," +
            " которое генерирует компьютер.");

        descriptionsReadProperty[6] = new TextLanguage(

            "A parameter that defines the minimum delay between script blocks.",

            "Параметр определяющий минимальную задержку между блоками скрипта.");

        descriptionsProperty[0] = new TextLanguage(

            "A parameter that determines the delay between script blocks." +
            DescriptionDependence(Language.LanguageType.english, "delay between script blocks", TypeDependence.LessBeter) +
            DescriptionRange(Language.LanguageType.english, min: "minimum delay between script blocks"),

            "Параметр определяющий задержку между блоками скрипта." +
            DescriptionDependence(Language.LanguageType.russian, "задержка между блоками скрипта", TypeDependence.LessBeter) +
            DescriptionRange(Language.LanguageType.russian, min: "минимальной задержки между блоками скрипта"));


        char[] cellNames = { 'A', 'B', 'C', 'D', 'E', 'F', 'G' };
        for (int i = 0; i < cellNames.Length; i++)
        {
            descriptionsProperty[i + 1] = new TextLanguage(

            "Value cell " + cellNames[i] + ".\n" +
            "Used to store, assign, and write values.\n" +
            "Also used to refer to a part index with a variable instead of a number.",

            "Ячейка значений " + cellNames[i] + ".\n" +
            "Используется для хранения, присваивания и записи значений.\n" +
            "Также используется для обращения к индексу детали с помощью переменной вместо числа.");
        }

    }
    /// <summary>
    /// Загрузка строк скрипта
    /// </summary>
    /// <param name="nameCode">имя скрипта</param>
    /// <param name="lineCode">строки кода</param>
    public void OnChangeScript(string nameCode, ref List<string> lineCode)
    {
        if (PlayerPrefs.HasKey(nameCode))
            save.LoadScript(nameCode, ref lineCode);
    }
    /// <summary>
    /// удалить все присоединённые детали
    /// </summary>
    public void DestroyAll()
    {
        for (int i = 0; i < Details.Count; i++)
            if (Details[i] && this != Details[i])
                DestroyDetail(Details[i]);

    }
    /// <summary>
    /// удалить присоединённую деталь
    /// </summary>
    /// <param name="detail">деталь</param>
    public void DestroyDetail(DetailObject detail)
    {
        detail.electricity = false;
        detail.transform.SetParent(GameMenu);

        if (Robot.Moves.Contains(detail.transform))
            Robot.Moves.Remove(detail.transform);

        if (Robot.Objects.Contains(detail))
            Robot.Objects.Remove(detail);
        if (detail.GetComponent<DetailGenerator>())
            if (Robot.Generators.Contains(detail.GetComponent<DetailGenerator>()))
                Robot.Generators.Remove(detail.GetComponent<DetailGenerator>());
        if (detail.GetComponent<DetailBattery>())
            if (Robot.Battery.Contains(detail.GetComponent<DetailBattery>()))
                Robot.Battery.Remove(detail.GetComponent<DetailBattery>());
        if (detail.GetComponent<DetailComputer>())
            if (Robot.Computers.Contains(detail.GetComponent<DetailComputer>()))
                Robot.Computers.Remove(detail.GetComponent<DetailComputer>());

        if (this == detail)
            DestroyAll();
        else
        {
            if (Details.Contains(detail))
                Details.Remove(detail);
        }
    }

    private IEnumerator Compiler(string nameCode, bool main, List<string> lineCodeTemp = null)
    {
        List<string> lineCode = new List<string>();
        if (lineCodeTemp == null)
            OnChangeScript(nameCode, ref lineCode);
        else
            lineCode.AddRange(lineCodeTemp);

        int i = 0;
        if (!beBreak && electricity)
            while (i < lineCode.Count)
            {
                yield return new WaitForSeconds(WaitTime);
                ChangeStr = lineCode[i].ToString();
                if (ChangeStr.StartsWith("`"))
                    yield return StartCoroutine(Compiler(ChangeStr.Substring(1), false));
                if (ChangeStr.StartsWith("$") && CountNeedScipBrace == 0 && !Inverce)
                {
                    OperatorStr = ChangeStr.Substring(ChangeStr.IndexOf("=") + 1);
                    int MyVal = 0;
                    bool Assignment = false;
                    bool Finish = false;
                    int CountBracket = 0;
                    string MyStr = OperatorStr;
                    while (!Finish)
                    {
                        if (Assignment)
                        {
                            SearchVar(ChangeStr.Substring(1, ChangeStr.IndexOf("€") - 1), ref result, BeChangeNum: true);
                            Finish = true;
                        }
                        else
                        {
                            if (!MyStr.Contains("%") && !MyStr.Contains("*") && !MyStr.Contains("/") && !MyStr.Contains("+") && !MyStr.Contains("_"))
                            {
                                if (!MyStr.Contains("$"))
                                {
                                    if (!MyStr.Contains("#"))
                                        Assignment = true;
                                    else
                                        MyStr = MyStr.Substring(1, MyStr.Length - 1);
                                }
                                else
                                {
                                    string s = MyStr.Substring(1, MyStr.Length - 1);
                                    float k = 0;
                                    SearchVar(s, ref k);
                                    MyStr = k.ToString();
                                }
                            }
                            else
                            {
                                if (MyStr.Contains("("))
                                {
                                    CountBracket++;
                                    MyVal += MyStr.IndexOf("(") + 1;
                                    MyStr = MyStr.Substring(MyStr.IndexOf("(") + 1, MyStr.IndexOf(")") - (MyStr.IndexOf("(") + 1));

                                }
                                else
                                {

                                    int MinValue = 0;
                                    int MaxValue = 0;
                                    int OperatorId = 0;
                                    int Operator1 = 2000000000, Operator2 = 2000000000;
                                    #region узнать знак
                                    if (MyStr.Contains("%"))
                                    {
                                        MaxValue = MyStr.IndexOf("%");
                                        OperatorId = 1;
                                    }
                                    else
                                    if (MyStr.Contains("*") || MyStr.Contains("/"))
                                    {
                                        if (MyStr.Contains("*"))
                                            Operator1 = MyStr.IndexOf("*");
                                        if (MyStr.Contains("/"))
                                            Operator2 = MyStr.IndexOf("/");
                                        if (Operator1 < Operator2)
                                        {
                                            MaxValue = Operator1;
                                            OperatorId = 2;
                                        }
                                        else
                                        {
                                            MaxValue = Operator2;
                                            OperatorId = 3;
                                        }

                                    }
                                    else
                                    if (MyStr.Contains("+") || MyStr.Contains("_"))
                                    {
                                        if (MyStr.Contains("+"))
                                            Operator1 = MyStr.IndexOf("+");
                                        if (MyStr.Contains("_"))
                                            Operator2 = MyStr.IndexOf("_");

                                        if (Operator1 < Operator2)
                                        {
                                            MaxValue = Operator1;
                                            OperatorId = 4;
                                        }
                                        else
                                        {
                                            MaxValue = Operator2;
                                            OperatorId = 5;
                                        }
                                    }
                                    #endregion
                                    string str = "";
                                    int StartValue = 0;
                                    int EndValue = 0;

                                    float Num1 = 0, Num2 = 0;
                                    for (int Val = MyStr.Length - 1; Val > -1; Val--)
                                    {
                                        if (MaxValue > Val)
                                        {
                                            if (MyStr[Val] == '$')
                                            {
                                                MinValue = Val;
                                                ItNumber = false;
                                                break;
                                            }
                                            if (MyStr[Val] == '#')
                                            {
                                                MinValue = Val;
                                                ItNumber = true;
                                                break;
                                            }
                                        }
                                    }
                                    StartValue = MinValue;

                                    str = MyStr.Substring(MinValue + 1, MaxValue - 1 - (MinValue + 1));
                                    if (!ItNumber)
                                    {
                                        SearchVar(str, ref Num1);
                                    }
                                    else
                                        Num1 = str.ToFloat();

                                    for (int Val = 0; Val < MyStr.Length; Val++)
                                    {
                                        if (MaxValue < Val)
                                        {
                                            if (MyStr[Val] == '€')
                                            {
                                                MinValue = Val;
                                                ItNumber = false;
                                                break;
                                            }
                                            if (MyStr[Val] == '№')
                                            {
                                                MinValue = Val;
                                                ItNumber = true;
                                                break;
                                            }
                                        }
                                    }
                                    EndValue = MinValue;
                                    str = MyStr.Substring(MaxValue + 2, MinValue - (MaxValue + 2));
                                    if (!ItNumber)
                                    {
                                        SearchVar(str, ref Num2);
                                    }
                                    else
                                        Num2 = str.ToFloat();
                                    switch (OperatorId)
                                    {
                                        case 1: result = Num1 % Num2; break;
                                        case 2: result = Num1 * Num2; break;
                                        case 3: result = Num1 / Num2; break;
                                        case 4: result = Num1 + Num2; break;
                                        case 5: result = Num1 - Num2; break;
                                        default: break;
                                    }
                                    MyStr = OperatorStr;
                                    MyStr = MyStr.Remove(StartValue + MyVal, EndValue - StartValue + 1);
                                    if (MyStr.Contains("()"))
                                        MyStr = MyStr.Replace("()", "#" + result + "№");
                                    else
                                        MyStr = MyStr.Insert(StartValue + MyVal, "#" + result + "№");
                                    MyVal = 0;
                                    OperatorStr = MyStr;

                                }
                            }

                        }
                    }

                }
                if (ChangeStr.StartsWith("if"))
                {
                    if (CountNeedRightBrace != 0 || WhileRank != 0)
                        CountNeedRightBrace++;
                    if (CountNeedScipBrace != 0)
                        CountNeedScipBrace++;
                    if (CountNeedScipBrace == 0 && !Inverce)
                    {

                        OperatorStr = ChangeStr.Substring(ChangeStr.IndexOf("if(") + 3, ChangeStr.LastIndexOf(")") - (ChangeStr.IndexOf("if(") + 3));
                        int MyVal = 0;
                        bool Finish = false;
                        int CountBracket = 0;
                        string MyStr = OperatorStr;
                        while (!Finish)
                        {
                            if (!MyStr.Contains("==") && !MyStr.Contains("!=") && !MyStr.Contains("&&") && !MyStr.Contains("||") && !MyStr.Contains("@>") && !MyStr.Contains("@<") && !MyStr.Contains(">=") && !MyStr.Contains("<="))
                            {
                                if (!BoolResult)
                                {
                                    if (IfRank > 0)
                                        CountIf++;
                                    CountNeedScipBrace++;
                                }
                                else
                                {
                                    CountIf++;
                                    IfRank++;
                                }
                                Finish = true;
                            }
                            else
                            {
                                if (MyStr.Contains("("))
                                {
                                    CountBracket++;
                                    MyVal += MyStr.IndexOf("(") + 1;
                                    MyStr = MyStr.Substring(MyStr.IndexOf("(") + 1, MyStr.IndexOf(")") - (MyStr.IndexOf("(") + 1));
                                }
                                else
                                {
                                    int MinValue = 0;
                                    int MaxValue = 0;
                                    int OperatorId = 0;
                                    SearchForASign(ref MaxValue, MyStr, ref OperatorId);

                                    string str = "";
                                    int StartValue = 0;
                                    int EndValue = 0;

                                    float Num1 = 0, Num2 = 0;
                                    bool LogicNum1 = false, LogicNum2 = false;
                                    for (int Val = MyStr.Length - 1; Val > -1; Val--)
                                    {
                                        if (MaxValue > Val)
                                        {
                                            if (MyStr[Val] == '$')
                                            {
                                                MinValue = Val;
                                                ItNumber = false;
                                                break;
                                            }
                                            if (MyStr[Val] == '#')
                                            {
                                                MinValue = Val;
                                                ItNumber = true;
                                                break;
                                            }
                                        }
                                    }
                                    StartValue = MinValue;
                                    str = MyStr.Substring(MinValue + 1, MaxValue - 1 - (MinValue + 1));
                                    if (!ItNumber)
                                        SearchVar(str, ref Num1);
                                    else
                                    {
                                        if (OperatorId < 7)
                                            Num1 = str.ToFloat();
                                        else
                                            LogicNum1 = str.ToBool();
                                    }

                                    for (int Val = 0; Val < MyStr.Length; Val++)
                                    {
                                        if (MaxValue < Val)
                                        {
                                            if (MyStr[Val] == '€')
                                            {
                                                MinValue = Val;
                                                ItNumber = false;
                                                break;
                                            }
                                            if (MyStr[Val] == '№')
                                            {
                                                MinValue = Val;
                                                ItNumber = true;
                                                break;
                                            }
                                        }
                                    }
                                    EndValue = MinValue;
                                    str = MyStr.Substring(MaxValue + 3, MinValue - (MaxValue + 3));
                                    if (!ItNumber)
                                        SearchVar(str, ref Num2);
                                    else
                                    {
                                        if (OperatorId < 7)
                                            Num2 = str.ToFloat();
                                        else
                                            LogicNum2 = str.ToBool();
                                    }

                                    switch (OperatorId)
                                    {
                                        case 1: BoolResult = Num1 > Num2; break;
                                        case 2: BoolResult = Num1 < Num2; break;
                                        case 3: BoolResult = Num1 >= Num2; break;
                                        case 4: BoolResult = Num1 <= Num2; break;
                                        case 5: BoolResult = Num1 == Num2; break;
                                        case 6: BoolResult = Num1 != Num2; break;
                                        case 7: BoolResult = LogicNum1 & LogicNum2; break;
                                        case 8: BoolResult = LogicNum1 | LogicNum2; break;
                                        default: break;
                                    }

                                    MyStr = OperatorStr;
                                    MyStr = MyStr.Remove(StartValue + MyVal, EndValue - StartValue + 1);
                                    if (MyStr.Contains("()"))
                                        MyStr = MyStr.Replace("()", "#" + BoolResult + "№");
                                    else
                                        MyStr = MyStr.Insert(StartValue + MyVal, "#" + BoolResult + "№");
                                    MyVal = 0;
                                    OperatorStr = MyStr;

                                }
                            }
                        }
                    }

                }
                if (ChangeStr.StartsWith("else"))
                {


                    if (CountWhile != 0)
                        CountNeedRightBrace++;
                    if (CountNeedScipBrace != 0)
                        CountNeedScipBrace++;
                    if (CountNeedScipBrace == 0 && !Inverce)
                    {
                        if (IfRank > 0)
                            CountIf++;
                        if (CountIf == IfRank)
                        {
                            CountNeedScipBrace++;
                            IfRank--;
                        }
                    }

                }
                if (ChangeStr.StartsWith("while"))
                {
                    if (IfRank > 0)
                        CountIf++;
                    CountNeedRightBrace++;
                    if (Inverce && CountNeedRightBrace == (WhileRank + 1))
                    {
                        CountWhile--;
                        Inverce = false;
                        if (WhileRank > 0)
                            WhileRank--;
                    }
                    if (CountNeedScipBrace != 0)
                        CountNeedScipBrace++;
                    if (CountNeedScipBrace == 0 && !Inverce)
                    {
                        OperatorStr = ChangeStr.Substring(ChangeStr.IndexOf("while(") + 6, ChangeStr.LastIndexOf(")") - (ChangeStr.IndexOf("while(") + 6));
                        int MyVal = 0;
                        bool Finish = false;
                        int CountBracket = 0;
                        string MyStr = OperatorStr;
                        while (!Finish)
                        {
                            if (!MyStr.Contains("==") && !MyStr.Contains("!=") && !MyStr.Contains("&&") && !MyStr.Contains("||") && !MyStr.Contains("@>") && !MyStr.Contains("@<") && !MyStr.Contains(">=") && !MyStr.Contains("<="))
                            {
                                if (!BoolResult)
                                {
                                    CountNeedScipBrace++;
                                    if (CountWhile == 0)
                                        CountNeedRightBrace = 0;
                                }
                                else
                                {

                                    if (CountWhile != 0)
                                        WhileRank++;
                                    CountWhile++;
                                }
                                Finish = true;
                            }
                            else
                            {

                                if (MyStr.Contains("("))
                                {
                                    CountBracket++;
                                    MyVal += MyStr.IndexOf("(") + 1;
                                    MyStr = MyStr.Substring(MyStr.IndexOf("(") + 1, MyStr.IndexOf(")") - (MyStr.IndexOf("(") + 1));
                                }
                                else
                                {
                                    int MinValue = 0;
                                    int MaxValue = 0;
                                    int OperatorId = 0;

                                    SearchForASign(ref MaxValue, MyStr, ref OperatorId);
                                    {
                                        string str = "";
                                        int StartValue = 0;
                                        int EndValue = 0;

                                        float Num1 = 0, Num2 = 0;
                                        bool LogicNum1 = false, LogicNum2 = false;
                                        for (int Val = MyStr.Length - 1; Val > -1; Val--)
                                        {
                                            if (MaxValue > Val)
                                            {
                                                if (MyStr[Val] == '$')
                                                {
                                                    MinValue = Val;
                                                    ItNumber = false;
                                                    break;
                                                }
                                                if (MyStr[Val] == '#')
                                                {
                                                    MinValue = Val;
                                                    ItNumber = true;
                                                    break;
                                                }
                                            }
                                        }
                                        StartValue = MinValue;
                                        str = MyStr.Substring(MinValue + 1, MaxValue - 1 - (MinValue + 1));
                                        if (!ItNumber)
                                            SearchVar(str, ref Num1);
                                        else
                                        {
                                            if (OperatorId < 7)
                                                Num1 = str.ToFloat();
                                            else
                                                LogicNum1 = str.ToBool();
                                        }

                                        for (int Val = 0; Val < MyStr.Length; Val++)
                                        {
                                            if (MaxValue < Val)
                                            {
                                                if (MyStr[Val] == '€')
                                                {
                                                    MinValue = Val;
                                                    ItNumber = false;
                                                    break;
                                                }
                                                if (MyStr[Val] == '№')
                                                {
                                                    MinValue = Val;
                                                    ItNumber = true;
                                                    break;
                                                }
                                            }
                                        }
                                        EndValue = MinValue;
                                        str = MyStr.Substring(MaxValue + 3, MinValue - (MaxValue + 3));
                                        if (!ItNumber)
                                            SearchVar(str, ref Num2);
                                        else
                                        {
                                            if (OperatorId < 7)
                                                Num2 = str.ToFloat();
                                            else
                                                LogicNum2 = str.ToBool();
                                        }
                                        switch (OperatorId)
                                        {
                                            case 1: BoolResult = Num1 > Num2; break;
                                            case 2: BoolResult = Num1 < Num2; break;
                                            case 3: BoolResult = Num1 >= Num2; break;
                                            case 4: BoolResult = Num1 <= Num2; break;
                                            case 5: BoolResult = Num1 == Num2; break;
                                            case 6: BoolResult = Num1 != Num2; break;
                                            case 7: BoolResult = LogicNum1 & LogicNum2; break;
                                            case 8: BoolResult = LogicNum1 | LogicNum2; break;
                                            default: break;
                                        }
                                        MyStr = OperatorStr;
                                        MyStr = MyStr.Remove(StartValue + MyVal, EndValue - StartValue + 1);
                                        if (MyStr.Contains("()"))
                                            MyStr = MyStr.Replace("()", "#" + BoolResult + "№");
                                        else
                                            MyStr = MyStr.Insert(StartValue + MyVal, "#" + BoolResult + "№");
                                        MyVal = 0;
                                        OperatorStr = MyStr;
                                    }
                                }
                            }
                        }
                    }

                }
                if (ChangeStr.StartsWith("}"))
                {
                    if (CountIf > 0)
                        CountIf--;
                    if (CountNeedScipBrace != 0)
                        CountNeedScipBrace--;
                    if (CountWhile != 0)
                    {
                        CountNeedRightBrace--;
                        if (CountNeedRightBrace == WhileRank && CountNeedScipBrace == 0)
                            Inverce = true;
                    }
                }
                if (Inverce)
                    i--;
                else
                    i++;
            }
        CodeOn = true;
    }

    /// <summary>
    /// Проверка использования клавиш
    /// </summary>
    private void CheckKeys()
    {
        if (InfoCon.BeConstructor || autopilot)
            return;
            List<string>
        allKeysDown = new List<string>(),/*нажатие*/
        allKeysPress = new List<string>(),/*зажатие*/
        allKeysUp = new List<string>();/*отпускание*/
        // Получаем код клавиш  
        foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(key))
                allKeysDown.Add(key.ToString());
            if (Input.GetKey(key))
                allKeysPress.Add(key.ToString());
            if (Input.GetKeyUp(key))
                allKeysUp.Add(key.ToString());
        }
        //сверяем с контролером
        if (Keys.Length > 0)
            for (int key = 0; key < Keys.Length; key++)
            {
                switch (typeKeys[key])
                {
                    case '0': OnKeyDown(allKeysDown, key); break;//Задать значение
                    case '1': OnKeySwitch(allKeysDown, key); break;//Переключение значения на заданный или по умолчанию при клике
                    case '2': OnKeyTempPress(allKeysDown, allKeysUp, key); break;//Задать значение и возвращение значения по умолчанию при клике
                    default: break;
                }
            }
                
    }
    /// <summary>
    /// Присвоение свойств при нажатии определённой клавиши
    /// </summary>
    /// <param name="allKeysDown">все нажатые клавишы</param>
    /// <param name="idKey">индекс ключа</param>
    private void OnKeyDown(List<string> allKeysDown, int idKey) 
    {
        if (allKeysDown.Contains(Keys[idKey]))
            SetKeyPropertyes(idKey);
        if (FindKeyScriptComputers(Keys[idKey], 0) != -1)
            SetKeyScripts(idKey);
    }
    /// <summary>
    /// Присвоение свойств при зажатии определённой клавиши
    /// </summary>
    /// <param name="allKeysPress">все зажатые клавишы</param>
    /// <param name="idKey">индекс ключа</param>
    private void OnKeyPress(List<string> allKeysPress, int idKey)
    {
        if (allKeysPress.Contains(Keys[idKey]))
            SetKeyPropertyes(idKey);
        if (FindKeyScriptComputers(Keys[idKey], 1) != -1)
            SetKeyScripts(idKey);
    }
    /// <summary>
    /// Присвоение свойств при отпускании определённой клавиши
    /// </summary>
    /// <param name="allKeysUp">все отпусщенные клавишы</param>
    /// <param name="idKey">индекс ключа</param>
    private void OnKeyUp(List<string> allKeysUp, int idKey)
    {
        if (allKeysUp.Contains(Keys[idKey]))
            SetKeyPropertyes(idKey);
        if (FindKeyScriptComputers(Keys[idKey], 2) != -1)
            SetKeyScripts(idKey);
    }
    /// <summary>
    /// Присвоение свойств при переключении клавишей
    /// </summary>
    /// <param name="allKeysDown">все нажатые клавишы</param>
    /// <param name="idKey">индекс ключа</param>
    private void OnKeySwitch(List<string> allKeysDown, int idKey)
    {
        if (allKeysDown.Contains(Keys[idKey]))
            SetKeyPropertyes(idKey, TypeValueProperty.OldOrNewValue);
        if (FindKeyScriptComputers(Keys[idKey], 0) != -1)
            SetKeyScripts(idKey);
    }
    /// <summary>
    /// Присвоение свойств при возвращении значения по умолчанию при клике
    /// </summary>
    /// <param name="allKeysDown">все нажатые клавишы</param>
    /// <param name="allKeysUp">все отпусщенные клавишы</param>
    /// <param name="idKey">индекс ключа</param>
    private void OnKeyTempPress(List<string> allKeysDown, List<string> allKeysUp, int idKey)
    {
        if (allKeysUp.Contains(Keys[idKey]))
            SetKeyPropertyes(idKey, TypeValueProperty.OldValue);
        else
        if (allKeysDown.Contains(Keys[idKey]))
            SetKeyPropertyes(idKey, TypeValueProperty.NewValue);
        if (FindKeyScriptComputers(Keys[idKey], 0) != -1)
            SetKeyScripts(idKey);
    }

    /// <summary>
    /// присвоение значений к свойствам от привязок
    /// </summary>
    /// <param name="key">ключ</param>
    private void SetKeyPropertyes(int key, TypeValueProperty typeValue = TypeValueProperty.NewValue)
    {
        for (int NameD = 0; NameD < propertyParts[key].Length; NameD++)
            for (int idD = 1; idD < propertyParts[key][NameD].Length; idD++)
                for (int idProp = 1; idProp < propertyParts[key][NameD][idD].Length - 1; idProp += 2)
                SearchAndSetProperty(
                    propertyParts[key][NameD][0][0],
                    propertyParts[key][NameD][idD][0].ToInt(),
                    propertyParts[key][NameD][idD][idProp].ToInt(),
                    propertyParts[key][NameD][idD][idProp + 1].ToFloat(),
                    typeValue);
    }
    /// <summary>
    /// присвоение скриптов к компьютерам от привязок
    /// </summary>
    /// <param name="key">ключ</param>
    private void SetKeyScripts(int key)
    {
        DetailObject detail;
        for (int i = 0; i < SaveScripts[key].idComputers.Count; i++)
            if (SearchDetail("Computer", SaveScripts[key].idComputers[i], out detail))
            {
                DetailComputer comp = detail.GetComponent<DetailComputer>();
                comp.mainNameCode = SaveScripts[key].Scripts[i];
                OnChangeScript("Script" + comp.mainNameCode, ref comp.mainLineCode);
            }

    }
    /// <summary>
    /// Поиск свойства детали и присвоение значения
    /// </summary>
    /// <param name="nameDetail">имя детали</param>
    /// <param name="idDetail">индекс детали</param>
    /// <param name="idProperty">индекс свойства</param>
    /// <param name="value">значение</param>
    /// <returns>нашёл ли свойство детали</returns>
    private bool SearchAndSetProperty(string nameDetail, int idDetail, int idProperty, float value, TypeValueProperty typeValue)
    {
        for (int i = 0; i < Details.Count; i++)
            if (Details[i].NameDetail == nameDetail && Details[i].IndexDetail == idDetail)
            {
                DetailObject detailObj = Details[i].GetComponent<DetailObject>();
                if (idProperty < detailObj.ValProperties.Length)
                {
                    float oldValue;
                    switch (typeValue)
                    {
                        //case TypeValueProperty.NewValue:
                        //    break;
                        case TypeValueProperty.OldValue:
                            value = GetDetailClass(detailObj).ValProperties[idProperty];
                            break;
                        case TypeValueProperty.OldOrNewValue:
                            oldValue = GetDetailClass(detailObj).ValProperties[idProperty];
                            if (detailObj.ValProperties[idProperty] != oldValue)
                                value = oldValue;
                            break;
                        default:
                            break;
                    }
                    detailObj.ValProperties[idProperty] = value;
                    return true;
                }
                else
                    return false;
            }
        return false;
    }
    /// <summary>
    /// Получение стандартной детали
    /// </summary>
    /// <param name="detailPrototype">прототип</param>
    /// <returns>стандартная деталь</returns>
    DetailObject GetDetailClass(DetailObject detailPrototype)
    {
        for (int i = 0; i < save.MyDetails.Length; i++)
            if(save.MyDetails[i].NameDetail == detailPrototype.NameDetail)
                return save.MyDetails[i];
        Debug.LogError("запрос на несуществующую деталь");
        return null;
    }

    private void SearchForASign(ref int Max, string s, ref int OpId)
    {
        int Op1 = Int32.MaxValue, Op2 = Int32.MaxValue;
        if (s.Contains("@>") || s.Contains("@<") || s.Contains(">=") || s.Contains("<=") || s.Contains("==") || s.Contains("!="))
        {
            if (s.Contains("@>"))
                Op1 = s.IndexOf("@>");
            if (s.Contains("@<"))
                Op2 = s.IndexOf("@<");
            if (Op1 < Op2)
            {
                Max = Op1;
                OpId = 1;
            }
            else
            {
                Max = Op2;
                OpId = 2;
            }
            if (s.Contains(">="))
                Op1 = s.IndexOf(">=");
            if (Op1 < Max)
            {
                Max = Op1;
                OpId = 3;
            }
            if (s.Contains("<="))
                Op1 = s.IndexOf("<=");
            if (Op1 < Max)
            {
                Max = Op1;
                OpId = 4;
            }
            if (s.Contains("=="))
                Op1 = s.IndexOf("==");
            if (Op1 < Max)
            {
                Max = Op1;
                OpId = 5;
            }
            if (s.Contains("!="))
                Op1 = s.IndexOf("!=");
            if (Op1 < Max)
            {
                Max = Op1;
                OpId = 6;
            }
        }
        else
        if (s.Contains("&&"))
        {
            Max = s.IndexOf("&&");
            OpId = 7;
        }
        else
        if (s.Contains("||"))
        {
            Max = s.IndexOf("||");
            OpId = 8;
        }
    }
    /// <summary>
    /// Поиск свойства 2 из кода и манипуляции с ним
    /// </summary>
    /// <param name="str">часть кода</param>
    /// <param name="Num">свойство 1</param>
    /// <param name="BeChangeNum">свойство 1 присваевает значение к свойству 2?</param>
    private void SearchVar(string str, ref float Num, bool BeChangeNum = false)
    {

        int start = str.IndexOf("[");
        int end = str.IndexOf("]");
        string Name = str.Substring(0, start);
        int Id = 0;
        string Index = str.Substring(start + 1, end - start - 1);
        try
        {
            Id = Index.ToInt();
        }
        catch (FormatException)
        {
            switch (Index)
            {
                case "A": Id = (int)A; break;
                case "B": Id = (int)B; break;
                case "C": Id = (int)C; break;
                case "D": Id = (int)D; break;
                case "E": Id = (int)E; break;
                case "F": Id = (int)F; break;
                case "G": Id = (int)G; break;
                default: Id = 0; break;
            }
        }
        string Propery = str.Substring(end + 2);
        foreach (DetailObject detail in Details)
        {
            if (detail.NameDetail == Name && detail.IndexDetail == Id)
            {
                if (!BeChangeNum)
                {
                    for (int ii = 0; ii < detail.NameReadProperties.Length; ii++)
                        if (detail.NameReadProperties[ii] == Propery)
                        {
                            Num = detail.ValReadProperties[ii];
                            break;
                        }
                    for (int ii = 0; ii < detail.NameProperties.Length; ii++)
                        if (detail.NameProperties[ii] == Propery)
                        {
                            Num = detail.ValProperties[ii];
                            break;
                        }
                }
                else
                {
                    for (int ii = 0; ii < detail.NameProperties.Length; ii++)
                        if (detail.NameProperties[ii] == Propery)
                        {
                            detail.ValProperties[ii] = Num;
                            break;
                        }
                }
            }
        }
    }
}