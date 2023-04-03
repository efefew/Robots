using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DetailResourceLocator : DetailObject
{
    float MaxVisibilityRadius, VisibilityRadius, CountResourceDetected, Angle, Distance;
    int CountDetected, IdDetected, IdResourceDetected;
    public Image[] sensors;
    void Awake()
    {
        MaxVisibilityRadius = ValueReadProperties[5];
    }
    protected override void Start()
    {
        StartCoroutine(UpdateSensorEnumerator());
        base.Start();
    }
    public override void ValidateOnDescription()
    {
        base.ValidateOnDescription();

        labelProperty[0] = new TextLanguage("Tracked Ore Index", "Индекс отслеживаемой руды");
        labelProperty[1] = new TextLanguage("Visibility radius", "Радиус видимости");

        labelReadProperty[5] = new TextLanguage("Maximum Visibility Radius", "Максимальный радиус видимости");
        labelReadProperty[6] = new TextLanguage("Distance", "Расстояние");
        labelReadProperty[7] = new TextLanguage("Number of ores found", "Количество найденных руд");
        labelReadProperty[8] = new TextLanguage("Angle", "Угол");
        labelReadProperty[9] = new TextLanguage("Resource index", "Индекс ресурса");
        labelReadProperty[10] = new TextLanguage("Number of discovered resources", "Количество обнаруженных ресурсов");

        descriptionDetail = new TextLanguage(

            "The Ore Finder learns the distance and direction of ores relative to the Ore Finder.",

            "Искатель руд узнаёт расстояние и направление руд относительно искателя руд.");
            

        descriptionsProperty[0] = new TextLanguage(

            "Parameter defining the index of the found ore" +
            "(determines the distance and direction of the ore with this index relative to the ore finder).\n" +
            DescriptionRange(Language.LanguageType.english, max: "amount of ores found"),

            "Параметр определяющий индекс найденой руды" +
            "(определяет расстояние и направление руды с этим индексом относительно искателя руд).\n" +
            DescriptionRange(Language.LanguageType.russian, max: "количества найденых руд"));
            
        descriptionsProperty[1] = new TextLanguage(

            "A parameter that specifies the desired visible radius." +
            DescriptionRange(Language.LanguageType.english, max: "maximum possible visible radius"),

            "Параметр определяющий желаемый видимый радиус." +
            DescriptionRange(Language.LanguageType.russian, max: "максимально возможного видимого радиуса"));
            

        descriptionsReadProperty[5] = new TextLanguage(

            "A parameter that determines the maximum possible visible radius.",

            "Параметр определяющий максимально возможный видимый радиус.");
            
        descriptionsReadProperty[6] = new TextLanguage(

            "A parameter that determines the distance of the ore relative to the ore finder.",

            "Параметр определяющий расстояние руды относительно искателя руд.");
            
        descriptionsReadProperty[7] = new TextLanguage(

            "A parameter that determines the amount of ores found.",

            "Параметр определяющий количество найденных руд.");
            
        descriptionsReadProperty[8] = new TextLanguage(

            "A parameter that determines the direction of the ore relative to the ore finder.",

            "Параметр определяющий направление руды относительно искателя руд.");
            
        descriptionsReadProperty[9] = new TextLanguage(

            "A parameter that determines the resource index of the found ore.",

            "Параметр определяющий индекс ресурса найденой руды.");
            
        descriptionsReadProperty[10] = new TextLanguage(

            "A parameter that determines the amount of resources found ore.",

            "Параметр определяющий количество ресурсов найденной руды.");
            
    }
    protected override void CalculationOfProperties(float[] newValProperties)
    {
        if (beBreak || !electricity && InfoCon.BeConstructor)
            return;
        IdDetected = (int)newValProperties[0];
        VisibilityRadius = Mathf.Clamp(newValProperties[1], 0, MaxVisibilityRadius);
        CheckRecources();

        ValueProperties[0] = IdDetected;
        ValueProperties[1] = VisibilityRadius;

        ValueReadProperties[6] = Distance;
        ValueReadProperties[7] = CountDetected;
        ValueReadProperties[8] = Angle;
        ValueReadProperties[9] = IdResourceDetected;
        ValueReadProperties[10] = CountResourceDetected;

        ValueReadProperties[3] = MaxElectricityConsumption * (VisibilityRadius / MaxVisibilityRadius);
        ElectricityConsumption = ValueReadProperties[3];
        UpdateSensor(sensors, 1 - Distance / VisibilityRadius);// насколько близко ресурс
    }
    void CheckRecources()
    {
        if (tr == null)
            tr = transform;
        List<ResourceMine> mines = new List<ResourceMine>();
        foreach (var coll in Physics2D.OverlapCircleAll(tr.position, VisibilityRadius))
            if (coll.transform.GetComponent<ResourceMine>())
                mines.Add(coll.transform.GetComponent<ResourceMine>());
        CountDetected = mines.Count;
        if (CountDetected > 0)
        {
            IdDetected = CountDetected > 1 ? Mathf.Clamp(IdDetected, 0, CountDetected - 1) : 0;
            IdResourceDetected = mines[IdDetected].IdResource;
            CountResourceDetected = mines[IdDetected].ResourceCurrent;
            Distance = Vector2.Distance(tr.position, mines[IdDetected].transform.position) - 
                (mines[IdDetected].GetComponent<CircleCollider2D>().radius * mines[IdDetected].transform.localScale.x) / 10;
            Angle = Vector2.Angle(mines[IdDetected].transform.position - tr.position, tr.up);
        }
        else
        {
            IdResourceDetected = -1;
            CountResourceDetected = 0;
            Distance = VisibilityRadius;
            Angle = 0;
        }
    }
    IEnumerator UpdateSensorEnumerator()
    {
        yield return new WaitForSeconds(1f);
        CheckRecources();
        if (!(beBreak || !electricity && InfoCon.BeConstructor))
            UpdateSensor(sensors, 1 - Distance / VisibilityRadius);// насколько близко ресурс
        else
            UpdateSensor(sensors, 0);
    }

}
