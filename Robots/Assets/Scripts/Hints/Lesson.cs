using System.Collections;
using UnityEngine;

[System.Serializable]
public class LessonTask
{
    /// <summary>
    /// Имя задачи для работы в разработке
    /// </summary>
    public string name;
    /// <summary>
    /// Прилагающееся пояснение к задаче
    /// </summary>
    public string[] descriptionTask = new string[Setting.countLanguage];
    /// <summary>
    /// объекты, которые необходимо деактивировать
    /// </summary>
    public CanvasGroup[] lockObjects;
    /// <summary>
    /// объекты, которые необходимо включить
    /// </summary>
    public GameObject[] helpObjects;
    /// <summary>
    /// объекты, которые необходимо выключить
    /// </summary>
    public GameObject[] hideObjects;
    public bool needNext;
}
public class Lesson : MonoBehaviour
{
    public MultipleLanguageLabel textTask;
    [HideInInspector]
    public bool taskComplite;
    /// <summary>
    /// Номер уровнял
    /// </summary>
    public int targetLevel;
    /// <summary>
    /// Тема уровней
    /// </summary>
    public string targetTheme = "✪DevelopTheme✪";
    /// <summary>
    /// кнопка дальше к пояснию задачи
    /// </summary>
    public GameObject nextBut;
    /// <summary>
    /// поясние задачи
    /// </summary>
    public GameObject labelTask;
    /// <summary>
    /// Заглушить все ненужные кнопки
    /// </summary>
    public GameObject darkAll;
    int idCurrentTask;
    /// <summary>
    /// Задачи, которые необходимо выполнить чтобы продвинуться в обучении
    /// </summary>
    public LessonTask[] lessonTasks;

    private void OnEnable()
    {
        if (Setting.themeLevelNow == targetTheme && targetLevel == Setting.idLevelNow)
            StartCoroutine(StepLesson());
    }

    private void Create(LessonTask lessonTask)
    {
        textTask.textOnTargetLanguage = lessonTask.descriptionTask;
        labelTask.SetActive(textTask.textOnTargetLanguage[0].Length > 0);
        if(textTask.textOnTargetLanguage[0].Length > 0)
            textTask.OnChangeLanguage(Language.language);
        taskComplite = false;
        darkAll.SetActive(lessonTask.needNext);
        nextBut.SetActive(lessonTask.needNext);
        OnLessonTask(lessonTask, true);
    }
    private void Clear(LessonTask lessonTask) => OnLessonTask(lessonTask, false);
    private void OnLessonTask(LessonTask lessonTask, bool on)
    {
        for (int i = 0; i < lessonTask.lockObjects.Length; i++)
            SetLock(lessonTask.lockObjects[i], !on);
        for (int i = 0; i < lessonTask.helpObjects.Length; i++)
            lessonTask.helpObjects[i].SetActive(on);
        for (int i = 0; i < lessonTask.hideObjects.Length; i++)
            lessonTask.hideObjects[i].SetActive(!on);
    }

    /// <summary>
    /// Деактивация объекта
    /// </summary>
    /// <param name="lockGroup">объект</param>
    /// <param name="on">Активация?</param>
    private void SetLock(CanvasGroup lockGroup, bool on)
    {
        lockGroup.interactable = on;
        lockGroup.alpha = on? 1f : 0.3f;
    }
    private IEnumerator StepLesson()
    {
        for (int idTask = 0; idTask < lessonTasks.Length; idTask++)
        {
            idCurrentTask = idTask;
            Create(lessonTasks[idTask]);
            yield return new WaitUntil(() => taskComplite);
            Clear(lessonTasks[idTask]);
        }
        darkAll.SetActive(false);
        labelTask.SetActive(false);
    }
    public void TaskComplite(int idTargetTask) 
    {
        if(idCurrentTask == idTargetTask)
            taskComplite = true;
    }
    public void OnClickNext() => taskComplite = true;
}
