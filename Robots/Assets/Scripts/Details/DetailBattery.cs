using UnityEngine;
using UnityEngine.UI;

public class DetailBattery : DetailObject
{
    [HideInInspector]
    public float AmountElectricityMax, AmountElectricity;
    public Image sliderBattery;

    private void Awake()
    {
        AmountElectricityMax = ValueReadProperties[5];
        AmountElectricity = AmountElectricityMax;
    }
    protected override void Start()
    {
        base.Start();
    }
    public override void ValidateOnDescription()
    {
        base.ValidateOnDescription();
        labelReadProperty[5] = new TextLanguage("Maximum amount of electricity", "Максимальное количество электроэнергии");
        labelReadProperty[6] = new TextLanguage("Amount of electricity", "Количество электроэнергии");
        descriptionDetail = new TextLanguage(

            "The battery stores excess electricity.\n" +
            "The robot will take electricity from the battery," +
            "when the total electricity income becomes negative.\n",

            "Аккумулятор хранит лишнее электричество.\n" +
            "Робот станет отнимать электричество из батареи, " +
            "когда суммарный доход электричества станет отрицательным.\n");
            
        descriptionsReadProperty[5] = new TextLanguage(

            "A parameter that determines the maximum capacity of electricity.\n",

            "Параметр определяющий максимальную ёмкость электричества.\n");
        
        descriptionsReadProperty[6] = new TextLanguage(

            "A parameter that determines the amount of electricity that the battery is currently storing.\n",

            "Параметр определяющий значение электричества, которое хранит аккумулятор в данный момент.\n");
    }
    public void UpdateBattery(float newAmount = -1)
    {
        AmountElectricity = newAmount >= 0 ? newAmount : AmountElectricity;
        if (beBreak)
            return;
        ValueReadProperties[6] = AmountElectricity;
        if (sliderBattery)
        {
            sliderBattery.fillAmount = AmountElectricity / AmountElectricityMax;
            sliderBattery.color = Color32.Lerp(red, green, AmountElectricity / AmountElectricityMax);
        }
    }
}
