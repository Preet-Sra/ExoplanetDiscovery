using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCommands : MonoBehaviour
{
    public BaseRoverController roverController;

    private void Start()
    {
        // Add a series of commands to the roverController
        Test();
    }

    private void Test()
    {
        // Ensure roverController is assigned
        if (roverController == null)
        {
            Debug.LogError("RoverController is not assigned.");
            return;
        }

        // Add commands to the queue
        roverController.AddMoveCommand(5);     // Move forward by 5 units
        roverController.AddRotateCommand(30);  // Rotate 30 degrees
        roverController.AddMoveCommand(10);    // Move forward by 10 units
        roverController.AddRotateCommand(-45); // Rotate -45 degrees (left)
        roverController.AddMoveCommand(7);     // Move forward by 7 units
        roverController.AddRotateCommand(60);  // Rotate 60 degrees
    }
}
