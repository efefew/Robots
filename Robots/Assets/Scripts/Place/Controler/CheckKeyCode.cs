using UnityEngine;

public class CheckKeyCode : MonoBehaviour
{
    public string MyKey;
    public bool ChangeKey;
    void Update()
    {
        
        if (Input.GetKeyUp(KeyCode.Mouse1))
            ChangeKey = true;
    }
    void OnGUI()
    {
        if (MyKey!="" && Event.current.Equals(Event.KeyboardEvent(MyKey)))
            Debug.Log(MyKey);
        if (ChangeKey && (Input.anyKeyDown))
        {
            Event e = Event.current;
            if (e.isKey)
            {
                ChangeKey = false;
                MyKey = e.keyCode.ToString();
            }
        }
        
    }
}
