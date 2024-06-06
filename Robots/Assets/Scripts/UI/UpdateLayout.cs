using System.Collections;

using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class UpdateLayout : MonoBehaviour
{
    #region Fields

    private ContentSizeFitter sizeFitter;
    private GameObject gameObj;
    private RectTransform[] rectTransforms;
    private VerticalLayoutGroup vertical;
    private HorizontalLayoutGroup horizontal;
    private ScrollRect scroll;
    private float verticalScroll, horizontalScroll;

    #endregion Fields

    #region Methods

    private void Awake()
    {
        if (transform.parent.parent.parent.GetComponent<ScrollRect>())
            scroll = transform.parent.parent.parent.GetComponent<ScrollRect>();
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

    private void OnEnable()
    {
        Canvas.ForceUpdateCanvases();
        StartCoroutine(RestartSizeFitter());
    }

    public IEnumerator RestartSizeFitter()
    {
        if (scroll)
        {
            if (scroll.horizontalScrollbar) horizontalScroll = scroll.horizontalScrollbar.value;
            if (scroll.verticalScrollbar) verticalScroll = scroll.verticalScrollbar.value;
        }
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
        if (scroll)
        {
            if (scroll.horizontalScrollbar) scroll.horizontalScrollbar.value = horizontalScroll;
            if (scroll.verticalScrollbar) scroll.verticalScrollbar.value = verticalScroll;
        }
    }

    #endregion Methods
}