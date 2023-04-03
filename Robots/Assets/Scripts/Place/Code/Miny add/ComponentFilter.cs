using System.Collections.Generic;
using UnityEngine;

public class ComponentFilter : MonoBehaviour {
    public List<GameObject> Components;
    public bool BeOutput, BeLogicCompare, BeOrAnd, BeMathematic;
    bool BeDeactive, oldTypeVariable;
    void OnEnable () {
        for (int i = 0; i < Components.Count; i++)
            Components[i].SetActive(true);
        oldTypeVariable = Info.ReadVariable;
    }
    void Update()
    {
        if (Info.ReadVariable != oldTypeVariable)
        {
            oldTypeVariable = Info.ReadVariable;
            for (int i = 0; i < Components.Count; i++)
                Components[i].SetActive(true);
        }
        BeDeactive = false;
        if (Info.IfOrFor)
        {
            if (BeLogicCompare && Info.LogicCompare)
                BeDeactive = true;
            if (BeOrAnd && !Info.LogicCompare)
                BeDeactive = true;
        }
        else
        {
            if (BeOutput && !Info.ReadVariable)
                BeDeactive = true;
            if (BeMathematic)
                BeDeactive = true;
        }
        if (BeDeactive)
            for (int i = 0; i < Components.Count; i++)
                Components[i].SetActive(false);
    }
}
