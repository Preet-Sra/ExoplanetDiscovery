using System.Collections;
using UnityEngine;

public class CommandExecutionManager : MonoBehaviour
{
    public CommandSlot[] commandSlots;  // Reference to all the command slots
    public BaseRoverController roverController;  // Reference to the BaseRoverController script
    public GameObject AnimationGuide;
    // Function to start executing commands
    public void ExecuteCommands()
    {
        StartCoroutine(ProcessCommands());
    }

    private IEnumerator ProcessCommands()
    {
        foreach (CommandSlot slot in commandSlots)
        {
            if (slot.assignedCommand != null)
            {
                // Get the command type based on the assignedCommand (e.g., MoveForward, MoveBackward, etc.)
                CommandType commandType = slot.assignedCommand.GetComponent<CommandType>();
                if (commandType != null)
                {
                   
                    yield return ExecuteCommand(commandType.commandName,commandType.currentUnit);  // Execute the command
                }
            }
        }

        StartCoroutine(roverController.ProcessCommands());
    }

  

    private IEnumerator ExecuteCommand(string commandName,int units)
    {
        // Handle different command types
        switch (commandName)
        {
            case "MoveForward":
                roverController.AddMoveCommand(units);  // Add move forward command
                break;
            case "MoveBackward":
                roverController.AddMoveCommand(units);  // Add move backward command
                break;
            case "RotateRight":
                roverController.AddRotateCommand(units);  // Add rotate right command
                break;
            case "RotateLeft":
                roverController.AddRotateCommand(units);  // Add rotate left command
                break;

            case "Search":
                roverController.AddSearchCommand();
                break;
        }

        yield return null;
    }


    public void RemoveCommands()
    {
        foreach (CommandSlot slot in commandSlots)
        {
            slot.RemoveCommand();
        }
    }
}
