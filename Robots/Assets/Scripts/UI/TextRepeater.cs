using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(Text))]
public class TextRepeater : MonoBehaviour
{
    public Text original;
    Text repeater;

    void Awake() => repeater = GetComponent<Text>();
    void OnEnable() => RepeatText();

    public void RepeatText() => repeater.text = original.text;
    public void InverseSetActive(bool on) => gameObject.SetActive(!on);
}
