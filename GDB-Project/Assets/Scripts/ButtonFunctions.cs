using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonFunctions : MonoBehaviour
{
    public void Resume()
    {
        GameManager.instance.stateUnPause();
    }
    public void restart()
    {
        RunState.WasRestarted = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
