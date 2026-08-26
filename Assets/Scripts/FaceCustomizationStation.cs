using System.Collections.Generic;
using UnityEngine;

public class FaceCustomizationStation : MonoBehaviour
{
    [Header("Editor")]
    [SerializeField] private FaceEditor faceEditor;

    private PixelFace nearbyPlayerFace;
    private PlayerLook nearbyPlayerLook;

    private readonly HashSet<Collider> localPlayerColliders =
        new HashSet<Collider>();

    private bool editorOpen = false;

    private bool dismissedUntilExit = false;

    private void OnTriggerEnter(Collider other)
    {
        PixelFace pixelFace =
            other.GetComponentInParent<PixelFace>();

        if (pixelFace == null)
            return;

        if (!pixelFace.IsOwner)
            return;

        localPlayerColliders.Add(other);

        nearbyPlayerFace = pixelFace;

        nearbyPlayerLook =
            pixelFace.GetComponentInChildren<PlayerLook>();

        if (editorOpen)
            return;

        if (dismissedUntilExit)
            return;

        OpenEditor();
    }

    private void OnTriggerExit(Collider other)
    {
        PixelFace pixelFace =
            other.GetComponentInParent<PixelFace>();

        if (pixelFace == null)
            return;

        if (!pixelFace.IsOwner)
            return;

        localPlayerColliders.Remove(other);

        if (localPlayerColliders.Count > 0)
            return;

        if (editorOpen)
        {
            CloseEditor();
        }

        dismissedUntilExit = false;

        nearbyPlayerFace = null;
        nearbyPlayerLook = null;
    }

    private void OpenEditor()
    {
        if (nearbyPlayerFace == null)
            return;

        if (!nearbyPlayerFace.IsOwner)
            return;

        faceEditor.OpenEditor(
            nearbyPlayerFace
        );

        editorOpen = true;

        if (nearbyPlayerLook != null)
        {
            nearbyPlayerLook.SetLookEnabled(
                false
            );
        }
    }

    private void CloseEditor()
    {
        if (!editorOpen)
            return;

        faceEditor.CloseEditor();

        editorOpen = false;

        if (nearbyPlayerLook != null)
        {
            nearbyPlayerLook.SetLookEnabled(
                true
            );
        }
    }

    public void DoneEditing()
    {
        if (!editorOpen)
            return;

        CloseEditor();

        dismissedUntilExit = true;
    }
}