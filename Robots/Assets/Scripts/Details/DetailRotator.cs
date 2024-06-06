using UnityEngine;

public class DetailRotator : DetailObject
{
    private float Angle, MaxAngle, MaxDeltaAngle, DeltaAngle, TargetAngle;
    private Transform trRB2d;
    private HingeJoint2D point;

    private void Awake()
    {
        MaxDeltaAngle = ValueReadProperties[5];
        MaxAngle = ValueReadProperties[6];
        if (NewConector)
            point = NewConector.GetComponent<HingeJoint2D>();
        ValueReadProperties[7] = point.motor.maxMotorTorque;
    }
    protected override void Start()
    {
        trRB2d = NewConector.transform;
        if (InfoCon.BeConstructor)
            NewConector.bodyType = RigidbodyType2D.Kinematic;
        UpdateOrentation();
        base.Start();
    }

    private void FixedUpdate()
    {
        if (InfoCon.BeConstructor || beBreak || !electricity)
        {
            if (point) point.motor = SetMotor(point.motor, 0);
            return;
        }
        ChangeAngle();
    }

    private void ChangeAngle()
    {
        TargetAngle *= orentation;
        Angle = trRB2d.localEulerAngles.z - 90f;
        float delta = Angle - TargetAngle;
        float minSpeed = delta < DeltaAngle / MaxDeltaAngle * 0.01f ? DeltaAngle * Mathf.Abs(delta) / MaxAngle : DeltaAngle;
        point.motor = SetMotor(point.motor, delta < 0 ? -minSpeed : minSpeed);
        TargetAngle *= orentation;
    }
    private JointMotor2D SetMotor(JointMotor2D motor, float speed)
    {
        JointMotor2D motorTemp = new JointMotor2D();
        motorTemp.motorSpeed = speed;
        motorTemp.maxMotorTorque = motor.maxMotorTorque;
        return motorTemp;
    }
    public override void ValidateOnDescription()
    {
        base.ValidateOnDescription();

        labelProperty[0] = new TextLanguage("Delta angle", "Скорость изменения угла");
        labelProperty[1] = new TextLanguage("Target angle", "Заданный угол");

        labelReadProperty[5] = new TextLanguage("Maximum delta angle", "Максимальная скорость изменения угла");
        labelReadProperty[6] = new TextLanguage("Maximum angle", "Максимальный угол");
        labelReadProperty[7] = new TextLanguage("Force", "Сила");
        labelReadProperty[8] = new TextLanguage("Angle", "Угол");

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

            "A parameter that determines the strength of the angle change.",

            "Параметр определяющий силу изменения угла.");
        descriptionsReadProperty[8] = new TextLanguage(

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
            ValueReadProperties[8] = Angle;
        }
        ValueReadProperties[3] = MaxElectricityConsumption * (DeltaAngle / MaxDeltaAngle);
        ElectricityConsumption = ValueReadProperties[3];
    }
}
