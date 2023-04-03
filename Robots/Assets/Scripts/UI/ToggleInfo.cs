using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(Toggle))]
public class ToggleInfo : MonoBehaviour {

	public float Height, indent;
    public int CountConteiners, addSizeCinteiners;
    public int idDetail;
    public Toggle toggle{ get; private set; }
    void Awake()
    {
        toggle = GetComponent<Toggle>();
    }
    void OnEnable()
    {
        toggle.isOn = false;
    }
}
