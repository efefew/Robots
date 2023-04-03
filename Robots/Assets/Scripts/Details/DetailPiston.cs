using UnityEngine;
using System.Collections;
public class DetailPiston : DetailObject
{
    public float minLength, lastMinLength, pointAddY;
    float maxLength, speed, targetLength, currentLength, maxSpeed;
    public Transform[] pistonMoveParts;
    public float[] maxLengths, addLength;
    public FixedJoint2D point;
    void Awake()
    {
        maxSpeed = ValueReadProperties[5];
    }
    protected override void Start()
    {
        for (int i = 0; i < pistonMoveParts.Length; i++)
            maxLengths[i] -= addLength[i];

        maxLength = maxLengths[pistonMoveParts.Length - 1];
        ValueReadProperties[6] = maxLength;
        base.Start();
    }
    public override void ValidateOnDescription()
    {
        base.ValidateOnDescription();

        labelProperty[0] = new TextLanguage("Speed", "Скорость");
        labelProperty[1] = new TextLanguage("Specified length", "Заданная длина");

        labelReadProperty[5] = new TextLanguage("Maximum speed", "Максимальная скорость");
        labelReadProperty[6] = new TextLanguage("Maximum length", "Максимальная длина");
        labelReadProperty[7] = new TextLanguage("Current length", "Текущая длина");

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

            "A parameter that specifies the current native length.",

            "Параметр определяющий текущую собственную длину.");
            
    }
    protected override void CalculationOfProperties(float[] newValProperties)
    {
        if (beBreak || InfoCon.BeConstructor)
            return;
        speed = Mathf.Clamp(newValProperties[0], 0, maxSpeed);
        if (electricity)
        {
            targetLength = Mathf.Clamp(newValProperties[1], minLength, maxLength);
            ValueReadProperties[7] = currentLength;
        }
        ValueProperties[0] = speed;
        ValueProperties[1] = targetLength;
        ValueReadProperties[3] = MaxElectricityConsumption * speed / maxSpeed;
        ElectricityConsumption = ValueReadProperties[3];
    }
    public override void OnConnectedToJoint2D(FixedJoint2D joint2D)
    {
        base.OnConnectedToJoint2D(joint2D);
        StartCoroutine(WaitOffAutoConnected(joint2D));
    }
    IEnumerator WaitOffAutoConnected(FixedJoint2D joint2D)
    {
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        joint2D.autoConfigureConnectedAnchor = false;
    }
    void FixedUpdate()
    {
        if (beBreak || InfoCon.BeConstructor || !electricity)
            return;
        ChangeLength();
    }
    void ChangeLength()
    {
        if(currentLength < targetLength)
        {
            currentLength += speed;
            if(currentLength > targetLength)
                currentLength = targetLength;
        }
        if(currentLength > targetLength)
        {
            currentLength -= speed;
            if (currentLength < targetLength)
                currentLength = targetLength;
        }
        for (int i = 0; i < pistonMoveParts.Length; i++)
        {
            if(currentLength > maxLengths[i])
                pistonMoveParts[i].localPosition = new Vector2(pistonMoveParts[i].localPosition.x, maxLengths[i] + addLength[i]);
            else
                pistonMoveParts[i].localPosition = new Vector2(pistonMoveParts[i].localPosition.x, currentLength + addLength[i]);
        }
        point.connectedAnchor = new Vector2(point.connectedAnchor.x, pointAddY + currentLength);

    }
}
