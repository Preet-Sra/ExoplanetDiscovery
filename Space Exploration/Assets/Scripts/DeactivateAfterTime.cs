using System.Collections;
using UnityEngine;

public class DeactivateAfterTime : MonoBehaviour
{
    public float lifetime=0.5f;
    Coroutine coroutine;

    private void OnEnable()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
        coroutine = StartCoroutine(DisableGameObject());
    }

    IEnumerator DisableGameObject()
    {
        yield return new WaitForSeconds(lifetime);
        gameObject.SetActive(false);
    }
}
