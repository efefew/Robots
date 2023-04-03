using UnityEngine;
using UnityEngine.UI;

public class DetailBaseLocator : DetailObject
{
    private DetailResourceConteiner conteiner;
    private PlayerInterface player;
    private int idBase, idRecource, idConteiner;
    private bool friendBase;
    private float countRecourceSend;
    protected override void Start()
    {
        base.Start();
        UpdateOrentation();
    }
    public override void ValidateOnDescription()
    {
        base.ValidateOnDescription();
        labelProperty[0] = new TextLanguage("Id recource", "Индекс ресурса");
        labelProperty[1] = new TextLanguage("Count recource", "Количество ресурсов");
        labelProperty[2] = new TextLanguage("Id conteiner", "Индекс контейнера");
        labelProperty[3] = new TextLanguage("Id base", "Индекс базы");
        labelProperty[4] = new TextLanguage("Whose base", "Чья база");

        labelReadProperty[5] = new TextLanguage("CountRecourceInBase", "Максимальное потребление электричства");
        labelReadProperty[6] = new TextLanguage("Distance", "Расстояние");
        labelReadProperty[7] = new TextLanguage("Angle", "Угол");
        labelReadProperty[8] = new TextLanguage("CountFriendBases", "Количество баз друзей");
        labelReadProperty[9] = new TextLanguage("CountEnemyBases", "Количество баз врагов");

        descriptionDetail = new TextLanguage(

            "The base finder knows the distance, direction, and resources of enemy and friendly bases," +
            "and can also deliver and collect resources from a friendly base.",

            "Искатель баз знает расстояние, направление и количество ресурсов вражеских и дружественных баз, " +
            "а также может доставлять и забирать ресурсы из дружественной базы.");

        descriptionsProperty[0] = new TextLanguage(

            "A parameter specifying the index of the resource required to transfer from or to the base.",

            "Параметр определяющий индекс ресурса, требуемого для передачи из базы или на базу.");

        descriptionsProperty[1] = new TextLanguage(

            "A parameter that determines the amount of resources required to transfer from the base or to the base. " +
            "When a resource transfer is performed, the value is set to zero." +
            "The transfer is instant, only inside the base and provided that the base is friendly. " +
            "If the value is negative, the resources are sent to the base, and " +
            "if positive, the resources are sent to the container.",

            "Параметр определяющий количество ресурсов, требуемых для передачи из базы или на базу. " +
            "При выполнении передачи ресурсов значение обнуляется." +
            "Передача происходит мгновенно, только внутри базы и при условии, что база дружественная. " +
            "При отрицательном зачении ресурсы отправляются на базу, а " +
            "при положительном зачении ресурсы отправляются в контейнер.");

        descriptionsProperty[2] = new TextLanguage(

            "A parameter that specifies the index of the resource container to transfer from or to the base.",

            "Параметр определяющий индекс контейнера ресурсов для передачи из базы или на базу.");

        descriptionsProperty[3] = new TextLanguage(

            "Parameter that defines the index of the base.",

            "Параметр определяющий индекс базы.");

        descriptionsProperty[4] = new TextLanguage(

            "Parameter that determines the base of which side to monitor (enemy or friendly). " +
            "0 - friendly base, 1 - enemy base.",

            "Параметр определяющий за базой какой стороны следить(вражеской или дружественной). " +
            "0 - дружественная база, 1 - вражеская база.");


        descriptionsReadProperty[5] = new TextLanguage(

            "A parameter that determines the amount of resources (with the selected index) on the base.",

            "Параметр определяющий количество ресурсов(с выбранным индексом) на базе.");

        descriptionsReadProperty[6] = new TextLanguage(

            "A parameter that determines the distance to the base.",

            "Параметр определяющий расстояние до базы.");

        descriptionsReadProperty[7] = new TextLanguage(

            "A parameter that determines the direction towards the base.",

            "Параметр определяющий направление в сторону базы.");

        descriptionsReadProperty[8] = new TextLanguage(

            "A parameter that determines the number of friendly bases.",

            "Параметр определяющий количество дружественных баз.");

        descriptionsReadProperty[9] = new TextLanguage(

            "A parameter that determines the number of enemy bases.",

            "Параметр определяющий количество вражеских баз.");

    }
    protected override void CalculationOfProperties(float[] newValProperties)
    {
        if (player == null)
        {
            if (MainBrain == null || MainBrain.Robot == null)
                return;
            player = MainBrain.Robot.player;
        }
        if (beBreak)
            return;
        idRecource = Mathf.Clamp((int)newValProperties[0], 0, ResourcesStat.countResource);
        countRecourceSend = newValProperties[1];
        idConteiner = Mathf.Min((int)newValProperties[2], 0);
        idBase = Mathf.Clamp((int)newValProperties[3], 0, player.MyBases.Count);
        friendBase = newValProperties[4] > 0;

        ValueProperties[0] = idRecource;
        ValueProperties[2] = idConteiner;
        ValueProperties[3] = idBase;
        ValueProperties[4] = friendBase ? 1 : 0;
        SendRecources();

        ValueReadProperties[5] = player.MyBases[idBase].resourcesInBase.resourceComponents[idRecource];
        ValueReadProperties[6] = Vector2.Distance(tr.position, player.MyBases[idBase].transform.position);
        ValueReadProperties[7] = Vector2.Angle(player.MyBases[idBase].transform.position - tr.position, tr.up);
        ValueReadProperties[8] = player.MyBases.Count;
        ValueReadProperties[9] = 0;
        ElectricityConsumption = ValueReadProperties[3];
    }

    private void SendRecources()
    {
        if (!player.MyBases[idBase].CheckAvailabilityObject(gameObject))
        {
            tr.GetChild(0).GetComponent<Image>().color = Color.red;
            return;
        }
        tr.GetChild(0).GetComponent<Image>().color = Color.green;
        conteiner = SearchDetail("ResourceConteiner", idConteiner, out DetailObject targetConteiner) ? targetConteiner.GetComponent<DetailResourceConteiner>() : null;
        if (conteiner == null || !(conteiner.IdResourceInConteiner == idRecource || -1 == conteiner.IdResourceInConteiner))
            return;
        if (countRecourceSend == 0)
            return;
        if (countRecourceSend > 0)
        {
            conteiner.CountResources = TransferOfResources(
                ref player.MyBases[idBase].resourcesInBase.resourceComponents[idRecource],
                countRecourceSend,
                conteiner.CountResources,
                conteiner.CountMaxResourceInConteiner);
        }
        else
        {
            countRecourceSend *= -1;
            player.MyBases[idBase].resourcesInBase.resourceComponents[idRecource] = TransferOfResources(
                ref conteiner.CountResources,
                countRecourceSend,
                player.MyBases[idBase].resourcesInBase.resourceComponents[idRecource]);
        }
        conteiner.UpdateResourcesInfo();
        player.UpdateLabelResourcesInTargetBase();
        countRecourceSend = 0;
        ValueProperties[1] = countRecourceSend;
    }
}
