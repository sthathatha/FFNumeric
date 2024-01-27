using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ManagerScript : MonoBehaviour
{
    private static ManagerScript _instance = null;
    public static ManagerScript GetInstance() { return _instance; }

    private string activeSceneName;

    // Start is called before the first frame update
    void Start()
    {
        Screen.SetResolution(917, 516, FullScreenMode.Windowed);
        Global.GetSaveData().Load();

        _instance = this;
        SoundManager.GetInstance().PlayBGM(SoundManager.BgmID.NONE);

        activeSceneName = "TitleScene";
        SceneManager.LoadSceneAsync(activeSceneName, LoadSceneMode.Additive);
    }

    /// <summary>
    /// ÉVÅ[ÉìêÿÇËë÷Ç¶
    /// </summary>
    /// <param name="sceneName"></param>
    public void ChangeScene(string sceneName)
    {
        SceneManager.UnloadSceneAsync(activeSceneName, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
        SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        activeSceneName = sceneName;
    }
}
