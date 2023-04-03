using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(Toggle))]
public class OptionsBlockText : MonoBehaviour
{
    public GameObject delete, change, addUp, addDown, operations;
    public CodeManager codeManager { get; set; }
    public CodeConstructor codeConstructor { get; set; }
    public void Delete()
    {
        codeConstructor.Delete();
    }
    public void Change()
    {
        codeConstructor.Change();
    }
    public void Add(bool down)
    {
        codeManager.AddLine(down);
    }
    public void OnToggle(bool on)
    {
        if(on && codeConstructor)
            codeConstructor.OnOptionsTextBlock(this);
        else
            operations.SetActive(false);
    }
}
