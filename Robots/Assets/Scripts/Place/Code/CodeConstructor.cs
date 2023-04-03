using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CodeConstructor : MonoBehaviour {
    #region variables
    bool lastBrace, wantBrace, beChange;
    public bool ChangeVariable { get; set; }
    public bool Subprogramme { get; set; }
    public bool If { get; set; }
    public bool While { get; set; }
    public bool BeChangeScript { get; set; }
    public bool NeedArithmetic;

    GameObject MyTextBlock, MyTextBlock1, MyTextBlock2, MyTextBlock3, MyTextBlock4, VisibleTextBlock;
    public GameObject Variables, Operators, TextBlock, RightBrace, LeftBrace, Number;
    public List<GameObject> TextBlocks;

    SaveArea saveArea;
    Text ShowText;
    public Transform Content, Space;
    public Button OkBut, EndBut, SubprogrammeEndBut, SaveBut;
    public ContentSizeFitter SizeFitter;
    public CreateSubprogrammeTextBlock createSubprogramme;

    public string InterfaceText = "", RealText = "", LookText, LookCode, NameCode;
    public List<string> InterfaceCode;
    public List<string> RealCode;

    public int ChangeId { get; set; }
    public int IdPosition { get; set; }
    public int LookId { get; set; }
    public int CountNeedBrace = 0, CountVar = 0;
    CodeManager codeManager;
    #endregion
    void Awake() {
        StartCoroutine(RestartSizeFitter());
        Info.LogicCompare = true;
        Info.Variable = true;
        Info.TextCodeEnd = null;
        MyTextBlock = null;
        MyTextBlock1 = null;
        MyTextBlock2 = null;
        codeManager = GetComponent<CodeManager>();
        saveArea = codeManager.saveArea;
    }
    void OnEnable()
    {
        for (int i = 0; i < TextBlocks.Count; i++)
            Destroy(TextBlocks[i]);
        TextBlocks.Clear();
        RealCode.Clear();
        InterfaceCode.Clear();
        if (BeChangeScript)
            ChangeScript();
    }
    /// <summary>
    /// Редактирование загруженного кода
    /// </summary>
    void ChangeScript()
    {
        codeManager.ChangedAdd();
        saveArea.LoadScript(NameCode, ref RealCode, ref InterfaceCode);
        for (int i = 0; i < RealCode.Count; i++)
            TextBlocks.Add(CreatorTextBlock(InterfaceCode[i]));
        Space.SetAsLastSibling();
        LookId = 0;
    }
    /// <summary>
    /// При оканчании создания блока кода
    /// </summary>
    void EndMethod()
    {
        StartCoroutine(RestartSizeFitter());
        CountVar = 0;
        CountNeedBrace = 0;
        Info.ReadVariable = false;
        Info.MyText = null;
        Info.TextCodeStart = null;
        Info.TextCodeEnd = null;
        Info.TextInterfaceStart = null;
        Info.TextInterfaceEnd = null;
        Info.Variable = true;
        Info.LogicCompare = true;
        Info.IfOrFor = false;
        Info.BeLeftBrace = false;
        Destroy(VisibleTextBlock);
        VisibleTextBlock = null;
        ChangeVariable = false;
        beChange = false;
        Variables.SetActive(true);
        Operators.SetActive(false);
        Subprogramme = false;
        If = false;
        While = false;
        LookCode = "";
        LookText = "";
        InterfaceText = "";
        RealText = "";
        codeManager.contentGroup.SetAllTogglesOff();
    }
    /// <summary>
    /// Проверяет можно ли осуществить оканчание создания блока кода
    /// </summary>
    void CheckEndButAndWriteInTextBlock()
    {
        if(Subprogramme && Info.MyText != null)
            SubprogrammeEndBut.interactable = true;
        else
            SubprogrammeEndBut.interactable = false;
        if ((ChangeVariable || !Info.LogicCompare) && (CountNeedBrace == 0 && Info.ReadVariable && Info.Variable || lastBrace) && !wantBrace)
            EndBut.interactable = true;
        else
            EndBut.interactable = false;
    }
    /// <summary>
    /// Сохраняет код
    /// </summary>
    public void Save()
    {
        saveArea.SaveScript(NameCode, RealCode.ToArray(), InterfaceCode.ToArray());
    }
    /// <summary>
    /// Изменение блока кода
    /// </summary>
    public void Change()
    {
        beChange = true;
        ChangeId = LookId;
        TextBlocks[ChangeId].SetActive(false);
        if (RealCode[LookId].StartsWith("$"))
            ChangeVariable = true;
        if (RealCode[LookId].StartsWith("if"))
            If = true;
        if (RealCode[LookId].StartsWith("while"))
            While = true;
        codeManager.FindTypeBlock();
    }
    /// <summary>
    /// Удаление блока кода
    /// </summary>
    public void Delete()
    {
        int StartId = LookId;
        if (!RealCode[StartId].StartsWith("$"))
        {
            int EndId = SearchRange(StartId);
            for (int i = StartId; i <= EndId; i++)
            {
                Destroy(TextBlocks[StartId]);
                RealCode.RemoveAt(StartId);
                InterfaceCode.RemoveAt(StartId);
                TextBlocks.RemoveAt(StartId);
            }
        }
        else
        {
            Destroy(TextBlocks[StartId]);
            RealCode.RemoveAt(StartId);
            InterfaceCode.RemoveAt(StartId);
            TextBlocks.RemoveAt(StartId);
        }
        if (TextBlocks.Count > 0)
            TextBlocks[0].GetComponent<Toggle>().isOn = true;
        else
            codeManager.BaseComponent();
        LookId = 0;
    }
    /// <summary>
    /// выйти в меню кода
    /// </summary>
    public void Back()
    {
        if (beChange)
            TextBlocks[ChangeId].SetActive(true);
        bool allNot = !(If || While || ChangeVariable || Subprogramme) && !VisibleTextBlock;
        codeManager.Home(!(Content.childCount > 2 || (allNot && Content.childCount > 1)));//выйти в меню кода
        EndMethod();
    }
    public void Menu()
    {
        BeChangeScript = false;
        createSubprogramme.Created = false;
        codeManager.CanvasMenu.SetActive(true);
    }

    void SubprogrammeEnd()
    {
        if (beChange)
            AddTextBlock();
        else
        {
            MyTextBlock = CreatorTextBlock(InterfaceText);
            if (TextBlocks.Count == 0)
            {
                InterfaceCode.Add(InterfaceText);
                RealCode.Add(RealText);
                TextBlocks.Add(MyTextBlock);
            }
            else
            {
                InterfaceCode.Insert(IdPosition, InterfaceText);
                RealCode.Insert(IdPosition, RealText);
                TextBlocks.Insert(IdPosition, MyTextBlock);
            }
        }
        EndMethod();
    }
    void IfOrForEnd()
    {
            string BraceColor = RandomColorIn16();
            InterfaceText = LookText + ")<color=#" + BraceColor + ">{</color>";
            RealText = LookCode + "){";
            string BraceStr = "<color=#" + BraceColor + ">}</color>";
            if (beChange)
                AddTextBlock();
            else
        {
                if (If)
                {
                    MyTextBlock4 = CreatorTextBlock(BraceStr);
                    MyTextBlock3 = CreatorTextBlock("else<color=#" + BraceColor + ">{</color>");
                }
                MyTextBlock2 = CreatorTextBlock(BraceStr);
                MyTextBlock1 = CreatorTextBlock(InterfaceText);
                if (TextBlocks.Count == 0)
                {
                    InterfaceCode.Add(InterfaceText);
                    RealCode.Add(RealText);
                    InterfaceCode.Add(BraceStr);
                    RealCode.Add("}");
                    if (If)
                    {
                        InterfaceCode.Add("else<color=#" + BraceColor + ">{</color>");
                        RealCode.Add("else{");
                        InterfaceCode.Add(BraceStr);
                        RealCode.Add("}");
                    }


                    TextBlocks.Add(MyTextBlock1);
                    TextBlocks.Add(MyTextBlock2);
                    if (If)
                    {
                        TextBlocks.Add(MyTextBlock3);
                        TextBlocks.Add(MyTextBlock4);
                    }

                }
                else
                {
                    if (If)
                    {
                        InterfaceCode.Insert(IdPosition, BraceStr);
                        InterfaceCode.Insert(IdPosition, "else<color=#" + BraceColor + ">{</color>");
                        RealCode.Insert(IdPosition, "}");
                        RealCode.Insert(IdPosition, "else{");
                    }
                    InterfaceCode.Insert(IdPosition, BraceStr);
                    InterfaceCode.Insert(IdPosition, InterfaceText);
                    RealCode.Insert(IdPosition, "}");
                    RealCode.Insert(IdPosition, RealText);
                    if (If)
                    {
                        TextBlocks.Insert(IdPosition, MyTextBlock4);
                        TextBlocks.Insert(IdPosition, MyTextBlock3);
                    }
                    TextBlocks.Insert(IdPosition, MyTextBlock2);
                    TextBlocks.Insert(IdPosition, MyTextBlock1);
                }
            }
            EndMethod();
    }
    void ChangeVariableEnd()
    {
        InterfaceText = LookText;
        RealText = LookCode;
        if (CountVar == 0)
            RealText += "+#0№";
        if (beChange)
            AddTextBlock();
        else
        {
            MyTextBlock = CreatorTextBlock(InterfaceText);
            if (TextBlocks.Count == 0)
            {
                InterfaceCode.Add(InterfaceText);
                RealCode.Add(RealText);
                TextBlocks.Add(MyTextBlock);
            }
            else
            {
                InterfaceCode.Insert(IdPosition, InterfaceText);
                RealCode.Insert(IdPosition, RealText);
                TextBlocks.Insert(IdPosition, MyTextBlock);
            }
        }
        EndMethod();
    }
    public void ChooseEnd()
    {
        if (If || While)
            IfOrForEnd();
        if (ChangeVariable)
            ChangeVariableEnd();
        if (Subprogramme)
            SubprogrammeEnd();
    }
    void AddTextBlock()
    {
        TextBlocks[ChangeId].GetComponent<Text>().text = InterfaceText;
        TextBlocks[ChangeId].SetActive(true);
        InterfaceCode[ChangeId] = InterfaceText;
        RealCode[ChangeId] = RealText;
    }

    void IfOrForOk()
    {
            InterfaceText = LookText;
            RealText = LookCode;
            if (Info.Variable)
            {
                Info.ReadVariable = true;
                if (!Info.BeLeftBrace)
                {
                    Info.MyText = null;
                    Info.Variable = false;
                }
                else
                    CountNeedBrace++;
            }
            else
            {
                Info.LogicCompare = !Info.LogicCompare;
                if (Info.MyText == ")")
                {
                    CountNeedBrace--;
                    Info.LogicCompare = false;
                }
                else
                    Info.Variable = true;

            }
        OperatorOrVariable();
    }
    void ChangeVariableOk()
    {
            InterfaceText = LookText;
            RealText = LookCode;
            if (Info.Variable)
            {
                if (!Info.ReadVariable)
                    Info.ReadVariable = true;
                else
                {
                    if (!Info.BeLeftBrace)
                    {
                        CountVar++;
                        Info.Variable = false;
                        Info.MyText = null;
                    }
                    else
                    {
                        NeedArithmetic = true;
                        CountNeedBrace++;
                    }
                }
            }
            else
            {
                if (Info.MyText != ")")
                {
                    Info.Variable = true;
                    NeedArithmetic = false;
                }
                else
                    CountNeedBrace--;
            }
        OperatorOrVariable();
    }
    public void ChooseOk()
    {
        if (If || While)
            IfOrForOk();
        if (ChangeVariable)
            ChangeVariableOk();
    }
    void OperatorOrVariable()
    {
        if (Info.Variable)
        {
            Variables.SetActive(true);
            Operators.SetActive(false);
        }
        else
        {
            Variables.SetActive(false);
            Operators.SetActive(true);
        }
        Info.TextCodeEnd = null;
    }

    public IEnumerator RestartSizeFitter()
    {
        SizeFitter.enabled = false;
        for (int i = 0; i < 2; i++)
            yield return new WaitForEndOfFrame();
        SizeFitter.enabled = true;
    }
    public IEnumerator TransTextBlocks()
    {
        for (int i = 0; i < 2; i++)
            yield return new WaitForEndOfFrame();
        if (VisibleTextBlock)
        {
            if (beChange)
                VisibleTextBlock.transform.SetSiblingIndex(ChangeId);
            else
                VisibleTextBlock.transform.SetSiblingIndex(IdPosition);
        }
        if (MyTextBlock)
        {
            MyTextBlock.transform.SetSiblingIndex(IdPosition);
            MyTextBlock = null;
        }
        if (MyTextBlock1)
        {
            MyTextBlock1.transform.SetSiblingIndex(IdPosition);
            MyTextBlock2.transform.SetSiblingIndex(IdPosition + 1);
            MyTextBlock1 = null;
            MyTextBlock2 = null;
            if (MyTextBlock3)
            {
                MyTextBlock3.transform.SetSiblingIndex(IdPosition + 2);
                MyTextBlock4.transform.SetSiblingIndex(IdPosition + 3);
                MyTextBlock3 = null;
                MyTextBlock4 = null;
            }
        }
        Space.SetAsLastSibling();
    }

    GameObject CreatorTextBlock(string IT)
    {
        GameObject Block = Instantiate(TextBlock, Content);
        Block.GetComponent<OptionsBlockText>().codeManager = codeManager;
        Block.GetComponent<OptionsBlockText>().codeConstructor = this;
        Block.GetComponent<Text>().text = IT;
        Block.GetComponent<Toggle>().group = Content.GetComponent<ToggleGroup>();
        if (TextBlocks.Count == 0)
            Block.GetComponent<Toggle>().isOn = true;

        StartCoroutine(TransTextBlocks());
        return Block;
    }
    int SearchRange(int StartId)
    {
        bool Condition = false;
        int EndId = StartId, CountBrace = 1;
        if (RealCode[StartId].StartsWith("if"))
            Condition = true;
        for (int i = StartId + 1; i < RealCode.Count; i++)
        {
            if (RealCode[i].Contains("{"))
                CountBrace++;
            if (RealCode[i].Contains("}"))
                CountBrace--;
            if (CountBrace == 0)
            {
                EndId = i;
                break;
            }
        }
        if (Condition)
        {
            CountBrace = 1;
            StartId = EndId + 1;
            for (int i = StartId + 1; i < RealCode.Count; i++)
            {
                if (RealCode[i].Contains("{"))
                    CountBrace++;
                if (RealCode[i].Contains("}"))
                    CountBrace--;
                if (CountBrace == 0)
                {
                    EndId = i;
                    break;
                }
            }
        }

        return EndId;
    }
    string RandomColorIn16()
    {
        byte R1 = (byte)Random.Range(200, 255);
        byte R2 = (byte)Random.Range(100, 255);
        byte R3 = (byte)(R1 - (byte)Random.Range(100, 190));
        int ChooseR = Random.Range(0, 6);
        Color RandColor = Color.white;
        if (ChooseR == 0)
            RandColor = new Color32(R1, R3, R2, 255);
        if (ChooseR == 1)
            RandColor = new Color32(R2, R1, R3, 255);
        if (ChooseR == 2)
            RandColor = new Color32(R3, R2, R1, 255);
        if (ChooseR == 3)
            RandColor = new Color32(R2, R3, R1, 255);
        if (ChooseR == 4)
            RandColor = new Color32(R3, R1, R2, 255);
        if (ChooseR == 5)
            RandColor = new Color32(R1, R2, R3, 255);

        string StringColor = ColorUtility.ToHtmlStringRGB(RandColor);
        return StringColor;
    }
    public void OnOptionsTextBlock(OptionsBlockText optionsBlock)
    {
        if (ChangeVariable || If || While || Subprogramme)
        {
            optionsBlock.GetComponent<Toggle>().SetIsOnWithoutNotify(false);
            optionsBlock.operations.SetActive(false);
            return;
        }
        optionsBlock.operations.SetActive(true);
        if (RealCode.Count > LookId && LookId >= 0)
        {
            int txtId = LookId;
            string txt = RealCode[txtId];
            optionsBlock.addUp.SetActive(true);
            optionsBlock.addDown.SetActive(true);
            optionsBlock.change.SetActive(true);
            optionsBlock.delete.SetActive(true);
            if (txt == "}")
            {
                optionsBlock.delete.SetActive(false);
                optionsBlock.change.SetActive(false);
                if (RealCode.Count > (txtId + 1))
                    if (RealCode[txtId + 1] == "else{")
                        optionsBlock.addDown.SetActive(false);
            }
            if (txt == "else{")
            {
                optionsBlock.addUp.SetActive(false);
                optionsBlock.delete.SetActive(false);
                optionsBlock.change.SetActive(false);
            }
        }
    }
    void Update () {
        SaveBut.interactable = Content.childCount > 1;
        if (ChangeVariable || If || While || Subprogramme)
        {
            if (Info.MyText == "(" && Info.TextCodeStart != "")
                Info.MyText = "";
            Info.IfOrFor = If || While;
            #region фигурные скобки
            if (!Subprogramme)
            {
                if ((ChangeVariable && Info.ReadVariable) || (Info.IfOrFor && Info.LogicCompare))
                    LeftBrace.SetActive(true);
                else
                    LeftBrace.SetActive(false);

                if (CountNeedBrace == 0 || Info.Variable || (Info.LogicCompare && Info.IfOrFor) || (NeedArithmetic && !Info.IfOrFor))
                    RightBrace.SetActive(false);
                else
                    RightBrace.SetActive(true);
                lastBrace = false;
                wantBrace = false;
            }
            #endregion
            if ((Info.TextCodeEnd != null || Info.BeLeftBrace) && Info.Variable || Info.MyText != null && (!Info.Variable || Subprogramme))
            {
                if (!VisibleTextBlock)
                {
                    VisibleTextBlock = Instantiate(TextBlock, Content);
                    ShowText = VisibleTextBlock.GetComponent<Text>();
                    if (beChange)
                        VisibleTextBlock.transform.SetSiblingIndex(ChangeId);
                    else
                        VisibleTextBlock.transform.SetSiblingIndex(IdPosition);
                }
                if (ChangeVariable)
                {
                        if (Info.Variable)
                        {
                            if (Info.MyText == "")
                                Info.MyText = "0";
                            if (!Info.ReadVariable)
                            {
                                LookText = Info.TextInterfaceStart + Info.MyText + Info.TextInterfaceEnd + "   =   ";
                                LookCode = Info.TextCodeStart + Info.MyText + Info.TextCodeEnd + "=";
                            }
                            else
                            {
                                if (!Info.BeLeftBrace)
                                {
                                    if (Info.TextCodeEnd == "№")
                                    {
                                        LookText = InterfaceText + Info.TextInterfaceStart + Info.TextInterfaceEnd;
                                        LookCode = RealText + Info.TextCodeStart + Info.TextCodeEnd;
                                    }
                                    else
                                    {
                                        LookText = InterfaceText + Info.TextInterfaceStart + Info.MyText + Info.TextInterfaceEnd;
                                        LookCode = RealText + Info.TextCodeStart + Info.MyText + Info.TextCodeEnd;
                                    }
                                }
                                else
                                {
                                    wantBrace = true;
                                    LookText = InterfaceText + "(";
                                    LookCode = RealText + "(";
                                }
                            }
                        }
                        else
                        {
                            if (Info.MyText != ")")
                            {
                                LookText = InterfaceText + "   " + Info.MyText + "   ";
                                if (Info.MyText != "-")
                                    LookCode = RealText + Info.MyText;
                                else
                                    LookCode = RealText + "_";
                            }
                            else
                            {
                                if (CountNeedBrace - 1 == 0)
                                    lastBrace = true;
                                LookText = InterfaceText + ")";
                                LookCode = RealText + ")";
                            }
                        }
                }
                if (If || While)
                {
                        if (Info.Variable)
                        {
                            if (Info.MyText == "")
                                Info.MyText = "0";
                            if (!Info.BeLeftBrace)
                            {
                                if (!Info.ReadVariable)
                                {
                                    if (If)
                                    {
                                        if (Info.TextCodeEnd == "№")
                                        {
                                            LookText = "if(" + Info.TextInterfaceStart + Info.TextInterfaceEnd;
                                            LookCode = "if(" + Info.TextCodeStart + Info.TextCodeEnd;
                                        }
                                        else
                                        {
                                            LookText = "if(" + Info.TextInterfaceStart + Info.MyText + Info.TextInterfaceEnd;
                                            LookCode = "if(" + Info.TextCodeStart + Info.MyText + Info.TextCodeEnd;
                                        }
                                    }
                                    else
                                    {
                                        if (Info.TextCodeEnd == "№")
                                        {
                                            LookText = "while(" + Info.TextInterfaceStart + Info.TextInterfaceEnd;
                                            LookCode = "while(" + Info.TextCodeStart + Info.TextCodeEnd;
                                        }
                                        else
                                        {
                                            LookText = "while(" + Info.TextInterfaceStart + Info.MyText + Info.TextInterfaceEnd;
                                            LookCode = "while(" + Info.TextCodeStart + Info.MyText + Info.TextCodeEnd;
                                        }
                                    }
                                }
                                else
                                {
                                    if (Info.TextCodeEnd == "№")
                                    {
                                        LookText = InterfaceText + Info.TextInterfaceStart + Info.TextInterfaceEnd;
                                        LookCode = RealText + Info.TextCodeStart + Info.TextCodeEnd;
                                    }
                                    else
                                    {
                                        LookText = InterfaceText + Info.TextInterfaceStart + Info.MyText + Info.TextInterfaceEnd;
                                        LookCode = RealText + Info.TextCodeStart + Info.MyText + Info.TextCodeEnd;
                                    }
                                }
                            }
                            else
                            {
                                if (!Info.ReadVariable)
                                {
                                    if (If)
                                    {
                                        LookText = "if((";
                                        LookCode = "if((";
                                    }
                                    else
                                    {
                                        LookText = "while((";
                                        LookCode = "while((";
                                    }
                                }
                                else
                                {
                                    LookText = InterfaceText + "(";
                                    LookCode = RealText + "(";
                                }
                                wantBrace = true;
                            }
                        }
                        else
                        {
                            if (Info.MyText != ")")
                            {
                                LookText = InterfaceText + "   " + Info.MyText.Replace('@', ' ') + "  ";
                                LookCode = RealText + Info.MyText;
                            }
                            else
                            {
                                if (CountNeedBrace - 1 == 0)
                                    lastBrace = true;
                                LookText = InterfaceText + ")";
                                LookCode = RealText + ")";
                            }
                        }
                }
                if (Subprogramme)   
                {
                    LookText = Info.MyText + "();";
                    InterfaceText = LookText;
                    RealText = "`Script" + Info.MyText;
                    
                }
                else
                    OkBut.interactable = true;

                ShowText.text = LookText;
                CheckEndButAndWriteInTextBlock();
            }
            else
            {
                OkBut.interactable = false;
                EndBut.interactable = false;
                SubprogrammeEndBut.interactable = false;
            }
        }
    }
}
