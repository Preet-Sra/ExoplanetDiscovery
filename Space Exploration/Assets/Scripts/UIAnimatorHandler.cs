using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIAnimatorHandler : MonoBehaviour
{
    [SerializeField] RectTransform AbovePanel, BelowPanel;
    Vector3 abovePanelStartPos, BelowPanelStartPos;
    public Ease moveEase, ScaleEase;
    [SerializeField] GameObject UnitSelecterPanel;
    CommandType commandType;
    [SerializeField] TMP_Text commandsText, unitsText;
    [SerializeField] Slider unitSlider;
    int maxUnit, minUnit;
    SoundManager soundManager;
    [SerializeField] GameObject GuidePanel, AnimationGuide;

    // Start is called before the first frame update
    void Start()
    {
        abovePanelStartPos = AbovePanel.anchoredPosition;
        BelowPanelStartPos = BelowPanel.anchoredPosition;
        unitSlider.onValueChanged.AddListener(ChangeUnitsValue);
        soundManager = SoundManager.instance;
        GuidePanel.transform.localScale = Vector3.zero;
        GuidePanel.SetActive(true);
        GuidePanel.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
    }
    public void HideGuideAnimation()
    {
        AnimationGuide.SetActive(false);
    }
    public void ShowUnits()
    {
        
        UnitSelecterPanel.transform.localScale = Vector3.zero;
        BelowPanel.DOAnchorPosY(BelowPanelStartPos.y - 200f, 0.5f).SetEase(moveEase);
        UnitSelecterPanel.SetActive(true);
        UnitSelecterPanel.transform.DOScale(Vector3.one, 0.2f).SetEase(ScaleEase);
        AbovePanel.DOAnchorPosY(BelowPanelStartPos.y, 0.5f).SetEase(moveEase);
    }

    public void HideUnits()
    {
        BelowPanel.DOAnchorPosY(BelowPanelStartPos.y, 0.5f).SetEase(moveEase);
        UnitSelecterPanel.transform.DOScale(Vector3.zero, 0.2f).SetEase(ScaleEase).OnComplete(() => { UnitSelecterPanel.SetActive(false); });
        AbovePanel.DOAnchorPosY(abovePanelStartPos.y, 0.5f).SetEase(moveEase);
        soundManager.PlayAudio(AudioType.Confirm);
    }

    public void SetCommandType(CommandType type)
    {
        commandType = type;

        if (type.commandName.Contains("Move"))
        {
            commandsText.text = "Select Distance Unit";
            minUnit = 0;
            maxUnit = 10;
        }
        else if (type.commandName.Contains("Rotate"))
        {
            commandsText.text = "Select Rotate Unit";
            minUnit = 0;
            maxUnit = 90;
        }

        unitSlider.minValue = minUnit;
        unitSlider.maxValue = maxUnit;

        // Reset slider value based on current unit and command type
        // Ensure to use the absolute value for the display
     

        unitSlider.value = Mathf.Abs(type.StartingUnit);

        // Update the units text to display the absolute value
        unitsText.text = $"{Mathf.Abs(commandType.currentUnit)} Unit";
    }


  

    public void ChangeUnitsValue(float val)
    {
        if (commandType == null)
            return; // Prevent changes if commandType is not set

        // Calculate the actual value to store based on command type and direction
        if (commandType.commandName.Contains("Move"))
        {
            commandType.currentUnit = commandType.commandName.Contains("Backward") ? -Mathf.RoundToInt(val) : Mathf.RoundToInt(val);
            
        }
        else if (commandType.commandName.Contains("Rotate"))
        {
            commandType.currentUnit = commandType.commandName.Contains("Left") ? -Mathf.RoundToInt(val) : Mathf.RoundToInt(val);
        }

        // Update the units text to display the absolute value
        Invoke(nameof(SetValueDelay), 0.1f);
    }

    void SetValueDelay()
    {
        unitsText.text = $"{Mathf.Abs(commandType.currentUnit)} Unit";

    }


    public void HideEvertything()
    {
        AbovePanel.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.OutBack);
        BelowPanel.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.OutBack);
    }

    public void ShowEveryThing()
    {
        AbovePanel.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
        BelowPanel.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
    }

    public void PlayCloseSound()
    {
        soundManager.PlayAudio(AudioType.Close);
    }

    public void ShowGuide()
    {
        soundManager.PlayAudio(AudioType.Close);
        GuidePanel.transform.localScale = Vector3.zero;
        GuidePanel.SetActive(true);
        GuidePanel.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
       
    }

    public void CloseGuide()
    {
        soundManager.PlayAudio(AudioType.Close);
        GuidePanel.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.OutBack).OnComplete( ()=>GuidePanel.SetActive(false));
    }
}
