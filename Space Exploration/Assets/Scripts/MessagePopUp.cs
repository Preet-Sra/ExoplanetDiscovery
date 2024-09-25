using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MessagePopUp : MonoBehaviour
{
    public TMP_Text messageText; // The TextMeshPro object that will display the message
    public CanvasGroup canvasGroup; // The CanvasGroup component to control the alpha

    private Coroutine messageCoroutine;


    public static MessagePopUp instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void ShowMessage(string message, float fadeInSpeed, float holdTime)
    {
        // Stop any existing fade-in/out processes if active
        if (messageCoroutine != null)
        {
            StopCoroutine(messageCoroutine);
        }

        // Start the message popup coroutine
        messageCoroutine = StartCoroutine(HandleMessageFade(message, fadeInSpeed, holdTime));
    }

    /// <summary>
    /// Handles the fade in, hold, and fade out of the message.
    /// </summary>
    private IEnumerator HandleMessageFade(string message, float fadeInSpeed, float holdTime)
    {
        // Set the initial message and alpha to 0
        messageText.text = message;
        canvasGroup.alpha = 0; // Set initial alpha to 0

        // Fade in
        float time = 0;
        while (time < fadeInSpeed)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(time / fadeInSpeed); // Clamp between 0 and 1
            yield return null;
        }

        // Wait for hold time
        yield return new WaitForSeconds(holdTime);

        // Fade out
        time = 0;
        while (time < fadeInSpeed)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(1 - (time / fadeInSpeed)); // Fade out
            yield return null;
        }

        // Reset the coroutine to null when done
        messageCoroutine = null;
    }
}
