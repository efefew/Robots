using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Load : MonoBehaviour
{ 
    public string Name;
    void Start()
    {
        StartCoroutine(loading());
    }
    public IEnumerator loading()
    {
        AsyncOperation operation;
        if (Name == "")
            operation = SceneManager.LoadSceneAsync(Setting.sceneLoad);
        else
            operation = SceneManager.LoadSceneAsync(Name);
        while (!operation.isDone)
        {
            // operation.progress;
            yield return null;
        }
    }
}
