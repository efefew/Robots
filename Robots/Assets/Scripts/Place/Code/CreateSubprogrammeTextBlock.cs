using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ToggleGroup))]
public class CreateSubprogrammeTextBlock : MonoBehaviour
{
    public GameObject subprogrammeObj;
    public SaveArea saveArea;
    ToggleGroup toggleGroup;
    public bool Created { private get; set; }
    void Awake()
    {
        toggleGroup = GetComponent<ToggleGroup>();
    }
    void OnEnable()
    {
        if (!Created)
        {
            if(transform.childCount > 0)
                for (int i = 0; i < transform.childCount; i++)
                    Destroy(transform.GetChild(i).gameObject);

            Created = true;
            string[] scriptsNames = saveArea.all.ScriptsNames.ToArray();
            for (int i = 0; i < scriptsNames.Length; i++)
            {
                string text = scriptsNames[i].Substring(6);

                Transform Obj = Instantiate(subprogrammeObj, transform).transform;
                Obj.GetChild(1).GetComponent<Text>().text = text;
                Obj.name = text;

                Toggle mainToggle = Obj.GetComponent<Toggle>();
                mainToggle.group = toggleGroup;

                Obj.gameObject.AddComponent<SubprogrammeBlock>();
                SubprogrammeBlock Sub = Obj.gameObject.GetComponent<SubprogrammeBlock>();
                mainToggle.onValueChanged.AddListener(Sub.OnSubprogramme);
                Sub.text = text;
            }
        }
    }
}
