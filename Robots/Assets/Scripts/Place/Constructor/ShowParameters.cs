using UnityEngine;
using UnityEngine.UI;

public class ShowParameters : MonoBehaviour
{
    public ConstructorManager constructor;
    public Transform content, resourceLabelsConteiner;
    public Parameter parameter;
    public Text title;
    public DescriptionManager descriptionManager;
    Text[] resourceLabels;
    void Awake()
    {
        resourceLabels = new Text[ResourcesStat.countResource];
        for (int id = 0; id < ResourcesStat.countResource; id++)
            resourceLabels[id] = resourceLabelsConteiner.GetChild(id).GetComponent<Text>();
    }
    public void OnChangeTarget(DetailObject detail)
    {
        //titleBackground.color = backColor;
        //title.color = mainColor;
        detail.ValidateOnDescription();
        title.text = detail.labelDetail.text[(int)Language.language];
        title.GetComponent<Description>().SetDescription(detail.descriptionDetail.text, detail.visualisationDetail);

        for (int id = 0; id < content.childCount; id++)
            Destroy(content.GetChild(id).gameObject);

        CreateCharacteristic(detail, 1);
        CreateCharacteristic(detail, 2);
        CreateCharacteristic(detail, 4);

        if (detail.MaxIdCharacteristic > 4)
            for (int id = 5; id <= detail.MaxIdCharacteristic; id++)
                CreateCharacteristic(detail, id);

        ResourceDetail resourceDetail = detail.GetComponent<ResourceDetail>();
        for (int id = 0; id < ResourcesStat.countResource; id++)
            resourceLabels[id].text = resourceDetail.resourceComponents[id].ToString();
    }
    void CreateCharacteristic(DetailObject detail, int id)
    {
        detail.ValidateOnDescription();
        Parameter tempParameter = Instantiate(parameter, content);
        tempParameter.title.text = detail.labelReadProperty[id].text[(int)Language.language];
        tempParameter.title.GetComponent<Description>().SetDescription(descriptionManager, detail.descriptionsReadProperty[id].text, detail.visualisationReadProperty[id]);
        tempParameter.readValue.GetComponent<Text>().text = detail.ValReadProperties[id].ToString();
    }
}
