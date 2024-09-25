using UnityEngine;

public class CommandType : MonoBehaviour
{
    public string commandName;  // Name of the command (e.g., "MoveForward", "MoveBackward")
    public int StartingUnit;
    public int currentUnit;
 


    private void OnEnable()
    {
        currentUnit = StartingUnit; 
    }


  
}
