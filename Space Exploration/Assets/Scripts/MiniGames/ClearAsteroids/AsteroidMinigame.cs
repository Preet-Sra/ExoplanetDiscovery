using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class AsteroidMinigame : MonoBehaviour, IPointerDownHandler
{
    public Transform[] spawnPoints;        // Array of spawn points outside the mask
    public RectTransform gamePanelMask;    // The masked panel
    public float minSpeed = 50f;           // Minimum speed of the asteroid
    public float maxSpeed = 150f;          // Maximum speed of the asteroid
    public float spawnRate = 0.5f;         // How often asteroids spawn (in seconds)

    public RectTransform crosshair;        // Reference to the crosshair UI element

    public float gameDuration = 60f;       // Duration of the game in seconds
    public int targetScore = 10;           // Target score to end the game
    public TMP_Text timerText;             // UI Text to display the timer
    public TMP_Text scoreText, TargetText; // UI Text to display the score

    private bool isGameOver = false;       // Game over state to stop asteroid spawning
    private List<GameObject> activeAsteroids = new List<GameObject>();
    private int currentScore = 0;          // Current score
    private ObjectPooler objectPooler;
    private SoundManager soundManager;
    private MiniGameHandler miniGameHandler;

    private void Start()
    {
        objectPooler = ObjectPooler.instance;
        miniGameHandler = GetComponent<MiniGameHandler>();
        soundManager = SoundManager.instance;

        // Start spawning asteroids and the game timer
       
    }

    IEnumerator SpawnAsteroidsContinuously()
    {
        while (!isGameOver)
        {
            SpawnAsteroid();
            yield return new WaitForSeconds(spawnRate);
        }
    }

    void SpawnAsteroid()
    {
        // Spawn the asteroid at a random spawn point
        Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject newAsteroid = objectPooler.GetObject("Asteroid");
        newAsteroid.transform.SetParent(gamePanelMask);
        newAsteroid.transform.position = randomSpawnPoint.position;
        newAsteroid.transform.rotation = Quaternion.identity;

        // Set a random rotation (clamped for right-to-left movement)
        float randomRotation = Random.Range(-45f, 45f); // Slight tilt, so it moves from right to left
        newAsteroid.transform.rotation = Quaternion.Euler(0f, 0f, randomRotation);

        // Set a random speed
        float randomSpeed = Random.Range(minSpeed, maxSpeed);
        StartCoroutine(MoveAsteroid(newAsteroid, randomSpeed));
        activeAsteroids.Add(newAsteroid);
    }

    IEnumerator MoveAsteroid(GameObject asteroid, float speed)
    {
        RectTransform asteroidRect = asteroid.GetComponent<RectTransform>();

        while (asteroid.activeSelf)
        {
            asteroidRect.anchoredPosition += Vector2.left * speed * Time.deltaTime;

            // Check if the asteroid has moved out of the game panel mask
            if (IsOutOfMask(asteroidRect))
            {
                DeactivateAsteroid(asteroid); // Deactivate and remove from the list
                break;
            }

            yield return null;
        }
    }

    bool IsOutOfMask(RectTransform asteroidRect)
    {
        // Check if the asteroid's right-most side is beyond the left border of the mask
        float leftBorder = -gamePanelMask.rect.width / 2;
        return asteroidRect.anchoredPosition.x + asteroidRect.rect.width / 2 < leftBorder;
    }

    void DeactivateAsteroid(GameObject asteroid)
    {
        asteroid.SetActive(false);
        activeAsteroids.Remove(asteroid); // Remove from the activeAsteroids list
    }

    public void EndGame()
    {
        isGameOver = true;

        // Deactivate and remove all active asteroids
        foreach (var asteroid in activeAsteroids.ToArray()) // Create a copy to avoid modification errors
        {
            if (asteroid != null)
                DeactivateAsteroid(asteroid);
        }

        activeAsteroids.Clear(); // Clear the list after deactivation
        Debug.Log("Game Over!");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Check if the click is within the game panel mask
        if (RectTransformUtility.RectangleContainsScreenPoint(gamePanelMask, eventData.position, eventData.pressEventCamera))
        {
            // Convert the screen point to a local position within the UI canvas
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(gamePanelMask, eventData.position, eventData.pressEventCamera, out localPoint);

            // Move the crosshair to the clicked position
            crosshair.anchoredPosition = localPoint;
            soundManager.PlayAudio(AudioType.Asteroid);
        }
    }

    public void HitAsteroid(PointerEventData eventData)
    {
        soundManager.PlayAudio(AudioType.AsteroidDestroyed);
        if (RectTransformUtility.RectangleContainsScreenPoint(gamePanelMask, eventData.position, eventData.pressEventCamera))
        {
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(gamePanelMask, eventData.position, eventData.pressEventCamera, out localPoint);

            // Move the crosshair to the clicked position
            crosshair.anchoredPosition = localPoint;

            GameObject explosion = objectPooler.GetObject("ExplosionUI");
            explosion.SetActive(true);
            explosion.transform.SetParent(gamePanelMask); // Avoid setting parent if it's already set
            explosion.transform.position = eventData.position;
            explosion.transform.rotation = Quaternion.identity;
        }
        currentScore++;
        UpdateScoreText();

        // Check if current score meets or exceeds the target score
        if (currentScore >= targetScore)
        {
            miniGameHandler.HideGame(true);
            EndGame();
        }
    }

    private IEnumerator Timer()
    {
        float remainingTime = gameDuration;

        while (remainingTime > 0 && !isGameOver)
        {
            remainingTime -= Time.deltaTime;
            timerText.text = "Time: " + Mathf.Ceil(remainingTime); // Update the timer text

            yield return null; // Wait until the next frame
        }

        if (!isGameOver)
        {
            miniGameHandler.HideGame(false);
            EndGame(); // End the game if the timer runs out
        }
    }

    private void UpdateScoreText()
    {
        scoreText.text = "Destroyed: " + currentScore; // Update the score text
    }

    public void ResetGame()
    {
        // Reset score and game state
        currentScore = 0;
        isGameOver = false;
        activeAsteroids.Clear();

        // Reset UI elements
        timerText.text = "Time: " + gameDuration; // Reset timer text
        UpdateScoreText(); // Reset score text

        // Restart spawning and timer
        StartCoroutine(SpawnAsteroidsContinuously());
        StartCoroutine(Timer());
    }
}
