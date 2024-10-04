using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup fadeCanvasGroup;  // CanvasGroup for the fade panel
    [SerializeField] private float fadeInDuration = 1f;    // Time for fade in
    [SerializeField] private float fadeOutDuration = 1f;   // Time for fade out

    private void Start()
    {
        // Start with the screen fully dark (alpha = 1)
        fadeCanvasGroup.alpha = 1f;

        // Fade out as soon as the scene starts
        StartCoroutine(FadeOut(fadeOutDuration));
    }

    /// <summary>
    /// Fades in the screen and then loads the next scene.
    /// </summary>
    /// <param name="sceneName">Name of the scene to load</param>
    /// <param name="fadeInTime">Duration of the fade-in effect</param>
    public void FadeInAndLoadScene(string sceneName, float fadeInTime)
    {
        // Start the fade in coroutine and load the scene afterward
        StartCoroutine(FadeIn(sceneName, fadeInTime));
    }

    /// <summary>
    /// Coroutine for fading out the screen (from alpha 1 to 0).
    /// </summary>
    /// <param name="duration">How long the fade out should take</param>
    private IEnumerator FadeOut(float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // Gradually reduce the alpha over time
            fadeCanvasGroup.alpha = 1f - (elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the alpha is fully set to 0 (completely transparent)
        fadeCanvasGroup.alpha = 0f;
    }

    /// <summary>
    /// Coroutine for fading in the screen (from alpha 0 to 1) and loading the next scene.
    /// </summary>
    /// <param name="sceneName">Name of the scene to load</param>
    /// <param name="duration">How long the fade-in should take</param>
    private IEnumerator FadeIn(string sceneName, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // Gradually increase the alpha over time
            fadeCanvasGroup.alpha = elapsedTime / duration;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the alpha is fully set to 1 (completely opaque)
        fadeCanvasGroup.alpha = 1f;

        // Load the next scene after the fade-in is complete
        SceneManager.LoadScene(sceneName);
    }
}
