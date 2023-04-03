using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotManager : MonoBehaviour
{
    public List<Transform> Moves;
    public Rigidbody2D rb2D{get; private set;}
    float maxSpeed = 100, speed;
    public List<DetailObject> Objects;
    public List<DetailGenerator> Generators;
    public List<DetailBattery> Battery;
    public List<DetailComputer> Computers;
    public DetailComputer mainComputer;
    public PlayerInterface player;
    public int idPlayer;

    [Header("остаток неиспользованого электричества на выходе")]
    public float Electricity;
    [Header("чистый доход")]
    public float income;
    [Header("чистый расход")]
    public float expenditure;
    [Header("электричество необходимое для обеспечения всех деталей")]
    public float needElectricity;
    [Header("доп. электричество (берётся всё электричество из батареи)")]
    public float AmountBatteryIncome;
    [Header("остаток на зарядку батареи")]
    public float AmountBatteryExpenditure;
    [Header("электричество из батареи без избытка")]
    public float AmountBatteryClearIncome;
    GameManagement gameManagement;
    void Start()
    {
        gameManagement = transform.parent.GetComponent<GameManagement>();
        mainComputer = Objects[0].MainBrain;
        rb2D = GetComponent<Rigidbody2D>();
        if (!InfoCon.BeConstructor)
            StartCoroutine(ElectricityBalancing());
    }
    [ContextMenu("Dead")]
    public void Dead(float time = 0)
    {
        gameManagement.DestroySomething(gameObject, idPlayer, time);
    }
    /// <summary>
    /// при смерти главной детали
    /// </summary>
    public void OnDeadMainDetail()
    {
        for (int i = 0; i < Objects.Count; i++)
        {
            if(Objects[i] != null)
            {
                Objects[i].electricity = false;
                Objects[i].transform.SetParent(transform.parent);
            }
        }
        Dead();
    }
    IEnumerator ElectricityBalancing()
    {
        while (true)
        {
            
            for (int i = 0; i < Objects.Count; i++)
                if (!Objects[i])
                    Objects.RemoveAt(i);
            if (Objects.Count == 0)
                Dead();

            Electricity = 0;
            AmountBatteryIncome = 0;
            AmountBatteryExpenditure = 0;
            #region доход
            for (int i = 0; i < Computers.Count; i++)
                if (!Computers[i].beBreak)
                    Electricity += Computers[i].ElectricityGenerate;
            for (int i = 0; i < Generators.Count; i++)
                if (!Generators[i].beBreak)
                    ElectricityTransaction(Generators[i]);
            income = Electricity;
            #endregion
            #region расход
            bool AmountInBattery = true;
            for (int idObject = 0; idObject < Objects.Count; idObject++)
            {
                float ElectricityConsumption = Objects[idObject].ElectricityConsumption;
#if UNITY_EDITOR
                ElectricityConsumption = Objects[idObject].ValReadProperties[3];
#endif
                if(ElectricityConsumption > 0 && Objects[idObject].electricity)
                {

                }
                if (ElectricityConsumption < Electricity){
                    Electricity -= ElectricityConsumption;
                    Objects[idObject].electricity = true;
                }
                else
                {
                    if (AmountInBattery && Battery.Count > 0)
                    {
                        #region взятие из батарей
                        for (int idBattery = 0; idBattery < Battery.Count; idBattery++){
                            if (ElectricityConsumption <= (Electricity + Battery[idBattery].AmountElectricity)){
                                AmountBatteryIncome += ElectricityConsumption - Electricity;
                                Objects[idObject].electricity = true;
                                Battery[idBattery].UpdateBattery(Battery[idBattery].AmountElectricity - (ElectricityConsumption - Electricity));
                                Electricity = 0;
                                break;
                            }
                            else
                            {
                                Electricity += Battery[idBattery].AmountElectricity;
                                AmountBatteryIncome += Battery[idBattery].AmountElectricity;
                                Battery[idBattery].UpdateBattery(0);
                            }
                            if ((idBattery + 1) == Battery.Count){
                                Objects[idObject].electricity = false;
                                AmountInBattery = false;
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        Objects[idObject].electricity = false;
                        AmountInBattery = false;
                    }
                }
            }
            expenditure = Electricity - income;
#endregion
            #region остаток
            for (int i = 0; i < Battery.Count; i++)
            {
                if (Electricity == 0)
                    break;
                TransInBattery(ref Battery[i].AmountElectricity, Battery[i].AmountElectricityMax);
                Battery[i].UpdateBattery();
            }
            #endregion
            AmountBatteryClearIncome = AmountBatteryIncome - AmountBatteryExpenditure;

            #region узнать электричество необходимое для обеспечения всех деталей
            needElectricity = 0;
            for (int i = 0; i < Objects.Count; i++)
                needElectricity += Objects[i].ElectricityConsumption;
            #endregion
            yield return new WaitForSeconds(1f);
            
        }
       
    }
    public bool SearchConteiner(int Id, ref DetailResourceConteiner conteiner)
    {
        string Name = "ResourceConteiner";
        foreach (DetailObject detail in Objects)
            if (detail.NameDetail == Name && detail.IndexDetail == Id)
            {
                conteiner = detail.GetComponent<DetailResourceConteiner>();
                return true;
            }
        conteiner = null;
        return false;
    }
    public void ElectricityTransaction(DetailGenerator generator)
    {
        if(generator.ResourceMaxСonsumption > 0)
        {
            DetailResourceConteiner conteiner = null;
            if (SearchConteiner((int)generator.TargetConteiner,ref conteiner) &&
                conteiner.IdResourceInConteiner == generator.IdResourceСonsumption)
            {
                Electricity += TransConteiner(ref conteiner.CountResources, generator.ResourceСonsumption) /
                    generator.ResourceMaxСonsumption * generator.ElectricityGenerateMax;
                conteiner.UpdateResourcesInfo();
            }
                    
        }
        else
            Electricity += generator.ElectricityGenerateMax;
    }
    public float TransConteiner(ref float sender, float maxSumm)
    {
        if ((sender - maxSumm) < 0)
            maxSumm = sender;
        sender -= maxSumm;
        return maxSumm;
    }
    public float TransInBattery(ref float recipient, float maxInRecipient)
    {
        float maxSumm = Electricity;
        if ((recipient + maxSumm) > maxInRecipient)
            maxSumm = maxInRecipient - recipient;
        Electricity -= maxSumm;
        recipient += maxSumm;
        AmountBatteryExpenditure += maxSumm;
        return recipient;
    }
    
    void FixedUpdate()
    {
        if (!InfoCon.BeConstructor)
            for (int i = 0; i < Moves.Count; i++)
            {
                if (!Moves[i])
                    Moves.RemoveAt(i);
                else
                    rb2D.AddForceAtPosition(Moves[i].right * 50 * Moves[i].GetComponent<DetailWhell>().ValReadProperties[6], Moves[i].position);
            }
        speed = rb2D.velocity.magnitude * 100;
        if (speed > maxSpeed)
            rb2D.velocity = rb2D.velocity.normalized * maxSpeed / 100;
    }
}
