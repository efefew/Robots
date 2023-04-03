using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DetailMiner : DetailObject
{
    DetailResourceConteiner conteiner;
    [HideInInspector]
    public ResourceMine resourceMine;
    float MineItensity, MaxMineItensity, CountResourceInMiner, CountMaxResourceInMiner, MinDelayTime, DelayTime;
    int IdResourceInMiner, MoveToIdConteiner;
    int id;
    /// <summary>
    /// цикл добычи
    /// </summary>
    bool MiningFrameEnd;
    public Image[] sensorsConteiner, sensorsMining;
    Color32 targetMiningColor;
    void Awake()
    {
        MinDelayTime = ValueReadProperties[5];
        MaxMineItensity = ValueReadProperties[6];
        CountMaxResourceInMiner = ValueReadProperties[7];
    }
    protected override void Start()
    {
        id = -1;
        IdResourceInMiner = -1;
        MiningFrameEnd = true;
        base.Start();
    }
    protected override void Update()
    {
        base.Update();
        if (beBreak || !electricity || InfoCon.BeConstructor)
        {
            animator.SetBool("mining", false);
            for (int idSensor = 0; idSensor < sensorsMining.Length; idSensor++)
                sensorsMining[idSensor].color = Color.white;
            return;
        }
        Mine();
        SendRecource();
    }
    void UpdateResourceInMiner()
    {
        ValueReadProperties[9] = IdResourceInMiner;
        ValueReadProperties[10] = CountResourceInMiner;
        SetMass(CountResourceInMiner, IdResourceInMiner);
        UpdateSensor(sensorsConteiner, CountResourceInMiner / CountMaxResourceInMiner);
    }
    public override void ValidateOnDescription()
    {
        base.ValidateOnDescription();

        labelProperty[0] = new TextLanguage("Mining intensity", "Интенсивность добычи");
        labelProperty[1] = new TextLanguage("Delay time", "Время задержки");
        labelProperty[2] = new TextLanguage("Target container", "Целевой контейнер");

        labelReadProperty[5] = new TextLanguage("Minimum delay time", "Минимальное время задержки");
        labelReadProperty[6] = new TextLanguage("Maximum mining intensity", "Максимальная интенсивность добычи");
        labelReadProperty[7] = new TextLanguage("Maximum number of resources in the miner", "Максимальное количество ресурсов в рудокопе");
        labelReadProperty[8] = new TextLanguage("Count of discovered resources", "Количество обнаруженных ресурсов");
        labelReadProperty[9] = new TextLanguage("Miner resource ID", "Идентификатор ресурса в рудокопе");
        labelReadProperty[10] = new TextLanguage("Count resources in the miner", "Количество ресурсов в рудокопе");
        labelReadProperty[11] = new TextLanguage("Discovered resource ID", "Обнаруженный идентификатор ресурса");

        descriptionDetail = new TextLanguage(

            "A miner mines resources and stores them in a container.\n" +
            "The resource index in the miner must match the resource index in the ore, " +
            "or the amount of the resource in the miner should be 0.\n" +
            "The index of the resource in the container must match the index of the resource in the miner, " +
            "or the amount of the resource in the container must be 0.",

            "Рудокоп добывает ресурсы и складирует в контейнер.\n" +
            "Индекс ресурса в рудокопе должен совпадать с индексом ресурса в руде, " +
            "или количество ресурса в рудокопе должно быть равно 0.\n" +
            "Индекс ресурса в контейнере должен совпадать с индексом ресурса в рудокопе, " +
            "или количество ресурса в контейнере должно быть равно 0.");
            
        descriptionsProperty[0] = new TextLanguage(

            "A parameter that determines the amount of the extracted resource in one step.\n" +
            "When the value is > 0, the miner mines resources equal to this value, " +
            "if the value is < 0, the miner stores it in a container.\n" +
            DescriptionDependence(Language.LanguageType.english, "amount of resource to be mined in one step") +
            DescriptionRange(Language.LanguageType.english,
            "the maximum possible amount of mined resource minus",
            "the maximum possible amount of extracted resource"),

            "Параметр определяющий количество добываемого ресурса за один шаг.\n" +
            "При значении > 0 рудокоп добывает ресурсы в количестве равному этому значению, " +
            "при значении < 0 рудокоп складирует в контейнер.\n" +
            DescriptionDependence(Language.LanguageType.russian, "количество добываемого ресурса за один шаг") +
            DescriptionRange(Language.LanguageType.russian,
            "максимально возможного количества добываемого ресурса c минусом",
            "максимально возможного количества добываемого ресурса"));
            
        descriptionsProperty[1] = new TextLanguage(

            "A parameter that determines the reload time of a step.\n" +
            DescriptionDependence(Language.LanguageType.english, "step recharge time", TypeDependence.LessBeter) +
            DescriptionRange(Language.LanguageType.english, min: "minimum possible step cooldown"),

            "Параметр определяющий время перезарядки шага.\n" +
            DescriptionDependence(Language.LanguageType.russian, "время перезарядки шага", TypeDependence.LessBeter) +
            DescriptionRange(Language.LanguageType.russian, min: "минимально возможного времени перезарядки шага"));
            
        descriptionsProperty[2] = new TextLanguage(

            "A parameter that defines the index of the container where the ores will be stored.",

            "Параметр определяющий индекс контейнера, куда будут складироваться руды.");
            

        descriptionsReadProperty[5] = new TextLanguage(

            "A parameter that determines the minimum possible reload time for a step.",

            "Параметр определяющий минимально возможное время перезарядки шага.");
            
        descriptionsReadProperty[6] = new TextLanguage(

            "A parameter that determines the maximum amount of a resource that can be mined.",

            "Параметр определяющий максимально возможное количество добываемого ресурса.");
            
        descriptionsReadProperty[7] = new TextLanguage(

            "A parameter that determines the maximum possible amount of a resource in a miner.",

            "Параметр определяющий максимально возможное количество ресурса в рудокопе.");
            
        descriptionsReadProperty[8] = new TextLanguage(

            "A parameter that determines the amount of resource in the ore.",

            "Параметр определяющий количество ресурса в руде.");
            
        descriptionsReadProperty[9] = new TextLanguage(

            "A parameter that determines the index of a resource in a miner.",

            "Параметр определяющий индекс ресурса в рудокопе.");
            
        descriptionsReadProperty[10] = new TextLanguage(

            "A parameter that determines the current amount of a resource in a miner." +
            DescriptionRange(Language.LanguageType.english, max: "the maximum possible amount of a resource in a miner"),

            "Параметр определяющий текущее количество ресурса в рудокопе." +
            DescriptionRange(Language.LanguageType.russian, max: "максимально возможного количества ресурса в рудокопе"));
            
        descriptionsReadProperty[11] = new TextLanguage(

            "A parameter that determines the index of the resource in the ore.",

            "Параметр определяющий индекс ресурса в руде.");
            
    }
    protected override void CalculationOfProperties(float[] newValProperties)
    {
        if (beBreak)
            return;
        DelayTime = newValProperties[1];
        ValueProperties[1] = DelayTime;

        MineItensity = Mathf.Clamp(newValProperties[0], -MaxMineItensity, MaxMineItensity);
        DelayTime = DelayTime < MinDelayTime ? MinDelayTime : DelayTime;

        if (electricity && !InfoCon.BeConstructor)
        {
            MoveToIdConteiner = (int)newValProperties[2];
            ValueProperties[2] = MoveToIdConteiner;

            ValueProperties[0] = MineItensity;
            if (resourceMine != null)
            {
                ValueReadProperties[11] = resourceMine.IdResource;//IdResourceDetected
                ValueReadProperties[8] = resourceMine.ResourceCurrent;//CountResourceDetected
            }
            else
            {
                ValueReadProperties[11] = -1;//IdResourceDetected
                ValueReadProperties[8] = 0;//CountResourceDetected
            }
        }
        if (resourceMine != null)
            ValueReadProperties[3] = MaxElectricityConsumption * (MineItensity / MaxMineItensity + MinDelayTime / DelayTime) / 2;
        else
            ValueReadProperties[3] = 0;
        ElectricityConsumption = ValueReadProperties[3];
    }
    /// <summary>
    /// добыча
    /// </summary>
    void Mine()
    {
        if (MiningFrameEnd)//обновлён цикл добычи (для задержки)
        {
            animator.SetBool("mining", false);
            targetMiningColor = red;
            if (resourceMine != null)// существуют ли залежи
            {
                targetMiningColor = yellow;
                if (MineItensity > 0)//скорость добычи больше 0
                {
                    animator.SetBool("mining", true);
                    if (resourceMine.ResourceCurrent > 0)// есть ли в залежах ресурсы
                    {
                        targetMiningColor = green;
                        if (IdResourceInMiner == resourceMine.IdResource || IdResourceInMiner == -1) //совпадает ли тип ресура с ресурсом в контейнере бура
                        {
                            if (CountResourceInMiner < CountMaxResourceInMiner) // в буре хватает места
                            {
                                targetMiningColor = blue;
                                MiningFrameEnd = false;
                                StartCoroutine(Mining());
                            }
                        }
                    }
                }
            }
        }
        for (int idSensor = 0; idSensor < sensorsMining.Length; idSensor++)
            sensorsMining[idSensor].color = targetMiningColor;
    }
    /// <summary>
    /// отправка в контейнеры
    /// </summary>
    void SendRecource()
    {
        if (id != MoveToIdConteiner)
        {
            id = MoveToIdConteiner;
            conteiner = SearchDetail("ResourceConteiner", MoveToIdConteiner, out DetailObject targetConteiner)? targetConteiner.GetComponent<DetailResourceConteiner>(): null;
        }
        if (MiningFrameEnd && MineItensity < 0 && CountResourceInMiner > 0)
        {
            MiningFrameEnd = false;
            StartCoroutine(Drop());
        }
    }
    IEnumerator Mining()
    {
        if (beBreak || !electricity)
            yield break;
        IdResourceInMiner = resourceMine.IdResource;
        CountResourceInMiner = TransferOfResources(ref resourceMine.ResourceCurrent, MineItensity, CountResourceInMiner, CountMaxResourceInMiner);

        UpdateResourceInMiner();
        ValueProperties[1] = DelayTime;
        yield return new WaitForSeconds(DelayTime);
        MiningFrameEnd = true;
    }
    IEnumerator Drop()
    {
        if (beBreak || !electricity)
            yield break;
        if (conteiner != null)
        {
            if (conteiner.IdResourceInConteiner == IdResourceInMiner || -1 == conteiner.IdResourceInConteiner)
            {
                conteiner.IdResourceInConteiner = IdResourceInMiner;
                conteiner.CountResources = TransferOfResources(ref CountResourceInMiner, Mathf.Abs(MineItensity), conteiner.CountResources, conteiner.CountMaxResourceInConteiner);

                UpdateResourceInMiner();
                conteiner.UpdateResourcesInfo();
            }
        }
        else
        {
            TransferOfResources(ref CountResourceInMiner, Mathf.Abs(MineItensity));
            UpdateResourceInMiner();
        }
        ValueProperties[1] = DelayTime;
        yield return new WaitForSeconds(DelayTime);
        MiningFrameEnd = true;

    }
}
