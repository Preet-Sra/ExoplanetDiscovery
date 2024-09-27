using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    ResearchPointSpawner researchPointSpawner;
    int TotalReaserachPoints;
    int currentResearchPointsReached;
    [SerializeField] GameObject WonCanvas;
    public static GameManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        researchPointSpawner = FindObjectOfType<ResearchPointSpawner>();
        TotalReaserachPoints = researchPointSpawner.samplesToSpawn;
    }

    public void LoadLevel(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }
 
    public void ResearchCompleted()
    {
        currentResearchPointsReached++;
        if (currentResearchPointsReached >= TotalReaserachPoints)
        {
            StartCoroutine(GameOver());
        }
    }

    IEnumerator GameOver()
    {
        yield return new WaitForSeconds(1f);
        SoundManager.instance.PlayAudio(AudioType.Won);
        WonCanvas.SetActive(true);
    }

}
