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
    [SerializeField] AudioSource searchSound;
    [SerializeField] GameObject InformationPanel, MiniGamePanel;
    [SerializeField] GameObject ExplodeObject;
    GameObject currentReasearchPoint;
    MiniGameHandler miniGameHandler;
    ResearchPointSpawner researchPointSpawner;

    [Space(1)]
    [Header("Sound")]
    [SerializeField] GameObject MoveSound;

    private bool isInRestrictedZone = false; // To check if the rover is in the restricted zone


    [Header("Last Position Tracker")]
    public float pushDistance;

    SoundManager soundManager;
    CameraHandler cameraHandler;
    GameManager gameManager;
    [SerializeField] GameObject AnimationGuide;
    float YPosition;

    private void Start()
    {
        uIAnimatorHandler = FindObjectOfType<UIAnimatorHandler>();
        researchPointSpawner = FindObjectOfType<ResearchPointSpawner>();
        messagePopUp = MessagePopUp.instance;
        miniGameHandler = FindObjectOfType<MiniGameHandler>();
        soundManager = SoundManager.instance;
        cameraHandler = FindObjectOfType<CameraHandler>();
        gameManager = GameManager.instance;
        YPosition = transform.position.y;
    }



    private void Update()
    {
        Vector3 temp = transform.position;
        temp.y = YPosition;
        transform.position = temp;
    }
    private void EnqueueCommand(IEnumerator command)
    {
        commandQueue.Enqueue(command);
    }


 

    public IEnumerator ProcessCommands()
    {
        if (followingCommand) yield break;
        followingCommand = true;

        uIAnimatorHandler.HideUnits();
        uIAnimatorHandler.HideEvertything();
        cameraHandler.CheckActiveCameraWhileExceutingCommands();
        while (commandQueue.Count > 0)
        {
            yield return StartCoroutine(commandQueue.Dequeue());
            yield return new WaitForSeconds(0.25f);
            uIAnimatorHandler.HideEvertything();
        }
        commandExecutionManager.RemoveCommands();
        
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
               // yield return StartCoroutine(MoveToLastSafePosition()); // Move back to last safe position
                break; // Stop movement after moving back
            }
            yield return null;
        }

        isInRestrictedZone = false;
        MoveSound.SetActive(false);
       
    }

    private IEnumerator CorrectPosition(Vector3 targetPosition)
    {

        while (Vector3.Distance( transform.position, new Vector3( targetPosition.x,transform.position.y,targetPosition.z)) > 0.1f)
        {
            // Move the player smoothly towards the target position
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(targetPosition.x, transform.position.y, targetPosition.z), moveSpeed * Time.deltaTime);
            Debug.Log("Here");
            yield return null;
        }

    }

    IEnumerator Rotate(float degrees)
    {
        Quaternion targetRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0, degrees, 0));
        followingCommand = true;
        soundManager.PlayAudio(AudioType.Rotate);
        while (Quaternion.Angle( transform.rotation, targetRotation) >= 0.1f)
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
        searchSound.Play();
        // Rotate AntennaBody one full circle (360 degrees) starting from its current Y rotation
        float currentYRotation = AntennaBody.transform.rotation.eulerAngles.y;
       AntennaBody.transform.DORotate(new Vector3(0, currentYRotation + 360, 0), 3f, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear);  // Use Linear easing for a smooth rotation


        messagePopUp.ShowMessage("Analysing.....", 0.5f, 3f);
        bool pointFound = ResearchPointFound();

      
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
            searchSound.Stop();
            soundManager.PlayAudio(AudioType.DataFound);
            miniGameHandler.ShowMiniGame();
            yield return new WaitUntil(() => MiniGameClosed());
            yield return new WaitUntil(() => InformationPanelClosed());
        }
        else
        {
            StartCoroutine(ContinuousRotation());
            yield return new WaitForSeconds(1f);
            soundManager.PlayAudio(AudioType.Error);
            searchSound.Stop();
            messagePopUp.ShowMessage("No Data Found !", 0.5f, 1.5f);
            Antenna.transform.DOScale(Vector3.zero, 1f).SetEase(Ease.OutBack);
            yield return new WaitForSeconds(1f);
        }
    }


    public IEnumerator CheckMinGameStae(bool won)
    {
        if (won)
        {
            InformationPanel.SetActive(true);
            yield return new WaitUntil(() => InformationPanelClosed());
            yield return new WaitForSeconds(0.3f);
            Antenna.transform.DOScale(Vector3.zero, 1f).SetEase(Ease.OutBack);
            currentReasearchPoint.SetActive(false);
            Instantiate(ExplodeObject, currentReasearchPoint.transform.position, Quaternion.identity);
            soundManager.PlayAudio(AudioType.Explosion);
            gameManager.ResearchCompleted();
        }
        else
        {
            yield return new WaitForSeconds(0.3f);
            Antenna.transform.DOScale(Vector3.zero, 1f).SetEase(Ease.OutBack);
            soundManager.PlayAudio(AudioType.Error);
            searchSound.Stop();
            messagePopUp.ShowMessage("Error 49: Time Over !", 0.5f, 1.5f);
            Antenna.transform.DOScale(Vector3.zero, 1f).SetEase(Ease.OutBack);
            yield return new WaitForSeconds(1f);
        }
        uIAnimatorHandler.ShowEveryThing();
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
    private bool MiniGameClosed()
    {
        return !MiniGamePanel.activeInHierarchy; // Simplified logic
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
            
            isInRestrictedZone = true; // Set the flag to indicate the rover is in the restricted zone
            StopAllMovementCommands(); // Clear any movement commands if necessary

            Vector3 direction = transform.position - other.transform.position;  // Get direction from zone to player
            Vector3 targetPosition = transform.position + direction.normalized * pushDistance;  // Move away from the zone
            StartCoroutine(CorrectPosition(targetPosition));  // Start moving the player
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("RestrictedZone"))
        {
           
            isInRestrictedZone = false; // Reset the flag when exiting the restricted zone
        }
    }

    private void StopAllMovementCommands()
    {
        // Clear movement commands but keep the command queue intact
        commandQueue.Clear(); // Clear movement-related commands

        followingCommand = false;

        // Optionally show the UI again, if needed
        uIAnimatorHandler.ShowEveryThing();
        MoveSound.SetActive(false);

    }
}
