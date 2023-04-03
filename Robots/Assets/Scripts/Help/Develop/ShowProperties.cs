using UnityEngine;

public class ShowProperties : MonoBehaviour
{
    public bool changeProperty, getProperty;
    public int  idProperty;
    public float setProperty;
    void SetProperty()
    {
        if(idProperty >= GetComponent<DetailObject>().ValProperties.Length)
        {
            Debug.LogError("ты вышел за границы массива лол");
            return;
        }
        GetComponent<DetailObject>().ValProperties[idProperty] = setProperty;
        //GetComponent<DetailObject>().ValProperties[idProperty] = setProperty;
    }
    void GetProperty()
    {
        if (idProperty >= GetComponent<DetailObject>().ValProperties.Length)
        {
            Debug.LogError("ты вышел за границы массива лол");
            return;
        }
        setProperty = GetComponent<DetailObject>().ValProperties[idProperty];
    }
    void Update()
    {
        if (changeProperty)
        {
            changeProperty = false;
            SetProperty();
        }

        if (getProperty)
        {
            getProperty = false;
            GetProperty();
        }
    }
}
