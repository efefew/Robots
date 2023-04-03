using UnityEngine.UI;
using UnityEngine;
/// <summary>
/// Представление описания
/// </summary>
public class DescriptionManager : MonoBehaviour
{
    public GameObject information;
    public Transform content;
    public Text textInformation;
    public GameObject animationInformation;
    public bool fixedDescription { get; set; }
    public void TargetActivator(string text, GameObject animation = null)
    {
        information.SetActive(true);
        if (animation != null)
            animationInformation = Instantiate(animation, content);
        textInformation.text = text;
    }
    public void TargetDeactivator()
    {
        information.SetActive(false);
        fixedDescription = false;
        if(animationInformation != null)
            Destroy(animationInformation);
    }
}
