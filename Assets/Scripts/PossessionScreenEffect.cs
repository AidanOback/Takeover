using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PossessionScreenEffect : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private Image blackOverlay;

    [Header("Pulse")]
    [SerializeField] private float minimumAlpha = 0.05f;
    [SerializeField] private float maximumAlpha = 0.65f;
    [SerializeField] private float pulseSpeed = 9f;

    [Header("Flicker")]
    [SerializeField] private float flickerChance = 0.12f;
    [SerializeField] private float flickerStrength = 0.25f;

    private PossessionManager manager;

    private void Start()
    {
        manager = PossessionManager.Instance;

        SetOverlayAlpha(0f);
    }

    private void Update()
    {
        // Only show this player's screen effect
        // on the computer that owns this player.
        if (!IsOwner)
            return;

        if (manager == null)
        {
            manager = PossessionManager.Instance;

            if (manager == null)
                return;
        }

        if (manager.Phase.Value ==
            PossessionManager.PossessionPhase.Countdown)
        {
            UpdateCountdownEffect();
        }
        else
        {
            SetOverlayAlpha(0f);
        }
    }

    private void UpdateCountdownEffect()
    {
        // Smooth pulsing darkness
        float pulse =
            (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f;

        float alpha =
            Mathf.Lerp(
                minimumAlpha,
                maximumAlpha,
                pulse
            );

        // Occasionally make the screen suddenly darker
        if (Random.value < flickerChance)
        {
            alpha += Random.Range(
                0f,
                flickerStrength
            );
        }

        alpha = Mathf.Clamp01(alpha);

        SetOverlayAlpha(alpha);
    }

    private void SetOverlayAlpha(float alpha)
    {
        if (blackOverlay == null)
            return;

        Color color = blackOverlay.color;

        color.r = 0f;
        color.g = 0f;
        color.b = 0f;
        color.a = alpha;

        blackOverlay.color = color;
    }
}