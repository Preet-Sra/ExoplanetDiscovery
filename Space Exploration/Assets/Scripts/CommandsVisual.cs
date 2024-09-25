using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandsVisual : MonoBehaviour
{
    public CommandSlot[] commandSlots;
    public Transform roverController;
    public LineRenderer linePrefab;
    public GameObject circlePrefab;
    public GameObject arrowPrefab;  // Add this field for the arrow prefab

    BaseRoverController baseRoverController;
    private List<LineRenderer> lines = new List<LineRenderer>();
    private List<GameObject> circles = new List<GameObject>();
    private List<GameObject> arrows = new List<GameObject>();  // Add a list to keep track of arrows
    private Vector3 currentStartPosition;
    private Quaternion currentRotation;
    private float totalRotation;
    public float directionincrease = 0.01f;
    public float directionDecrease=0.1f;
    void Start()
    {
        baseRoverController = roverController.GetComponent<BaseRoverController>();
        UpdateCurrentPositionAndRotation();
    }

    void Update()
    {
        if (!baseRoverController.followingCommand)
        {
            UpdateCurrentPositionAndRotation();
            VisualizeCommands();
        }
        else
        {
            ClearExistingVisuals();
        }
    }

    void UpdateCurrentPositionAndRotation()
    {
        currentStartPosition = roverController.position;
        currentRotation = roverController.rotation;
        totalRotation = currentRotation.eulerAngles.y;  // Start with the rover's current Y rotation
    }

    void VisualizeCommands()
    {
        ClearExistingVisuals();

        for (int i = 0; i < commandSlots.Length; i++)
        {
            if (commandSlots[i].assignedCommand != null)
            {
                string commandType = commandSlots[i].assignedCommand.commandName;
                if (commandType.Contains("Move"))
                {
                    VisualizeMoveCommand(commandSlots[i].assignedCommand);
                }
                else if (commandType.Contains("Rotate"))
                {
                    VisualizeRotateCommand(commandSlots[i].assignedCommand);
                }
            }
        }
    }

    void VisualizeMoveCommand(CommandType command)
    {
        // Add a small offset to the Y position to prevent overlapping visuals
        currentStartPosition.y += directionincrease;  // Adjust the Y position by 0.1 units for each new command

        LineRenderer newLine = Instantiate(linePrefab, currentStartPosition, Quaternion.identity);
        newLine.positionCount = 2;
        newLine.SetPosition(0, currentStartPosition);

        Vector3 nextPosition = currentStartPosition + currentRotation * Vector3.forward * command.currentUnit;
        newLine.SetPosition(1, nextPosition);

        // Instantiate and position the arrow at the end of the line
        GameObject newArrow = Instantiate(arrowPrefab, nextPosition- new Vector3(0,directionDecrease,0), Quaternion.identity);
        newArrow.transform.LookAt(nextPosition + currentRotation * Vector3.forward);  // Point arrow in the direction of movement
        if (command.commandName.Contains("MoveBackward"))
        {
            // Rotate the arrow to point backwards
            newArrow.transform.rotation = currentRotation * Quaternion.Euler(0, 180f, 0);
        }
        else
        {
            // Make the arrow look in the forward direction
            newArrow.transform.rotation = currentRotation;
        }


        arrows.Add(newArrow);
        lines.Add(newLine);
        currentStartPosition = nextPosition;  // Update the start position for the next command
    }


    void VisualizeRotateCommand(CommandType command)
    {
        // Add a small offset to the Y position to prevent overlapping visuals
        currentStartPosition.y += directionincrease;  // Adjust the Y position by 0.1 units for each new command

        GameObject newCircle = Instantiate(circlePrefab, currentStartPosition, Quaternion.identity);

        float rotationAmount = command.currentUnit;
        totalRotation += rotationAmount;
        newCircle.transform.rotation = Quaternion.Euler(90, totalRotation, 0);

        circles.Add(newCircle);
        currentRotation *= Quaternion.Euler(0, rotationAmount, 0);  // Update the current rotation for the next command
    }

    void ClearExistingVisuals()
    {
        foreach (LineRenderer line in lines)
        {
            Destroy(line.gameObject);
        }
        lines.Clear();

        foreach (GameObject circle in circles)
        {
            Destroy(circle);
        }
        circles.Clear();

        foreach (GameObject arrow in arrows)
        {
            Destroy(arrow);
        }
        arrows.Clear();
    }
}
