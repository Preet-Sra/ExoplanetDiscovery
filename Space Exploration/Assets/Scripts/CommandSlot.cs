using UnityEngine;
using UnityEngine.EventSystems;

public class CommandSlot : MonoBehaviour,IPointerDownHandler
{
    public CommandType assignedCommand;  // Holds the currently assigned command UI element
    GameObject currentCommandObj;
    public UIAnimatorHandler uIAnimatorHandler;
    public GameObject visual;
   

   
    // Function to assign a command to this slot
    public void AssignCommand(CommandType command)
    {
       
        if (currentCommandObj != null)
        {
            assignedCommand = null;
            Destroy(currentCommandObj);
        }

        currentCommandObj = command.transform.gameObject;
        assignedCommand = command;
        SoundManager.instance.PlayAudio(AudioType.UiDroppedSucces);
        if(!command.name.Contains("Search"))
            uIAnimatorHandler.ShowUnits();
        uIAnimatorHandler.SetCommandType(assignedCommand);
        uIAnimatorHandler.HideGuideAnimation();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (assignedCommand == null || assignedCommand.name== "Search")
            return;
        SoundManager.instance.PlayAudio(AudioType.PointerDown);
        uIAnimatorHandler.SetCommandType(assignedCommand);
        if (!assignedCommand.name.Contains("Search"))
            uIAnimatorHandler.ShowUnits();

    }

    public void RemoveCommand()
    {
        if (currentCommandObj != null)
        {
            assignedCommand = null;
            Destroy(currentCommandObj);
        }

    }
}
