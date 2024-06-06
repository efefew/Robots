using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class ConstructorManager : MonoBehaviour
{
    #region Properties

    public bool BeActiveOrientation { get; set; }

    /// <summary>
    /// при изменении робота
    /// </summary>
    public bool BeChangeRobot { get; set; }

    public bool BeDelete { get; set; }
    public bool BeIndexVisible { get; set; }
    public bool BeInfoDetailControler { get; set; }

    #endregion Properties

    #region Fields

    private const float scaleCoefficent = 16.66666f;
    private AreaScript areaScript, areaScriptFantom;
    private bool beArea, checkArea;
    private ControlerManager controler;
    private int Id;
    private DetailObject[] MyDetails;
    private Vector3 pos, ScreenPos;
    private Rigidbody2D RobotBody;
    private Transform trCamera;
    public List<GameObject> AllDetails, AllParentDetails, orientationButs, IndexButs;
    public List<int> AllIdDetails;
    public GameObject Area, AreaFantom;
    public GameObject BaseMenu, Robot, CanvasRobot, MainComputer, controls, detailSV, parametersObj;
    public Lesson lesson;
    public string NameRobot;
    public ShowParameters parameters;
    public List<string> RobotNameList;
    public SaveArea saveArea;
    public Material solidImage;
    public Toggle ToggleOrientation, TogglePosition, ToggleDelete;
    public ChangeValue typePositionBuild, typeRotateBuild;

    #endregion Fields

    #region Methods

    private void Awake()
    {
        MyDetails = saveArea.MyDetails;
        detailSV.SetActive(true);
        controls.SetActive(false);
        beArea = false;
        BeInfoDetailControler = true;
        BeDelete = false;
        ToggleOrientation.isOn = false;

        controler = GetComponent<ControlerManager>();
        areaScript = Area.GetComponent<AreaScript>();
        areaScriptFantom = AreaFantom.GetComponent<AreaScript>();
        RobotBody = Robot.GetComponent<Rigidbody2D>();
        MainComputer.GetComponent<DetailObject>().constructor = this;
        trCamera = Camera.main.transform;
    }

    private void OnDisable() => Clear();

    private void OnEnable()
    {
        RobotNameList.AddRange(saveArea.all.RobotNames);
        saveArea.GetComponent<GlobalSetting>().SetThePhysicsCalculationDelay(1);
        BaseMenu.SetActive(false);
        CanvasRobot.SetActive(true);
        AllDetails.Add(MainComputer);
        if (BeChangeRobot)//при изменении робота
        {
            BeChangeRobot = false;
            OnChangeRobot();
        }
        CanvasRobot.SetActive(true);
        for (int CountDetails = 0; CountDetails < AllDetails.Count; CountDetails++)
            AllDetails[CountDetails].GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        RobotBody.simulated = false;
    }

    private void OnGUI()
    {
        Camera c = Camera.main;
        Event e = Event.current;
        Vector2 mousePos = new()
        {
            x = e.mousePosition.x,
            y = c.pixelHeight - e.mousePosition.y
        };
        ScreenPos = new Vector3(mousePos.x, mousePos.y, c.nearClipPlane);
        pos = c.ScreenToWorldPoint(ScreenPos);
    }

    private void Update()
    {
        if (InfoCon.BeDelete != BeDelete)
        {
            InfoCon.BeDelete = BeDelete;
            _ = StartCoroutine(CheckConteinersOnChangeDeleteButton());
        }
        if (InfoCon.BeController)
        {
            OnController();
            return;
        }
        if (BeDelete)
        {
            CheckDelete();
            ClearAreas();
            return;
        }
        checkArea = beArea && !BeIndexVisible && !BeActiveOrientation;
        if (checkArea)
        {
            Area.SetActive(true);
            CheckCreate(Area, AreaFantom, areaScript, areaScriptFantom);
        }
        else
        {
            ClearAreas();
        }
    }

    /// <summary>
    /// Расчёт стоимости робота
    /// </summary>
    /// <returns>стоимость робота</returns>
    private float[] CalculateCountNeedResources()
    {
        float[] resources = new float[ResourcesStat.countResource];
        for (int i = 0; i < ResourcesStat.countResource; i++)
            resources[i] = 0;
        for (int idDetail = 0; idDetail < AllDetails.Count; idDetail++)
        {
            float[] resDetail = AllDetails[idDetail].GetComponent<ResourceDetail>().resourceComponents;
            for (int idResource = 0; idResource < ResourcesStat.countResource; idResource++)
                resources[idResource] += resDetail[idResource];
        }
        return resources;
    }

    /// <summary>
    /// Проверка контейнеров при изменении кнопки удаления
    /// </summary>
    /// <returns></returns>
    private IEnumerator CheckConteinersOnChangeDeleteButton()
    {
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        InfoCon.checkConteiners();
    }

    /// <summary>
    /// Проверка зоны на возможность построить деталь
    /// </summary>
    /// <param name="area">зона</param>
    /// <param name="areaFantom">зона-фантом</param>
    /// <param name="areaScript">скрипт зоны</param>
    /// <param name="areaScriptFantom">скрипт зоны-фантома</param>
    private void CheckCreate(GameObject area, GameObject areaFantom, AreaScript areaScript, AreaScript areaScriptFantom)
    {
        _ = areaScript.UpdateArea();
        Transform targetConteiner = RaycastHitConteiner(areaScript);//ячейка в курсоре
        if (ValidateArea(area.transform, areaScript, targetConteiner))
        {
            areaFantom.gameObject.SetActive(false);
            return;
        }
        if (areaScript.conteiners.Count < areaScript.CountConteinersNeed)
        {
            areaFantom.gameObject.SetActive(false);
            return;
        }

        _ = areaScriptFantom.UpdateArea();
        area.gameObject.SetActive(false);
        areaFantom.gameObject.SetActive(true);
        targetConteiner = areaScript.conteiners[0].transform;
        if (!ValidateArea(areaFantom.transform, areaScriptFantom, targetConteiner))
            SetAreaPosition(areaFantom.transform);
    }

    /// <summary>
    /// Проверка на возможность удаление деталей
    /// </summary>
    private void CheckDelete()
    {
        if (!Input.GetKey(KeyCode.Mouse0))
            return;
        DetailObject detailRay = CheckMouseRay();
        if (detailRay && detailRay.gameObject != MainComputer)
        {
            if (detailRay.imageConector)
                Destroy(detailRay.imageConector.gameObject);
            DestroyObj(detailRay.gameObject);
        }
    }

    /// <summary>
    /// Проверяет есть ли деталь под курсором
    /// </summary>
    /// <returns>деталь (если есть)</returns>
    private DetailObject CheckMouseRay()
    {
        Vector3 EndRay = Camera.main.transform.forward * 110f;
        Vector3 StartRay = pos;
        RaycastHit2D[] h = Physics2D.RaycastAll(StartRay, EndRay, LayerMask.GetMask("Detail"));
        if (h.Length > 0)
        {
            for (int i = 0; i < h.Length; i++)
            {
                if (h[i].collider.GetComponent<DetailObject>() && h[i].collider.gameObject.layer == 8)
                    return h[i].collider.GetComponent<DetailObject>();
            }
        }

        return null;
    }

    /// <summary>
    /// Очистка робота
    /// </summary>
    private void Clear()
    {
        typeRotateBuild.ValueChanged(8);
        ChangedRotateBuild();
        detailSV.SetActive(true);
        controls.SetActive(false);
        IndexButs.Clear();
        orientationButs.Clear();
        RobotNameList.Clear();
        beArea = false;
        BeInfoDetailControler = true;
        BeDelete = false;
        AllIdDetails.Clear();
        AllParentDetails.Clear();
        AllDetails.Clear();
        controler.saveScripts.Clear();
        UpdateControlerToggle(false);
        ToggleOrientation.isOn = false;
        ToggleDelete.isOn = false;
        if (MainComputer && MainComputer.transform.childCount > 0)
        {
            for (int child = 0; child < MainComputer.transform.childCount; child++)
            {
                if (MainComputer.transform.GetChild(child).name.Replace(" ", "") == "conector")
                    Destroy(MainComputer.transform.GetChild(child).gameObject);
            }
        }

        if (Robot && Robot.transform.childCount > 1)
        {
            for (int child = 1; child < Robot.transform.childCount; child++)
                Destroy(Robot.transform.GetChild(child).gameObject);
        }
    }

    /// <summary>
    /// Выключить все зоны
    /// </summary>
    private void ClearAreas()
    {
        Area.SetActive(false);
        AreaFantom.SetActive(false);
    }

    /// <summary>
    /// Убирает коллайдеры в объекте для визуализации детали в зоне строительства
    /// </summary>
    /// <param name="obj">объект</param>
    private void ClearColliders(Transform obj)
    {
        if (obj.GetComponent<Collider2D>())
            Destroy(obj.GetComponent<Collider2D>());
        if (obj.GetComponent<Joint2D>())
            Destroy(obj.GetComponent<Joint2D>());
        if (obj.GetComponent<Rigidbody2D>())
            obj.GetComponent<Rigidbody2D>().simulated = false;
    }

    /// <summary>
    /// Убирает коллайдеры в объекте и его наследниках и возвращает их изображения для визуализации детали в зоне строительства
    /// </summary>
    /// <param name="obj">объект</param>
    /// <param name="main">это главный объект?</param>
    /// <returns>изображения</returns>
    private Image[] ClearCollidersAndGetImages(Transform obj, bool main = true)
    {
        List<Image> images = new();
        if (main)
        {
            obj.GetComponent<DetailObject>().enabled = false;
            ClearColliders(obj);
            if (obj.GetComponent<Image>())
                images.Add(obj.GetComponent<Image>());
        }

        if (obj.childCount > 0)
        {
            for (int i = 0; i < obj.childCount; i++)
            {
                Transform child = obj.GetChild(i);
                ClearColliders(child);
                if (child.GetComponent<Image>())
                    images.Add(child.GetComponent<Image>());

                images.AddRange(ClearCollidersAndGetImages(child, false));
            }
        }

        return images.ToArray();
    }

    /// <summary>
    /// Находит количество подходящих ячеек
    /// </summary>
    /// <param name="countConteinersNeed">количество необходимых ячеек</param>
    /// <param name="conteiners">все ячейки</param>
    /// <param name="rotation">требуемый наклон</param>
    /// <returns>количество подходящих ячеек, суммарная позиция подходящих ячеек по оси x, суммарная позиция подходящих ячеек по оси y</returns>
    private (int, float, float) CountConteinersTargetAngle(int countConteinersNeed, List<DetailConteiner> conteiners, Quaternion rotation)
    {
        int count = 0;
        float positionY = 0, positionX = 0;
        for (int id = 0; id < conteiners.Count; id++)
        {
            if ((conteiners[0].uniqe || conteiners[id].uniqe) && conteiners[0].Conector != conteiners[id].Conector)//для уникальных (например ротатор)
                continue;
            if (conteiners[id].transform.rotation == rotation)//тот же угол
            {
                positionX += conteiners[id].transform.position.x;
                positionY += conteiners[id].transform.position.y;
                count++;
                if (typePositionBuild.value == 1 && countConteinersNeed == count)
                    break;
            }
        }
        return (count, positionX, positionY);
    }

    /// <summary>
    /// Строительство детали
    /// </summary>
    /// <param name="conteiner">ячейка, к которой деталь привязана</param>
    /// <param name="posTr">позиция детали</param>
    private void CreateDetail(DetailConteiner conteiner, Transform posTr)
    {
        if (conteiner == null)
        {
            Debug.LogError("ячейка равна null");
            return;
        }
        DetailObject detail = Instantiate(MyDetails[Id], new Vector3(posTr.position.x, posTr.position.y, 256.1f), posTr.rotation, Robot.transform);
        detail.FilterDetailConteiner(typeRotateBuild.value);
        detail.constructor = GetComponent<ConstructorManager>();
        detail.GetComponent<FixedJoint2D>().connectedBody = conteiner.Conector;
        detail.MyConector = conteiner.Conector;
        int MaxIndex = 0;
        if (AllDetails.Count > 1)
        {
            for (int i = 0; i < AllDetails.Count; i++)
            {
                if (detail.NameDetail == AllDetails[i].GetComponent<DetailObject>().NameDetail && AllDetails[i].GetComponent<DetailObject>().IndexDetail >= MaxIndex)
                    MaxIndex = AllDetails[i].GetComponent<DetailObject>().IndexDetail + 1;
            }
        }

        detail.IndexDetail = MaxIndex;
        detail.IndexObject.GetComponent<InputField>().text = detail.IndexDetail.ToString();
        detail.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;

        AllDetails.Add(detail.gameObject);
        AllParentDetails.Add(conteiner.Conector.gameObject);
        AllIdDetails.Add(Id);

        InfoCon.checkConteiners();
        lesson.TaskComplite(5);
        lesson.TaskComplite(4);
    }

    /// <summary>
    /// При включении контроллера
    /// </summary>
    private void OnController()
    {
        if (!Input.GetKeyDown(KeyCode.Mouse0))
            return;
        if (!BeInfoDetailControler)
        {
            controler.InfoDetailConteiner.SetActive(false);
            return;
        }
        DetailObject detailRay = CheckMouseRay();
        if (detailRay)
            controler.CreateInfoDetailControler(detailRay);
    }

    /// <summary>
    /// Луч, который ищет ячейки
    /// </summary>
    /// <param name="area">зона</param>
    /// <param name="areaScript">скрипт зоны</param>
    /// <returns></returns>
    private Transform RaycastHitConteiner(AreaScript areaScript)
    {
        Vector3 EndRay = trCamera.forward * 110f;
        Vector3 StartRay = pos;
        RaycastHit2D hitRay = Physics2D.Raycast(StartRay, EndRay, LayerMask.GetMask("Conteiner"));
        if (!hitRay || //по пути не встречен объект
            !hitRay.transform.GetComponent<DetailConteiner>()/*это не ячейка*/)
        {
            //SetAreaPosition(area);//Я это вынес вне метода
            if (areaScript.conteinerOnCursor)
            {
                areaScript.conteinerOnCursor = false;
                areaScript.ChangeImages();
            }
            return null;
        }
        return hitRay.transform;
    }

    /// <summary>
    /// Изменение положения зоны в сторону места ячеек
    /// </summary>
    /// <param name="area">зона</param>
    /// <param name="areaScript">скрипт зоны</param>
    /// <param name="count">количество подходящих ячеек</param>
    /// <param name="posX">суммарная позиция подходящих ячеек по оси x</param>
    /// <param name="posY">суммарная позиция подходящих ячеек по оси y</param>
    private void SetAreaPosition(Transform area, AreaScript areaScript, int count, float posX, float posY)
    {
        area.position = new Vector3(posX / count, posY / count, 256.1f);
        float defaultIndent = 0.1f;
        float intensiveUp = ((area.localScale.y - 1) * scaleCoefficent) + defaultIndent + areaScript.indent;
        area.localPosition += area.up * intensiveUp;
    }

    /// <summary>
    /// Изменение положения зоны в сторону места курсора
    /// </summary>
    /// <param name="area">зона</param>
    private void SetAreaPosition(Transform area) => area.position = new Vector3(pos.x, pos.y, 256);

    /// <summary>
    /// Устанавливает материал на изображения
    /// </summary>
    /// <param name="images">изображения</param>
    /// <param name="material">материал</param>
    private void SetMaterialImages(Image[] images, Material material)
    {
        for (int id = 0; id < images.Length; id++)
            images[id].material = material;
    }

    /// <summary>
    /// Проводит проверки зоны
    /// </summary>
    /// <param name="area">зона</param>
    /// <param name="areaScript">скрипт зоны</param>
    /// <param name="targetConteiner">ячейка, к которой присоеденится деталь</param>
    /// <returns>Прошёл ли проверки?</returns>
    private bool ValidateArea(Transform area, AreaScript areaScript, Transform targetConteiner)
    {
        if (targetConteiner == null)
        {
            SetAreaPosition(area);
            return false;
        }
        areaScript.ChangeRotation(targetConteiner.eulerAngles.z);
        float posY, posX;
        int count;
        (count, posX, posY) = CountConteinersTargetAngle(areaScript.CountConteinersNeed, areaScript.conteiners, targetConteiner.rotation);
        if (count < areaScript.CountConteinersNeed)
        {
            SetAreaPosition(area);
            return false;
        }//в зоне недостаточно ячеек
        SetAreaPosition(area, areaScript, count, posX, posY);
        if (areaScript.countDetails != 0)//в зоне есть детали
            return false;

        ///здесь все условия соблюдены
        areaScript.conteinerOnCursor = true;
        areaScript.ChangeImages();
        if (Input.GetKeyUp(KeyCode.Mouse0))
            CreateDetail(targetConteiner.GetComponent<DetailConteiner>(), area);
        return true;
    }

    /// <summary>
    /// Изменяет привязку угла зоны строительства
    /// </summary>
    public void ChangedRotateBuild()
    {
        if (Area)
            Area.transform.eulerAngles = new Vector3(0, 0, typeRotateBuild.value * 45f);
        for (int i = 0; i < AllDetails.Count; i++)
        {
            if (AllDetails[i])
                AllDetails[i].GetComponent<DetailObject>().FilterDetailConteiner(typeRotateBuild.value);
        }
    }

    /// <summary>
    /// Удаление детали
    /// </summary>
    /// <param name="obj">деталь</param>
    public void DestroyObj(GameObject obj)
    {
        int Id = AllDetails.IndexOf(obj);
        if (Id == -1)
            return;
        AllIdDetails.RemoveAt(Id - 1);
        AllParentDetails.RemoveAt(Id - 1);
        _ = AllDetails.Remove(obj);
        Destroy(obj);
    }

    /// <summary>
    /// Выход в главное меню
    /// </summary>
    public void Menu()
    {
        saveArea.GetComponent<GlobalSetting>().SetThePhysicsCalculationDelay(0);
        BaseMenu.SetActive(true);
    }

    /// <summary>
    /// Выбор детали в зоне строительства
    /// </summary>
    /// <param name="toggleInfo">переключатель, хранящий информацию о детали, которую выбрали</param>
    public void OnChangedToggleTargetDetail(ToggleInfo toggleInfo)
    {
        if (Id == toggleInfo.idDetail)
            beArea = false;
        if (toggleInfo.toggle.isOn)
        {
            Id = toggleInfo.idDetail;
            parametersObj.SetActive(true);
            if (Area.transform.childCount > 0)
                Destroy(Area.transform.GetChild(0).gameObject);
            if (AreaFantom.transform.childCount > 0)
                Destroy(AreaFantom.transform.GetChild(0).gameObject);
            DetailObject tempDetail = Instantiate(MyDetails[Id], Area.transform);
            parameters.OnChangeTarget(tempDetail);

            beArea = true;
            areaScript.images = ClearCollidersAndGetImages(tempDetail.transform);
            //SetMaterialImages(areaScript.images, solidImage);
            areaScript.Height = toggleInfo.Height;
            areaScript.indent = toggleInfo.indent;
            areaScript.CountConteinersNeed = toggleInfo.CountConteiners;
            areaScript.ChangeSizeCollider(tempDetail.transform, toggleInfo.addSizeCinteiners);
            areaScript.ChangeRotation(Area.transform.eulerAngles.z);
            tempDetail.ActiveSolidImages(false);

            Transform trFantomDetail = Instantiate(MyDetails[Id], AreaFantom.transform).transform;
            areaScriptFantom.images = ClearCollidersAndGetImages(trFantomDetail);
            //SetMaterialImages(areaScriptFantom.images, solidImage);
            areaScriptFantom.Height = toggleInfo.Height;
            areaScriptFantom.indent = toggleInfo.indent;
            areaScriptFantom.CountConteinersNeed = toggleInfo.CountConteiners;
            areaScriptFantom.ChangeSizeCollider(trFantomDetail, toggleInfo.addSizeCinteiners);
            areaScriptFantom.ChangeRotation(AreaFantom.transform.eulerAngles.z);
        }
        else
        {
            parametersObj.SetActive(false);
        }
    }

    /// <summary>
    /// При изменении робота
    /// </summary>
    public void OnChangeRobot()
    {
        AllIdDetails.Clear();
        AllParentDetails.Clear();
        AllDetails.Clear();

        AllDetails.Add(MainComputer);

        _ = saveArea.LoadRobot(NameRobot);
        float[] AngleStr = saveArea.SaverRobot.AngleDetails;
        Vector2[] PositionStr = saveArea.SaverRobot.PositionDetails;
        int[]
            ParentStr = saveArea.SaverRobot.ParentDetails,
            IndexStr = saveArea.SaverRobot.IndexDetails,
            IdStr = saveArea.SaverRobot.IdDetails;
        bool[] Orientations = saveArea.SaverRobot.orientations;
        string[] Keys = saveArea.SaverRobot.Keys;
        string typeKeys = saveArea.SaverRobot.typeKeys;
        controler.saveScripts.AddRange(saveArea.SaverRobot.saveScripts);

        controler.OnEnableKeysWhenRobotChange(Keys, typeKeys, saveArea.SaverRobot.saveLine);
        for (int i = 0; i < IdStr.Length; i++)
        {
            AllIdDetails.Add(IdStr[i]);
            AllDetails.Add(Instantiate(MyDetails[AllIdDetails[i]], Robot.transform).gameObject);
            DetailObject DetailObjInfo = AllDetails[i + 1].GetComponent<DetailObject>();
            DetailObjInfo.constructor = this;

            if (Orientations[i + 1])
            {
                DetailObjInfo.beOrentation = true;
                DetailObjInfo.OnChangeOrentation(true);
            }
            DetailObjInfo.IndexDetail = IndexStr[i];
            AllDetails[i + 1].transform.localPosition = new Vector3(PositionStr[i].x, PositionStr[i].y, 256.1f);
            AllDetails[i + 1].transform.eulerAngles = new Vector3(0, 0, AngleStr[i]);
        }
        for (int i = 0; i < AllDetails.Count; i++)
        {
            if (AllDetails[i].GetComponent<DetailComputer>())
            {
                AllDetails[i].GetComponent<DetailComputer>().Details.Clear();
                for (int ii = 0; ii < AllDetails.Count; ii++)
                    AllDetails[i].GetComponent<DetailComputer>().Details.Add(AllDetails[ii].GetComponent<DetailObject>());
            }
        }

        for (int i = 0; i < ParentStr.Length; i++)
        {
            AllParentDetails.Add(AllDetails[ParentStr[i]]);
            AllDetails[i + 1].GetComponent<FixedJoint2D>().connectedBody = AllDetails[ParentStr[i]].GetComponent<Rigidbody2D>();
            AllDetails[i + 1].GetComponent<DetailObject>().MyConector = AllDetails[ParentStr[i]].GetComponent<Rigidbody2D>();
        }
    }

    /// <summary>
    /// Сохранение робота
    /// </summary>
    public void Save()
    {
        bool[] orientations = new bool[AllDetails.Count];
        for (int i = 0; i < orientations.Length; i++)
            orientations[i] = AllDetails[i].GetComponent<DetailObject>().beOrentation;
        saveArea.SaveRobot(NameRobot, AllDetails, AllParentDetails.ToArray(), AllIdDetails.ToArray(), orientations, CalculateCountNeedResources(), controler.saveScripts.ToArray(), controler.Keys.ToArray(), controler.typeKeys, controler.saveLine);
    }

    /// <summary>
    /// При изменении индекса заменяет индексы, если новый индекс уже у где-то есть
    /// </summary>
    /// <param name="oldIndex">старый индекс (для замены на него)</param>
    /// <param name="newIndex">новый индекс</param>
    /// <param name="changedDetail">деталь, индекс которой изменяют</param>
    public void SearchSwichIndex(int oldIndex, int newIndex, DetailObject changedDetail)
    {
        changedDetail.IndexDetail = newIndex;
        changedDetail.IndexObject.GetComponent<InputField>().text = newIndex.ToString();
        for (int i = 0; i < AllDetails.Count; i++)
        {
            DetailObject detail = AllDetails[i].GetComponent<DetailObject>();
            if (detail.NameDetail == changedDetail.NameDetail &&
                detail.IndexDetail == newIndex &&
                detail != changedDetail)
            {
                detail.IndexDetail = oldIndex;
                detail.IndexObject.GetComponent<InputField>().text = oldIndex.ToString();
                break;
            }
        }
    }

    /// <summary>
    /// Переключатель панелей направления
    /// </summary>
    /// <param name="on">Включить?</param>
    public void SetActiveOrientationButtons(bool on)
    {
        BeActiveOrientation = on;
        if (on)
            TogglePosition.isOn = false;
        for (int i = 0; i < orientationButs.Count; i++)
        {
            if (orientationButs[i])
                orientationButs[i].SetActive(on);
            else
                orientationButs.RemoveAt(i);
        }
    }

    /// <summary>
    /// Переключатель панелей индекса
    /// </summary>
    /// <param name="on">Включить?</param>
    public void SetShowIndex(bool on)
    {
        BeIndexVisible = on;
        if (on)
            ToggleOrientation.isOn = false;
        for (int i = 0; i < IndexButs.Count; i++)
        {
            if (IndexButs[i])
                IndexButs[i].SetActive(on);
            else
                IndexButs.RemoveAt(i);
        }
    }

    /// <summary>
    /// Переключение на контроллер и обратно
    /// </summary>
    /// <param name="on">контроллер включать?</param>
    public void UpdateControlerToggle(bool on)
    {
        InfoCon.BeController = on;
        controls.SetActive(on);
        detailSV.SetActive(!on);
        if (on)
        {
            parametersObj.SetActive(false);
            ClearAreas();
        }
        controler.InfoDetailConteiner.SetActive(false);
    }

    #endregion Methods
}