using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public int levelId;
    public string levelTheme, taskDescription;
    public TaskManager[] taskManagers;
    GameManagement gameManager;
    [SerializeField]
    private void Start()
    {
        gameManager = transform.parent.GetComponent<GameManagement>();
    }
    public void CheckTasks()
    {
        for (int i = 0; i < taskManagers.Length; i++)
            if(taskManagers[i].taskType == TaskManager.TypeTask.mandatoryTask)
            {
                if (!taskManagers[i].taskCanWillBeComplete)
                {
                    gameManager.OnLevelComplite(false);
                    StopTasks();
                    return;
                }
                if (!taskManagers[i].taskCompleted)
                    return;
            }
        SaveArea saveArea = gameManager.saveArea;
        if (saveArea.all.LevelsOpenTheme.Contains(levelTheme))
        {
            if (levelTheme == "✪DevelopTheme✪")
            {
               int LevelOpen = saveArea.LevelOpen(levelTheme);
                saveArea.all.LevelsOpenId[saveArea.all.LevelsOpenTheme.IndexOf(levelTheme)] = LevelOpen <= levelId ? (levelId + 1) : LevelOpen;
            }
            saveArea.Save();
        }
        StopTasks();
        gameManager.OnLevelComplite(true);
    }
    void StopTasks()
    {
        for (int i = 0; i < taskManagers.Length; i++)
            taskManagers[i].stop = true;
    }
}
