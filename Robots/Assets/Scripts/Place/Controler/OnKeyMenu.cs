using UnityEngine;

public class OnKeyMenu : MonoBehaviour
{
    public ControlerManager controler;

    void OnEnable()
    {
        controler.detailTarget = null;
        controler.InfoDetailConteiner.SetActive(false);
    }
}
