using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorSelector : MonoBehaviour
{
    public enum ColorOption
    {
        Green,
        Blue,
        Yellow,
        Magenta,
        Cyna
    }

    [Header("Color Settings")]
    [SerializeField] private ColorOption currentColor = ColorOption.Green;
    [SerializeField] private List<SpriteRenderer> targetImage = new List<SpriteRenderer>();

    // Colors corresponding to the enum
    private readonly Color[] colorMap = new Color[]
    {
        Color.green,
        Color.blue,
        Color.yellow,
        Color.magenta,
        Color.cyan

    };

    // Property with custom setter to update color when changed in inspector
    public ColorOption CurrentColor
    {
        get => currentColor;
        set
        {
            currentColor = value;
            UpdateImageColor();
        }
    }

    // Method to change the image color based on the selected option
    private void UpdateImageColor()
    {
        //foreach (SpriteRenderer spriteRenderer in targetImage)
        //    spriteRenderer.color = ModelManager.Instance.GetColorByColorIndex((int)currentColor);

    }

    // Ensure color is set when component is first added or reset
    private void Reset()
    {
        
        UpdateImageColor();
    }

    // Update color in editor and on start
    private void OnValidate()
    {
        UpdateImageColor();
    }

    // Ensure color is set when the game starts
    private void Start()
    {
        UpdateImageColor();
    }
}