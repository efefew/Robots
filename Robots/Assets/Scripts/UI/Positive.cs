using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(InputField))]
public class Positive : MonoBehaviour
{
    InputField I;
    public bool replacing_a_DotWith_a_Comma;
    void Start()
    {
        I = GetComponent<InputField>();
    }
    public void OnChangeValue()
    {
        if (I.text.Contains("-"))
            I.text = I.text.Replace("-", "");
        if(replacing_a_DotWith_a_Comma)
            I.text = I.text.Replace(".", ",");
    }
}
