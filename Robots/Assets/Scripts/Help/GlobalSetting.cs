using UnityEngine;
using System.Collections;

public class GlobalSetting : MonoBehaviour
{
    /// <summary>
    /// анимация в начале
    /// </summary>
    [SerializeField]
    private GameObject onStartScene;
    public void Awake()
    {
        StartCoroutine(DestroyOnStartScene());
    }
    /// <summary>
    /// удаляет анимацию в начале чтобы не повторялась заново при запуске меню уровня
    /// </summary>
    /// <returns></returns>
    private IEnumerator DestroyOnStartScene()
    {
        yield return new WaitForSeconds(0.51f);
        Destroy(onStartScene);
    }
    public void SetThePhysicsCalculationDelay(float time)
    {
        time = Mathf.Clamp(time, 0.003f, 0.3f);
        Time.fixedDeltaTime = time;
    }
    public void SetTime(float time)
    {
        time = Mathf.Clamp(time, 0, 3);
        Time.timeScale = time;
    }
 
    #region FPS
#if UNITY_EDITOR
    //float averageFPS, maxFPS, minFPS;
    //int index;
    //float[] fpsArrea;
    //void Start()
    //{
    //    index = 0;
    //    fpsArrea = new float[100];
    //}
    //void Update()
    //{
    //    fpsArrea[index] = 1 / Time.unscaledDeltaTime;
    //    index++;
    //    if(index >= fpsArrea.Length)
    //    {
    //        index = 0;
    //        averageFPS = 0;
    //        float max = fpsArrea[0], min = fpsArrea[0];
    //        for (int i = 0; i < fpsArrea.Length; i++)
    //        {
    //            averageFPS += fpsArrea[i];
    //            if (max < fpsArrea[i])
    //                max = fpsArrea[i];
    //            if (min > fpsArrea[i])
    //                min = fpsArrea[i];
    //        }
    //        maxFPS = max;
    //        minFPS = min;
    //        averageFPS /= fpsArrea.Length;
    //    }
    //}
    //void OnGUI()
    //{
    //    GUI.Label(new Rect(5, 5, 1000, 1000),  "max fps = " + ((int)maxFPS));
    //    GUI.Label(new Rect(5, 15, 1000, 1000), "average fps = " + ((int)averageFPS));
    //    GUI.Label(new Rect(5, 25, 1000, 1000), "min fps = " + ((int)minFPS));
    //}
#endif
    #endregion
}
