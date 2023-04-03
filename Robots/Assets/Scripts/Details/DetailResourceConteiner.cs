using UnityEngine;
using UnityEngine.UI;

public class DetailResourceConteiner : DetailObject
{
    [HideInInspector]
    public float CountResources, CountMaxResourceInConteiner;
    [HideInInspector]
    public int IdResourceInConteiner;
    public Image[] sensors;
    void Awake()
    {
        CountMaxResourceInConteiner = ValueReadProperties[5];
    }
    protected override void Start()
    {
        base.Start();
    }
    public override void ValidateOnDescription()
    {
        base.ValidateOnDescription();

        labelReadProperty[5] = new TextLanguage("Maximum number of resources in a container", "Максимальное количество ресурсов в контейнере");
        labelReadProperty[6] = new TextLanguage("Count resources in a container", "Количество ресурсов в контейнере");
        labelReadProperty[7] = new TextLanguage("Resource ID in the container", "Идентификатор ресурса в контейнере");

        descriptionDetail = new TextLanguage(

            "The container stores resources with one index.",

            "Контейнер хранит ресурсы с одним индексом.");
            

        descriptionsReadProperty[5] = new TextLanguage(

            "A parameter that defines the maximum possible number of resources in the container.",

            "Параметр определяющий максимально возможное количество ресурсов в контейнере.");
            
        descriptionsReadProperty[6] = new TextLanguage(

            "A parameter that determines the amount of resources in the container." +
            DescriptionRange(Language.LanguageType.english, max: "the maximum possible number of resources in the container"),

            "Параметр определяющий количество ресурсов в контейнере." +
            DescriptionRange(Language.LanguageType.russian, max: "максимально возможного количества ресурсов в контейнере"));
            
        descriptionsReadProperty[7] = new TextLanguage(

            "A parameter that specifies the index of the resource in the container.",

            "Параметр определяющий индекс ресурса в контейнере.");
            
    }
    public void UpdateResourcesInfo()
    {
        if (beBreak || !electricity)
            return;
        ValueReadProperties[6] = CountResources;
        ValueReadProperties[7] = IdResourceInConteiner;
        SetMass(CountResources, IdResourceInConteiner);
        UpdateSensor(sensors, CountResources / CountMaxResourceInConteiner);
    }
    [ContextMenu("AddRecource")]
    public void AddRecource()
    {
        CountResources = CountMaxResourceInConteiner;
        IdResourceInConteiner = 0;
        UpdateResourcesInfo();
    }
}
