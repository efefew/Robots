using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
/// <summary>
/// Описание объекта
/// </summary>
public class Description : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public DescriptionManager manager;
    /// <summary>
    /// Описание в виде илюстрации
    /// </summary>
    public GameObject visualisationDescription;
    /// <summary>
    /// Описание в виде текста
    /// </summary>
    public string[] textDescription = new string[2];
    /// <summary>
    /// Может ли быть зафиксировано(не пропадать после наведения)?
    /// </summary>
    public bool canFixed = true;
    /// <summary>
    /// Время, после которого описание фиксируется
    /// </summary>
    public float timeFixed = 3f;
    /// <summary>
    /// Время, после которого появляется описание
    /// </summary>
    public float timeShow = 0.5f;

    /// <summary>
    /// Курсор наведён
    /// </summary>
    private bool CursorEnter;

    /// <summary>
    /// Таймер появления и фиксации
    /// </summary>
    private Coroutine timer;
    /// <summary>
    /// Создание описания
    /// </summary>
    /// <param name="manager">показывающий описание</param>
    /// <param name="textDescription">описание в виде текста</param>
    /// <param name="visualisationDescription">описание в виде илюстрации</param>
    public void SetDescription(DescriptionManager manager, string[] textDescription, GameObject visualisationDescription)
    {
        if(manager == null)
        {
            Debug.LogError("нет блока, где могло бы быть объяснение");
            return;
        }    
        if (textDescription.Length != Setting.countLanguage)
        {
            Debug.LogError("текст не на всех языках");
            return;
        }
        this.manager = manager;
        this.textDescription = new string[Setting.countLanguage];
        for (int i = 0; i < textDescription.Length; i++)
            this.textDescription[i] = textDescription[i];
        this.visualisationDescription = visualisationDescription;
    }
    /// <summary>
    /// Создание описания
    /// </summary>
    /// <param name="textDescription">описание в виде текста</param>
    /// <param name="visualisationDescription">описание в виде илюстрации</param>
    public void SetDescription(string[] textDescription, GameObject visualisationDescription)
    {
        if(textDescription.Length != Setting.countLanguage)
        {
            Debug.LogError("текст не на всех языках");
            return;
        }
        this.textDescription = new string[Setting.countLanguage];
        for (int i = 0; i < Setting.countLanguage; i++)
            this.textDescription[i] = textDescription[i];
        this.visualisationDescription = visualisationDescription;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!manager.fixedDescription)
        {
            CursorEnter = true;
            if (timer == null)
                timer = StartCoroutine(TimersDescription());
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        CursorEnter = false;
        if (timer != null)
        {
            StopCoroutine(timer);
            timer = null;
        }
        if (!manager.fixedDescription)
            manager.TargetDeactivator();
    }

    /// <summary>
    /// Таймер появления и фиксации
    /// </summary>
    /// <returns></returns>
    private IEnumerator TimersDescription()
    {
        yield return new WaitForSeconds(timeShow);
        if (CursorEnter)
            manager.TargetActivator(textDescription[(int)Language.language], visualisationDescription);
        if (canFixed)
            yield return new WaitForSeconds(timeFixed);
        if (CursorEnter)
            manager.fixedDescription = true;
    }
}
