using UnityEngine;

public class SubprogrammeBlock : MonoBehaviour
{
    public string text;
    public void OnSubprogramme(bool be)
    {
        if(be)
            Info.MyText = text;
    }
}
