using UnityEngine;
/// <summary>
/// Генератор электричества
/// </summary>
public class DetailGenerator : DetailObject
{
    [HideInInspector]
    public float ResourceСonsumption, TargetConteiner, ResourceMaxСonsumption, ElectricityGenerate, ElectricityGenerateMax, IdResourceСonsumption;

    private void Awake()
    {
        ElectricityGenerateMax = ValueReadProperties[5];
        ResourceMaxСonsumption = ValueReadProperties[6];
    }
    protected override void Start()
    {
        base.Start();
    }
    public override void ValidateOnDescription()
    {
        base.ValidateOnDescription();

        labelProperty[0] = new TextLanguage("Resource consumption", "Потребление ресурсов");
        labelProperty[1] = new TextLanguage("Target container", "Целевой контейнер");

        labelReadProperty[5] = new TextLanguage("Maximum electricity production", "Максимальное производство электроэнергии");
        labelReadProperty[6] = new TextLanguage("Maximum resource consumption", "Максимальное потребление ресурсов");
        labelReadProperty[7] = new TextLanguage("Power generation", "Производство электроэнергии");
        labelReadProperty[8] = new TextLanguage("Consumed resource index", "Индекс потребляемого ресурса");

        descriptionDetail = new TextLanguage(

            "A generator generates electricity by consuming a certain resource from a container.",

            "Генератор вырабатывает электричество, потребляя определённый ресурс из контейнера.");

        descriptionsProperty[0] = new TextLanguage(

            "A parameter that specifies the desired resource consumption." +
            DescriptionDependence(Language.LanguageType.english, dependence1: "resource consumption", dependence2: "electricity generation") +
            DescriptionRange(Language.LanguageType.english, max: "maximum possible resource consumption"),

            "Параметр определяющий желаемое потребление ресурса." +
            DescriptionDependence(Language.LanguageType.russian, dependence1: "потребление ресурса", dependence2: "выработка электричества") +
            DescriptionRange(Language.LanguageType.russian, max: "максимально возможного потребления ресурса"));

        descriptionsProperty[1] = new TextLanguage(

            "A parameter that specifies the index of the container from which the resource is being consumed.\n" +
            "The index of the resource in this container must match the index of the resource required to generate electricity.",

            "Параметр определяющий индекс контейнера, из которого потребляется ресурс.\n" +
            "Индекс ресурса в этом контейнере должен совпадать с индексом ресурса, тербуемого для генерации электричества.");


        descriptionsReadProperty[5] = new TextLanguage(

            "A parameter that determines the maximum possible electricity generation.",

            "Параметр определяющий максимально возможную выработку электричества.");

        descriptionsReadProperty[6] = new TextLanguage(

            "A parameter that determines the maximum possible resource consumption.",

            "Параметр определяющий максимально возможное потребление ресурса.");

        descriptionsReadProperty[7] = new TextLanguage(

            "A parameter that determines the desired generation of electricity.",

            "Параметр определяющий желаемую выработку электричества.");

        descriptionsReadProperty[8] = new TextLanguage(

            "A parameter that specifies the index of the resource required to generate electricity.",

            "Параметр определяющий индекс ресурса, требуемого для генерации электричества.");
    }
    protected override void CalculationOfProperties(float[] newValProperties)
    {
        if (beBreak)
            return;
        TargetConteiner = newValProperties[1];
        ResourceСonsumption = Mathf.Clamp(newValProperties[0], 0, ResourceMaxСonsumption);

        ValueProperties[0] = ResourceСonsumption;
        ValueProperties[1] = TargetConteiner;
        ValueReadProperties[7] = ElectricityGenerate;
        ValueReadProperties[8] = IdResourceСonsumption;
    }
}
