using UnityEngine;
using UnityEngine.EventSystems;

public class FaceCanvasInput : MonoBehaviour,
    IPointerDownHandler,
    IDragHandler
{
    [SerializeField] private FaceEditor faceEditor;

    public void OnPointerDown(
        PointerEventData eventData
    )
    {
        faceEditor.DrawAtScreenPosition(
            eventData
        );
    }

    public void OnDrag(
        PointerEventData eventData
    )
    {
        faceEditor.DrawAtScreenPosition(
            eventData
        );
    }
}