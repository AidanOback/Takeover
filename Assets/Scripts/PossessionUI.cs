using Unity.Netcode;
using UnityEngine;
using TMPro;

public class PossessionUI : NetworkBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject possessedTitle;
    [SerializeField] private GameObject possessedInstruction;

    private PossessionManager manager;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
            return;

        manager = PossessionManager.Instance;

        SetPossessionUI(false);
    }

    private void Update()
    {
        if (!IsOwner || manager == null)
            return;

        bool shouldShow =
            manager.Phase.Value ==
            PossessionManager.PossessionPhase.Active
            &&
            manager.PossessedClientId.Value ==
            OwnerClientId;

        SetPossessionUI(shouldShow);
    }

    private void SetPossessionUI(bool show)
    {
        if (possessedTitle != null)
            possessedTitle.SetActive(show);

        if (possessedInstruction != null)
            possessedInstruction.SetActive(show);
    }
}