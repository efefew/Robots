using System.Collections;

using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Текст на разных языках
/// </summary>
[System.Serializable]
public struct TextLanguage
{
    public string[] text;
    // да, абстрактно, но вариант говно, так как необходимо помнить количество языков
    //public TextLanguage(params string[] txt)
    //{
    //    if(txt.Length != Setting.countLanguage)
    //        throw new System.Exception("текст не на всех языках");
    //    text = new string[Setting.countLanguage];
    //    for (int idLanguage = 0; idLanguage < Setting.countLanguage; idLanguage++)
    //        text[idLanguage] = txt[idLanguage];
    //}
    /// <summary>
    /// Текст на разных языках
    /// </summary>
    /// <param name="english">Английский</param>
    /// <param name="russian">Русский</param>
    public TextLanguage(string english, string russian)
    {
        text = new string[Setting.countLanguage];
        text[0] = english;
        text[1] = russian;
    }
}
/// <summary>
/// Деталь
/// </summary>
[RequireComponent(typeof(ResourceDetail))]
[RequireComponent(typeof(Rigidbody2D))]
public class DetailObject : MonoBehaviour
{
    protected readonly Color32
    blue = new Color32(41, 201, 202, 255),
    green = new Color32(30, 255, 173, 255),
    yellow = new Color32(232, 255, 70, 255),
    orange = new Color32(232, 145, 49, 255),
    red = new Color32(255, 30, 32, 255),
    black = new Color32(10, 10, 10, 255);

    public enum DamageType
    {
        Punch,
        Explosion,
        Shooting,
        Burning,
        ElectricShock,
        DissolutionInAcid,
        Healing,
        Absolute
    }

    [Header("характеристики, которые можно показать при покупке детали (>4)")]
    public int MaxIdCharacteristic;
    [HideInInspector]
    public int IndexDetail;

    public Rigidbody2D NewConector;
    [HideInInspector]
    public Rigidbody2D MyConector;
    private Rigidbody2D rb2D;

    public bool BeMainDetail;
    public bool beBreak { get; private set; }
    [HideInInspector]
    public bool electricity, beOrentation;
    protected bool firstValidateDescription;
    private bool delete = false, controler, free;

    public Transform imageConector;
    protected Transform tr;

    public GameObject visualisationDetail, orentationObject, IndexObject;
    [HideInInspector]
    public GameObject ConectorObj;
    public GameObject[] UselessObjects, DeactiveInConsructor,
        visualisationProperty = new GameObject[] { null },
        visualisationReadProperty = new GameObject[] { null, null, null, null, null };
    /// <summary>
    /// Выгрузка ресурсов
    /// </summary>
    private GameObject dustParticle, boomParticle, spark;


    public Sprite[] destroySprites;
    public Image[] solidImages;
    public Image destroyEffect;
    public ConstructorManager constructor { get; set; }
    [HideInInspector]
    public DetailComputer MainBrain;
    protected GameManagement game;
    private FixedJoint2D joint2D;
    private Text Index;
    private DetailObject detailConector;
    protected Animator animator;

    public string NameDetail;
    public string[] NameProperties = new string[] { "NULL" }, NameReadProperties = new string[] {
        "Health",
        "MaxHealth",
        "Mass",
        "ElectricityConsumption",
        "MaxElectricityConsumption" };

    public TextLanguage labelDetail, descriptionDetail;
    public TextLanguage[] labelProperty,
        labelReadProperty = new TextLanguage[]
        {
            new TextLanguage("Health", "Здоровье"),
            new TextLanguage("MaxHealth", "Максимальное здоровье"),
            new TextLanguage("Mass", "Масса"),
            new TextLanguage("ElectricityConsumption", "Потребление электричства"),
            new TextLanguage("MaxElectricityConsumption", "Максимальное потребление электричства"),
        },
        descriptionsProperty, descriptionsReadProperty;

    public float addMass;
    [HideInInspector]
    public float MaxElectricityConsumption, ElectricityConsumption;
    public float[] ValProperties
    {
        get
        {
            CalculationOfProperties(ValueProperties); //сообщаем о показе
            return ValueProperties;
        }
        set
        {
            CalculationOfProperties(value); //сообщаем о изменении
        }
    }
    public float[] ValReadProperties
    {
        get
        {
            CalculationOfProperties(ValueProperties); //сообщаем о показе
            return ValueReadProperties;
        }
    }
    protected float orentation;
    [SerializeField]
    protected float[] ValueProperties = new float[] { 0 }, ValueReadProperties = new float[] { 100, 100, 1, 0, 0 };
    private float Health, MaxHealth, Mass, baseMass;
    private const float forceMinimum = 10f;

    protected virtual void Start()
    {
        OnLoadResources();
        tr = transform;

        if (!destroyEffect)
            Debug.LogError("Забыл про эффект разрушения");
        animator = GetComponent<Animator>();
        MaxHealth = ValueReadProperties[1];
        MaxElectricityConsumption = ValueReadProperties[4];
        electricity = true;
        ChangeHealth(MaxHealth, DamageType.Healing);
        rb2D = GetComponent<Rigidbody2D>();
        baseMass = rb2D.mass;
        Mass = baseMass + addMass;
        ValueReadProperties[2] = Mass;
        StartCoroutine(EnableJoint2D());
        if (!BeMainDetail)
        {
            if (!MyConector)
                Debug.LogError("деталь " + tr.name + " не присоеденилась");
            ConectorObj = MyConector.gameObject;
            joint2D = GetComponent<FixedJoint2D>();
            detailConector = ConectorObj.GetComponent<DetailObject>();
            detailConector.OnConnectedToJoint2D(joint2D);
        }
        if (imageConector)
        {
            if (detailConector.NewConector == null)
                imageConector.SetParent(ConectorObj.transform);
            else
                imageConector.SetParent(detailConector.NewConector.transform);
            imageConector.name = "conector ";
            imageConector.SetAsFirstSibling();
        }

        if (InfoCon.BeConstructor)
        {
            OnConstructor();
            //Destroy(Instantiate(dustParticle, tr.position, Quaternion.identity, tr), 2);
            return;
        }
        if (orentationObject)
        {
            Destroy(orentationObject);
            //if (BeOrentation)
            //    OnChangeOrentation(true);
            //orentationObject.SetActive(false);
            //Destroy(orentationObject.GetComponent<Button>());
        }
        game = MainBrain.transform.parent.parent.GetComponent<GameManagement>();
        //Destroy(Instantiate(dustParticle, tr.position, Quaternion.identity, game.transform), 2);
        if (UselessObjects.Length > 0)
            for (int i = 0; i < UselessObjects.Length; i++)
                Destroy(UselessObjects[i]);
        UselessObjects = null;
        CalculationOfProperties(ValueProperties);
    }
    private void OnEnable()
    {
        StartCoroutine(AnimateOnCreate(1));
    }
    protected virtual void Update()
    {
        if (beBreak)
            return;

        if (InfoCon.BeConstructor)
        {
            SwichActiveConteiner(ref delete, InfoCon.BeDelete);
            SwichActiveConteiner(ref controler, InfoCon.BeController);
            if (!BeMainDetail && !MyConector)
                constructor.DestroyObj(gameObject);
        }
        else
        {
            DisconnectTheConnection();
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        float Force = Mathf.Abs(collision.relativeVelocity.x) + Mathf.Abs(collision.relativeVelocity.y);
        if (Force < forceMinimum)
            return;
        Vector3 posSpark = collision.contacts[0].point;
        posSpark = new Vector3(posSpark.x, posSpark.y, tr.position.z);
        Destroy(Instantiate(spark, posSpark, Quaternion.Euler(0, 0, Random.Range(0f, 360f)), tr), 1);
        //Debug.Log(
        //    transform.name + " столкнулся с " + collision.transform.name +
        //    ", скорость при ударе: " + collision.relativeVelocity +
        //    ", сила удара: " + Force);
        ChangeHealth(Force, DamageType.Punch);
    }


    private void OnLoadResources()
    {
        //IndexObject = (GameObject)Resources.Load("Constructor/Index");

        dustParticle = (GameObject)Resources.Load("Effects/dust");
        boomParticle = (GameObject)Resources.Load("Effects/particle boom");
        spark = (GameObject)Resources.Load("Effects/spark");
    }
    /// <summary>
    /// Расчёт свойств, фильтруя новые значения свойств
    /// </summary>
    /// <param name="newValProperties">новые значения свойств</param>
    protected virtual void CalculationOfProperties(float[] newValProperties)
    {
    }

    /// <summary>
    /// Если деталь в конструкторе
    /// </summary>
    private void OnConstructor()
    {
        if (!BeMainDetail)
        {
            for (int i = 0; i < DeactiveInConsructor.Length; i++)
                DeactiveInConsructor[i].SetActive(false);
            IndexObject = Instantiate(IndexObject, transform.position, Quaternion.identity, transform);
            IndexObject.GetComponent<InputField>().onEndEdit.AddListener(delegate { CheckChangeIndex(); });
            Index = IndexObject.GetComponentInChildren<Text>();
            IndexObject.GetComponent<InputField>().text = IndexDetail.ToString();
            IndexObject.SetActive(false);
            if (orentationObject)
            {
                orentationObject.SetActive(false);
                constructor.orientationButs.Add(orentationObject);
            }
        }
        constructor.IndexButs.Add(IndexObject);
    }

    /// <summary>
    /// включает joint2D с задержкой
    /// </summary>
    /// <returns></returns>
    private IEnumerator EnableJoint2D()
    {
        yield return new WaitForFixedUpdate();
        GetComponent<FixedJoint2D>().enabled = true;
    }
    /// <summary>
    /// Проигрывание анимации при создании детали
    /// </summary>
    private IEnumerator AnimateOnCreate(float duration = 1f)
    {
        float elapsed = 0f;
        if (solidImages.Length == 0)
        {
            Debug.LogError("забыл добавить однотонные изображения");
            yield break;
        }
        for (int id = 0; id < solidImages.Length; id++)
            solidImages[id].enabled = true;
        while (elapsed < duration)
        {
            for (int id = 0; id < solidImages.Length; id++)
                solidImages[id].color = new Color(1f, 1f, 1f, (duration - elapsed) / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        for (int id = 0; id < solidImages.Length; id++)
            solidImages[id].enabled = false;
    }
    /// <summary>
    /// Задать активность сплошных изображений
    /// </summary>
    /// <param name="active"></param>
    public void ActiveSolidImages(bool active)
    {
        for (int id = 0; id < solidImages.Length; id++)
            solidImages[id].enabled = active;
    }
    /// <summary>
    /// Задать цвет сплошным изображениям
    /// </summary>
    /// <param name="color">цвет</param>
    public void SetColorSolidImages(Color32 color)
    {
        for (int id = 0; id < solidImages.Length; id++)
            solidImages[id].color = color;
    }
    /// <summary>
    /// Расчёт транспортировки ресурсов
    /// </summary>
    /// <param name="sender">количество ресурсов в хранилище откуда забирают ресурсы</param>
    /// <param name="maxSumm">максимальное количество ресурсов, которые будут отправленны</param>
    /// <param name="recipient">количество ресурсов в хранилище куда отправляются ресурсы</param>
    /// <param name="maxInRecipient">максимально возможное количество ресурсов в хранилище</param>
    /// <returns>новое количество ресурсов в хранилище куда отправляются ресурсы</returns>
    public float TransferOfResources(ref float sender, float maxSumm, float recipient = 0, float maxInRecipient = 0)
    {
        if ((sender - maxSumm) < 0)
            maxSumm = sender;
        if (maxInRecipient != 0 && (maxSumm + recipient) > maxInRecipient)
            maxSumm = maxInRecipient - recipient;
        sender -= maxSumm;
        recipient += maxSumm;
        return recipient;
    }
    /// <summary>
    /// Ищет деталь в роботе по имени и индексу
    /// </summary>
    /// <param name="Name">имя детали</param>
    /// <param name="Id">индекс детали</param>
    /// <param name="detailNeed">деталь, которую ищут</param>
    /// <returns>нашёл деталь?</returns>
    public bool SearchDetail(string Name, int Id, out DetailObject detailNeed)
    {
        foreach (DetailObject detail in MainBrain.Details)
            if (detail.NameDetail == Name && detail.IndexDetail == Id)
            {
                detailNeed = detail;
                return true;
            }
        detailNeed = null;
        return false;
    }
    /// <summary>
    /// Расчёт потребления ресурсов
    /// </summary>
    /// <param name="sender">хранилище откуда заберут ресурсы</param>
    /// <param name="summ">сколько заберут?</param>
    /// <returns>хватило?</returns>
    public bool Transaction(ref float sender, float summ)
    {
        if (sender < summ)
            return false;
        sender -= summ;
        return true;
    }
    [ContextMenu("TestDamage")]
    public void TestDamage()
    {
        ChangeHealth(MaxHealth * 0.1f, DamageType.Absolute);
        Debug.Log("<color=#00aa00>Здоровье: " + (Health * 100 / MaxHealth) + "%</color>");
    }
    /// <summary>
    /// Изменяет количество здоровья и расчитывает последствия
    /// </summary>
    /// <param name="damage">величина изменения здоровья</param>
    public void ChangeHealth(float damage, DamageType damageType)
    {
        switch (damageType)
        {
            case DamageType.Punch:
                Health -= damage;
                break;
            case DamageType.Explosion:
                Health -= damage;
                break;
            case DamageType.Shooting:
                Health -= damage;
                break;
            case DamageType.Burning:
                Health -= damage;
                break;
            case DamageType.ElectricShock:
                Health -= damage;
                break;
            case DamageType.DissolutionInAcid:
                Health -= damage;
                break;
            case DamageType.Healing:
                Health += damage;
                break;
            case DamageType.Absolute:
                Health -= damage;
                break;
            default:
                Debug.Log("как это произошло?");
                break;
        }

        if (Health > MaxHealth)
        {
            Health = MaxHealth;
            destroyEffect.enabled = false;
            beBreak = true;
        }
        ValueReadProperties[0] = Health;
        if (Health <= MaxHealth * 0.75)
        {
            destroyEffect.enabled = true;
            destroyEffect.sprite = destroySprites[0];
        }
        if (Health <= MaxHealth * 0.5)
        {
            destroyEffect.sprite = destroySprites[1];
        }
        if (Health <= MaxHealth * 0.25)
        {
            destroyEffect.sprite = destroySprites[2];
            beBreak = true;
        }
        if (Health <= MaxHealth * 0.1 && MainBrain != null && !BeMainDetail)
        {
            ActiveSolidImages(false);
            electricity = false;
            free = true;
            ConectorObj = null;
            if (imageConector)
                Destroy(imageConector.gameObject);
            if (GetComponent<FixedJoint2D>())
                Destroy(GetComponent<FixedJoint2D>());
            MainBrain.DestroyDetail(this);
            MainBrain = null;
        }
        if (Health <= 0)
            Dead();
    }
    /// <summary>
    /// Анигиляция детали
    /// </summary>
    [ContextMenu("Dead")]
    public void Dead()
    {
        Destroy(Instantiate(boomParticle, tr.position, Quaternion.identity, game.transform), 2);
        if (BeMainDetail && tr.parent.GetComponent<RobotManager>())
            tr.parent.GetComponent<RobotManager>().OnDeadMainDetail();
        game.DestroySomething(gameObject, time: 0.1f);
    }
    /// <summary>
    /// фильтр ячеек по направлению
    /// </summary>
    /// <param name="angle">требуемый угол</param>
    public void FilterDetailConteiner(float angle)
    {
        angle *= 45f;
        if (UselessObjects.Length > 0 && angle != 360)
        {
            for (int i = 0; i < UselessObjects.Length; i++)
            {
                if (UselessObjects[i].transform.eulerAngles.z <= (angle + 5f) && UselessObjects[i].transform.eulerAngles.z >= (angle - 5f))
                    UselessObjects[i].SetActive(true);
                else
                    UselessObjects[i].SetActive(false);
            }
        }
        else
            for (int i = 0; i < UselessObjects.Length; i++)
                UselessObjects[i].SetActive(true);
    }
    /// <summary>
    /// Вычисляет массу
    /// </summary>
    /// <param name="countResource">количество ресурсов в детали</param>
    /// <param name="idResource">тип ресурса</param>
    public void SetMass(float countResource, int idResource = -1)
    {
        Mass = baseMass + ResourcesStat.GetMass(idResource) * countResource;
        rb2D.mass = Mass;
        Mass += addMass;
        ValueReadProperties[2] = Mass;
    }
    /// <summary>
    /// Поменять направление в конструкторе
    /// </summary>
    /// <param name="_default">поумолчанию</param>
    public void OnChangeOrentation(bool _default = false)
    {
        beOrentation = _default || !beOrentation;
        orentationObject.GetComponent<Image>().color = beOrentation ? red : green;
        orentationObject.transform.localEulerAngles = beOrentation ? new Vector3(0, 0, 180) : new Vector3(0, 0, 0);
    }

    /// <summary>
    /// Включает и выключает ячейки
    /// </summary>
    /// <param name="Be">локальное условие</param>
    /// <param name="InfoConBe">глобальное условие</param>
    private void SwichActiveConteiner(ref bool Be, bool InfoConBe)
    {
        if (Be != InfoConBe)
        {
            if (InfoConBe)
            {
                if (UselessObjects.Length > 0)
                    for (int i = 0; i < UselessObjects.Length; i++)
                        UselessObjects[i].SetActive(false);
            }
            else
                FilterDetailConteiner(constructor.typeRotateBuild.value);
            Be = InfoConBe;
        }
    }

    /// <summary>
    /// Проверка изменения индекса
    /// </summary>
    private void CheckChangeIndex()
    {
        if (Index.text == "" || (NameDetail == "Computer" && Index.text == "0"))
            Index.text = IndexDetail.ToString();
        if (Index.text != IndexDetail.ToString())
            constructor.SearchSwichIndex(IndexDetail, Index.text.ToInt(), this);
    }

    /// <summary>
    /// Проверка на отсоединение от робота
    /// </summary>
    private void DisconnectTheConnection()
    {
        if (!BeMainDetail)
        {
            if (joint2D && joint2D.connectedBody == null)
                Destroy(joint2D);
            if (MainBrain != null && (detailConector == null || detailConector.free))
            {
                free = true;
                beBreak = true;
                electricity = false;
                MainBrain.DestroyDetail(this);
                MainBrain = null;
            }
        }
    }
    /// <summary>
    /// Обновление направления
    /// </summary>
    public void UpdateOrentation() => orentation = beOrentation ? -1 : 1;
    /// <summary>
    /// Присоединение к детали
    /// </summary>
    /// <param name="joint2D">присоеденитель</param>
    public virtual void OnConnectedToJoint2D(FixedJoint2D joint2D)
    {
        if (NewConector != null)
            joint2D.connectedBody = NewConector;
    }
    /// <summary>
    /// Обновление описания детали
    /// </summary>
    public virtual void ValidateOnDescription()
    {
        if (firstValidateDescription)
            return;
        firstValidateDescription = true;
        descriptionsProperty = new TextLanguage[NameProperties.Length];
        descriptionsReadProperty = new TextLanguage[NameReadProperties.Length];

        labelProperty = new TextLanguage[NameProperties.Length];
        labelReadProperty = new TextLanguage[NameReadProperties.Length];

        labelReadProperty[0] = new TextLanguage("Health", "Здоровье");
        labelReadProperty[1] = new TextLanguage("MaxHealth", "Максимальное здоровье");
        labelReadProperty[2] = new TextLanguage("Mass", "Масса");
        labelReadProperty[3] = new TextLanguage("Electricity consumption", "Потребление электричества");
        labelReadProperty[4] = new TextLanguage("Max electricity consumption", "Максимальное потребление электричества");


        descriptionsReadProperty[0] = new TextLanguage(

            "A parameter that determines health at the current time.\n" +
            "Ranges from 0 to max health.\n" +
            "At 75% of max health, cracks appear, at 50% the cracks get stronger, at 25% the part stops working (but can be repaired), at 10% the part comes off the robot and cannot be repaired.\n" +
            "When health reaches zero, the detail will be annihilated.",

            "Параметр определяющий здоровье в текущий момент времени.\n" +
            "Находится в диапазоне от 0 до максимального здоровья.\n" +
            "При 75% от максимального здоровья появляются трещины, при 50% трещины усиливаются, при 25% деталь перестаёт работать(но её можно починить), при 10% деталь отрывается от робота и её нельзя починить.\n" +
            "Когда здоровье достигнет нуля деталь аннигилируется.");

        descriptionsReadProperty[1] = new TextLanguage(

            "The parameter that determines the maximum health.",

            "Параметр определяющий максимальное здоровье.");

        descriptionsReadProperty[2] = new TextLanguage(

            "A parameter that determines the mass.\n" +
            "Affects the inertia of the robot.",

            "Параметр определяющий массу.\n" +
            "Влияет на инерцию робота.");

        descriptionsReadProperty[3] = new TextLanguage(

            "A parameter that determines electricity consumption.\n" +
            "Consumption depends on the sum of all the stats that contribute to the power of the part.\n" +
            "Ranges from 0 to maximum electricity consumption.",

            "Параметр определяющий потребление электричества.\n" +
            "Потребление зависит от суммы всех характеристик, которые влияют на мощь детали.\n" +
            "Находится в диапазоне от 0 до максимального потребления электричества.");

        descriptionsReadProperty[4] = new TextLanguage(

            "A parameter that determines the maximum consumption of electricity.",

            "Параметр определяющий максимальное потребление электричества.");
    }
    /// <summary>
    /// тип зависимости параметра
    /// </summary>
    public enum TypeDependence
    {
        BeterBeter,
        LessLess,
        BeterLess,
        LessBeter
    }
    /// <summary>
    /// Зависимость параметра
    /// </summary>
    /// <param name="dependence1">зависимость 1</param>
    /// <param name="dependence2">зависимость 2</param>
    /// <returns>текст, описывающий зависимость</returns>
    protected string DescriptionDependence(Language.LanguageType languageType, string dependence1, TypeDependence type = TypeDependence.BeterBeter, string dependence2 = "потребление электричества")
    {
        switch (languageType)
        {
            case Language.LanguageType.english:
                if (dependence2 == "потребление электричества")
                    dependence2 = "electricity consumption";
                switch (type)
                {
                    case TypeDependence.BeterBeter: return "The greater the " + dependence1 + ", the greater the " + dependence2 + ".\n";
                    case TypeDependence.LessLess: return "The lower the " + dependence1 + ", the lower the " + dependence2 + ".\n";
                    case TypeDependence.BeterLess: return "The greater the " + dependence1 + ", the lower the " + dependence2 + ".\n";
                    case TypeDependence.LessBeter: return "The lower the " + dependence1 + ", the greater the " + dependence2 + ".\n";
                    default: return "";
                }
            case Language.LanguageType.russian:
                switch (type)
                {
                    case TypeDependence.BeterBeter: return "Чем больше " + dependence1 + ", тем больше " + dependence2 + ".\n";
                    case TypeDependence.LessLess: return "Чем меньше " + dependence1 + ", тем меньше " + dependence2 + ".\n";
                    case TypeDependence.BeterLess: return "Чем больше " + dependence1 + ", тем меньше " + dependence2 + ".\n";
                    case TypeDependence.LessBeter: return "Чем меньше " + dependence1 + ", тем больше " + dependence2 + ".\n";
                    default: return "";
                }
        }
        return "";
    }
    /// <summary>
    /// Диапозон параметра
    /// </summary>
    /// <param name="min">минимальное значение</param>
    /// <param name="max">максимальное значение</param>
    /// <returns>текст, описывающий диапозон</returns>
    protected string DescriptionRange(Language.LanguageType languageType, string min = "0", string max = "∞")
    {
        switch (languageType)
        {
            case Language.LanguageType.english:
                return "Is in the range from " + min + " to " + max + ".\n";
            case Language.LanguageType.russian:
                return "Находится в диапазоне от " + min + " до " + max + ".\n";
            default: return "";
        }
    }
    /// <summary>
    /// Обновление данных сенсора
    /// </summary>
    /// <param name="value">процент</param>
    /// <param name="sensors">массив сенсоров</param>
    protected void UpdateSensor(Image[] sensors, float value)
    {
        value = Mathf.Clamp(value, 0f, 1f);
        Color32 colorTarget = RainbowColor(value);
        for (int idSensor = 0; idSensor < sensors.Length; idSensor++)
            sensors[idSensor].color = ((idSensor + 1f) / sensors.Length - 0.15f) <= value ? colorTarget : black;
    }
    /// <summary>
    /// Превращает процент в цвет
    /// </summary>
    /// <param name="value">процент</param>
    /// <returns>цвет</returns>
    protected Color32 RainbowColor(float value)
    {
        if (value > 0.95)
            return blue;
        if (value > 0.75)
            return green;
        if (value > 0.50)
            return yellow;
        if (value > 0.35)
            return orange;
        return red;
    }
}
