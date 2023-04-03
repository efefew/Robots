using System.Collections.Generic;
using UnityEngine;

public class LevelsGroup : MonoBehaviour
{
    public string nameGroup;
    public List<LevelInfo> levels;
    public GameObject[] levelObjs;
    public Transform place;
    void Awake()
    {
        levels = new List<LevelInfo>();
    }
    public void CreateLevel(int idLevel)
    {
        for (int idObj = 0; idObj < levels[idLevel].levelObjects.Length; idObj++)
        {
            LevelObject levelObject = levels[idLevel].levelObjects[idObj];
            Instantiate(levelObjs[levelObject.idObject], levelObject.location.position, levelObject.location.rotation, place);
        }

    }
}
[SerializeField]
public class LevelInfo
{
    public LevelObject[] levelObjects;
    public LevelObject[] tasks;
    public string[] tasksLabel;
}
[SerializeField]
public class LevelObject
{
    public int idObject;
    public Transform location;
    public float[] properties;
}