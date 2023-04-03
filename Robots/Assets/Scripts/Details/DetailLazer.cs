using System.Collections;
using UnityEngine;

public class DetailLazer : DetailObject
{

    float MaxDistance, MinDelayTime, Distance, MaxDuration;
    float DelayTime, NeedMaxDistance, Duration;
    public float add;
    public LineRenderer MyRay;
    bool BeDuration, ElectricityOff;
    void Awake()
    {
        MaxDistance = ValueReadProperties[5];
        MinDelayTime = ValueReadProperties[6];
        MaxDuration = ValueReadProperties[7];
    }
    protected override void Start()
    {
        StartCoroutine(CheckLazer());
        base.Start();
    }
    protected override void Update()
    {
        base.Update();
        if (beBreak)
            return;

        if (electricity && !InfoCon.BeConstructor)
        {
            if (ElectricityOff)
            {
                StartCoroutine(CheckLazer());
                ElectricityOff = false;
            }

            if (BeDuration)
            {
                Vector3 StartRay = MyRay.transform.position;
                Vector3 EndRay = MyRay.transform.up * NeedMaxDistance;
                RaycastHit2D h = Physics2D.Raycast(StartRay, EndRay, NeedMaxDistance);
                if (h)
                {
                    Distance = h.distance;
                    EndRay = MyRay.transform.up * Distance;
                }
                else
                    Distance = NeedMaxDistance;

                MyRay.SetPosition(0, StartRay);
                MyRay.SetPosition(1, StartRay + EndRay);
            }
        }
        else
        {
            MyRay.gameObject.SetActive(false);
            ElectricityOff = true;
        }
    }
    public override void ValidateOnDescription()
    {
        base.ValidateOnDescription();

        labelProperty[0] = new TextLanguage("Delay time", "Время задержки");
        labelProperty[1] = new TextLanguage("Specified maximum distance", "Указанное максимальное расстояние");
        labelProperty[2] = new TextLanguage("Duration", "продолжительность");

        labelReadProperty[5] = new TextLanguage("Max Distance", "Максимальное расстояние");
        labelReadProperty[6] = new TextLanguage("Minimum delay time", "Минимальное время задержки");
        labelReadProperty[7] = new TextLanguage("Maximum duration", "Максимальная продолжительность");
        labelReadProperty[8] = new TextLanguage("Distance", "Расстояние");

        descriptionDetail = new TextLanguage(

            "The laser fires a beam that registers objects encountered.",

            "Лазер пускает луч, который регистрирует встречающиеся объекты.");
        descriptionsProperty[0] = new TextLanguage(

            "A parameter that determines the reload time of a step.\n" +
            DescriptionDependence(Language.LanguageType.english, "step recharge time", TypeDependence.LessBeter) +
            DescriptionRange(Language.LanguageType.english, min: "minimum possible step cooldown"),

            "Параметр определяющий время перезарядки шага.\n" +
            DescriptionDependence(Language.LanguageType.russian, "время перезарядки  шага", TypeDependence.LessBeter) +
            DescriptionRange(Language.LanguageType.russian, min: "минимально возможного времени перезарядки шага"));
            
        descriptionsProperty[1] = new TextLanguage(

            "A parameter that determines the range of the beam.\n" +
            DescriptionDependence(Language.LanguageType.english, "beam distance") +
            DescriptionRange(Language.LanguageType.english, max: "maximum possible beam range"),

            "Параметр определяющий дальность луча.\n" +
            DescriptionDependence(Language.LanguageType.russian, "дальность луча") +
            DescriptionRange(Language.LanguageType.russian, max: "максимально возможной дальности луча"));
            
        descriptionsProperty[2] = new TextLanguage(

            "A parameter that determines the duration of the laser in one step.\n" +
            DescriptionDependence(Language.LanguageType.english, "duration of the laser in one step") +
            DescriptionRange(Language.LanguageType.english, max: "maximum possible laser duration"),

            "Параметр определяющий длительность работы лазера за один шаг.\n" +
            DescriptionDependence(Language.LanguageType.russian, "длительность работы лазера за один шаг") +
            DescriptionRange(Language.LanguageType.russian, max: "максимально возможной длительности работы лазера"));
            

        descriptionsReadProperty[5] = new TextLanguage(

            "A parameter that determines the maximum possible range of the beam.",

            "Параметр определяющий максимально возможную дальность луча.");
            
        descriptionsReadProperty[6] = new TextLanguage(

            "A parameter that determines the minimum possible reload time for a step.",

            "Параметр определяющий минимально возможное время перезарядки шага.");
            
        descriptionsReadProperty[7] = new TextLanguage(

            "The parameter that determines the maximum possible duration of the laser.",

            "Параметр определяющий максимально возможную длительность работы лазера.");
            
        descriptionsReadProperty[8] = new TextLanguage(

            "A parameter that determines the range of the object registered by the beam.",

            "Параметр определяющий дальность объекта, зарегистрированного лучом.");
            
    }
    protected override void CalculationOfProperties(float[] newValProperties)
    {
        if (beBreak)
            return;
        DelayTime = newValProperties[0];

        NeedMaxDistance = Mathf.Clamp(newValProperties[1], 0, MaxDistance);
        Duration = Mathf.Clamp(newValProperties[2], 0, MaxDuration);
        DelayTime = DelayTime < MinDelayTime ? MinDelayTime : DelayTime;

        ValueReadProperties[3] = MaxElectricityConsumption * (NeedMaxDistance / MaxDistance + Duration / MaxDuration + MinDelayTime / DelayTime) / 3;
        ElectricityConsumption = ValueReadProperties[3];
        if (!electricity || InfoCon.BeConstructor)
            return;
        ValueProperties[0] = DelayTime;
        ValueProperties[1] = NeedMaxDistance;
        ValueProperties[2] = Duration;
        ValueReadProperties[8] = Distance;
    }

    IEnumerator CheckLazer()
    {
        while (true)
        {
            if (beBreak || !electricity)
                break;
            if (!InfoCon.BeConstructor)
            {
                BeDuration = true;
                MyRay.gameObject.SetActive(true);
                yield return new WaitForSeconds(Duration);
                BeDuration = false;
                MyRay.gameObject.SetActive(false);
            }
            yield return new WaitForSeconds(DelayTime);
        }
    }
}
