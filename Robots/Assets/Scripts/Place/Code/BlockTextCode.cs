using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BlockTextCode : MonoBehaviour {

    public string TextCodeStart, TextInterfaceStart, TextCodeEnd, TextInterfaceEnd, MyText;
    public bool Operator;
    //Operators: 0 = Arifmetic, 1 = Logic1, 2 = Logic2
    //Variables: 0 = OutPut, 1 = InPut
    Toggle MyToggle;
	void Awake () {
        MyToggle = GetComponent<Toggle>();
        MyToggle.onValueChanged.AddListener(MyToggleIsOn);
    }
    void OnEnable()
    {
        if (MyToggle.isOn)
            MyToggleIsOn(true);
    }
    IEnumerator WaitIsOn()
    {
        for (int i = 0; i < 2; i++)
            yield return new WaitForEndOfFrame();

        if (Operator)
        {
            Info.MyText = MyText;
            if (Info.MyText == "(")
                Info.BeLeftBrace = true;
        }
        else
        {
            Info.TextCodeStart = TextCodeStart;
            Info.TextInterfaceStart = TextInterfaceStart;
            Info.TextCodeEnd = TextCodeEnd;
            Info.TextInterfaceEnd = TextInterfaceEnd;
        }
    }
    void OnDisable()
    {
        if (MyText == ")" && Info.MyText == ")")
            Info.MyText = null;
    }
    public void MyToggleIsOn(bool be)
    {
        if (be)
            StartCoroutine(WaitIsOn());
        else
            if(MyText == "(")
                Info.BeLeftBrace = false;
    }
}
