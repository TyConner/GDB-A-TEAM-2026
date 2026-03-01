using UnityEngine;
using UnityEngine.SceneManagement;
public class TitleScreen : MonoBehaviour
{
    public void Match_Config()
    {
        SceneManager.LoadSceneAsync(1);
    }

    public void QuickPlayFFA()
    {
        SceneManager.LoadSceneAsync(3);
    }

    public void quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
