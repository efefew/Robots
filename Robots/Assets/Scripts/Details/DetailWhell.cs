using UnityEngine;
public class DetailWhell : DetailObject
{
    public float softStop;
    [HideInInspector]
    public float Force;
    float MaxForce, TargetForce;
    void Awake()
    {
        MaxForce = ValueReadProperties[5];
    }
    protected override void Start()
    {
        base.Start();
        UpdateOrentation();
    }
    protected override void Update()
    {
        base.Update();
        if (beBreak || !electricity)
        {
            Force = 0;
            animator.SetBool("forward", false);
            animator.SetBool("back", false);
            return;
        }
        CalculationOfDynamicProperties();
        if (animator)
        {
            animator.SetBool("forward", Force < 0);
            animator.SetBool("back", Force > 0);
        }
    }
    public override void ValidateOnDescription()
    {
        base.ValidateOnDescription();

        labelProperty[0] = new TextLanguage("Target force", "Заданная сила");

        labelReadProperty[5] = new TextLanguage("Maximum force", "Максимальная сила");
        labelReadProperty[6] = new TextLanguage("Force", "Сила");

        descriptionDetail = new TextLanguage(

            "The wheel moves the robot.",

            "Колесо двигает робота.");
            
        descriptionsProperty[0] = new TextLanguage(

            "A parameter that determines the desired force" +
            "(Robot speed depends on force).\n" +
            DescriptionRange(Language.LanguageType.english,
                "the maximum possible force with a minus",
                "maximum possible force"),

            "Параметр определяющий желаемую силу" +
            "(от силы зависит скорость робота).\n" +
            DescriptionRange(Language.LanguageType.russian,
                "максимально возможной силы с минусом",
                "максимально возможной силы"));
            

        descriptionsReadProperty[5] = new TextLanguage(

            "A parameter that determines the maximum possible force.",

            "Параметр определяющий максимально возможную силу.");
            
        descriptionsReadProperty[6] = new TextLanguage(

            "A parameter that determines the current force.",

            "Параметр определяющий текущую силу.");
    }
    protected override void CalculationOfProperties(float[] newValProperties)
    {
        if (beBreak)
            return;
        
        TargetForce = Mathf.Clamp(newValProperties[0], -MaxForce, MaxForce);
        ValueProperties[0] = TargetForce;
        CalculationOfDynamicProperties();
        ValueReadProperties[6] = Force;
        ValueReadProperties[3] = MaxElectricityConsumption * (Mathf.Abs(Force) / MaxForce);
        ElectricityConsumption = ValueReadProperties[3];
    }
    void CalculationOfDynamicProperties()
    {
        if (electricity)
            Force = TargetForce * orentation;
        else
        {
            if (Force < 0)
            {
                Force += softStop;
                if (Force > 0)
                    Force = 0;
            }
            else
            {
                Force -= softStop;
                if (Force < 0)
                    Force = 0;
            }
        }
    }
    
}
