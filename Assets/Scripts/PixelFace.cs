using UnityEngine;
using Unity.Netcode;

public class PixelFace : NetworkBehaviour
{
    [Header("Screen")]
    [SerializeField] private Renderer screenRenderer;

    [Header("Face Resolution")]
    [SerializeField] private int width = 32;
    [SerializeField] private int height = 24;

    private Texture2D faceTexture;
    private Material screenMaterial;

    private byte[] faceData;
    private NetworkList<byte> networkFaceData;

    public int Width => width;
    public int Height => height;

    private readonly Color32[] palette =
    {
        new Color32(0, 0, 0, 255),
        new Color32(255, 255, 255, 255),
        new Color32(160, 160, 160, 255),
        new Color32(70, 70, 70, 255),

        new Color32(230, 50, 50, 255),
        new Color32(255, 130, 40, 255),
        new Color32(255, 220, 50, 255),
        new Color32(170, 255, 40, 255),

        new Color32(40, 190, 70, 255),
        new Color32(30, 170, 150, 255),
        new Color32(40, 220, 220, 255),
        new Color32(90, 180, 255, 255),

        new Color32(50, 90, 230, 255),
        new Color32(25, 45, 110, 255),
        new Color32(130, 70, 210, 255),
        new Color32(220, 50, 210, 255),

        new Color32(255, 100, 170, 255),
        new Color32(255, 175, 210, 255),
        new Color32(125, 75, 40, 255),
        new Color32(205, 150, 100, 255),

        new Color32(235, 215, 170, 255),
        new Color32(220, 170, 50, 255),
        new Color32(130, 235, 190, 255),
        new Color32(195, 160, 240, 255)
    };

    private void Awake()
    {
        networkFaceData =
            new NetworkList<byte>();

        CreateFaceTexture();
    }

    public override void OnNetworkSpawn()
    {
        networkFaceData.OnListChanged +=
            OnNetworkFaceChanged;

        if (
            IsServer &&
            networkFaceData.Count == 0
        )
        {
            CreateDefaultNetworkFace();
        }

        if (
            networkFaceData.Count ==
            width * height
        )
        {
            CopyNetworkDataToLocal();
            RefreshTexture();
        }
    }

    public override void OnNetworkDespawn()
    {
        networkFaceData.OnListChanged -=
            OnNetworkFaceChanged;
    }

    private void CreateFaceTexture()
    {
        faceTexture =
            new Texture2D(
                width,
                height,
                TextureFormat.RGBA32,
                false
            );

        faceTexture.filterMode =
            FilterMode.Point;

        faceTexture.wrapMode =
            TextureWrapMode.Clamp;

        screenMaterial =
            screenRenderer.material;

        if (
            screenMaterial.HasProperty(
                "_BaseColor"
            )
        )
        {
            screenMaterial.SetColor(
                "_BaseColor",
                Color.white
            );
        }

        screenMaterial.mainTexture =
            faceTexture;

        if (
            screenMaterial.HasProperty(
                "_BaseMap"
            )
        )
        {
            screenMaterial.SetTexture(
                "_BaseMap",
                faceTexture
            );
        }

        faceData =
            new byte[width * height];

        FillArray(faceData, 1);

        RefreshTexture();
    }

    private void CreateDefaultNetworkFace()
    {
        byte[] defaultFace =
            new byte[width * height];

        FillArray(defaultFace, 1);

        FillRectangleInArray(
            defaultFace,
            6, 15,
            4, 4,
            0
        );

        FillRectangleInArray(
            defaultFace,
            22, 15,
            4, 4,
            0
        );

        FillRectangleInArray(
            defaultFace,
            9, 7,
            14, 3,
            0
        );

        PushArrayToNetwork(
            defaultFace
        );
    }

    public void ApplyPreset(
        int presetIndex
    )
    {
        if (!IsOwner)
            return;

        BuildPreset(
            faceData,
            presetIndex
        );

        RefreshTexture();
    }

    private void BuildPreset(
        byte[] targetData,
        int presetIndex
    )
    {
        switch (presetIndex)
        {
            case 0:
                FillArray(targetData, 1);

                FillRectangleInArray(
                    targetData,
                    6, 15,
                    4, 4,
                    0
                );

                FillRectangleInArray(
                    targetData,
                    22, 15,
                    4, 4,
                    0
                );

                FillRectangleInArray(
                    targetData,
                    7, 16,
                    2, 2,
                    12
                );

                FillRectangleInArray(
                    targetData,
                    23, 16,
                    2, 2,
                    12
                );

                FillRectangleInArray(
                    targetData,
                    10, 6,
                    12, 2,
                    0
                );

                FillRectangleInArray(
                    targetData,
                    8, 8,
                    2, 2,
                    0
                );

                FillRectangleInArray(
                    targetData,
                    22, 8,
                    2, 2,
                    0
                );
                break;

            case 1:
                FillArray(targetData, 17);

                FillRectangleInArray(
                    targetData,
                    6, 15,
                    5, 3,
                    15
                );

                FillRectangleInArray(
                    targetData,
                    21, 15,
                    5, 3,
                    15
                );

                FillRectangleInArray(
                    targetData,
                    3, 10,
                    4, 2,
                    16
                );

                FillRectangleInArray(
                    targetData,
                    25, 10,
                    4, 2,
                    16
                );

                FillRectangleInArray(
                    targetData,
                    11, 6,
                    10, 2,
                    15
                );

                FillRectangleInArray(
                    targetData,
                    9, 8,
                    2, 2,
                    15
                );

                FillRectangleInArray(
                    targetData,
                    21, 8,
                    2, 2,
                    15
                );
                break;

            case 2:
                FillArray(targetData, 13);

                DrawX(
                    targetData,
                    5, 14,
                    6,
                    10
                );

                DrawX(
                    targetData,
                    21, 14,
                    6,
                    10
                );

                FillRectangleInArray(
                    targetData,
                    9, 6,
                    14, 2,
                    10
                );

                FillRectangleInArray(
                    targetData,
                    1, 1,
                    4, 2,
                    14
                );

                FillRectangleInArray(
                    targetData,
                    27, 21,
                    4, 2,
                    14
                );
                break;

            case 3:
                FillArray(targetData, 6);

                FillRectangleInArray(
                    targetData,
                    6, 15,
                    4, 4,
                    0
                );

                FillRectangleInArray(
                    targetData,
                    22, 15,
                    4, 4,
                    0
                );

                FillRectangleInArray(
                    targetData,
                    5, 20,
                    6, 2,
                    5
                );

                FillRectangleInArray(
                    targetData,
                    21, 20,
                    6, 2,
                    5
                );

                FillRectangleInArray(
                    targetData,
                    12, 5,
                    8, 7,
                    0
                );

                FillRectangleInArray(
                    targetData,
                    14, 5,
                    4, 2,
                    4
                );
                break;

            case 4:
                FillArray(targetData, 22);

                FillRectangleInArray(
                    targetData,
                    5, 15,
                    7, 3,
                    14
                );

                FillRectangleInArray(
                    targetData,
                    21, 16,
                    6, 3,
                    15
                );

                FillRectangleInArray(
                    targetData,
                    2, 12,
                    8, 1,
                    10
                );

                FillRectangleInArray(
                    targetData,
                    24, 12,
                    6, 1,
                    12
                );

                FillRectangleInArray(
                    targetData,
                    9, 6,
                    5, 2,
                    14
                );

                FillRectangleInArray(
                    targetData,
                    14, 7,
                    5, 2,
                    15
                );

                FillRectangleInArray(
                    targetData,
                    19, 6,
                    4, 2,
                    14
                );
                break;

            default:
                FillArray(
                    targetData,
                    1
                );
                break;
        }
    }

    public byte[] GetFaceDataCopy()
    {
        if (faceData == null)
            return null;

        byte[] copy =
            new byte[faceData.Length];

        System.Array.Copy(
            faceData,
            copy,
            faceData.Length
        );

        return copy;
    }

    public void ApplyCustomFace(
        byte[] data
    )
    {
        if (!IsOwner)
            return;

        if (data == null)
            return;

        if (
            data.Length !=
            width * height
        )
        {
            Debug.LogWarning(
                "Custom face data was the wrong size."
            );

            return;
        }

        for (
            int i = 0;
            i < faceData.Length;
            i++
        )
        {
            byte paletteIndex =
                data[i];

            if (
                paletteIndex >=
                palette.Length
            )
            {
                paletteIndex = 1;
            }

            faceData[i] =
                paletteIndex;
        }

        RefreshTexture();
    }

    public Texture2D CreatePresetPreview(
        int presetIndex
    )
    {
        byte[] previewData =
            new byte[width * height];

        BuildPreset(
            previewData,
            presetIndex
        );

        return CreatePreviewFromData(
            previewData
        );
    }

    public Texture2D CreatePreviewFromData(
        byte[] data
    )
    {
        if (data == null)
            return null;

        if (
            data.Length !=
            width * height
        )
            return null;

        Texture2D preview =
            new Texture2D(
                width,
                height,
                TextureFormat.RGBA32,
                false
            );

        preview.filterMode =
            FilterMode.Point;

        preview.wrapMode =
            TextureWrapMode.Clamp;

        for (
            int y = 0;
            y < height;
            y++
        )
        {
            for (
                int x = 0;
                x < width;
                x++
            )
            {
                int index =
                    GetArrayIndex(
                        x,
                        y
                    );

                byte paletteIndex =
                    data[index];

                if (
                    paletteIndex >=
                    palette.Length
                )
                {
                    paletteIndex = 1;
                }

                preview.SetPixel(
                    x,
                    y,
                    palette[paletteIndex]
                );
            }
        }

        preview.Apply();

        return preview;
    }

    public void SetPixel(
        int x,
        int y,
        byte paletteIndex
    )
    {
        if (!IsOwner)
            return;

        if (
            !IsValidCoordinate(
                x,
                y
            )
        )
            return;

        if (
            paletteIndex >=
            palette.Length
        )
            return;

        int index =
            GetArrayIndex(
                x,
                y
            );

        faceData[index] =
            paletteIndex;

        faceTexture.SetPixel(
            x,
            y,
            palette[paletteIndex]
        );

        faceTexture.Apply();
    }

    public byte GetPixelIndex(
        int x,
        int y
    )
    {
        if (
            !IsValidCoordinate(
                x,
                y
            )
        )
            return 1;

        return faceData[
            GetArrayIndex(
                x,
                y
            )
        ];
    }

    public void ClearFace()
    {
        if (!IsOwner)
            return;

        FillArray(
            faceData,
            1
        );

        RefreshTexture();
    }

    public void SubmitFaceToNetwork()
    {
        if (!IsOwner)
            return;

        SubmitFaceServerRpc(
            faceData
        );
    }

    [ServerRpc]
    private void SubmitFaceServerRpc(
        byte[] newFaceData,
        ServerRpcParams rpcParams = default
    )
    {
        if (newFaceData == null)
            return;

        if (
            newFaceData.Length !=
            width * height
        )
            return;

        if (
            rpcParams.Receive
                .SenderClientId !=
            OwnerClientId
        )
            return;

        networkFaceData.Clear();

        for (
            int i = 0;
            i < newFaceData.Length;
            i++
        )
        {
            byte paletteIndex =
                newFaceData[i];

            if (
                paletteIndex >=
                palette.Length
            )
            {
                paletteIndex = 1;
            }

            networkFaceData.Add(
                paletteIndex
            );
        }
    }

    private void OnNetworkFaceChanged(
        NetworkListEvent<byte> changeEvent
    )
    {
        if (
            networkFaceData.Count !=
            width * height
        )
            return;

        CopyNetworkDataToLocal();
        RefreshTexture();
    }

    private void CopyNetworkDataToLocal()
    {
        if (
            faceData == null ||
            faceData.Length !=
            width * height
        )
        {
            faceData =
                new byte[
                    width * height
                ];
        }

        for (
            int i = 0;
            i < faceData.Length;
            i++
        )
        {
            faceData[i] =
                networkFaceData[i];
        }
    }

    private void PushArrayToNetwork(
        byte[] data
    )
    {
        networkFaceData.Clear();

        for (
            int i = 0;
            i < data.Length;
            i++
        )
        {
            networkFaceData.Add(
                data[i]
            );
        }
    }

    private void RefreshTexture()
    {
        if (
            faceTexture == null ||
            faceData == null
        )
            return;

        for (
            int y = 0;
            y < height;
            y++
        )
        {
            for (
                int x = 0;
                x < width;
                x++
            )
            {
                int index =
                    GetArrayIndex(
                        x,
                        y
                    );

                byte paletteIndex =
                    faceData[index];

                if (
                    paletteIndex >=
                    palette.Length
                )
                {
                    paletteIndex = 1;
                }

                faceTexture.SetPixel(
                    x,
                    y,
                    palette[paletteIndex]
                );
            }
        }

        faceTexture.Apply();
    }

    private void DrawX(
        byte[] data,
        int startX,
        int startY,
        int size,
        byte paletteIndex
    )
    {
        for (
            int i = 0;
            i < size;
            i++
        )
        {
            SetArrayPixel(
                data,
                startX + i,
                startY + i,
                paletteIndex
            );

            SetArrayPixel(
                data,
                startX + i,
                startY + size - 1 - i,
                paletteIndex
            );
        }
    }

    private void SetArrayPixel(
        byte[] data,
        int x,
        int y,
        byte paletteIndex
    )
    {
        if (
            !IsValidCoordinate(
                x,
                y
            )
        )
            return;

        data[
            GetArrayIndex(
                x,
                y
            )
        ] = paletteIndex;
    }

    private void FillArray(
        byte[] data,
        byte paletteIndex
    )
    {
        for (
            int i = 0;
            i < data.Length;
            i++
        )
        {
            data[i] =
                paletteIndex;
        }
    }

    private void FillRectangleInArray(
        byte[] data,
        int startX,
        int startY,
        int rectangleWidth,
        int rectangleHeight,
        byte paletteIndex
    )
    {
        for (
            int y = startY;
            y < startY +
                rectangleHeight;
            y++
        )
        {
            for (
                int x = startX;
                x < startX +
                    rectangleWidth;
                x++
            )
            {
                SetArrayPixel(
                    data,
                    x,
                    y,
                    paletteIndex
                );
            }
        }
    }

    public Color GetPaletteColor(
        int index
    )
    {
        if (
            index < 0 ||
            index >= palette.Length
        )
            return Color.white;

        return palette[index];
    }

    public int GetPaletteSize()
    {
        return palette.Length;
    }

    private int GetArrayIndex(
        int x,
        int y
    )
    {
        return y * width + x;
    }

    private bool IsValidCoordinate(
        int x,
        int y
    )
    {
        return
            x >= 0 &&
            x < width &&
            y >= 0 &&
            y < height;
    }

    public Texture2D GetTexture()
    {
        return faceTexture;
    }
}