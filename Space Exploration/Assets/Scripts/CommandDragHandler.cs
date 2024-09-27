using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CommandDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GameObject commandPrefab;
    GameObject currentDragged;

    public void OnBeginDrag(PointerEventData eventData)
    {
        SoundManager.instance.PlayAudio(AudioType.PointerDown);
        currentDragged = Instantiate(commandPrefab, transform);
        currentDragged.transform.localScale = Vector3.one * 2f;
        currentDragged.transform.position = Input.mousePosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Move the visual instance with the mouse pointer
        if (currentDragged != null)
        {
            currentDragged.transform.position = Input.mousePosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Raycast to detect if the visual was dropped on a CommandSlot
        var raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastResults);

        bool droppedOnSlot = false;
        foreach (var result in raycastResults)
        {
            CommandSlot slot = result.gameObject.GetComponent<CommandSlot>();
            if (slot != null)
            {
                // Assign the command to the slot
                slot.AssignCommand(currentDragged.GetComponent<CommandType>());
                droppedOnSlot = true;

                // Center the command in the slot
                RectTransform commandRect = currentDragged.GetComponent<RectTransform>();
                RectTransform slotRect = slot.GetComponent<RectTransform>();

                commandRect.position = slotRect.position;
                commandRect.anchoredPosition = Vector2.zero;
                currentDragged.transform.localScale = Vector3.one;
                currentDragged.transform.SetParent(slot.transform, false);
                break;
            }
        }

        if (!droppedOnSlot)
        {
            Destroy(currentDragged);
        }
    }
}
