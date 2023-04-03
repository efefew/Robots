using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(Dropdown))]
public class ControlerInfo : MonoBehaviour
{
    public ControlerManager controler;
    private Dropdown dropdown;
    public Button delete, change;
    private void Awake()
    {
        dropdown = GetComponent<Dropdown>();
    }
    public void OnKeyChange(int id)
    {
        controler.detailTarget = null;
        controler.InfoDetailConteiner.SetActive(false);
        string keyName = dropdown.options[id].text.Split('\n')[1];
        string keyTypeStr = dropdown.options[id].text.Split('\n')[0];
        int keyType = controler.StringKeyTypeToInt(keyTypeStr);
        controler.key = keyName;
        controler.typeKey = keyType;
        if(keyName == "OnStartGame")
        {
            delete.interactable = false;
            change.interactable = false;
        }
        else
        {
            delete.interactable = true;
            change.interactable = true;
        }
    }
}
