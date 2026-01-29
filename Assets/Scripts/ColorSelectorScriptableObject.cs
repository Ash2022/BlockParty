using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "ColorData", menuName = "Custom/Color Selector", order = 1)]
public class ColorSelectorScriptableObject : ScriptableObject
{
    public enum ColorOption
    {
        Green,
        Blue,
        Yellow
    }

    [Header("Image Reference")]
    public SpriteRenderer targetImage;

    [Header("Color Settings")]
    public ColorOption currentColor = ColorOption.Green;

    // Colors corresponding to the enum
    private readonly Color[] colorMap = new Color[]
    {
        Color.green,
        Color.blue,
        Color.yellow
    };

    // Method to change the image color based on the selected option
    public void UpdateImageColor()
    {
        if (targetImage != null)
        {
            targetImage.color = colorMap[(int)currentColor];
        }
        else
        {
            Debug.LogWarning("No image reference set in the Scriptable Object!");
        }
    }

    // Inspector method to trigger color update in edit mode
    private void OnValidate()
    {
        UpdateImageColor();
    }
}