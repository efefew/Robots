using UnityEngine;
public class ChangeValue : MonoBehaviour
{
    public GameObject[] State;
    public int value;


    public void ValueChanged(int targetValue)
    {
        if (targetValue < 0 || targetValue >= State.Length)
            return;
        State[value].SetActive(false);
        State[targetValue].SetActive(true);
        value = targetValue;
        return;
    }
    public void ValueChanged()
    {
        State[value].SetActive(false);
        value++;
        if (value >= State.Length)
            value = 0;
        State[value].SetActive(true);
    }
}
