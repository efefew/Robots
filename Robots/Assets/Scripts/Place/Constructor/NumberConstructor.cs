using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(BlockTextCode))]
public class NumberConstructor : MonoBehaviour {

    BlockTextCode BlockNumber;
    public Text BlockText;
    bool Changed { get; set; }
    public bool Operator;
    string oldText;
    void OnEnable()
    {
        oldText = BlockText.text;
    }
    void Start () {
        BlockNumber = GetComponent<BlockTextCode>();
	}
	void Update ()
    {
        if (oldText != BlockText.text)
        {
            oldText = BlockText.text;
            if (Operator)
            {
                if (BlockText.text == "")
                    BlockNumber.MyText = "0";
                else
                    BlockNumber.MyText = BlockText.text;
            }
            else
            {
                if (BlockText.text == "")
                {
                    BlockNumber.TextCodeStart = "#0";
                    BlockNumber.TextInterfaceStart = "<color=#AAFFAA>0";
                }
                else
                {
                    BlockNumber.TextCodeStart = "#" + BlockText.text;
                    BlockNumber.TextInterfaceStart = "<color=#AAFFAA>" + BlockText.text;
                }
            }
            BlockNumber.MyToggleIsOn(true);
        }
           
    }
}
