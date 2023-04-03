using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[DisallowMultipleComponent]
public class UpdateLayout : MonoBehaviour
{
    ContentSizeFitter sizeFitter;
    GameObject gameObj;
    RectTransform[] rectTransforms;
    VerticalLayoutGroup vertical;
    HorizontalLayoutGroup horizontal;
    void Awake()
    {
        if (GetComponent<ContentSizeFitter>())
            sizeFitter = GetComponent<ContentSizeFitter>();
        if (transform.parent.GetComponent<VerticalLayoutGroup>())
            vertical = transform.parent.GetComponent<VerticalLayoutGroup>();
        if (transform.parent.GetComponent<HorizontalLayoutGroup>())
            horizontal = transform.parent.GetComponent<HorizontalLayoutGroup>();
        gameObj = gameObject;
        LayoutGroup[] Layouts = gameObj.GetComponentsInChildren<LayoutGroup>();
        rectTransforms = new RectTransform[Layouts.Length];
        for (int i = 0; i < Layouts.Length; i++)
            rectTransforms[i] = Layouts[i].GetComponent<RectTransform>();
    }
    void OnEnable()
    {
       

        Canvas.ForceUpdateCanvases();
        
        StartCoroutine(RestartSizeFitter());
    }
    public IEnumerator RestartSizeFitter()
    {
        sizeFitter.enabled = false;

        if (vertical)
        {
            vertical.enabled = false;
            yield return new WaitForEndOfFrame();
            vertical.enabled = true;
        }
        if (horizontal)
        {
            horizontal.enabled = false;
            yield return new WaitForEndOfFrame();
            horizontal.enabled = true;
        }

        yield return new WaitForEndOfFrame();
        sizeFitter.enabled = true;
        for (int i = 0; i < rectTransforms.Length; i++)
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransforms[i]);
    }
}
