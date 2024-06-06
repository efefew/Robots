using System.Collections;

using UnityEngine;

public class DetailPiston : DetailObject
{
    #region Fields

    private float speed, currentLength, targetLength, maxSpeed;
    public float addOffset, maxLength;
    public Transform[] pistonIntermediateParts;
    public float[] maxLengths;
    private SliderJoint2D point;
    private Transform pointTr;
    #endregion Fields

    #region Methods

    private void Awake()
    {
        maxSpeed = ValueReadProperties[5];
        if (NewConector)
            point = NewConector.GetComponent<SliderJoint2D>();
        pointTr = point.transform;
    }

    private IEnumerator WaitOffAutoConnected(FixedJoint2D joint2D)
    {
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        joint2D.autoConfigureConnectedAnchor = false;
    }

    private void FixedUpdate()
    {
        if (!InfoCon.BeConstructor)
        {
            currentLength = pointTr.localPosition.y;
            for (int i = 0; i < pistonIntermediateParts.Length; i++)
                pistonIntermediateParts[i].localPosition = pistonIntermediateParts[i].localPosition.Y(currentLength + addOffset > maxLengths[i] ? maxLengths[i] : currentLength + addOffset);
        }
        if (beBreak || InfoCon.BeConstructor || !electricity)
        {
            if (point) point.motor = SetMotor(point.motor, 0);
            return;
        }
        ChangeLength();
    }

    private void ChangeLength()
    {
        float target = targetLength / ValueReadProperties[6] * maxLength;
        float delta = currentLength - target;
        float minSpeed = delta < speed / maxSpeed * 0.1f ? speed * Mathf.Abs(delta) / maxLength : speed;
        point.motor = SetMotor(point.motor, delta < 0 ? -minSpeed : minSpeed);
    }
    private JointMotor2D SetMotor(JointMotor2D motor, float speed)
    {
        JointMotor2D motorTemp = new JointMotor2D();
        motorTemp.motorSpeed = speed;
        motorTemp.maxMotorTorque = motor.maxMotorTorque;
        return motorTemp;
    }
    protected override void Start()
    {
        ValueReadProperties[6] = -point.limits.min;
        ValueReadProperties[7] = point.motor.maxMotorTorque;
        base.Start();
    }

    protected override void CalculationOfProperties(float[] newValProperties)
    {
        if (beBreak || InfoCon.BeConstructor)
            return;
        speed = Mathf.Clamp(newValProperties[0], -maxSpeed, maxSpeed);
        targetLength = Mathf.Clamp(newValProperties[1], 0, ValueReadProperties[6]);
        ValueReadProperties[8] = Mathf.Clamp(currentLength / maxLength * ValueReadProperties[6], 0, ValueReadProperties[6]);
        ValueProperties[0] = speed;
        ValueProperties[1] = targetLength;
        ValueReadProperties[3] = MaxElectricityConsumption * Mathf.Abs(speed / maxSpeed);
        ElectricityConsumption = ValueReadProperties[3];
    }

    public override void ValidateOnDescription()
    {
        base.ValidateOnDescription();

        labelProperty[0] = new TextLanguage("Speed", "Скорость");
        labelProperty[1] = new TextLanguage("Specified length", "Заданная длина");

        labelReadProperty[5] = new TextLanguage("Maximum speed", "Максимальная скорость");
        labelReadProperty[6] = new TextLanguage("Maximum length", "Максимальная длина");
        labelReadProperty[7] = new TextLanguage("Force", "Сила");
        labelReadProperty[8] = new TextLanguage("Current length", "Текущая длина");

        descriptionDetail = new TextLanguage(

            "The piston changes its own length.",

            "Поршень изменяет собственную длину.");

        descriptionsProperty[0] = new TextLanguage(

            "A parameter that determines the rate of change of its own length" +
            DescriptionDependence(Language.LanguageType.english, "native length change rate") +
            DescriptionRange(Language.LanguageType.english, max: "the maximum possible rate of change of own length"),

            "Параметр определяющий скорость изменения собственной длины" +
            DescriptionDependence(Language.LanguageType.russian, "скорость изменения собственной длины") +
            DescriptionRange(Language.LanguageType.russian, max: "максимально возможного скорости изменения собственной длины"));

        descriptionsProperty[1] = new TextLanguage(

            "A parameter specifying the desired native length." +
            DescriptionRange(Language.LanguageType.english, max: "maximum native length"),

            "Параметр определяющий желаемую собственную длину." +
            DescriptionRange(Language.LanguageType.russian, max: "максимальной собственной длины"));

        descriptionsReadProperty[5] = new TextLanguage(

            "A parameter that determines the maximum possible rate of change of its own length.",

            "Параметр определяющий максимально возможную скорость изменения собственной длины.");

        descriptionsReadProperty[6] = new TextLanguage(

            "A parameter that specifies the maximum natural length.",

            "Параметр определяющий максимальную собственную длину.");

        descriptionsReadProperty[7] = new TextLanguage(

            "A parameter that determines the strength of the change in length.",

            "Параметр определяющий силу изменения длины.");
        descriptionsReadProperty[8] = new TextLanguage(

            "A parameter that specifies the current native length.",

            "Параметр определяющий текущую собственную длину.");
    }

    public override void OnConnectedToJoint2D(FixedJoint2D joint2D)
    {
        base.OnConnectedToJoint2D(joint2D);
        StartCoroutine(WaitOffAutoConnected(joint2D));
    }

    #endregion Methods
}