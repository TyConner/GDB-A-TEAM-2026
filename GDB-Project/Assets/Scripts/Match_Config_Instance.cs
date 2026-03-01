using UnityEngine;

public class Match_Config_Instance : MonoBehaviour
{
    public static Match_Config_Instance instance;

    public GameMode_Config config;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

}
