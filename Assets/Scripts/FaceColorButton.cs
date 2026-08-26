using UnityEngine;
using UnityEngine.UI;

public class FaceColorButton : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image buttonImage;

    private FaceEditor faceEditor;
    private int paletteIndex;

    private Outline outline;

    private void Awake()
    {
        outline = GetComponent<Outline>();
    }

    public void Setup(
        FaceEditor editor,
        int index,
        Color color
    )
    {
        faceEditor = editor;
        paletteIndex = index;

        if (buttonImage != null)
        {
            buttonImage.color = color;
        }

        SetSelected(false);
    }

    public void SelectColor()
    {
        if (faceEditor == null)
            return;

        faceEditor.SelectPaletteColor(
            paletteIndex
        );
    }

    public void SetSelected(bool selected)
    {
        if (outline == null)
            return;

        if (selected)
        {
            outline.effectColor =
                Color.white;

            outline.effectDistance =
                new Vector2(
                    3f,
                    -3f
                );
        }
        else
        {
            outline.effectColor =
                new Color(
                    0.1f,
                    0.1f,
                    0.1f,
                    1f
                );

            outline.effectDistance =
                new Vector2(
                    1.5f,
                    -1.5f
                );
        }
    }
}