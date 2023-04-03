using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerInterface : MonoBehaviour
{
    public enum AnalyticState
    {
        Off,
        Electric,
        Mass,
        Health
    }
    public AnalyticState State;
    private Color32
        controlColor = new Color32(38, 168, 141, 100),
        green = new Color32(73, 245, 122, 200),
        red = new Color32(245, 73, 73, 200),
        orange = new Color32(250, 132, 22, 200),
        blue = new Color32(64, 245, 233, 200),
        purple = new Color32(163, 64, 245, 200);
    private Vector3 pos, ScreenPos;
    public GameManagement game { get; set; }
    public Dropdown dropdownRobots, dropdownBases;
    public Text titleNameRobot, titleNameDetail;
    public ToggleGroup states;
    public Toggle autopilotToggle, swichBaseToggle;
    public Text[] resurcesLabels = new Text[ResourcesStat.countResource];
    private List<InputField> PropertyInputs = new List<InputField>();
    private List<Text> readPropertyTexts = new List<Text>();
    public Transform contentBase, contentControl;
    public GameObject robotBuy, controlDetailInterface;
    public Parameter parameterWrite, parameterRead;
    public List<RobotManager> MyRobots;
    public List<BaseController> MyBases;
    private DetailComputer targetMainComp;
    private RobotManager targetRobot;
    private bool autopilot = true;
    private int idTargetRobot, indexNewRobot;
    public int myIdPlayer { get; set; }
    public int targetBase { get; set; }
    public CameraOperator Cam;
    private Transform cameraTr;
    public List<DetailObject> controlDetails = new List<DetailObject>();
    public DescriptionManager descriptionManager;

    private void Awake()
    {
        cameraTr = Camera.main.transform;
    }
    private void OnEnable()
    {
        Camera.main.orthographicSize = Cam.MaxZoom;
        swichBaseToggle.GetComponent<G1SwitchG2>().OnChangedToggleValue(true);
        swichBaseToggle.isOn = true;
        UpdateSwitchBaseToRobot(baseTarget: true);
    }
    private void Update()
    {
            DetailTargetUpdate();
            OnTargetRobotOnClick();
        if (State != AnalyticState.Off)
            OnAnalyticRobot();
    }
    private void OnGUI()
    {
        Camera cameraMain = Camera.main;
        Event e = Event.current;
        Vector2 mousePos = new Vector2();
        mousePos.x = e.mousePosition.x;
        mousePos.y = cameraMain.pixelHeight - e.mousePosition.y;
        ScreenPos = new Vector3(mousePos.x, mousePos.y, cameraMain.nearClipPlane);
        pos = cameraMain.ScreenToWorldPoint(ScreenPos);
    }

    public void SetStateElectric(bool on)
    {
        OnState(on);
        State = on ? AnalyticState.Electric : AnalyticState.Off;
    }
    public void SetStateMass(bool on)
    {
        OnState(on);
        State = on ? AnalyticState.Mass : AnalyticState.Off;
    }
    public void SetStateHealth(bool on)
    {
        OnState(on);
        State = on ? AnalyticState.Health : AnalyticState.Off;
    }
    private void OnState(bool on)
    {
        if(on)
            controlDetailInterface.SetActive(false);
        ActiveAllSolidImages(on);
    }
    private void OnAnalyticRobot()
    {
        switch (State)
        {
            case AnalyticState.Electric:
                OnAnalyticElectric();
                break;
            case AnalyticState.Mass:
                OnAnalyticMass();
                break;
            case AnalyticState.Health:
                OnAnalyticHealth();
                break;
            default:
                break;
        }
    }
    public void OnAnalyticElectric()
    {
        if (targetRobot == null || !targetRobot.mainComputer || targetRobot.mainComputer.Details.Count == 0)
            return;
        List<DetailObject> details = targetRobot.mainComputer.Details;
        for (int i = 0; i < details.Count; i++)
        {
            if (!details[i].electricity)
            {
                details[i].SetColorSolidImages(red);
                continue;
            }
            if (details[i].NameDetail == "Computer")
            {
                details[i].SetColorSolidImages(green);
                continue;
            }
            if (details[i].NameDetail == "Generator")
            {
                Color32 greenInvisible = new Color32(green.r, green.g, green.b, 0);
                DetailGenerator generator = details[i].GetComponent<DetailGenerator>();
                details[i].SetColorSolidImages(Color32.Lerp(greenInvisible, green, generator.ElectricityGenerate / generator.ElectricityGenerateMax));
                continue;
            }
            if (details[i].NameDetail == "Battery")
            {
                Color32 blueInvisible = new Color32(blue.r, blue.g, blue.b, 0);
                DetailBattery battery = details[i].GetComponent<DetailBattery>();
                details[i].SetColorSolidImages(Color32.Lerp(blueInvisible, blue, battery.AmountElectricity / battery.AmountElectricityMax));
                continue;
            }
            Color32 purpleInvisible = new Color32(purple.r, purple.g, purple.b, 0);
            details[i].SetColorSolidImages(Color32.Lerp(purpleInvisible, purple, details[i].ValReadProperties[3] / details[i].ValReadProperties[4]));
        }
    }
    public void OnAnalyticMass()
    {
        if (targetRobot == null || !targetRobot.mainComputer || targetRobot.mainComputer.Details.Count == 0)
            return;
        List<DetailObject> details = targetRobot.mainComputer.Details;
        float maxMass = details[0].ValReadProperties[2];
        for (int i = 0; i < details.Count; i++)
            if(maxMass < details[i].ValReadProperties[2])
                maxMass = details[i].ValReadProperties[2];
        Color32 orangeInvisible = new Color32(orange.r, orange.g, orange.b, 0);
        for (int i = 0; i < details.Count; i++)
            details[i].SetColorSolidImages(Color32.Lerp(orangeInvisible, orange, details[i].ValReadProperties[2] / maxMass));
    }
    public void OnAnalyticHealth()
    {
            if (!targetRobot || !targetRobot.mainComputer || targetRobot.mainComputer.Details.Count == 0)
                return;
        List<DetailObject> details = targetRobot.mainComputer.Details;
        for (int i = 0; i < details.Count; i++)
        {
            details[i].SetColorSolidImages(Color32.Lerp(red, green, details[i].ValReadProperties[0]/ details[i].ValReadProperties[1]));
        }
    }
    private void DetailTargetUpdate()
    {
        if (controlDetails.Count > 0)
            controlDetails.RemoveAll((DetailObject detail) => detail == null);
        if (targetRobot == null || controlDetails.Count == 0)
            return;
        string lastPropertyValue;
        for (int i = 0; i < PropertyInputs.Count; i++)
        {
            if (!PropertyInputs[i].isFocused)
            {
                lastPropertyValue = controlDetails[0].ValProperties[i].ToString();
                PropertyInputs[i].text = lastPropertyValue;
                for (int indexDetail = 0; indexDetail < controlDetails.Count; indexDetail++)
                    if (lastPropertyValue != controlDetails[indexDetail].ValProperties[i].ToString())
                    {
                        PropertyInputs[i].text = "разные значения";
                        break;
                    }
            }

        }
        for (int i = 0; i < readPropertyTexts.Count; i++)
        {
            lastPropertyValue = controlDetails[0].ValReadProperties[i].ToString();
            readPropertyTexts[i].text = lastPropertyValue;
            for (int indexDetail = 0; indexDetail < controlDetails.Count; indexDetail++)
                if (lastPropertyValue != controlDetails[indexDetail].ValReadProperties[i].ToString())
                {
                    readPropertyTexts[i].text = "разные значения";
                    break;
                }
        }
    }
    public void OnTargetRobotOnClick()
    {
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            DetailObject detail;
            RobotManager robot;
            (robot, detail) = RaycastRobot();
            if (robot != null)
            {
                if (State == AnalyticState.Off)
                {
                    if (robot == targetRobot)
                    {
                        if (Input.GetKey(KeyCode.LeftShift))
                            OnTargetDetails(detail);
                        else
                            OnTargetDetail(detail);
                    }
                    else
                    {
                        if (controlDetails.Count > 0)
                            OnTargetDetail(controlDetails[0]);
                    }
                }
                SetTargetRobot(robot);
                swichBaseToggle.isOn = false;
            }
        }
    }
    /// <summary>
    /// Очистка контролируемых деталей
    /// </summary>
    private void ClearControlDetails()
    {
        for (int id = 0; id < controlDetails.Count; id++)
            controlDetails[id].ActiveSolidImages(false);
        controlDetails.Clear();
    }
    /// <summary>
    /// Добавление контролируемой детали
    /// </summary>
    /// <param name="detail">деталь</param>
    private void AddControlDetail(DetailObject detail)
    {
        detail.ActiveSolidImages(true);
        detail.SetColorSolidImages(controlColor);
        controlDetails.Add(detail);
    }
    private void ActiveAllSolidImages(bool on)
    {
        if (!targetRobot || !targetRobot.mainComputer || targetRobot.mainComputer.Details.Count == 0)
            return;
        List<DetailObject> details = targetRobot.mainComputer.Details;
        for (int i = 0; i < details.Count; i++)
            details[i].ActiveSolidImages(on);
    }
    private void OnTargetDetail(DetailObject detail)
    {
        bool detailExist = controlDetails.Contains(detail);
        controlDetailInterface.SetActive(!detailExist);
        ClearControlDetails();
        if (detailExist)
            return;
        AddControlDetail(detail);

        detail.ValidateOnDescription();
        titleNameDetail.GetComponent<Description>().SetDescription(detail.descriptionDetail.text, detail.visualisationDetail);
        titleNameDetail.text = detail.labelDetail.text[(int)Language.language] + ": " + detail.IndexDetail;
        CreateParameters(detail);
    }

    private void OnTargetDetails(DetailObject detail)
    {
        if (controlDetails.Count > 0 && detail.GetType() != controlDetails[0].GetType())
            return;
        bool detailExist = controlDetails.Contains(detail);
        if (detailExist)
        {
            detail.ActiveSolidImages(false);
            controlDetails.Remove(detail);
        }
        else
            AddControlDetail(detail);
        controlDetailInterface.SetActive(controlDetails.Count > 0);
        if (controlDetails.Count == 0)
            return;

        detail.ValidateOnDescription();
        titleNameDetail.GetComponent<Description>().SetDescription(detail.descriptionDetail.text, detail.visualisationDetail);
        titleNameDetail.text = detail.descriptionDetail.text[(int)Language.language] + ": " + detail.IndexDetail;
        if (controlDetails.Count > 1)
            for (int id = 1; id < controlDetails.Count; id++)
                titleNameDetail.text += ", " + controlDetails[id].IndexDetail;
            
        CreateParameters(detail);
    }
    private void CreateParameters(DetailObject detail)
    {
        if (contentControl.childCount > 0)
            for (int i = 0; i < contentControl.childCount; i++)
                Destroy(contentControl.GetChild(i).gameObject);
        PropertyInputs.Clear();
        readPropertyTexts.Clear();
        if (detail.NameProperties[0] != "NULL")
            for (int i = 0; i < detail.NameProperties.Length; i++)
            {
                Parameter parameter = Instantiate(parameterWrite, contentControl);
                parameter.player = this;
                parameter.title.GetComponent<Description>().SetDescription(descriptionManager, detail.descriptionsProperty[i].text, detail.visualisationProperty[i]);
                parameter.title.text = detail.labelProperty[i].text[(int)Language.language];
                parameter.id = i;
                PropertyInputs.Add(parameter.inputValue);
                parameter.inputValue.text = detail.ValProperties[i].ToString();
            }
        for (int i = 0; i < detail.NameReadProperties.Length; i++)
        {
            Parameter parameter = Instantiate(parameterRead, contentControl);
            parameter.player = this;
            parameter.title.GetComponent<Description>().SetDescription(descriptionManager, detail.descriptionsReadProperty[i].text, detail.visualisationReadProperty[i]);
            parameter.title.text = detail.labelReadProperty[i].text[(int)Language.language];
            parameter.id = i;
            readPropertyTexts.Add(parameter.readValue);
            parameter.readValue.text = detail.ValReadProperties[i].ToString();
        }
    }
    public void OnChangeProperty(int id, float value)
    {
        if (controlDetails.Count == 0)
            return;
        for (int indexDetail = 0; indexDetail < controlDetails.Count; indexDetail++)
        {
            if (id >= controlDetails[indexDetail].ValProperties.Length)
            {
                Debug.LogError("ты вышел за границы массива");
                return;
            }
            controlDetails[indexDetail].ValProperties[id] = value;
            //controlDetails[indexDetail].ValProperties[id] = value;
        }

    }

    private (RobotManager, DetailObject) RaycastRobot()
    {
        Vector3 EndRay = cameraTr.forward * 110f;
        Vector3 StartRay = pos;
        RaycastHit2D[] hitsRay = Physics2D.RaycastAll(StartRay, EndRay, LayerMask.GetMask("Detail"));
        for (int i = 0; i < hitsRay.Length; i++)
            if (hitsRay[i].transform.GetComponent<DetailObject>()/*это деталь*/)
            {
                DetailObject detail = hitsRay[i].transform.GetComponent<DetailObject>();
                if (detail.MainBrain != null)
                {
                    RobotManager robot = detail.transform.parent.GetComponent<RobotManager>();
                    if (robot.idPlayer == myIdPlayer)
                        return (robot, detail);
                }
            }
        return (null, null);
    }
    public void Clear()
    {
        controlDetailInterface.SetActive(false);
        indexNewRobot = 0;
        autopilot = true;
        autopilotToggle.isOn = true;
        MyRobots.Clear();
        MyBases.Clear();
        dropdownRobots.ClearOptions();
        dropdownBases.ClearOptions();
        for (int id = 0; id < contentBase.childCount; id++)
            Destroy(contentBase.GetChild(id).gameObject);
    }
    public void Create(SaveArea save)
    {
        string[] names = save.all.RobotNames.ToArray();
        if (names != null && names.Length > 0)
            for (int i = 0; i < names.Length; i++)
            {
                BuyRobot buy = Instantiate(robotBuy, contentBase).GetComponent<BuyRobot>();
                buy.player = this;
                buy.nameRobot.text = names[i].Substring(5);
                buy.resourcesValue = save.all.needResourcesForRobots[i].needResources;
                for (int ii = 0; ii < ResourcesStat.countResource; ii++)
                    buy.resources[ii].text = buy.resourcesValue[ii].ToString();
            }
    }
    public void AddTargetRobot(RobotManager newRobot)
    {
        indexNewRobot++;
        MyRobots.Add(newRobot);
        newRobot.name += " " + indexNewRobot;
        dropdownRobots.options.Add(new Dropdown.OptionData(newRobot.name));
        dropdownRobots.value = dropdownRobots.options.Count - 1;
        titleNameRobot.text = newRobot.name;
        SetTargetRobot(MyRobots.Count - 1);
        swichBaseToggle.isOn = false;
        return;
    }
    public void SetTargetRobot(int newTargetID)
    {
        states.SetAllTogglesOff();
        if (newTargetID >= MyRobots.Count || newTargetID < 0)
        {
            Debug.LogWarning("робот вне диапозона:" + newTargetID);
            return;
        }
        if (targetMainComp)
            targetMainComp.autopilot = true;
        idTargetRobot = newTargetID;
        targetMainComp = MyRobots[idTargetRobot].transform.GetChild(0).GetComponent<DetailComputer>();

        if (targetRobot != MyRobots[idTargetRobot].GetComponent<RobotManager>()) 
            ActiveAllSolidImages(false);
        targetRobot = MyRobots[idTargetRobot].GetComponent<RobotManager>();
        SetAutopilot(autopilot);
        Cam.TargetPos(MyRobots[idTargetRobot].transform, true);
    }
    public void SetTargetRobot(RobotManager newTargetRobot)
    {
        states.SetAllTogglesOff();
        if (!MyRobots.Contains(newTargetRobot))
        {
            Debug.LogWarning("робот " + newTargetRobot.name + " не наш");
            return;
        }
        if (targetMainComp)
            targetMainComp.autopilot = true;
        idTargetRobot = MyRobots.IndexOf(newTargetRobot);
        targetMainComp = MyRobots[idTargetRobot].mainComputer;
        if(targetRobot != newTargetRobot)
            ActiveAllSolidImages(false);
        targetRobot = newTargetRobot;
        dropdownRobots.value = idTargetRobot;
        SetAutopilot(autopilot);
        Cam.TargetPos(MyRobots[idTargetRobot].transform, true);
    }
    public void SetAutopilot(bool be)
    {
        autopilot = be;
        if (targetMainComp)
            targetMainComp.autopilot = be;
        if (be)
            Cam.GoToTarget();
        else
            Cam.limit = CameraOperator.Limits.point;
    }
    /// <summary>
    /// при уничтожении робота
    /// </summary>
    /// <param name="destroyRobot">робот, который уничтожен</param>
    public void OnDestroyRobot(RobotManager destroyRobot)
    {
        //Debug.Log("робот уничтожен");
        if (MyRobots.Contains(destroyRobot))
        {
            MyRobots.Remove(destroyRobot);
            #region пересоздаёт список роботов
            dropdownRobots.ClearOptions();
            for (int i = 0; i < MyRobots.Count; i++)
                dropdownRobots.options.Add(new Dropdown.OptionData(MyRobots[i].name));
            #endregion
            if (MyRobots.Count > 0)
                idTargetRobot = MyRobots.Count - 1;
            else
                swichBaseToggle.isOn = true;
            SetTargetRobot(idTargetRobot);
            if (dropdownRobots.options.Count > 0)
            {
                dropdownRobots.value = dropdownRobots.options.Count - 1;
                titleNameRobot.text = dropdownRobots.options[dropdownRobots.value].text;
            }
            else
                titleNameRobot.text = "";
        }
    }

    public void AddTargetBase(BaseController newBase)
    {
        MyBases.Add(newBase);
        dropdownBases.options.Add(new Dropdown.OptionData(newBase.name));
        dropdownBases.value = dropdownRobots.options.Count - 1;
        SetTargetBase(MyBases.Count - 1);
    }
    public void SetTargetBase(int newTarget)
    {
        if (newTarget >= MyBases.Count || newTarget < 0)
        {
            Debug.LogError("база вне диапозона:" + newTarget);
            return;
        }
        targetBase = newTarget;
        Cam.TargetPos(MyBases[targetBase].transform, true);
        Cam.GoToTarget();
        UpdateLabelResourcesInTargetBase();
    }
    public void UpdateLabelResourcesInTargetBase()
    {
        for (int i = 0; i < ResourcesStat.countResource; i++)
            resurcesLabels[i].text = MyBases[targetBase].GetComponent<ResourceDetail>().resourceComponents[i].ToString();
    }
    /// <summary>
    /// при переключении цели камеры с базы на робота
    /// </summary>
    /// <param name="baseTarget">переключение на базу?</param>
    public void UpdateSwitchBaseToRobot(bool baseTarget)
    {
        ActiveAllSolidImages(false);
        states.SetAllTogglesOff();
        if (baseTarget)
        {
            controlDetailInterface.SetActive(false);
            if (MyBases.Count > 0)
            {
                Cam.TargetPos(MyBases[targetBase].transform, true);
                Cam.GoToTarget();
            }
        }
        else
            if (MyRobots.Count > 0)
        {
            SetAutopilot(autopilot);
            Cam.TargetPos(MyRobots[idTargetRobot].transform, true);
        }
    }
}
