using UnityEngine.SceneManagement;
using UnityEngine;

public class FrameRateMaintainer : MonoBehaviour
{
    SceneTransitionManager sceneLoader;
    // Start is called before the first frame update
    void Start()
    {
        sceneLoader = FindObjectOfType<SceneTransitionManager>();
        Application.targetFrameRate = 60;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (SceneManager.GetActiveScene().name != "Menu")
            {
                sceneLoader.FadeInAndLoadScene("Menu", 0.2f);
            }
            else
            {
                Application.Quit();
            }
        }
    }


}
