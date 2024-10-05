using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Asteroid : MonoBehaviour, IPointerDownHandler
{
    AsteroidMinigame asteroidMinigame;

    void OnEnable()
    {
            asteroidMinigame = FindObjectOfType<AsteroidMinigame>();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
       
        asteroidMinigame.HitAsteroid (eventData);
        gameObject.SetActive(false);
    }
}
