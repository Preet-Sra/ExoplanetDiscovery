using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BaseRoverController : MonoBehaviour
{
    public float moveSpeed;
    public int moveUnit;
    public bool followingCommand;
    public float rotateSpeed;
    public float rotateUnit;
    public CommandExecutionManager commandExecutionManager;
    private Queue<IEnumerator> commandQueue = new Queue<IEnumerator>();
    UIAnimatorHandler uIAnimatorHandler;
    [SerializeField] GameObject Antenna, AntennaBody;

    [Header("SearchFn")]
    public float sphereRadius = 1f; // Radius of the sphere cast
    public float maxDistance = 10f; // Max distance for the sphere cast
    public string targetTag = "Target"; // Tag to check for collisions
    public Vector3 castDirection = Vector3.forward; // Direction of the sphere cast
    public PlanetInformationShowcase planetInformationShowcase;
    private bool hitDetected;
    MessagePopUp messagePopUp;

    [SerializeField] GameObject InformationPanel;
    [SerializeField] GameObject ExplodeObject;
    GameObject currentReasearchPoint;
    ResearchPointSpawner researchPointSpawner;

    [Space(1)]
    [Header("Sound")]
    [SerializeField] GameObject MoveSound;

    private bool isInRestrictedZone = false; // To check if the rover is in the restricted zone


    [Header("Last Position Tracker")]
   
    private Vector3 lastSafePosition;
    public float positionTrackingInterval = 0.5f;

    private void Start()
    {
        uIAnimatorHandler = FindObjectOfType<UIAnimatorHandler>();
        researchPointSpawner = FindObjectOfType<ResearchPointSpawner>();
        messagePopUp = MessagePopUp.instance;
        StartCoroutine(TrackPosition());
    }

    private void EnqueueCommand(IEnumerator command)
    {
        commandQueue.Enqueue(command);
    }
    IEnumerator TrackPosition()
    {
        while (true)
        {
            // Only update the last safe position if we're not in the restricted zone
            if (!isInRestrictedZone)
            {
                lastSafePosition = transform.position;
            }
            yield return new WaitForSeconds(positionTrackingInterval);
        }
    }
    public IEnumerator ProcessCommands()
    {
        uIAnimatorHandler.HideUnits();
        uIAnimatorHandler.HideEvertything();
        while (commandQueue.Count > 0)
        {
            yield return StartCoroutine(commandQueue.Dequeue());
            yield return new WaitForSeconds(0.25f);
        }
        commandExecutionManager.RemoveCommands();
        Debug.Log("All commands executed");
        followingCommand = false;
        uIAnimatorHandler.ShowEveryThing();
    }

    public void AddMoveCommand(int units)
    {
        EnqueueCommand(Move(units));
    }

    public void AddRotateCommand(float degrees)
    {
        EnqueueCommand(Rotate(degrees));
    }

    public void AddSearchCommand()
    {
        EnqueueCommand(Search());
    }

    IEnumerator Move(int units)
    {
        if (isInRestrictedZone)
        {
            Debug.Log("Cannot move, in restricted zone!");
            yield break; // Prevent movement if in the restricted zone
        }

        MoveSound.SetActive(true);
        Vector3 commandDestination = transform.position + transform.forward * units;
        followingCommand = true;

        while (Vector3.Distance(transform.position, commandDestination) >= 0.1f)
        {
            if (!isInRestrictedZone) // Only move if not in restricted zone
            {
                transform.position = Vector3.MoveTowards(transform.position, commandDestination, moveSpeed * Time.deltaTime);
            }
            else
            {
                Debug.Log("Entered restricted zone, moving back to last safe position.");
                yield return StartCoroutine(MoveToLastSafePosition()); // Move back to last safe position
                break; // Stop movement after moving back
            }
            yield return null;
        }

        isInRestrictedZone = false;
        MoveSound.SetActive(false);
       
    }

    IEnumerator MoveToLastSafePosition()
    {
        MoveSound.SetActive(true);

        // Move towards the last safe position smoothly
        while (Vector3.Distance(transform.position, lastSafePosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, lastSafePosition, moveSpeed * Time.deltaTime);
            yield return null;
        }

        // Snap to the exact last safe position once we're close enough
        transform.position = lastSafePosition;

        MoveSound.SetActive(false);
    }


    IEnumerator Rotate(float degrees)
    {
        Quaternion targetRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0, degrees, 0));
        followingCommand = true;

        while (Quaternion.Angle(transform.rotation, targetRotation) >= 0.1f)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
            yield return null;
        }
        transform.rotation = targetRotation;
    }

    IEnumerator Search()
    {
        // Antenna pops up with a smooth bounce effect
        Antenna.transform.DOScale(Vector3.one, 1f).SetEase(Ease.OutBounce);
        yield return new WaitForSeconds(1f);

        // Rotate AntennaBody one full circle (360 degrees) starting from its current Y rotation
        float currentYRotation = AntennaBody.transform.rotation.eulerAngles.y;
        AntennaBody.transform.DORotate(new Vector3(0, currentYRotation + 360, 0), 1.5f, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear);  // Use Linear easing for a smooth rotation
        messagePopUp.ShowMessage("Analysing.....", 0.5f, 1.5f);
        bool pointFound = ResearchPointFound();

        // Wait for 0.7 seconds for the rotation to complete
        yield return new WaitForSeconds(1.5f);

        if (pointFound)
        {
            StartCoroutine(ContinuousRotation());

            messagePopUp.ShowMessage("Gathering Information... !", 0.5f, 1.5f);
            yield return new WaitForSeconds(1.5f);
            messagePopUp.ShowMessage("Sending Information to Earth.... !", 0.5f, 1.5f);

            ResearchPoint researchPoint = currentReasearchPoint.GetComponent<ResearchPoint>();
            string PlanetName = researchPointSpawner.selectedPlanet.planetName;
            Sprite planetSprite = researchPointSpawner.selectedPlanet.planetIcon;
            string info = researchPoint.GetInformation();
            planetInformationShowcase.SetInfo(PlanetName, planetSprite, info);

            yield return new WaitForSeconds(1.5f);
            InformationPanel.SetActive(true);
            yield return new WaitUntil(() => InformationPanelClosed());
            yield return new WaitForSeconds(0.3f);
            Antenna.transform.DOScale(Vector3.zero, 1f).SetEase(Ease.OutBack);
            currentReasearchPoint.SetActive(false);
            Instantiate(ExplodeObject, currentReasearchPoint.transform.position, Quaternion.identity);
        }
        else
        {
            messagePopUp.ShowMessage("No Data Found !", 0.5f, 1.5f);
            Antenna.transform.DOScale(Vector3.zero, 1f).SetEase(Ease.OutBack);
            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator ContinuousRotation()
    {
        // Continuous rotation until stopped
        while (true)
        {
            float currentYRotation = AntennaBody.transform.rotation.eulerAngles.y;
            AntennaBody.transform.DORotate(new Vector3(0, currentYRotation + 360, 0), 2f, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear);
            yield return new WaitForSeconds(2f);  // Wait for the duration of the rotation
        }
    }

    private bool InformationPanelClosed()
    {
        return !InformationPanel.activeInHierarchy; // Simplified logic
    }

    bool ResearchPointFound()
    {
        // Find all colliders within the specified radius
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, sphereRadius);

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag(targetTag))
            {
                Debug.Log($"Hit object with tag: {targetTag}");
                hitDetected = true;
                currentReasearchPoint = hitCollider.gameObject;
                return true;
            }
        }

        Debug.Log("No object with the target tag found in the radius.");
        hitDetected = false;
        currentReasearchPoint = null;
        return false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = hitDetected ? Color.red : Color.green;

        // Draw the sphere at the starting point
        Gizmos.DrawWireSphere(transform.position, sphereRadius);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("RestrictedZone"))
        {
            Debug.Log("Rover entered the restricted zone.");
            isInRestrictedZone = true; // Set the flag to indicate the rover is in the restricted zone
            StopAllMovementCommands(); // Clear any movement commands if necessary
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("RestrictedZone"))
        {
            Debug.Log("Rover exited the restricted zone.");
            isInRestrictedZone = false; // Reset the flag when exiting the restricted zone
        }
    }

    private void StopAllMovementCommands()
    {
        // Clear movement commands but keep the command queue intact
        commandQueue.Clear(); // Clear movement-related commands
        Debug.Log("All movement commands cleared.");
    }
}
