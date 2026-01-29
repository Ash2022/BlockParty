using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class RestartUIView : MonoBehaviour
{
    int numCols;
    int numRows;
    int numLines;
    int numPPLPerLine;
    int numColors;

    bool use12Shapes;
    bool use3Shapes;
    bool use4Shapes;

    public TMP_InputField colsInput;
    public TMP_InputField rowsInput;
    public TMP_InputField linesInput;
    public TMP_InputField pplInput;
    public TMP_InputField colorsInput;

    public Toggle shapes1_2_toggle;
    public Toggle shapes3_toggle;
    public Toggle shapes4_toggle;

    Grid gameGrid;

    public void InitRestartView(int cols, int rows, int numLInes, int numPPL, int numColor, bool[] shapes, Grid grid)
    {
        gameObject.SetActive(true);
        gameGrid = grid;

        numCols = cols;
        numRows = rows;
        numLines = numLInes;
        numPPLPerLine = numPPL;
        numColors = numColor;

        use12Shapes = shapes[0];
        use3Shapes = shapes[1];
        use4Shapes = shapes[2];
        
        colsInput.text = numCols.ToString();
        rowsInput.text = numRows.ToString();
        linesInput.text = numLines.ToString();
        colorsInput.text = numColors.ToString();
        pplInput.text = numPPLPerLine.ToString();

        shapes1_2_toggle.isOn = use12Shapes;
        shapes3_toggle.isOn = use3Shapes;
        shapes4_toggle.isOn = use4Shapes;
    }

    public void OnPlayClicked()
    {
        bool[] shapesArr = new bool[3];

        shapesArr[0] = use12Shapes;
        shapesArr[1] = use3Shapes;
        shapesArr[2] = use4Shapes;



        //gameGrid.StartGameWithNewParams(numCols, numRows, numLines, numPPLPerLine, numColors, shapesArr);

        gameObject.SetActive(false);
    }

    public void OnToggle12ValueChanged(bool newValue)
    {
        use12Shapes = newValue;
    }

    public void OnToggle3ValueChanged(bool newValue)
    {
        use3Shapes = newValue;
    }

    public void OnToggle4ValueChanged(bool newValue)
    {
        use4Shapes = newValue;
    }

    public void OnColsInputFieldValueChanged(string newValue)
    {
        numCols = Convert.ToInt32(newValue);

        numCols = Math.Min(9, numCols);
        numCols = Math.Max(3, numCols);

        colsInput.text = numCols.ToString();
    }

    public void OnRowsInputFieldValueChanged(string newValue)
    {
        numRows = Convert.ToInt32(newValue);

        numRows = Math.Min(9, numRows);
        numRows = Math.Max(3, numRows);

        rowsInput.text = numRows.ToString();
    }

    public void OnNumLinesInputFieldValueChanged(string newValue)
    {
        numLines = Convert.ToInt32(newValue);

        numLines = Math.Min(6, numLines);
        numLines = Math.Max(1, numLines);

        linesInput.text = numLines.ToString();
    }

    public void OnPPLPerLineInputFieldValueChanged(string newValue)
    {
        numPPLPerLine = Convert.ToInt32(newValue);

        numPPLPerLine = Math.Min(15, numLines);
        numPPLPerLine = Math.Max(3, numLines);

        pplInput.text = numPPLPerLine.ToString();
    }

    public void OnColorsInputFieldValueChanged(string newValue)
    {
        numColors = Convert.ToInt32(newValue);

        numColors = Math.Min(5, numColors);
        numColors = Math.Max(2, numColors);

        colorsInput.text = numColors.ToString();
    }
}
