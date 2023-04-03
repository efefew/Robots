using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AreaScript : MonoBehaviour
{
    public float Height { private get; set; }
    public float indent { get; set; }
    public int CountConteinersNeed { get; set; }
    /// <summary>
    /// Видна ли на курсоре ячейка
    /// </summary>
    public bool conteinerOnCursor { get; set; }
    public int countDetails;
    public List<DetailConteiner> conteiners;
    public Image[] images;
    public BoxCollider2D box;
    private readonly Color32
        white = new Color32(255, 255, 255, 255),
        red = new Color32(230, 100, 100, 255),
        yellow = new Color32(238, 255, 49, 255),
        green = new Color32(44, 215, 168, 255);
    /// <summary>
    /// Изменяет цвет зоны строительства в зависимости от ситуации
    /// </summary>
    public void ChangeImages()
    {
        if (images == null || images.Length <= 0)
            return;
        if (countDetails > 0)
        {
            Paint(red);
            return;
        }
        if(conteiners.Count > 0)
        {
            for (int id = 0; id < conteiners.Count; id++)
                if ((conteiners[0].uniqe || conteiners[id].uniqe) && conteiners[0].Conector != conteiners[id].Conector)//для уникальных (например ротатор)
                {
                    Paint(yellow);
                    return;
                }
            if (conteiners.Count >= CountConteinersNeed)
            {
                Paint(green);
                return;
            }
            else
            {
                Paint(yellow);
                return;
            }
        }
        
        Paint(white);
    }
    /// <summary>
    /// Закрашивает все изображения зоны строительства в выбраный цвет
    /// </summary>
    /// <param name="color">выбраный цвет</param>
    private void Paint(Color32 color)
    {
        for (int id = 0; id < images.Length; id++)
            images[id].color = color;
    }
    /// <summary>
    /// Устанавливает размер зоны строительства
    /// </summary>
    /// <param name="child">объект, у которого будет такой же размер (временный наследник)</param>
    /// <param name="addSizeCinteiners">виртуальное добавление ширины, не затрагивая количество требуемых ячеек (как сделано в колесе)</param>
    public void ChangeSizeCollider(Transform child = null, int addSizeCinteiners = 0)
    {
        transform.localScale = new Vector3(CountConteinersNeed + addSizeCinteiners, Height, 1f);
        if (child)
            child.localScale = new Vector3(1f / (CountConteinersNeed + addSizeCinteiners), 1f / Height, 1f);
        else
            transform.GetChild(0).localScale = new Vector3(1f / (CountConteinersNeed + addSizeCinteiners), 1f / Height, 1f);
    }
    /// <summary>
    /// Изменяет угол наследника зоны строительства
    /// </summary>
    /// <param name="zAngle">угол по оси z</param>
    public void ChangeRotation(float zAngle)
    {
        transform.eulerAngles = new Vector3(0, 0, zAngle);
        if(transform.childCount == 0)
        {
            Debug.LogError("у зоны нет детали для визуализации");
            return;
        }
        transform.GetChild(0).eulerAngles = transform.eulerAngles;
    }
    /// <summary>
    /// Обновление проверки внутри площади
    /// </summary>
    public bool UpdateArea()
    {
        countDetails = 0;
        conteiners.Clear();
        List<Collider2D> colls = new List<Collider2D>();
        if (Physics2D.OverlapCollider(box, new ContactFilter2D().NoFilter(), colls) > 0)
        {
            foreach (var coll in colls)
            {
                if (coll.gameObject.layer == 8)//Detail
                    countDetails++;
                if (coll.gameObject.layer == 9 && coll.GetComponent<DetailConteiner>())//Conteiner
                    conteiners.Add(coll.GetComponent<DetailConteiner>());
            }
            ChangeImages();
            return true;
        }
        ChangeImages();
        return false;
    }
}
