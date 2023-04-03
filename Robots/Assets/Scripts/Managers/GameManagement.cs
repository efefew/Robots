using System.Collections.Generic;
using UnityEngine;

public class GameManagement : MonoBehaviour {
    public SaveArea saveArea;
    public List<PlayerInterface> players;

    public GameObject[] Levels;
    /// <summary>
    /// все существующие детали
    /// </summary>
    DetailObject[] MyDetails;
    public GameObject Menu, RobotShell;

    public DetailComputer MainComputer;

    public GameObject interfaceOnLevelComplite, gameplayInterface, butNext, finishLabel;

    public CameraOperator camScript;
    public int PointId;
    Transform tr;
    ConstructorManager constructor;

    void Awake()
    {
        tr = transform;
        MyDetails = saveArea.MyDetails;
        constructor = GetComponent<ConstructorManager>();
    }
    void OnEnable()
    {
        Menu.SetActive(false);
        CreateLevel();
    }
    void OnDisable()
    {
        ClearLevel();
    }
    /// <summary>
    /// уничтожает объект
    /// </summary>
    /// <param name="g">объект</param>
    /// <param name="id">если индекс указан, то этот объект - робот и id = индекс игрока, привязаный к нему</param>
    public void DestroySomething(GameObject g, int id = -1, float time = 0)
    {
        if (id >= 0)
            players[id].OnDestroyRobot(g.GetComponent<RobotManager>());
        Destroy(g, time);
    }
    /// <summary>
    /// очищает уровень
    /// </summary>
    void ClearLevel()
    {
        for (int i = 0; i < tr.childCount; i++)
                Destroy(tr.GetChild(i).gameObject);
    }
    /// <summary>
    /// создаёт уровень
    /// </summary>
    void CreateLevel()
    {
        for (int i = 0; i < players.Count; i++)
        {
            players[i].Clear();
            players[i].Create(saveArea);
            players[i].myIdPlayer = i;
            players[i].game = this;
        }
#if UNITY_EDITOR
        Setting.themeLevelNow = "✪DevelopTheme✪";
#endif
        if (Setting.themeLevelNow == "✪DevelopTheme✪")//пока для моих уровней так
        {
            GameObject LevelNow = Instantiate(Levels[Setting.idLevelNow], transform);
            LevelNow.transform.localPosition = Vector3.zero;
            LevelNow.transform.localScale = Vector3.one;
            LevelNow.SetActive(true);
        }
        //CreateRobots(NameRobot.Substring(5), startLevelPoints[saveArea.all.LevelNow]);
    }
    /// <summary>
    /// перезагрузка уровня
    /// </summary>
    public void RestartLevel()
    {
        interfaceOnLevelComplite.SetActive(false);
        gameplayInterface.SetActive(true);
        ClearLevel();
        CreateLevel();
    }
    /// <summary>
    /// следующий уровень
    /// </summary>
    public void NextLevel()
    {
        interfaceOnLevelComplite.SetActive(false);
        gameplayInterface.SetActive(true);
        ClearLevel();
        Setting.idLevelNow++;
        CreateLevel();
    }
    /// <summary>
    /// Создаёт робота
    /// </summary>
    /// <param name="nameRobot">имя</param>
    /// <param name="location">позиция</param>
    /// <param name="idPlayer">индекс игрока, к которому он будет привязан</param>
    /// <returns></returns>
    public RobotManager CreateRobot(string nameRobot, Transform location, int idPlayer = 0)
    {
        List<DetailObject> details = new List<DetailObject>();
        List<int> idDetails = new List<int>();
        RobotManager robot = Instantiate(RobotShell, location.position, location.rotation, transform).GetComponent<RobotManager>();
        DetailComputer Brain = Instantiate(MainComputer, location.position, location.rotation, robot.transform);
        robot.player = players[idPlayer];
        robot.idPlayer = idPlayer;
        robot.name = nameRobot;
        
        Brain.propertyParts = saveArea.LoadRobot("Robot" + nameRobot);
        Brain.SaveScripts = saveArea.SaverRobot.saveScripts;
        Brain.Keys = saveArea.SaverRobot.Keys;
        Brain.typeKeys = saveArea.SaverRobot.typeKeys;

        float[] AngleStr = saveArea.SaverRobot.AngleDetails;
        Vector2[] PositionStr = saveArea.SaverRobot.PositionDetails;
        int[] 
            ParentStr = saveArea.SaverRobot.ParentDetails,
            IndexStr = saveArea.SaverRobot.IndexDetails,
            idDetailsStr = saveArea.SaverRobot.IdDetails;
        bool[] Orientations = saveArea.SaverRobot.orientations;


        details.Add(Brain);
        for (int i = 0; i < idDetailsStr.Length; i++)
        {
            idDetails.Add(idDetailsStr[i]);
            details.Add(Instantiate(MyDetails[idDetails[i]], robot.transform).GetComponent<DetailObject>());
            details[i + 1].transform.localPosition = new Vector3(PositionStr[i].x, PositionStr[i].y, 256.1f);
            details[i + 1].transform.localEulerAngles = new Vector3(0, 0, AngleStr[i]);
            details[i + 1].constructor = constructor;
            details[i + 1].IndexDetail = IndexStr[i];
            details[i + 1].beOrentation = Orientations[i + 1];
        }


        details[0].GetComponent<FixedJoint2D>().connectedBody = robot.GetComponent<Rigidbody2D>();
        details[0].GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        for (int i = 0; i < ParentStr.Length; i++)
            OnConnectedDetail(details[i + 1], details[ParentStr[i]].GetComponent<Rigidbody2D>());

        for (int id = 0; id < details.Count; id++)
        {
            details[id].MainBrain = Brain;
            OnCreateDetail(details[id], details, robot, idPlayer);
        }
        robot.GetComponent<Rigidbody2D>().simulated = true;
        players[idPlayer].AddTargetRobot(robot);
        return robot;
    }
    /// <summary>
    /// При присоединении детали
    /// </summary>
    /// <param name="detail">деталь ,которую присоединяют</param>
    /// <param name="connect">то, к чему присоединяют</param>
    void OnConnectedDetail(DetailObject detail, Rigidbody2D connect)
    {
        detail.GetComponent<FixedJoint2D>().connectedBody = connect;
        detail.MyConector = connect;
        detail.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
    }
    /// <summary>
    /// При создании детали
    /// </summary>
    /// <param name="detail">деталь, которую создают</param>
    /// <param name="details">все детали робота</param>
    /// <param name="robot">робот</param>
    void OnCreateDetail(DetailObject detail, List<DetailObject> details, RobotManager robot, int idPlayer)
    {

        robot.Objects.Add(detail);
        
        if (detail.GetType() == typeof(DetailWhell))
        {
            robot.Moves.Add(detail.transform);
            return;
        }
        if (detail.GetType() == typeof(DetailGenerator))
        {
            robot.Generators.Add((DetailGenerator)detail);
            return;
        }
        if (detail.GetType() == typeof(DetailBattery))
        {
            robot.Battery.Add((DetailBattery)detail);
            return;
        }
        if (detail.GetType() == typeof(DetailComputer))
        {
            robot.Computers.Add((DetailComputer)detail);
            OnCreateDetailComputer((DetailComputer)detail, details);
            return;
        }
    }
    /// <summary>
    /// При создании компьютера
    /// </summary>
    /// <param name="detailComputer">компьютер, который создают</param>
    /// <param name="details">все детали робота</param>
    void OnCreateDetailComputer(DetailComputer detailComputer, List<DetailObject> details)
    {
        detailComputer.save = saveArea;
        for (int id = 0; id < details.Count; id++)
            detailComputer.Details.Add(details[id]);
    }
    /// <summary>
    /// при завершении уровня
    /// </summary>
    /// <param name="win">выйграл?</param>
    public void OnLevelComplite(bool win)
    {
        interfaceOnLevelComplite.SetActive(true);
        gameplayInterface.SetActive(false);
        finishLabel.SetActive(false);
        int maxIdLevel = saveArea.all.LevelsMax[saveArea.all.LevelsOpenTheme.IndexOf(Setting.themeLevelNow)];
        if (Setting.idLevelNow < maxIdLevel)
            butNext.SetActive(win);
        else
            finishLabel.SetActive(win);
    }
}
