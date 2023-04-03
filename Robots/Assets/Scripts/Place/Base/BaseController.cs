using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(ResourceDetail))]
public class BaseController : MonoBehaviour
{
    Transform tr;
    /// <summary>
    /// индекс пользователя владеющий базой
    /// </summary>
    public int playerId;
    /// <summary>
    /// пользователь владеющий базой
    /// </summary>
    [SerializeField]
    private PlayerInterface playerTarget;
    public ResourceDetail resourcesInBase { get; private set; }
    GameManagement game;
    CircleCollider2D coll;
    void Awake()
    {
        tr = transform;
        coll = GetComponent<CircleCollider2D>();
        playerTarget = tr.parent.parent.GetComponent<GameManagement>().players[playerId];
        game = playerTarget.game;
        resourcesInBase = GetComponent<ResourceDetail>();
    }
    void OnEnable()
    {
        playerTarget.AddTargetBase(this);
    }
    /// <summary>
    /// Проверка на возможность покупки робота
    /// </summary>
    /// <param name="needResourcesForRobot">количество требуемых ресурсов для покупки</param>
    /// <param name="nameRobot">имя, покупаемого робота</param>
    /// <returns>результат проверки</returns>
    public bool ValidateCreateRobot(float[] needResourcesForRobot, string nameRobot)
    {
        foreach (var coll in Physics2D.OverlapCircleAll(tr.position, coll.radius / 10))
            if (coll.GetComponent<DetailObject>())
            {
                DetailObject detail = coll.GetComponent<DetailObject>();
                if (coll.GetComponent<DetailObject>().MainBrain)
                    Debug.LogWarning("робот " + detail.MainBrain.transform.parent.name + " мешает строительству на базе " + tr.name);
                else
                    Debug.LogWarning("мусор " + detail.name + " мешает строительству на базе " + tr.name);
                return false;
            }
        for (int i = 0; i < ResourcesStat.countResource; i++)
            if (resourcesInBase.resourceComponents[i] < needResourcesForRobot[i])
            {
                Debug.LogWarning("не хватает ресурса " + ResourcesStat.GetName(i) + " в количестве " + (needResourcesForRobot[i] - resourcesInBase.resourceComponents[i]));
                return false;
            }

        for (int i = 0; i < resourcesInBase.resourceComponents.Length; i++)
            resourcesInBase.resourceComponents[i] -= needResourcesForRobot[i];

        game.CreateRobot(nameRobot, tr, playerTarget.myIdPlayer);
        playerTarget.UpdateLabelResourcesInTargetBase();
        return true;
    }
    /// <summary>
    /// проверяет на наличие объекта
    /// </summary>
    /// <returns>результат проверки</returns>
    public bool CheckAvailabilityObject(GameObject obj)
    {
        foreach (var coll in Physics2D.OverlapCircleAll(tr.position, coll.radius / 10))
            if (coll.gameObject == obj)
                return true;
        return false;
    }
}
