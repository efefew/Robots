using UnityEngine;

public class DescriptionRepeat : Description
{
    public Description descriptionTarget;
    private void Start()
    {
        RepeatDescription();
    }
    public void RepeatDescription()
    {
        if (descriptionTarget.manager == null)
        {
            Debug.LogError("нет блока, где могло бы быть объяснение");
            return;
        }
        if (descriptionTarget.textDescription.Length != Setting.countLanguage)
        {
            Debug.LogError("текст не на всех языках");
            return;
        }
        manager = descriptionTarget.manager;
        textDescription = new string[Setting.countLanguage];
        for (int i = 0; i < textDescription.Length; i++)
            textDescription[i] = descriptionTarget.textDescription[i];
        visualisationDescription = descriptionTarget.visualisationDescription;
    }
}
