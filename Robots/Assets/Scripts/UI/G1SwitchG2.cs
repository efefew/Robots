using UnityEngine;

public class G1SwitchG2 : MonoBehaviour
{
    public GameObject g1, g2;
    public void OnChangedToggleValue(bool on)
    {
        if (on)
        {
            g1.SetActive(false);
            g2.SetActive(true);
        }
        else
        {
            g1.SetActive(true);
            g2.SetActive(false);
        }
    }
}
