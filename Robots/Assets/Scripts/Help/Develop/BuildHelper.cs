using UnityEngine;
using UnityEngine.UI;

public class BuildHelper : MonoBehaviour
{
    public DetailObject[] details;
    public DescriptionManager descriptionManager;
    public GameObject contentProperties, Number;
    public PropertyScript scriptClass, scriptProperty;
    Transform tr;
    ToggleGroup toggleGroup;
    //public Color32 mainColor;
    //string strMainColor = "#2cd7a8";//"#" + mainColor.r.ToString("x") + mainColor.g.ToString("x") + mainColor.b.ToString("x");
    public bool addReadProperties;
    bool oldReadVariable, oldIfOrFor;
    private void Awake()
    {
        tr = transform;
    }
    private void OnEnable()
    {
        oldReadVariable = Info.ReadVariable;
        oldIfOrFor = Info.IfOrFor;
        addReadProperties = Info.ReadVariable || Info.IfOrFor;
        Clear();
        Create();
    }
    void Update()
    {
        if(oldReadVariable != Info.ReadVariable || oldIfOrFor != Info.IfOrFor)
        {
            oldReadVariable = Info.ReadVariable;
            oldIfOrFor = Info.IfOrFor;
            addReadProperties = Info.ReadVariable || Info.IfOrFor;
            Clear();
            Create();
        }
    }
    void Create()
    {
        Number.gameObject.SetActive(addReadProperties);
        for (int index = 0; index < details.Length; index++)
        {
            if (!addReadProperties && details[index].NameProperties[0] == "NULL")
                continue;
            details[index].ValidateOnDescription();
            toggleGroup = tr.GetComponent<ToggleGroup>();
            PropertyScript MainComponent = Instantiate(scriptClass, tr);
            Toggle mainToggle = MainComponent.BuildCommands(details[index].NameDetail, descriptionManager, details[index].descriptionDetail.text, details[index].visualisationDetail);
            mainToggle.group = toggleGroup;

            Transform content = Instantiate(contentProperties, tr).transform;
            content.name = "Content " + details[index].NameDetail;
            ToggleGroup contentTG = content.GetComponent<ToggleGroup>();

            mainToggle.onValueChanged.AddListener(content.gameObject.SetActive);
            CreateProperties(details[index], contentTG, content);
            if(addReadProperties)
                CreateReadProperties(details[index], contentTG, content);

        }
    }
    void Clear()
    {
        int countSkip = 2;
        if (tr.childCount <= countSkip)
            return;
        for (int index = countSkip; index < tr.childCount; index++)
            Destroy(tr.GetChild(index).gameObject);
    }
    void CreateProperties(DetailObject detail, ToggleGroup group, Transform content)
    {
        for (int i = 0; i < detail.NameProperties.Length; i++)
        {
            if (detail.NameProperties[i] == "NULL")
                break;
            PropertyScript property = Instantiate(scriptProperty, content);
            property.BuildCommands(detail.NameProperties[i], descriptionManager, detail.descriptionsProperty[i].text, detail.visualisationProperty[i]).group = group;

            BlockTextCode block = property.GetComponent<BlockTextCode>();
            block.TextCodeStart = "$" + detail.NameDetail + "[";
            block.TextInterfaceStart = detail.NameDetail + "[";
            block.TextCodeEnd = "]." + detail.NameProperties[i] + "€";
            block.TextInterfaceEnd = "]." + detail.NameProperties[i];
        }
    }
    void CreateReadProperties(DetailObject detail, ToggleGroup group, Transform content)
    {
        for (int i = 0; i < detail.NameReadProperties.Length; i++)
        {
            if (detail.NameReadProperties[i] == "NULL")
                break;
            PropertyScript Obj = Instantiate(scriptProperty, content);
            Obj.BuildCommands(detail.NameReadProperties[i], descriptionManager, detail.descriptionsReadProperty[i].text, detail.visualisationReadProperty[i]).group = group;

            BlockTextCode block = Obj.GetComponent<BlockTextCode>();
            block.TextCodeStart = "$" + detail.NameDetail + "[";
            block.TextInterfaceStart = detail.NameDetail + "[";
            block.TextCodeEnd = "]." + detail.NameReadProperties[i] + "€";
            block.TextInterfaceEnd = "]." + detail.NameReadProperties[i];
        }
    }
}
