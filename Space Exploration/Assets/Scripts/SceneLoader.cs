using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] string sceneName;
    [SerializeField] bool changeOnStart;
    private void Start()
    {
        if (changeOnStart)
        {
            LoadScene();
        }
    }
    public void LoadScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}
