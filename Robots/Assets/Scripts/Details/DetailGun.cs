using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DetailGun : DetailObject
{

    DetailObject detailConteiner;
    float Damage, Speed, DeadTime, TimeRecharge, MinTimeRecharge, CountNeedResource;
    int IdConteiner, IdNeedResource;
    bool Shoot, Recharge;
    public float  addForward, scaleRecoil;
    public GameObject bulletObj;
    public Bullet bulletStats;
    public Image sensor, lightSensor;
    Rigidbody2D rbRobot;
    Transform Robot;
    DetailResourceConteiner conteiner;
    //[HideInInspector]
    //public float AmountElectricityMax, AmountElectricity;
    void Awake()
    {
        MinTimeRecharge = ValueReadProperties[5];
        Damage = ValueReadProperties[6];
        Speed = ValueReadProperties[7];
        DeadTime = ValueReadProperties[8];
        IdNeedResource = (int)ValueReadProperties[10];
        CountNeedResource = ValueReadProperties[11];
    }
    protected override void Start()
    {
        conteiner = null;
        Recharge = true;
        sensor.color = green;
        lightSensor.color = green;
        if (!InfoCon.BeConstructor)
        {
            Robot = MainBrain.transform.parent;
            rbRobot = Robot.GetComponent<Rigidbody2D>();
        }
        base.Start();
    }
    public override void ValidateOnDescription()
    {
        if (firstValidateDescription)
            return;
        base.ValidateOnDescription();

        labelProperty[0] = new TextLanguage("Shoot", "Выстрел");
        labelProperty[1] = new TextLanguage("Recharge time", "Время перезарядки");
        labelProperty[2] = new TextLanguage("Target container", "Целевой контейнер");

        labelReadProperty[5] = new TextLanguage("Minimum recharge time", "Минимальное время перезарядки");
        labelReadProperty[6] = new TextLanguage("Damage", "Урон");
        labelReadProperty[7] = new TextLanguage("Bullet speed", "Скорость пули");
        labelReadProperty[8] = new TextLanguage("Bullet lifetime", "Время жизни пули");
        labelReadProperty[9] = new TextLanguage("Recharge", "Перезарядка");
        labelReadProperty[10] = new TextLanguage("Resource ID", "Идентификатор Ресурса");
        labelReadProperty[11] = new TextLanguage("Amount of resources required", "Количество необходимых ресурсов");

        descriptionDetail = new TextLanguage(

            "The cannon fires projectiles that deal damage.",

            "Пушка стреляет снарядами, наносящими урон.");
        
        descriptionsProperty[0] = new TextLanguage(

            "A parameter that determines whether the gun will shoot.\n" +
            "At 0 it doesn't shoot, at 1 it shoots if all conditions are met (there is electricity and enough resources).\n" +
            "If you assign a value less than 0, it will be replaced by 0.\n" +
            "If you assign a value greater than 0, it will be replaced by 1.",

            "Параметр определяющий будет ли стрелять пушка.\n" +
            "При 0 не стреляет, при 1 стреляет если все условия соблюдены(есть электричество и достаточно ресурсов).\n" +
            "Если присвоить значение меньше 0, то оно заменится на 0.\n" +
            "Если присвоить значение больше 0, то оно заменится на 1.");
            
        descriptionsProperty[1] = new TextLanguage(

            "A parameter that determines the desired reload time for a shot." +
            DescriptionDependence(Language.LanguageType.english, "recharge time", TypeDependence.LessBeter) +
            DescriptionRange(Language.LanguageType.english, min: "minimum possible shot reload time"),

            "Параметр определяющий желаемое время перезарядки выстрела." +
            DescriptionDependence(Language.LanguageType.russian, "время перезарядки", TypeDependence.LessBeter) +
            DescriptionRange(Language.LanguageType.russian, min: "минимально возможного времени перезарядки выстрела"));
            
        descriptionsProperty[2] = new TextLanguage(

            "A parameter that defines the index of the container from which the resource is consumed (if the resource is needed for the gun).\n" +
            "The index of the resource in this container must match the index of the resource, " +
            "required for the production of shells by a cannon.",

            "Параметр определяющий индекс контейнера, из которого потребляется ресурс(если для пушки необходим ресурс).\n" +
            "Индекс ресурса в этом контейнере должен совпадать с индексом ресурса, " +
            "тербуемого для производства снарядов пушкой.");
            

        descriptionsReadProperty[5] = new TextLanguage(

            "A parameter that determines the minimum possible reload time for a shot.",

            "Параметр определяющий минимально возможное время перезарядки выстрела.");
            
        descriptionsReadProperty[6] = new TextLanguage(

            "A parameter that determines projectile damage.",

            "Параметр определяющий урон снаряда.");
            
        descriptionsReadProperty[7] = new TextLanguage(

            "A parameter that determines the speed of the projectile.",

            "Параметр определяющий скорость снаряда.");
            
        descriptionsReadProperty[8] = new TextLanguage(

            "A parameter that determines the lifespan of a projectile.",

            "Параметр определяющий продолжительность жизни снаряда.");
            
        descriptionsReadProperty[9] = new TextLanguage(

            "A parameter that determines whether the gun is reloaded.\n" +
            "0 - not recharged, 1 - recharged.",

            "Параметр определяющий перезарядилась ли пушка.\n" +
            "0 - не перезарядилась, 1 - перезарядилась.");
            
        descriptionsReadProperty[10] = new TextLanguage(

            "Parameter defining resource index, " +
            "required for the production of shells by a cannon.",

            "Параметр определяющий индекс ресурса, " +
            "тербуемый для производства снарядов пушкой.");
            
        descriptionsReadProperty[11] = new TextLanguage(

            "The parameter that determines the amount of resource, " +
            "required for the production of shells by a cannon.",

            "Параметр определяющий количество ресурса, " +
            "тербуемого для производства снарядов пушкой.");
    }
    protected override void CalculationOfProperties(float[] newValProperties)
    {
        if (beBreak)
            return;

        Shoot = newValProperties[0] > 0 ? true : false;
        TimeRecharge = newValProperties[1];
        if (TimeRecharge < MinTimeRecharge)
            TimeRecharge = MinTimeRecharge;

        ValueReadProperties[3] = MaxElectricityConsumption * (MinTimeRecharge / TimeRecharge);
        ElectricityConsumption = ValueReadProperties[3];
        if (!electricity || InfoCon.BeConstructor)
            return;
        IdConteiner = (int)newValProperties[2];
        ValueProperties[0] = Shoot ? 1 : 0;
        ValueProperties[1] = TimeRecharge;
        ValueProperties[2] = IdConteiner;
        ValueReadProperties[9] = Recharge ? 1 : 0;
        if (SearchDetail("ResourceConteiner", IdConteiner, out detailConteiner))
            conteiner = detailConteiner.GetComponent<DetailResourceConteiner>();
        else
            conteiner = null;
    }
    protected override void Update()
    {
        base.Update();
        if (beBreak || !electricity || InfoCon.BeConstructor)
        {
            sensor.gameObject.SetActive(false);
            return;
        }
        else
        {
            sensor.gameObject.SetActive(true);
        }
        if (Shoot && Recharge)
        {
            animator.SetBool("shoot", true);
            if (CountNeedResource > 0)
            {
                if (conteiner != null &&
                conteiner.IdResourceInConteiner == IdNeedResource &&
                Transaction(ref conteiner.CountResources, CountNeedResource))
                {
                    conteiner.UpdateResourcesInfo();
                    StartCoroutine(RechargeCoroutine());
                }
            }
            else
                StartCoroutine(RechargeCoroutine());
        }
        else
            animator.SetBool("shoot", false);
    }
    IEnumerator RechargeCoroutine()
    {
        if (beBreak || !electricity)
            yield break;
        Recharge = false;
        sensor.color = red;
        lightSensor.color = red;
        Transform trBullet;
        trBullet = Instantiate(bulletObj, transform.position + transform.up * addForward, transform.rotation, Robot == null ? transform : Robot.parent).transform;
        Bullet bullet = trBullet.GetComponent<Bullet>();
        bullet.Damage = Damage;
        bullet.Speed = Speed;
        bullet.DeadTime = DeadTime;
        bullet.camSnaker = game.camScript;
        if (rbRobot != null)
            rbRobot.AddForceAtPosition(-trBullet.up * scaleRecoil * bullet.Damage, trBullet.position);
        yield return new WaitForSeconds(TimeRecharge);
        Recharge = true;
        sensor.color = green;
        lightSensor.color = green;
    }
}
