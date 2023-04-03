using UnityEngine;
[System.Serializable]
public class SameTypeDetails
{
    public string name;
    public GameObject[] Details;
    public void SetActiveDetails(bool on)
    {
        for (int i = 0; i < Details.Length; i++)
            Details[i].SetActive(on);
    }
}
public class FilterTypeDetail : MonoBehaviour
{
    public SameTypeDetails[] TypesDetail;
    public int targetTypeDetail;
    public void UpdateFilter(int newTargetTypeDetail)
    {
        TypesDetail[targetTypeDetail].SetActiveDetails(false);
        targetTypeDetail = newTargetTypeDetail;
        TypesDetail[targetTypeDetail].SetActiveDetails(true);
    }
}
