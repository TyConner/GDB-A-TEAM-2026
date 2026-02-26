using UnityEngine;
using UnityEngine.SceneManagement;
public class TitleScreen : MonoBehaviour
{
    public void PlayGameTDM()
    {
        SceneManager.LoadSceneAsync(1);
    }

    public void PlayGameFFA()
    {
        SceneManager.LoadSceneAsync(2);
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
