using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; }

    protected virtual void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this as T;
    }

    //protected virtual void OnApplicationQuit()
    //{
    //    Instance = null;
    //    Destroy(gameObject);
    //}

    protected virtual void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

}
