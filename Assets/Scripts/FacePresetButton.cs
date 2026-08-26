using UnityEngine;
using UnityEngine.UI;

public class FacePresetButton : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private RawImage previewImage;

    [SerializeField] private GameObject emptySlotVisual;

    private Button button;

    private FaceEditor faceEditor;

    private bool isBuiltIn;
    private int presetIndex;

    private Texture2D ownedPreviewTexture;

    private void Awake()
    {
        button = GetComponent<Button>();

        if (button != null)
        {
            button.onClick.AddListener(
                HandleClick
            );
        }
    }

    private void OnDestroy()
    {
        if (ownedPreviewTexture != null)
        {
            Destroy(
                ownedPreviewTexture
            );
        }
    }

    public void SetupBuiltIn(
        FaceEditor editor,
        PixelFace pixelFace,
        int index
    )
    {
        faceEditor = editor;
        isBuiltIn = true;
        presetIndex = index;

        SetPreview(
            pixelFace.CreatePresetPreview(
                index
            )
        );

        if (emptySlotVisual != null)
        {
            emptySlotVisual.SetActive(false);
        }
    }

    public void SetupCustom(
        FaceEditor editor,
        PixelFace pixelFace,
        int slotIndex
    )
    {
        faceEditor = editor;
        isBuiltIn = false;
        presetIndex = slotIndex;

        byte[] data =
            FacePresetStorage.LoadPreset(
                slotIndex
            );

        if (data == null)
        {
            ClearPreview();

            if (emptySlotVisual != null)
            {
                emptySlotVisual.SetActive(true);
            }

            return;
        }

        if (emptySlotVisual != null)
        {
            emptySlotVisual.SetActive(false);
        }

        SetPreview(
            pixelFace.CreatePreviewFromData(
                data
            )
        );
    }

    private void HandleClick()
    {
        if (faceEditor == null)
            return;

        if (isBuiltIn)
        {
            faceEditor.BuiltInPresetClicked(
                presetIndex
            );
        }
        else
        {
            faceEditor.CustomPresetClicked(
                presetIndex
            );
        }
    }

    private void SetPreview(
        Texture2D newTexture
    )
    {
        ClearPreview();

        ownedPreviewTexture =
            newTexture;

        if (previewImage != null)
        {
            previewImage.texture =
                ownedPreviewTexture;

            previewImage.enabled =
                ownedPreviewTexture != null;
        }
    }

    private void ClearPreview()
    {
        if (ownedPreviewTexture != null)
        {
            Destroy(
                ownedPreviewTexture
            );

            ownedPreviewTexture = null;
        }

        if (previewImage != null)
        {
            previewImage.texture = null;
            previewImage.enabled = false;
        }
    }
}