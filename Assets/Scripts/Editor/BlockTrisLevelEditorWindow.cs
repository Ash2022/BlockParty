using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using static ShapesDB; // Ensure you have the Newtonsoft package in your project



public class BlockTrisLevelEditorWindow : EditorWindow
{
    private ShapesDB shapesDB;

    // --- Editor-managed data ---
    private LevelData currentLevelData;

    // --- For shape combos ---
    private static readonly string[] group1Shapes = new string[]
    {
        "Null","SingleCube",
        "2Line_Hor", "2Line_Ver",
        "3Line_Hor", "3Line_Ver",
        "3LShape_R0", "3LShape_R90", "3LShape_R180", "3LShape_R270",
        "4Line_Hor", "4Line_Ver", "4Cube",
        "4basePlus_R0", "4basePlus_R90", "4basePlus_R180", "4basePlus_R270",
        "4baseL_R0", "4baseL_R90", "4baseL_R180", "4baseL_R270",
        "4baseL_M_R0", "4baseL_M_R90", "4baseL_M_R180", "4baseL_M_R270",
        "4baseLMirror_R0", "4baseLMirror_R90", "4baseLMirror_R180", "4baseLMirror_R270",
        "4baseLMirror_M_R0", "4baseLMirror_M_R90", "4baseLMirror_M_R180", "4baseLMirror_M_R270",
        "4baseZ_R0", "4baseZ_R90", "4baseZ_M_R0", "4baseZ_M_R90"
    };

    private static readonly string[] group2Shapes = new string[]
    {
        "5F_R0", "5F_R90", "5F_R180", "5F_R270",
        "5F_M_R0", "5F_M_R90", "5F_M_R180", "5F_M_R270",
        "5I_R0", "5I_R90",
        "5L_R0", "5L_R90", "5L_R180", "5L_R270",
        "5L_M_R0", "5L_M_R90", "5L_M_R180", "5L_M_R270",
        "5N_R0", "5N_R90", "5N_R180", "5N_R270",
        "5N_M_R0", "5N_M_R90", "5N_M_R180", "5N_M_R270",
        "5P_R0", "5P_R90", "5P_R180", "5P_R270",
        "5P_M_R0", "5P_M_R90", "5P_M_R180", "5P_M_R270",
        "5T_R0", "5T_R90", "5T_R180", "5T_R270",
        "5U_R0", "5U_R90", "5U_R180", "5U_R270",
        "5V_R0", "5V_R90", "5V_R180", "5V_R270",
        "5W_R0", "5W_R90", "5W_R180", "5W_R270",
        "5X_R0",
        "5Y_R0", "5Y_R90", "5Y_R180", "5Y_R270",
        "5Y_M_R0", "5Y_M_R90", "5Y_M_R180", "5Y_M_R270",
        "5Z_R0", "5Z_R90", "5Z_R180", "5Z_R270"
    };

    // Editor states
    private int numberOfQueueLines = 3;
    private int numberOfPeoplePerLine = 8;
    private float queueFragmentation = 0.5f; // 0 = same color entire line, 1 = color switches every person
    private float boardFullness = 0f;        // 0 = empty, 1 = full
    private Vector2 scrollPos;

    [MenuItem("BlockTris/Level Editor")]
    public static void ShowWindow()
    {
        var window = GetWindow<BlockTrisLevelEditorWindow>("BlockTris Level Editor");
        window.minSize = new Vector2(700, 600);
        window.Show();
    }

    private void OnEnable()
    {
        // Initialize with some default data if no data loaded
        if (currentLevelData == null)
        {
            currentLevelData = new LevelData(5, 4);
            currentLevelData.NumberOfColors = 3;
            currentLevelData.RandomSeed = 1234;
            InitializeBoard();
        }

        shapesDB = new ShapesDB();
        shapesDB.Init();
    }

    private void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        DrawLoadSaveButtons();
        EditorGUILayout.Space();

        DrawBasicSettings();
        EditorGUILayout.Space();

        DrawQueueSettings();
        EditorGUILayout.Space();

        DrawBoardSettings();
        EditorGUILayout.Space();

        DrawShapesLinesSettings();
        EditorGUILayout.Space();

        EditorGUILayout.EndScrollView();
    }

    // -------------------------------------------------------
    // 1) LOAD/SAVE
    // -------------------------------------------------------
    private void DrawLoadSaveButtons()
    {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Load JSON"))
        {
            string path = EditorUtility.OpenFilePanel("Load Level JSON", "", "json");
            if (!string.IsNullOrEmpty(path))
            {
                string jsonText = File.ReadAllText(path);
                currentLevelData = JsonConvert.DeserializeObject<LevelData>(jsonText);

                if (currentLevelData.Board == null
                    || currentLevelData.Board.Count == 0
                    || currentLevelData.Board[0].Count == 0)
                {
                    InitializeBoard();
                }
                if (currentLevelData.NumberOfColors <= 0)
                {
                    currentLevelData.NumberOfColors = 3;
                }
                Repaint();
            }
        }

        if (GUILayout.Button("Save JSON"))
        {
            string path = EditorUtility.SaveFilePanel("Save Level JSON", "", "LevelData.json", "json");
            if (!string.IsNullOrEmpty(path))
            {
                // If you want to serialize NumberOfColors, remove [JsonIgnore] in LevelData
                string jsonText = JsonConvert.SerializeObject(currentLevelData, Formatting.Indented);
                File.WriteAllText(path, jsonText);
                AssetDatabase.Refresh();
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    // -------------------------------------------------------
    // 2) BASIC SETTINGS (Cols, Rows, #Colors, Seed)
    // -------------------------------------------------------
    private void DrawBasicSettings()
    {
        EditorGUILayout.LabelField("Board Dimensions & Basic Settings", EditorStyles.boldLabel);

        EditorGUI.BeginChangeCheck();

        currentLevelData.Columns = EditorGUILayout.IntField("Columns", currentLevelData.Columns);
        currentLevelData.Rows = EditorGUILayout.IntField("Rows", currentLevelData.Rows);
        currentLevelData.NumberOfColors = EditorGUILayout.IntField("Number of Colors", currentLevelData.NumberOfColors);
        currentLevelData.RandomSeed = EditorGUILayout.IntField("Random Seed", currentLevelData.RandomSeed);
        

        if (EditorGUI.EndChangeCheck())
        {
            // Re-initialize board if user changed rows/cols
            InitializeBoard();
        }

        currentLevelData.Tutorial = EditorGUILayout.Toggle("Tutorial", currentLevelData.Tutorial);
        currentLevelData.ExcludeShape5 = EditorGUILayout.Toggle("ExcludeShape5", currentLevelData.ExcludeShape5);
    }

    // -------------------------------------------------------
    // 3) PEOPLE QUEUES (Vertical)
    // -------------------------------------------------------
    private void DrawQueueSettings()
    {
        EditorGUILayout.LabelField("People Queues", EditorStyles.boldLabel);

        numberOfQueueLines = EditorGUILayout.IntField("Number of queue lines", numberOfQueueLines);
        numberOfPeoplePerLine = EditorGUILayout.IntField("People per line", numberOfPeoplePerLine);
        queueFragmentation = EditorGUILayout.Slider("Fragmentation (0=none,1=full)", queueFragmentation, 0f, 1f);

        if (GUILayout.Button("Generate Random Queues"))
        {
            GenerateRandomQueues();
        }

        // Now, display current queues side by side.
        if (currentLevelData.PeopleQueues != null && currentLevelData.PeopleQueues.Count > 0)
        {
            EditorGUILayout.LabelField("Current Queues:", EditorStyles.boldLabel);

            // Put all queues in one horizontal row
            EditorGUILayout.BeginHorizontal();

            for (int q = 0; q < currentLevelData.PeopleQueues.Count; q++)
            {
                List<int> queueLine = currentLevelData.PeopleQueues[q];

                // Each queue is a vertical column
                EditorGUILayout.BeginVertical("box", GUILayout.Width(50));

                // (Optional) If you want a small label for each queue, uncomment:
                //EditorGUILayout.LabelField($"Queue #{q}", GUILayout.Width(60));

                for (int i = queueLine.Count-1; i >=0 ; i--)
                {
                    int colorIndex = queueLine[i];
                    DrawColorSwatch(ref colorIndex, 20f);
                    queueLine[i] = colorIndex;
                }

                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndHorizontal();
        }
    }


    private void GenerateRandomQueues()
    {
        currentLevelData.PeopleQueues.Clear();

        Random.InitState(currentLevelData.RandomSeed);

        for (int i = 0; i < numberOfQueueLines; i++)
        {
            List<int> queueLine = new List<int>();

            int currentColor = Random.Range(0, currentLevelData.NumberOfColors);
            for (int j = 0; j < numberOfPeoplePerLine; j++)
            {
                // fragmentation = chance to switch color
                if (Random.value < queueFragmentation)
                    currentColor = Random.Range(0, currentLevelData.NumberOfColors);

                queueLine.Add(currentColor);
            }
            currentLevelData.PeopleQueues.Add(queueLine);
        }
    }

    // -------------------------------------------------------
    // 4) BOARD (Color Swatches)
    // -------------------------------------------------------
    private void DrawBoardSettings()
    {
        EditorGUILayout.LabelField("Board (Start Grid)", EditorStyles.boldLabel);

        boardFullness = EditorGUILayout.Slider("Fullness (0=empty,1=full)", boardFullness, 0f, 1f);

        if (GUILayout.Button("Randomize Board"))
        {
            RandomizeBoard();
        }

        // Draw board as a clickable grid of color swatches
        if (currentLevelData.Board == null)
        {
            InitializeBoard();
        }

        // Each row
        for (int r = 0; r < currentLevelData.Rows; r++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int c = 0; c < currentLevelData.Columns; c++)
            {
                int colorValue = currentLevelData.Board[r][c];
                DrawColorSwatch(ref colorValue, 25f);
                currentLevelData.Board[r][c] = colorValue;
            }
            EditorGUILayout.EndHorizontal();
        }
    }

    private void RandomizeBoard()
    {
        Random.InitState(currentLevelData.RandomSeed);
        for (int r = 0; r < currentLevelData.Rows; r++)
        {
            for (int c = 0; c < currentLevelData.Columns; c++)
            {
                float chanceToFill = boardFullness;
                if (Random.value <= chanceToFill)
                {
                    // random color
                    currentLevelData.Board[r][c] = Random.Range(0, currentLevelData.NumberOfColors);
                }
                else
                {
                    currentLevelData.Board[r][c] = -1;
                }
            }
        }
    }

    private void InitializeBoard()
    {
        currentLevelData.Board = new List<List<int>>();
        for (int r = 0; r < currentLevelData.Rows; r++)
        {
            var rowList = new List<int>();
            for (int c = 0; c < currentLevelData.Columns; c++)
            {
                rowList.Add(-1); // empty
            }
            currentLevelData.Board.Add(rowList);
        }
    }

    // -------------------------------------------------------
    // 5) SHAPE LINES (with color swatch)
    // -------------------------------------------------------
    private void DrawShapesLinesSettings()
    {
        EditorGUILayout.LabelField("Shapes Lines (Groups of 3 shapes)", EditorStyles.boldLabel);

        // If shapesLines is not multiple of 3, show warning
        if (currentLevelData.ShapesLines.Count % 3 != 0)
        {
            EditorGUILayout.HelpBox("Shape list is not in multiples of 3!", MessageType.Warning);
        }

        if (GUILayout.Button("Add New Shape Line (3 shapes)"))
        {
            // Add 3 default shapes
            currentLevelData.ShapesLines.Add(new ShapeInfo { ShapeName = group1Shapes[0], ShapeColor = 0 });
            currentLevelData.ShapesLines.Add(new ShapeInfo { ShapeName = group1Shapes[0], ShapeColor = 0 });
            currentLevelData.ShapesLines.Add(new ShapeInfo { ShapeName = group1Shapes[0], ShapeColor = 0 });
        }

        // Show each line (3 shapes per line)
        int lineIndex = 0;
        for (int i = 0; i < currentLevelData.ShapesLines.Count; i += 3)
        {
            if (i + 2 >= currentLevelData.ShapesLines.Count) break;

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField($"Shape Line #{lineIndex}");

            EditorGUILayout.BeginHorizontal();
            for (int j = 0; j < 3; j++)
            {
                ShapeInfo shapeInfo = currentLevelData.ShapesLines[i + j];
                DrawShapeSelector(shapeInfo);
            }
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Remove This Shape Line"))
            {
                RemoveShapeLine(i);
                break; // avoid iterating on removed items
            }

            EditorGUILayout.EndVertical();
            lineIndex++;
        }
    }

    private void OLDDrawShapeSelector(ShapeInfo shapeInfo)
    {
        bool isGroup2 = shapeInfo.ShapeName.StartsWith("5");
        int groupIdx = isGroup2 ? 1 : 0;

        // Group popup
        int newGroupIdx = EditorGUILayout.Popup(groupIdx, new string[] { "Group1 (non-5)", "Group2 (5...)" }, GUILayout.Width(120));
        if (newGroupIdx != groupIdx)
        {
            // If changed, reset shape
            shapeInfo.ShapeName = (newGroupIdx == 0) ? group1Shapes[0] : group2Shapes[0];
        }

        // Now choose shape from that group
        string[] shapesArray = (newGroupIdx == 0) ? group1Shapes : group2Shapes;
        int selectedIndex = 0;
        for (int i = 0; i < shapesArray.Length; i++)
        {
            if (shapesArray[i] == shapeInfo.ShapeName)
            {
                selectedIndex = i;
                break;
            }
        }

        int newShapeIndex = EditorGUILayout.Popup(selectedIndex, shapesArray, GUILayout.Width(120));
        shapeInfo.ShapeName = shapesArray[newShapeIndex];

        // Color swatch
        DrawColorSwatch(ref shapeInfo.ShapeColor, 20f);
    }

    private void DrawShapeSelector(ShapeInfo shapeInfo)
    {
        bool isGroup2 = shapeInfo.ShapeName.StartsWith("5");
        int groupIdx = isGroup2 ? 1 : 0;

        // (Existing group & shape selection logic)
        int newGroupIdx = EditorGUILayout.Popup(groupIdx, new string[] { "Group1 (non-5)", "Group2 (5...)" }, GUILayout.Width(120));
        if (newGroupIdx != groupIdx)
        {
            shapeInfo.ShapeName = (newGroupIdx == 0) ? group1Shapes[0] : group2Shapes[0];
        }

        string[] shapesArray = (newGroupIdx == 0) ? group1Shapes : group2Shapes;
        int selectedIndex = 0;
        for (int i = 0; i < shapesArray.Length; i++)
        {
            if (shapesArray[i] == shapeInfo.ShapeName)
            {
                selectedIndex = i;
                break;
            }
        }

        int newShapeIndex = EditorGUILayout.Popup(selectedIndex, shapesArray, GUILayout.Width(120));
        shapeInfo.ShapeName = shapesArray[newShapeIndex];

        // Draw color swatch
        DrawColorSwatch(ref shapeInfo.ShapeColor, 20f);

        // ADD THIS: draw shape preview
        DrawShapePreview(shapeInfo.ShapeName, shapeInfo.ShapeColor);
    }

    private void DrawShapePreview(string shapeName, int colorIndex)
    {
        if (shapesDB == null) return;

        // 1) Get the ShapeDefinition
        ShapeDefinition shapeDef = GetShapeDefinitionByName(shapeName);
        if (shapeDef == null) return;

        // 2) Calculate bounding box of the shape
        int minX = int.MaxValue, maxX = int.MinValue;
        int minY = int.MaxValue, maxY = int.MinValue;

        foreach (var cell in shapeDef.Cells)
        {
            if (cell.x < minX) minX = cell.x;
            if (cell.x > maxX) maxX = cell.x;
            if (cell.y < minY) minY = cell.y;
            if (cell.y > maxY) maxY = cell.y;
        }
        int width = maxY - minY + 1;  // typically y is "column"
        int height = maxX - minX + 1; // typically x is "row"

        // 3) Reserve a rect for the shape preview (say 60x60)
        Rect previewRect = EditorGUILayout.GetControlRect(false, 60, GUILayout.Width(60));
        // Optionally draw a light background
        EditorGUI.DrawRect(previewRect, new Color(0.1f, 0.1f, 0.1f, 0.2f));

        // 4) Draw each cell
        // We'll assume each cell is a small square within the previewRect
        // We'll scale so the shape fits into the 60x60 area
        float cellSize = Mathf.Min(previewRect.width / width, previewRect.height / height);

        Color shapeColor = GetColorByIndex(colorIndex);

        foreach (var cell in shapeDef.Cells)
        {
            // cell.x => row offset, cell.y => col offset
            float xPos = previewRect.x + (cell.y - minY) * cellSize;
            float yPos = previewRect.y + (cell.x - minX) * cellSize;

            Rect cellRect = new Rect(xPos, yPos, cellSize, cellSize);
            EditorGUI.DrawRect(cellRect, shapeColor);
        }
    }


    private void RemoveShapeLine(int startIndex)
    {
        // Remove 3 shapes
        currentLevelData.ShapesLines.RemoveRange(startIndex, 3);
    }

    // -------------------------------------------------------
    // COLOR UTILS
    // -------------------------------------------------------
    /// <summary>
    /// Draws a color swatch (square) for the given color index,
    /// allowing click to cycle color index.
    /// </summary>
    private void DrawColorSwatch(ref int colorIndex, float size = 20f)
    {
        // Reserve layout rect
        Rect rect = GUILayoutUtility.GetRect(size, size, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false));
        // Draw color
        EditorGUI.DrawRect(rect, GetColorByIndex(colorIndex));

        // Check for clicks in that rect
        if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
        {
            colorIndex = GetNextColorIndex(colorIndex);
            Event.current.Use();
        }
    }

    /// <summary>
    /// Cycle color: -1 -> 0 -> 1 -> 2 -> ... -> (#colors-1) -> -1
    /// </summary>
    private int GetNextColorIndex(int currentColor)
    {
        // If currently -1, go to 0
        if (currentColor < 0)
            return 0;

        int next = currentColor + 1;
        if (next >= currentLevelData.NumberOfColors)
        {
            // wrap back to -1
            return -1;
        }
        return next;
    }

    /// <summary>
    /// Returns the UnityEngine.Color that corresponds to the given color index.
    /// 0 - Yellow
    /// 1 - Pink
    /// 2 - Light Blue
    /// 3 - Green
    /// 4 - Purple
    /// 5 - Red
    /// 6 - Orange
    /// If index is -1 or out of range, return a light gray.
    /// </summary>
    private Color GetColorByIndex(int colorIndex)
    {
        switch (colorIndex)
        {
            case 0: return Color.yellow;
            case 1: return new Color(1f, 0.4f, 0.7f);  // "pink" - tweak as you like
            case 2: return Color.cyan;                // "light blue"
            case 3: return Color.green;
            case 4: return new Color(0.5f, 0f, 0.5f); // "purple"
            case 5: return Color.red;
            case 6: return new Color(1f, 0.5f, 0.1f);//orange
            default: return new Color(0.8f, 0.8f, 0.8f); // fallback gray
        }
    }

    // Pseudocode
    public ShapeDefinition GetShapeDefinitionByName(string name)
    {
        // If you maintain a direct dictionary, do:
        // return shapeLookup.ContainsKey(name) ? shapeLookup[name] : null;

        // Otherwise, search in shapeGroups:
        foreach (var kvp in shapesDB.shapeGroups)
        {
            foreach (var def in kvp.Value)
            {
                if (def.Name == name) return def;
            }
        }
        return null;
    }

}
