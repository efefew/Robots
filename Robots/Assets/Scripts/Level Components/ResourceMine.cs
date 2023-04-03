using UnityEngine;

public class ResourceMine : LevelComponentManager
{
    public float ResourceMax, Max, Min, ResourceCurrent;
    public int IdResource;
    void Start()
    {
        //Min = values[0];
        //Max = values[1];
        IdResource = 0;//(int)values[2];
        ResourceMax = Random.Range(Min, Max);
        ResourceCurrent = ResourceMax;
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.GetComponent<DetailMiner>())
            col.GetComponent<DetailMiner>().resourceMine = this;
    }
    void OnTriggerExit2D(Collider2D col)
    {
        if (col.GetComponent<DetailMiner>() && col.GetComponent<DetailMiner>().resourceMine == this)
            col.GetComponent<DetailMiner>().resourceMine = null;
    }
}
