using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(Toggle))]
public class PropertyScript : MonoBehaviour
{
    public Description description;
    public Text text;
    public Toggle toggle;

    public Toggle BuildCommands(string label, DescriptionManager manager, string[] textDescription, GameObject visualisationDescription)
    {
        text.text = label;
        description.SetDescription(manager, textDescription, visualisationDescription);
        name = label;
        return toggle;
    }
}
