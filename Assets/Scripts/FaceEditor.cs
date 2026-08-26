using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FaceEditor : MonoBehaviour
{
    private enum PresetMode
    {
        Normal,
        Save,
        Delete
    }

    private enum ConfirmationType
    {
        None,
        OverwritePreset,
        DeletePreset
    }

    [Header("Face Canvas")]
    [SerializeField] private RawImage faceCanvas;

    [Header("Built-In Presets")]
    [SerializeField] private FacePresetButton[] builtInPresetButtons;

    [Header("Custom Presets")]
    [SerializeField] private FacePresetButton[] customPresetButtons;

    [Header("Preset Mode UI")]
    [SerializeField] private TMP_Text presetModeText;

    [Header("Confirmation")]
    [SerializeField] private GameObject confirmationPanel;
    [SerializeField] private TMP_Text confirmationText;

    [Header("Color Palette")]
    [SerializeField] private Transform colorPaletteContainer;
    [SerializeField] private FaceColorButton colorButtonPrefab;

    [Header("Drawing")]
    [SerializeField] private int selectedPaletteIndex = 0;

    private PixelFace currentFace;

    private readonly List<FaceColorButton> colorButtons =
        new List<FaceColorButton>();

    private bool paletteCreated = false;

    private PresetMode presetMode =
        PresetMode.Normal;

    private ConfirmationType confirmationType =
        ConfirmationType.None;

    private int pendingCustomSlot = -1;

    //Open and close

    public void OpenEditor(PixelFace face)
    {
        if (face == null)
            return;

        if (!face.IsOwner)
            return;

        currentFace = face;

        faceCanvas.texture =
            currentFace.GetTexture();

        gameObject.SetActive(true);

        presetMode = PresetMode.Normal;

        HideConfirmation();

        if (!paletteCreated)
        {
            CreateColorPalette();
        }

        SetupPresetButtons();

        UpdateSelectedColorVisual();
        UpdatePresetModeText();
    }

    public void CloseEditor()
    {
        if (currentFace != null)
        {
            currentFace.SubmitFaceToNetwork();
        }

        currentFace = null;

        presetMode = PresetMode.Normal;

        HideConfirmation();

        gameObject.SetActive(false);
    }

    //Clear

    public void ClearCurrentFace()
    {
        if (currentFace == null)
            return;

        currentFace.ClearFace();
    }

    //Preset Setup
    
    private void SetupPresetButtons()
    {
        if (currentFace == null)
            return;

        if (builtInPresetButtons != null)
        {
            for (int i = 0; i < builtInPresetButtons.Length; i++)
            {
                if (builtInPresetButtons[i] == null)
                    continue;

                builtInPresetButtons[i].SetupBuiltIn(
                    this,
                    currentFace,
                    i
                );
            }
        }

        RefreshCustomPresetButtons();
    }

    private void RefreshCustomPresetButtons()
    {
        if (currentFace == null)
            return;

        if (customPresetButtons == null)
            return;

        for (int i = 0; i < customPresetButtons.Length; i++)
        {
            if (customPresetButtons[i] == null)
                continue;

            customPresetButtons[i].SetupCustom(
                this,
                currentFace,
                i
            );
        }
    }

    //Preset Clicks

    public void BuiltInPresetClicked(int presetIndex)
    {
        if (currentFace == null)
            return;

        if (presetMode == PresetMode.Save)
        {
            SetModeMessage(
                "Built-in presets cannot be overwritten."
            );

            return;
        }

        if (presetMode == PresetMode.Delete)
        {
            SetModeMessage(
                "Built-in presets cannot be deleted."
            );

            return;
        }

        currentFace.ApplyPreset(presetIndex);
    }

    public void CustomPresetClicked(int slotIndex)
    {
        if (currentFace == null)
            return;

        switch (presetMode)
        {
            case PresetMode.Normal:
                LoadCustomPreset(slotIndex);
                break;

            case PresetMode.Save:
                TrySaveCustomPreset(slotIndex);
                break;

            case PresetMode.Delete:
                TryDeleteCustomPreset(slotIndex);
                break;
        }
    }

    //Save Mode

    public void EnterSavePresetMode()
    {
        if (currentFace == null)
            return;

        presetMode =
            presetMode == PresetMode.Save
            ? PresetMode.Normal
            : PresetMode.Save;

        HideConfirmation();
        UpdatePresetModeText();
    }

    private void TrySaveCustomPreset(int slotIndex)
    {
        if (!FacePresetStorage.HasPreset(slotIndex))
        {
            SaveCurrentFaceToSlot(slotIndex);

            presetMode = PresetMode.Normal;

            UpdatePresetModeText();

            return;
        }

        pendingCustomSlot = slotIndex;

        confirmationType =
            ConfirmationType.OverwritePreset;

        ShowConfirmation(
            "Overwrite this preset?"
        );
    }

    private void SaveCurrentFaceToSlot(int slotIndex)
    {
        byte[] data =
            currentFace.GetFaceDataCopy();

        FacePresetStorage.SavePreset(
            slotIndex,
            data
        );

        RefreshCustomPresetButtons();
    }

    //Delete Mode

    public void EnterDeletePresetMode()
    {
        if (currentFace == null)
            return;

        presetMode =
            presetMode == PresetMode.Delete
            ? PresetMode.Normal
            : PresetMode.Delete;

        HideConfirmation();
        UpdatePresetModeText();
    }

    private void TryDeleteCustomPreset(int slotIndex)
    {
        if (!FacePresetStorage.HasPreset(slotIndex))
        {
            SetModeMessage(
                "That preset slot is already empty."
            );

            return;
        }

        pendingCustomSlot = slotIndex;

        confirmationType =
            ConfirmationType.DeletePreset;

        ShowConfirmation(
            "Delete this preset?"
        );
    }

    //Custom Preset Loading

    private void LoadCustomPreset(int slotIndex)
    {
        byte[] data =
            FacePresetStorage.LoadPreset(
                slotIndex
            );

        if (data == null)
            return;

        currentFace.ApplyCustomFace(
            data
        );
    }

    //Confirm

    public void ConfirmPendingAction()
    {
        if (pendingCustomSlot < 0)
        {
            HideConfirmation();
            return;
        }

        switch (confirmationType)
        {
            case ConfirmationType.OverwritePreset:
                SaveCurrentFaceToSlot(
                    pendingCustomSlot
                );
                break;

            case ConfirmationType.DeletePreset:
                FacePresetStorage.DeletePreset(
                    pendingCustomSlot
                );

                RefreshCustomPresetButtons();
                break;
        }

        presetMode = PresetMode.Normal;

        HideConfirmation();
        UpdatePresetModeText();
    }

    public void CancelPendingAction()
    {
        HideConfirmation();

        presetMode = PresetMode.Normal;

        UpdatePresetModeText();
    }

    private void ShowConfirmation(string message)
    {
        if (confirmationText != null)
        {
            confirmationText.text = message;
        }

        if (confirmationPanel != null)
        {
            confirmationPanel.SetActive(true);
        }
    }

    private void HideConfirmation()
    {
        confirmationType =
            ConfirmationType.None;

        pendingCustomSlot = -1;

        if (confirmationPanel != null)
        {
            confirmationPanel.SetActive(false);
        }
    }

    //Mode Text

    private void UpdatePresetModeText()
    {
        switch (presetMode)
        {
            case PresetMode.Normal:
                SetModeMessage("");
                break;

            case PresetMode.Save:
                SetModeMessage(
                    "Choose a custom preset slot to save."
                );
                break;

            case PresetMode.Delete:
                SetModeMessage(
                    "Choose a custom preset to delete."
                );
                break;
        }
    }

    private void SetModeMessage(string message)
    {
        if (presetModeText != null)
        {
            presetModeText.text = message;
        }
    }

    //Color Palette

    private void CreateColorPalette()
    {
        if (currentFace == null)
            return;

        if (colorPaletteContainer == null)
            return;

        if (colorButtonPrefab == null)
            return;

        colorButtons.Clear();

        int paletteSize =
            currentFace.GetPaletteSize();

        for (int i = 0; i < paletteSize; i++)
        {
            FaceColorButton newButton =
                Instantiate(
                    colorButtonPrefab,
                    colorPaletteContainer
                );

            newButton.Setup(
                this,
                i,
                currentFace.GetPaletteColor(i)
            );

            colorButtons.Add(
                newButton
            );
        }

        paletteCreated = true;
    }

    public void SelectPaletteColor(
        int paletteIndex
    )
    {
        if (currentFace == null)
            return;

        selectedPaletteIndex =
            Mathf.Clamp(
                paletteIndex,
                0,
                currentFace.GetPaletteSize() - 1
            );

        UpdateSelectedColorVisual();
    }

    private void UpdateSelectedColorVisual()
    {
        for (int i = 0; i < colorButtons.Count; i++)
        {
            colorButtons[i].SetSelected(
                i == selectedPaletteIndex
            );
        }
    }

    //Drawing
    
    public void DrawAtScreenPosition(
        PointerEventData eventData
    )
    {
        if (currentFace == null)
            return;

        if (
            confirmationPanel != null &&
            confirmationPanel.activeSelf
        )
            return;

        RectTransform rectTransform =
            faceCanvas.rectTransform;

        Vector2 localPoint;

        bool converted =
            RectTransformUtility
                .ScreenPointToLocalPointInRectangle(
                    rectTransform,
                    eventData.position,
                    eventData.pressEventCamera,
                    out localPoint
                );

        if (!converted)
            return;

        Rect rect =
            rectTransform.rect;

        float normalizedX =
            (localPoint.x - rect.xMin)
            / rect.width;

        float normalizedY =
            (localPoint.y - rect.yMin)
            / rect.height;

        if (
            normalizedX < 0f ||
            normalizedX > 1f ||
            normalizedY < 0f ||
            normalizedY > 1f
        )
            return;

        int pixelX =
            Mathf.FloorToInt(
                normalizedX *
                currentFace.Width
            );

        int pixelY =
            Mathf.FloorToInt(
                normalizedY *
                currentFace.Height
            );

        pixelX =
            Mathf.Clamp(
                pixelX,
                0,
                currentFace.Width - 1
            );

        pixelY =
            Mathf.Clamp(
                pixelY,
                0,
                currentFace.Height - 1
            );

        currentFace.SetPixel(
            pixelX,
            pixelY,
            (byte)selectedPaletteIndex
        );
    }
}