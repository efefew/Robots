using System.Collections.Generic;
using UnityEngine;

public class LevelCreator : MonoBehaviour
{
    List<TaskManager> tasks = new List<TaskManager>();
    List<int> idTasks = new List<int>();

    List<BuyRobot> bases = new List<BuyRobot>();

    List<LevelComponentManager> gameplayObjects = new List<LevelComponentManager>();
    List<int> idGameplayObjects = new List<int>();

    List<string> nameRobots = new List<string>();
    List<Transform> trRobots = new List<Transform>();

    List<Transform> trDecorations = new List<Transform>();
    List<int> idDecorations = new List<int>();
    void Start()
    {
        
    }

    void Update()
    {
        ///перетаскивание объектов
    }
    void CreateObj()
    {

    }
    void ChangeObj()
    {

    }
    void DeleteObj()
    {

    }
    void SaveLevel()
    {

    }
    void LoadLevel()
    {

    }
    void DeleteLevel()
    {

    }
}
