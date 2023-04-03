using UnityEngine;
using UnityEngine.UI;

public class Parameter : MonoBehaviour
{
    public ControlerManager managerInConstructor { private get; set; }
    public PlayerInterface player { private get; set; }
    public InputField inputValue;
    public Text title, readValue;
    public Toggle toggle;
    public int id;
    private void Awake()
    {
        if (inputValue && inputValue.text == "")
            inputValue.text = "0";
    }
    public void OnChangeControllerToggleProperty(bool on) 
    { 
        managerInConstructor.OnChangedInfo(on, id);
        managerInConstructor.AddInfoItems();
    }
    public void SaveOnEndWrite()
    {
        if (inputValue.text == "")
            inputValue.text = "0";
        managerInConstructor.AddInfoItems();
    }

    public void SaveOnEndDropdown() => managerInConstructor.AddComputerScript();
    public void OnControlInputProperty(string strValue)
    {
        if (strValue == "разные значения" || !player)
            return;
        float value;
        try
        {
            value = float.Parse(strValue.Replace('.',','));
            value = Mathf.Clamp(value, -999999999, 999999999);
        }
        catch
        {
            value = 0;
        }
        player.OnChangeProperty(id, value);
    }
}
