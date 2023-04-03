using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Регулятор размера повторяющегося изображения
/// </summary>
[RequireComponent(typeof(Image))]
public class ChangerSizeTiledImage : MonoBehaviour
{
    public Vector2 posA, posB;
    public float pixelsA, pixelsB;
    Image image;
    RectTransform tr;
    void Awake()
    {
        image = GetComponent<Image>();
        tr = GetComponent<RectTransform>();
    }
    public void SetSizeTiledImage(bool itsA)
    {
        if (itsA)
        {
            image.pixelsPerUnitMultiplier = pixelsA;
            tr.anchoredPosition = posA;
        }
        else
        {
            image.pixelsPerUnitMultiplier = pixelsB;
            tr.anchoredPosition = posB;
        }
        image.SetVerticesDirty();
    }
}
