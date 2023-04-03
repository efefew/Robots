public class TaskManager : LevelComponentManager
{
    public enum TypeTask
    {
        /// <summary>
        /// обязательная задача
        /// </summary>
        mandatoryTask,
        /// <summary>
        /// дополнительная задача
        /// </summary>
        additionalTask
    }
    /// <summary>
    /// задача выполнена?
    /// </summary>
    public bool taskCompleted { get; protected set; }
    /// <summary>
    /// задача может быть выполнена?
    /// </summary>
    public bool taskCanWillBeComplete { get; protected set; }
    /// <summary>
    /// остановить проверку выполнения задачи
    /// </summary>
    public bool stop;
    public TypeTask taskType;
    public LevelManager manager;
    protected void OnChangeStatusTask()
    {
        manager.CheckTasks();
    }
}
