using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public PlanetInfo[] AllPlanets;
    public GameObject MainInterface, SelectLevelInterface;
    SoundManager soundManager;
    private int CurrentIndex;
    public Image planetImage;
    SceneTransitionManager sceneTransitionManager;
    public TMP_Text planetName;
    public float fadeInSpeed;
    private void Start()
    {
        soundManager = SoundManager.instance;
        sceneTransitionManager = FindObjectOfType<SceneTransitionManager>();
    }

    public void PlayButtonPress()
    {
        soundManager.PlayAudio(AudioType.ButtonPress);
        MainInterface.SetActive(false);
        SelectLevelInterface.SetActive(true);
        CurrentIndex = 0;
        ShowLevel();
    }


    void ShowLevel()
    {
        PlanetInfo currentPlanet = AllPlanets[CurrentIndex];

        planetImage.sprite = currentPlanet.planetIcon;
        planetName.text = currentPlanet.planetName;
    }

    public void NextLevel()
    {
        soundManager.PlayAudio(AudioType.ButtonPress);
        CurrentIndex++;
        if (CurrentIndex > AllPlanets.Length-1)
        {
            CurrentIndex = 0;
        }
        ShowLevel();
    }

    public void PreviousLevel()
    {
        soundManager.PlayAudio(AudioType.ButtonPress);
        CurrentIndex--;
        if (CurrentIndex < 0)
        {
            CurrentIndex = AllPlanets.Length - 1;
        }
        ShowLevel();
    }


    public void PlayGame()
    {
        soundManager.PlayAudio(AudioType.ButtonPress);
        PlanetInfo currentPlanet = AllPlanets[CurrentIndex];
        PlayerPrefs.SetString("CurrentPlanet", currentPlanet.planetName);
        sceneTransitionManager.FadeInAndLoadScene("Cutscene", fadeInSpeed);
        //SceneManager.LoadScene("Game");
    }
}
