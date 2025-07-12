using UnityEngine;
using UnityEngine.SceneManagement;

public class TapToNextScene : MonoBehaviour
{
    void Update()
    {
        // For mouse click (PC) and mobile tap
        if (Input.GetMouseButtonDown(0))
        {
            LoadNextScene();
        }
    }

    void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        SceneManager.LoadScene(nextSceneIndex);
    }
}

