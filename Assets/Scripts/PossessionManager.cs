using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PossessionManager : NetworkBehaviour
{
    public static PossessionManager Instance;

    public enum PossessionPhase : byte
    {
        None,
        Countdown,
        Active
    }

    public NetworkVariable<PossessionPhase> Phase =
        new NetworkVariable<PossessionPhase>(PossessionPhase.None);

    public NetworkVariable<ulong> PossessedClientId =
        new NetworkVariable<ulong>(ulong.MaxValue);

    [Header("Testing")]
    [SerializeField] private float countdownLength = 3f;

    private bool possessionRunning;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        // For now P only needs to be pressed by the HOST.
        if (!IsServer)
            return;

        if (Keyboard.current != null &&
            Keyboard.current.pKey.wasPressedThisFrame)
        {
            if (!possessionRunning)
                StartCoroutine(StartPossessionRoutine());
        }

        // Temporary test reset key.
        if (Keyboard.current != null &&
            Keyboard.current.oKey.wasPressedThisFrame)
        {
            EndPossession();
        }
    }

    private IEnumerator StartPossessionRoutine()
    {
        possessionRunning = true;

        // Nobody knows who is possessed yet.
        PossessedClientId.Value = ulong.MaxValue;

        // Begin the face-spazz countdown.
        Phase.Value = PossessionPhase.Countdown;

        yield return new WaitForSeconds(countdownLength);

        // Pick a random connected player.
        List<ulong> playerIds = new List<ulong>();

        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            playerIds.Add(clientId);
        }

        if (playerIds.Count == 0)
        {
            EndPossession();
            yield break;
        }

        int randomIndex = Random.Range(0, playerIds.Count);

        PossessedClientId.Value = playerIds[randomIndex];

        // Everybody gets the evil face.
        // Selected player becomes ghost.
        Phase.Value = PossessionPhase.Active;
    }

    public void EndPossession()
    {
        if (!IsServer)
            return;

        Phase.Value = PossessionPhase.None;
        PossessedClientId.Value = ulong.MaxValue;

        possessionRunning = false;
    }
}