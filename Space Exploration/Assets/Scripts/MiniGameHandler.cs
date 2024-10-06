
using UnityEngine;
using DG.Tweening;
using System.Collections;

public class MiniGameHandler : MonoBehaviour
{
    [SerializeField] RectTransform miniGameUI;
    AsteroidMinigame asteroidMinigame;
    BaseRoverController baseRoverController;
    // Start is called before the first frame update
    void Start()
    {
        baseRoverController = FindObjectOfType<BaseRoverController>();
        asteroidMinigame = GetComponent<AsteroidMinigame>();
      
    }

    public void ShowMiniGame()
    {
        StartCoroutine(showgameWait());
    }

    IEnumerator showgameWait()
    {
        miniGameUI.gameObject.SetActive(true);
        miniGameUI.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.InOutBack);
        yield return new WaitForSeconds(0.35f);
        asteroidMinigame.ResetGame();
        
    }


    public void HideGame(bool won)
    {
        StartCoroutine(HideGameWait(won));
    }

    IEnumerator HideGameWait(bool won)
    {
        yield return new WaitForSeconds(1f);
        miniGameUI.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InOutBack);
        yield return new WaitForSeconds(0.2f);
        miniGameUI.gameObject.SetActive(false);
        baseRoverController.StartCoroutine(baseRoverController.CheckMinGameStae(won));
    }
}
