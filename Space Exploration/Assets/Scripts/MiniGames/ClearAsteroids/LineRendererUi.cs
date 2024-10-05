using UnityEngine;
using UnityEngine.UI;

public class LineController : MonoBehaviour
{
    public RectTransform bottomLeftObject; // Assign this in the inspector
    public RectTransform crosshair; // Assign this in the inspector
    public RectTransform lineImage; // Assign this in the inspector

    private void Update()
    {
        // Get the positions of the two points in world space
        Vector3 bottomLeftPosition = bottomLeftObject.position;
        Vector3 crosshairPosition = crosshair.position;

        // Calculate the direction and distance between the two points
        Vector3 direction = crosshairPosition - bottomLeftPosition;
        float distance = direction.magnitude;

        // Normalize the direction for rotation
        direction.Normalize();

        // Set the rotation of the line to look at the crosshair from the bottom left
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        lineImage.rotation = Quaternion.Euler(0, 0, angle);

        // Set the scale of the line
        lineImage.sizeDelta = new Vector2(distance, lineImage.sizeDelta.y); // Adjust width based on distance

        // Set the position to the middle of the two points
        Vector3 midPoint = bottomLeftPosition + direction * (distance / 2);
        lineImage.position = midPoint;
    }
}
