using UnityEngine;
public class DetailRotator : DetailObject
{

    float Angle, MaxAngle, MaxDeltaAngle, DeltaAngle, TargetAngle;
    Transform trRB2d;
    void Awake()
    {
        MaxDeltaAngle = ValueReadProperties[5];
        MaxAngle = ValueReadProperties[6];
    }
    protected override void Start()
    {
        trRB2d = NewConector.transform;
        if (InfoCon.BeConstructor)
            NewConector.bodyType = RigidbodyType2D.Kinematic;
        UpdateOrentation();
        base.Start();
    }
    void FixedUpdate()
    {
        if (InfoCon.BeConstructor || beBreak || !electricity)
            return;
        TargetAngle *= orentation;
        Angle = trRB2d.localEulerAngles.z - 90f;
        if (Angle != TargetAngle && (Mathf.Abs(Angle) < MaxAngle || (TargetAngle > Angle && Angle < 0) || (TargetAngle < Angle && Angle > 0)))
        {
            float Torque = DeltaAngle * Mathf.Abs(Angle - TargetAngle) / (2 * MaxAngle);

            if (Angle < TargetAngle)
                NewConector.angularVelocity = Torque;
            else
                NewConector.angularVelocity = -Torque;
        }
        else
            NewConector.angularVelocity = 0;
        TargetAngle *= orentation;
    }
    public override void ValidateOnDescription()
    {
        base.ValidateOnDescription();

        labelProperty[0] = new TextLanguage("Delta angle", "Скорость изменения угла");
        labelProperty[1] = new TextLanguage("Target angle", "Заданный угол");

        labelReadProperty[5] = new TextLanguage("Maximum delta angle", "Максимальная скорость изменения угла");
        labelReadProperty[6] = new TextLanguage("Maximum angle", "Максимальный угол");
        labelReadProperty[7] = new TextLanguage("Angle", "Угол");

        descriptionDetail = new TextLanguage(

            "The rotating device rotates the robot part.",

            "Вращающее устройство вращает часть робота.");
            

        descriptionsProperty[0] = new TextLanguage(

            "A parameter that determines the rate at which the angle of the rotator changes.\n" +
            DescriptionRange(Language.LanguageType.english,
                "the maximum possible rate of change of the angle of the rotator with a minus",
                "the maximum possible rate of change of the angle of the rotator"),

            "Параметр определяющий скорость изменения угла вращающего устройства.\n" +
            DescriptionRange(Language.LanguageType.russian,
                "максимально возможной скорости изменения угла вращающего устройства с минусом",
                "максимально возможной скорости изменения угла вращающего устройства"));
            
        descriptionsProperty[1] = new TextLanguage(

            "A parameter that specifies the desired angle of the rotator." +
            DescriptionRange(Language.LanguageType.english,
                "the maximum possible angle of the rotator with a minus",
                "maximum possible angle of the rotator"),

            "Параметр определяющий желаемый угол вращающего устройства." +
            DescriptionRange(Language.LanguageType.russian,
                "максимально возможного угла вращающего устройства с минусом",
                "максимально возможного угла вращающего устройства"));
            

        descriptionsReadProperty[5] = new TextLanguage(

            "A parameter that determines the maximum possible rate of change in the angle of the rotator.",

            "Параметр определяющий максимально возможную скорость изменения угла вращающего устройства.");
            
        descriptionsReadProperty[6] = new TextLanguage(

            "A parameter that determines the maximum possible angle of the rotator.",

            "Параметр определяющий максимально возможный угол вращающего устройства.");
            
        descriptionsReadProperty[7] = new TextLanguage(

            "A parameter that defines the current angle of the rotator.",

            "Параметр определяющий текущий угол вращающего устройства.");
            
    }
    protected override void CalculationOfProperties(float[] newValProperties)
    {
        if (beBreak || InfoCon.BeConstructor)
            return;
        DeltaAngle = Mathf.Clamp(newValProperties[0], 0, MaxDeltaAngle);
        if (electricity)
        {
            Angle = trRB2d.localEulerAngles.z - 90f;
            TargetAngle = Mathf.Clamp(newValProperties[1], -MaxAngle, MaxAngle);
            ValueProperties[0] = DeltaAngle;
            ValueProperties[1] = TargetAngle;
            ValueReadProperties[7] = Angle;
        }
        ValueReadProperties[3] = MaxElectricityConsumption * (DeltaAngle / MaxDeltaAngle);
        ElectricityConsumption = ValueReadProperties[3];
    }
}
