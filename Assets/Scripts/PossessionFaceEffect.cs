using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class PossessionFaceEffect : NetworkBehaviour
{
    [Header("Face")]
    [SerializeField] private Renderer faceRenderer;

    [Header("Face Size")]
    [SerializeField] private int width = 32;
    [SerializeField] private int height = 24;

    [Header("Glitch")]
    [SerializeField] private float glitchSpeed = 0.07f;

    private Material faceMaterial;
    private Texture originalTexture;
    private Texture2D effectTexture;

    private Coroutine glitchCoroutine;

    private PossessionManager manager;

    private PossessionManager.PossessionPhase lastPhase =
        PossessionManager.PossessionPhase.None;

    public override void OnNetworkSpawn()
    {
        if (faceRenderer == null)
        {
            Debug.LogError(
                "PossessionFaceEffect is missing Face Renderer!"
            );

            return;
        }

        faceMaterial = faceRenderer.material;

        originalTexture = GetCurrentTexture();

        manager = PossessionManager.Instance;

        if (manager == null)
        {
            Debug.LogError(
                "PossessionFaceEffect could not find PossessionManager."
            );

            return;
        }

        lastPhase = manager.Phase.Value;

        ApplyPhase(lastPhase);
    }

    private void Update()
    {
        if (manager == null)
            return;

        if (manager.Phase.Value != lastPhase)
        {
            lastPhase = manager.Phase.Value;
            ApplyPhase(lastPhase);
        }
    }

    private void ApplyPhase(
        PossessionManager.PossessionPhase phase)
    {
        if (glitchCoroutine != null)
        {
            StopCoroutine(glitchCoroutine);
            glitchCoroutine = null;
        }

        switch (phase)
        {
            case PossessionManager.PossessionPhase.None:

                RestoreNormalFace();
                break;

            case PossessionManager.PossessionPhase.Countdown:

                originalTexture = GetCurrentTexture();

                glitchCoroutine =
                    StartCoroutine(GlitchFace());

                break;

            case PossessionManager.PossessionPhase.Active:

                ShowEvilFace();
                break;
        }
    }

    private IEnumerator GlitchFace()
    {
        while (true)
        {
            CreateRandomGlitch();

            yield return new WaitForSeconds(
                glitchSpeed
            );
        }
    }

    private void CreateRandomGlitch()
    {
        CreateTextureIfNeeded();

        Color[] pixels =
            new Color[width * height];

        for (int i = 0; i < pixels.Length; i++)
        {
            float value =
                Random.value > 0.5f
                ? 1f
                : 0f;

            if (Random.value < 0.15f)
            {
                value =
                    Random.Range(0.2f, 0.8f);
            }

            pixels[i] =
                new Color(
                    value,
                    value,
                    value,
                    1f
                );
        }

        effectTexture.SetPixels(pixels);
        effectTexture.Apply();

        SetFaceTexture(effectTexture);
    }

    private void ShowEvilFace()
    {
        CreateTextureIfNeeded();

        Color[] pixels =
            new Color[width * height];

        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.white;
        }

        effectTexture.SetPixels(pixels);

        // Left angry eye
        DrawLine(6, 7, 12, 10, Color.black);
        DrawLine(6, 8, 12, 11, Color.black);

        // Right angry eye
        DrawLine(19, 10, 25, 7, Color.black);
        DrawLine(19, 11, 25, 8, Color.black);

        FillRectangle(
            8, 11, 4, 3, Color.black
        );

        FillRectangle(
            20, 11, 4, 3, Color.black
        );

        // Smile
        DrawLine(9, 17, 13, 19, Color.black);
        DrawLine(13, 19, 18, 19, Color.black);
        DrawLine(18, 19, 23, 16, Color.black);

        effectTexture.Apply();

        SetFaceTexture(effectTexture);
    }

    private void RestoreNormalFace()
    {
        if (originalTexture != null)
        {
            SetFaceTexture(originalTexture);
        }
    }

    private void CreateTextureIfNeeded()
    {
        if (effectTexture != null)
            return;

        effectTexture =
            new Texture2D(
                width,
                height,
                TextureFormat.RGBA32,
                false
            );

        effectTexture.filterMode =
            FilterMode.Point;

        effectTexture.wrapMode =
            TextureWrapMode.Clamp;
    }

    private void FillRectangle(
        int startX,
        int startY,
        int rectWidth,
        int rectHeight,
        Color color)
    {
        for (
            int x = startX;
            x < startX + rectWidth;
            x++
        )
        {
            for (
                int y = startY;
                y < startY + rectHeight;
                y++
            )
            {
                if (
                    x >= 0 &&
                    x < width &&
                    y >= 0 &&
                    y < height
                )
                {
                    effectTexture.SetPixel(
                        x,
                        y,
                        color
                    );
                }
            }
        }
    }

    private void DrawLine(
        int x0,
        int y0,
        int x1,
        int y1,
        Color color)
    {
        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);

        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;

        int err = dx - dy;

        while (true)
        {
            if (
                x0 >= 0 &&
                x0 < width &&
                y0 >= 0 &&
                y0 < height
            )
            {
                effectTexture.SetPixel(
                    x0,
                    y0,
                    color
                );
            }

            if (x0 == x1 && y0 == y1)
                break;

            int e2 = 2 * err;

            if (e2 > -dy)
            {
                err -= dy;
                x0 += sx;
            }

            if (e2 < dx)
            {
                err += dx;
                y0 += sy;
            }
        }
    }

    private Texture GetCurrentTexture()
    {
        if (faceMaterial.HasProperty("_BaseMap"))
        {
            return faceMaterial.GetTexture(
                "_BaseMap"
            );
        }

        if (faceMaterial.HasProperty("_MainTex"))
        {
            return faceMaterial.GetTexture(
                "_MainTex"
            );
        }

        return null;
    }

    private void SetFaceTexture(Texture texture)
    {
        if (faceMaterial.HasProperty("_BaseMap"))
        {
            faceMaterial.SetTexture(
                "_BaseMap",
                texture
            );
        }

        if (faceMaterial.HasProperty("_MainTex"))
        {
            faceMaterial.SetTexture(
                "_MainTex",
                texture
            );
        }
    }
}