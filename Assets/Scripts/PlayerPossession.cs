using Unity.Netcode;
using UnityEngine;

public class PlayerPossession : NetworkBehaviour
{
    [Header("Ghost")]
    [SerializeField] private GhostMovement ghostMovement;
    [SerializeField] private Transform ghostRig;

    [Header("Normal Player")]
    [SerializeField] private Camera normalPlayerCamera;

    [Tooltip("Put your normal movement/look scripts here.")]
    [SerializeField] private Behaviour[] disableWhileGhost;

    [Header("Ghost Spawn")]
    [SerializeField] private Vector3 ghostSpawnOffset =
        new Vector3(0f, 1.5f, -1f);

    private PossessionManager manager;

    private Transform originalGhostParent;

    private bool isGhost;

    public override void OnNetworkSpawn()
    {
        manager = PossessionManager.Instance;

        if (ghostRig != null)
            originalGhostParent = ghostRig.parent;

        if (ghostMovement != null)
            ghostMovement.SetGhostEnabled(false);

        if (manager == null)
        {
            Debug.LogError("PlayerPossession could not find PossessionManager.");
            return;
        }

        manager.Phase.OnValueChanged += OnPossessionPhaseChanged;
        manager.PossessedClientId.OnValueChanged += OnPossessedPlayerChanged;

        CheckPossessionState();
    }

    public override void OnNetworkDespawn()
    {
        if (manager != null)
        {
            manager.Phase.OnValueChanged -= OnPossessionPhaseChanged;
            manager.PossessedClientId.OnValueChanged -= OnPossessedPlayerChanged;
        }
    }

    private void OnPossessionPhaseChanged(
        PossessionManager.PossessionPhase previous,
        PossessionManager.PossessionPhase current)
    {
        CheckPossessionState();
    }

    private void OnPossessedPlayerChanged(
        ulong previous,
        ulong current)
    {
        CheckPossessionState();
    }

    private void CheckPossessionState()
    {
        // Only control ghost mode for THIS computer's player.
        if (!IsOwner)
            return;

        bool shouldBeGhost =
            manager.Phase.Value == PossessionManager.PossessionPhase.Active &&
            manager.PossessedClientId.Value == OwnerClientId;

        if (shouldBeGhost && !isGhost)
        {
            EnterGhostMode();
        }
        else if (!shouldBeGhost && isGhost)
        {
            ExitGhostMode();
        }
    }

    private void EnterGhostMode()
    {
        isGhost = true;

        Debug.Log("YOU HAVE BEEN POSSESSED.");

        // Place ghost where body currently is.
        ghostRig.position =
            transform.TransformPoint(ghostSpawnOffset);

        ghostRig.rotation =
            Quaternion.Euler(0f, transform.eulerAngles.y, 0f);

        // Detach so later the body can move independently.
        ghostRig.SetParent(null, true);

        SetNormalControls(false);

        if (normalPlayerCamera != null)
            normalPlayerCamera.gameObject.SetActive(false);

        ghostMovement.SetGhostEnabled(true);
    }

    private void ExitGhostMode()
    {
        isGhost = false;

        ghostMovement.SetGhostEnabled(false);

        // Put ghost object back underneath player.
        ghostRig.SetParent(originalGhostParent, true);

        ghostRig.localPosition = Vector3.zero;
        ghostRig.localRotation = Quaternion.identity;

        if (normalPlayerCamera != null)
            normalPlayerCamera.gameObject.SetActive(true);

        SetNormalControls(true);
    }

    private void SetNormalControls(bool enabled)
    {
        foreach (Behaviour component in disableWhileGhost)
        {
            if (component != null)
                component.enabled = enabled;
        }
    }
}