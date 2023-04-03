using UnityEngine;

public class Mover : MonoBehaviour {

    public Transform Tr = null;
    public GameObject Obj = null, FatherObj = null, End = null;
    public bool MoveCodeButton { get; set; }
    public int IfLineCount = -1;
}
