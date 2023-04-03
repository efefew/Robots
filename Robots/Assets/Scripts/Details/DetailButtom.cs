public class DetailButtom : DetailObject
{
    public ButtomTriger Triger;
    void Awake()
    {
        MaxElectricityConsumption = ValueReadProperties[4];
        ValueReadProperties[3] = MaxElectricityConsumption;
        ElectricityConsumption = MaxElectricityConsumption;
    }
    protected override void Start()
    {
        base.Start();
    }
    public override void ValidateOnDescription()
    {
        base.ValidateOnDescription();
        labelReadProperty[5] = new TextLanguage("Press", "Нажатие");
        descriptionDetail = new TextLanguage(

            "The button registers the pressing of various objects.",

            "Кнопка регистрирует нажатие различных объектов."); 
        
        descriptionsReadProperty[5] = new TextLanguage(

            "The parameter that determines the force of pressing the button" +
            "between 0 and 1, where 0 is no press, 1 is full press.",

            "Параметр определяющий силу нажатия кнопки " +
            "в диапазоне от 0 до 1, где 0 - это отсутствие нажатия, 1 - это полное нажатие.");
    }
    protected override void CalculationOfProperties(float[] newValProperties) {
        if (beBreak)
            return;
        if (electricity && !InfoCon.BeConstructor)
            ValueReadProperties[5] = Triger.FloatValue;
    }
}
