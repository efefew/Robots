using System.Collections;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(Dropdown))]
public class ValidateDropdown : MonoBehaviour
{
    private Dropdown dropdown;
    public int frames, minCountOptions;

    private void Awake() => dropdown = GetComponent<Dropdown>();

    private void OnEnable()
    {
        StartCoroutine(NextFrame(frames));
    }

    private IEnumerator NextFrame(int countFrame)
    {
        for (int i = 0; i < countFrame; i++)
            yield return new WaitForFixedUpdate();
        dropdown.interactable = dropdown.options.Count >= minCountOptions;
    }
}
