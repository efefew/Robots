using UnityEngine;

public class ButtomTriger : MonoBehaviour {
    [HideInInspector]
    public int CountObjects = 0;
    Transform tr;
    public Transform TrNotChecked;
    //public Transform TR;
    public float SpeedPositive, SpeedNegative, MinPos;
   [HideInInspector]
    public float FloatValue, MaxPos;
    void Start()
    {
        tr = transform;
        MaxPos = tr.localPosition.y;
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.transform != TrNotChecked &&
            col.gameObject.layer != 2)//ignore raycast
        { 
            CountObjects++;
            //TR = col.transform;
        }
    }
    void OnTriggerExit2D(Collider2D col)
    {
        if(col.transform!= TrNotChecked && 
            col.gameObject.layer != 2)//ignore raycast
            CountObjects--;

    }

    void FixedUpdate()
    {
        if (!InfoCon.BeConstructor)
        {
            if (CountObjects > 0 && MinPos < tr.localPosition.y)
                tr.Translate(new Vector3(-SpeedNegative, 0, 0));
            else
            {
                if (MaxPos > tr.localPosition.y)
                    tr.Translate(new Vector3(SpeedPositive, 0, 0));
            }
            float f = 1 - (tr.localPosition.y - MinPos) / (MaxPos - MinPos);
            if (f > 1)
                f = 1;
            if (f < 0)
                f = 0;
            FloatValue = f;
        }
    }
}
