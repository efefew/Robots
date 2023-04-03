using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Ячейка
/// </summary>
public class DetailConteiner : MonoBehaviour
{
    public Rigidbody2D Conector;
    public bool uniqe;
    [HideInInspector]
    public float localAngle, globalAngle;
    Collider2D coll, collConector;
    void OnEnable()  
    {
        collConector = Conector.GetComponent<Collider2D>();
        globalAngle = transform.eulerAngles.z;
        localAngle = transform.localEulerAngles.z;
        coll = GetComponent<Collider2D>();
        InfoCon.checkConteiners += CheckConteiner;
    }
    private void Start()
    {
#if UNITY_EDITOR
        transform.name = transform.name + ' ' + Conector.GetComponent<DetailObject>().NameDetail + ' ' + Conector.GetComponent<DetailObject>().IndexDetail;
#endif
    }
    void CheckConteiner()
    {
        if (coll == null || collConector == null)
            return;
        coll.enabled = !DetailInConteiner();
    }
    /// <summary>
    /// Узнаёт присутствует ли деталь в ячейке
    /// </summary>
    /// <returns>Присутствует ли деталь в ячейке?</returns>
    public bool DetailInConteiner()
    {
        coll.enabled = true;    
        List<Collider2D> colls = new List<Collider2D>();
        int count = Physics2D.OverlapCollider(coll, new ContactFilter2D().NoFilter(), colls);
        if (count == 0)
            return false;
        for (int i = 0;i < count; i++)
            if (colls[i].gameObject.layer == 8 &&//Detail
                collConector != colls[i])//это не деталь родитель
                return true;
        return false;
    }
}
