using Unity.Netcode;
using UnityEngine;

public class PlayerSceneReset : NetworkBehaviour
{
    [SerializeField] private Camera gameplayCamera;
    [SerializeField] private Behaviour[] gameplayControls;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
            return;

        ResetForGameplay();
    }

    public void ResetForGameplay()
    {
        if (!IsOwner)
            return;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (gameplayCamera != null)
            gameplayCamera.enabled = true;

        foreach (Behaviour control in gameplayControls)
        {
            if (control != null)
                control.enabled = true;
        }
    }
}