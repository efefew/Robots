using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Bullet : MonoBehaviour
{
    public float Damage, Speed, DeadTime, SafeTime, TimeEndAnimateDead;
    public int CountRicochet;
    public bool Punching, Incendiary;
    Transform tr;
    public CameraOperator camSnaker {private get; set; }
    public GameObject AnimatorFire;
    bool BeDanger = false;
    public int MyId;
    void Start()
    {
        tr = transform;
        camSnaker.ShakeCamera(0.4f, 3, 300, tr.position, 1);
        tr.localPosition = new Vector3(tr.localPosition.x, tr.localPosition.y, 0);
        StartCoroutine(TimeDead());
    }
    void FixedUpdate()
    {
        tr.Translate(tr.up * Speed, Space.World);
    }
    void Update()
    {
        if (BeDanger)
        {
           
            Ray2D ray = new Ray2D(tr.position, tr.up);
            RaycastHit2D h = Physics2D.Raycast(tr.position, tr.up, Speed);
            if (h)
            {
                Transform hTr = h.transform;
                
                if (hTr.tag == "Wall")
                {

                    if (CountRicochet > 0)
                    {
                        Vector2 ricohset = Vector2.Reflect(ray.direction, h.normal);
                        float r = -Mathf.Atan2(ricohset.x, ricohset.y) * Mathf.Rad2Deg;
                        tr.eulerAngles = new Vector3(0, 0, r);
                        CountRicochet--;
                    }
                    else
                    {
                        camSnaker.ShakeCamera(1f, 5, 800, tr.position);
                        TimeBoom();
                    }
                }
                if (hTr.GetComponent<DetailObject>())
                {
                    DetailObject dObj = hTr.GetComponent<DetailObject>();
                    dObj.ChangeHealth(Damage, DetailObject.DamageType.Shooting);
                    if (!Punching)
                    {
                        camSnaker.ShakeCamera(1f, 5, 800, tr.position);
                        BeDanger = false;
                        TimeBoom();
                    }
                }
            }
        }
    }

    void TimeBoom()
    {
        GetComponent<Image>().enabled = false;
        if (AnimatorFire != null)
        {
            AnimatorFire.SetActive(true);
            AnimatorFire.transform.SetParent(tr.parent);
            Destroy(AnimatorFire, TimeEndAnimateDead);
        }
        Destroy(gameObject);
    }
    IEnumerator TimeDead()
    {
        yield return new WaitForSeconds(SafeTime);
        BeDanger = true;
        yield return new WaitForSeconds(DeadTime);
        TimeBoom();

    }
}
