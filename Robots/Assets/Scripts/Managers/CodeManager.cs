using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CodeManager : MonoBehaviour {
    CodeConstructor codeConstructor;
    public ToggleGroup contentGroup { get; private set; }
    public SaveArea saveArea;
    public List<string> ScriptNameList;
    public GameObject addFirstLine, addLine, TypeBlock, AddSubprogramme, AddComponents, CanvasMenu;
    public Transform Content;
    void Start() {
        codeConstructor = GetComponent<CodeConstructor>();
        contentGroup = Content.GetComponent<ToggleGroup>();
    }
    void OnEnable()
    {
        ScriptNameList.Clear();
        ScriptNameList.AddRange(saveArea.all.ScriptsNames);
        CanvasMenu.SetActive(false);
        addLine.SetActive(false);
        addFirstLine.SetActive(true);
    }

    public void FindTypeBlock()
    {
        addLine.SetActive(false);
        addFirstLine.SetActive(false);
        TypeBlock.SetActive(false);
        AddComponents.SetActive(true);
    }
    public void FindSubprogramme()
    {
        addLine.SetActive(false);
        addFirstLine.SetActive(false);
        TypeBlock.SetActive(false);
        AddSubprogramme.SetActive(true);
    }
    public void Home(bool first = false)
    {
        AddSubprogramme.SetActive(false);
        AddComponents.SetActive(false);
        TypeBlock.SetActive(false);
        if (!first)
        {
            for (int i = 0; i < Content.childCount - 1; i++)
                Content.GetChild(i).GetComponent<Toggle>().interactable = true;
            addLine.SetActive(true);
        }
        else
            addFirstLine.SetActive(true);
    }
    public void ChangedAdd()
    {
        addLine.SetActive(true);
        addFirstLine.SetActive(false);
    }
    public void BaseComponent()
    {
        addFirstLine.SetActive(true);
        addLine.SetActive(false);
    }
    public void AddLine(bool downLine)
    {
        if (TypeBlock.activeInHierarchy)
            return;
       addLine.SetActive(false);
       addFirstLine.SetActive(false);
       TypeBlock.SetActive(true);
       if (Content.childCount > 1)
       {
           codeConstructor.IdPosition = codeConstructor.LookId;
           if (downLine)
               codeConstructor.IdPosition++;
           contentGroup.SetAllTogglesOff();
       }
       else
           codeConstructor.IdPosition = 0;
    }
    void OnChangeLook()
    {
        if (Content.childCount == 1)
            return;
        Toggle theActiveToggle = contentGroup.ActiveToggles().FirstOrDefault();
        if (!theActiveToggle)
            return;
        codeConstructor.LookId = theActiveToggle.transform.GetSiblingIndex();
    }
    void Update()
    {
            OnChangeLook();
    }
}
