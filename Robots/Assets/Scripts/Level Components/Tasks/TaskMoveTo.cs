using UnityEngine;
[RequireComponent(typeof(CircleCollider2D))]
public class TaskMoveTo : TaskManager
{
    bool someRobot;
    public GameObject someObject;
    [Min(0f)]
    public float radius;
    int idPlayer = 0;
    public void Start()
    {
        taskCanWillBeComplete = true;
        values = new float[1];
        if (values[0] == -1)
            someRobot = true;
        else
        {
            someRobot = false;
            //someObject = ...;
        }
        someRobot = true;
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if(stop)
            return;
        if (someRobot)
        {
            if(col.gameObject.GetComponent<DetailObject>())//Detail
            {
                DetailObject detail = col.gameObject.GetComponent<DetailObject>();
                if(detail.MainBrain == null)
                    return;
                int id = detail.MainBrain.transform.parent.GetComponent<RobotManager>().idPlayer;
                if (id == idPlayer)
                    taskCompleted = true;
                OnChangeStatusTask();
            }
        }
        else
        {
            if(someObject == col.gameObject)
            {
                taskCompleted = true;
                OnChangeStatusTask();
            }
        }
    }
}
