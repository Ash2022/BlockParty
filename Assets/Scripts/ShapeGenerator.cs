using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

/// <summary>
/// Minimal shape info structure (name + color).
/// </summary>
public class ShapeInfo
{
    public string ShapeName;
    public int ShapeColor;

    public ShapeInfo(string name, int color)
    {
        ShapeName = name;
        ShapeColor = color;
    }

    public ShapeInfo()
    {

    }
}

public static class ShapeGenerator
{
    
    /// <summary>
    /// Our "big method" that returns 3 shapes,
    /// given the board, group availability, queue data, difficulty, and a random seed.
    /// 
    /// Explanations are returned in 'out debugReasons'.
    /// </summary>
    /// 

    public static List<ShapeInfo> GenerateNextThreeShapes(
    int[,] board,
    bool[] groupsAvailability,
    List<List<int>> queueData,
    int difficulty,
    out List<string> debugReasons, int levelNumColors)
    {
        //Debug.Log("Generating new shapes from shapes generator");

        debugReasons = new List<string>();

        

        debugReasons.Add($"Queue distinct color count = {levelNumColors}.");

        // We'll produce exactly 3 shapes
        List<ShapeInfo> resultShapes = new List<ShapeInfo>(3);

        // Determine how many must definitely fit + complete line
        //int mustFitCount = DetermineMustFitCount(difficulty);
        //debugReasons.Add($"Difficulty={difficulty}, mustFitCount={mustFitCount}.");

        int mustFitCount = 2;
        // 2) Consider board fullness
        //    We'll get the fraction of empties. If it's very empty => more likely to allow group5.
        //    If only a few empties => restrict to smaller shapes.
        float fullnessFactor = ComputeBoardFullness(board, out int emptyCount,0 );
        debugReasons.Add($"Board empties={emptyCount}, fullnessFactor={fullnessFactor:0.##}.");

        // Keep track of chosen colors (to avoid all 3 the same if multiple colors available)
        List<int> chosenColors = new List<int>();

        // Also track shape names used so far to avoid duplicates
        HashSet<string> usedShapeNames = new HashSet<string>();



        for (int i = 0; i < 3; i++)
        {
            bool mustFit = (i < mustFitCount);
            ShapeInfo shape = null;
            string reason = "";
            const int MAX_RETRIES = 20; // for re-picking shape if duplicates

            // We'll attempt up to some limit if we pick a shape name that's already used
            for (int attempt = 0; attempt < MAX_RETRIES; attempt++)
            {
                if (mustFit)
                {
                    shape = PickSmartRandomShape(board, groupsAvailability, fullnessFactor, queueData, debugReasons, levelNumColors);
                    //shape = FindShapeThatCompletesLine(board, groupsAvailability, queueData, fullnessFactor, randomSeed, debugReasons, difficulty);
                    if (shape == null)
                    {
                        // fallback shape
                        shape = new ShapeInfo("2CubeHor", 0);
                        reason += " -> fallback used 2CubeHor color=0";
                    }
                    debugReasons.Add($"Shape #{i + 1} mustFit => {reason}");
                }
                else
                {
                    // random shape
                    shape = PickRandomShape(board, groupsAvailability, fullnessFactor, queueData, debugReasons, levelNumColors);
                    debugReasons.Add($"Shape #{i + 1} random => {reason}");
                }

                // If shape's name was already used, we re-pick
                if (usedShapeNames.Contains(shape.ShapeName))
                {
                    debugReasons.Add(
                        $"Shape name {shape.ShapeName} was already used. Retrying pick... (attempt {attempt + 1})");
                }
                else
                {
                    // shape name not yet used, break out
                    break;
                }
            }

            // After up to 10 tries, we keep whatever we got
            usedShapeNames.Add(shape.ShapeName);

            // Enforce color variety if multiple queue colors exist
            if (levelNumColors > 1)
            {
                // If i≥2, it means we might have 3 shapes all same color
                // Check if new shape's color plus the old ones leads to triple color
                int triesColor = 0;
                while (triesColor < 500 && AllSameColorSoFar(chosenColors, shape.ShapeColor, i))
                {
                    // re-pick color for shape
                    int oldColor = shape.ShapeColor;
                    shape = RePickColor(shape, queueData, difficulty, out string colorReason);
                    debugReasons.Add(
                        $"Repicked color from {oldColor} to {shape.ShapeColor} to avoid triple same color. attempt={triesColor + 1}");
                    triesColor++;
                }
            }

            resultShapes.Add(shape);
            chosenColors.Add(shape.ShapeColor);

            //update fullness level - when some shapes are added it changes the board fullness level - 
            //but this isnt refleted on the board yet - so i send dummy values for this

            int addedNotOnBoard = 0;

            foreach (ShapeInfo shapeInfo in resultShapes)
                addedNotOnBoard += GridUtils.GetShapeOffsets(new ShapeInfo(shapeInfo.ShapeName, 0)).Count;

            fullnessFactor = ComputeBoardFullness(board, out emptyCount, addedNotOnBoard);
        }

        return resultShapes;
    }


    /// <summary>
    /// Returns how many shapes must definitely fit+complete a line, based on difficulty.
    /// E.g. difficulty=1 => 3 shapes must fit, difficulty=100 => 1 shape, else we do a linear approach.
    /// </summary>
    private static int DetermineMustFitCount(int difficulty)
    {
        // Map difficulty in [1..100] => mustFit in [3..1]
        // We'll do an integer lerp from 3 down to 1
        //   ratio = (difficulty - 1)/99 => 0..1
        //   mustFit = 3 - ratio*2
        float ratio = (difficulty - 1) / 99f; // 0..1
        float val = 3f - (ratio * 2f);      // 3..1
        int mustFit = Mathf.RoundToInt(val);
        return Mathf.Clamp(mustFit, 1, 3);
    }

    /// <summary>
    /// Compute board fullness. 
    /// We define fullnessFactor = occupied / total.
    /// Also returns 'emptyCount' as nominal.
    /// </summary>
    private static float ComputeBoardFullness(int[,] board, out int emptyCount, int addedNotOnBoard)
    {
        int rows = board.GetLength(0);
        int cols = board.GetLength(1);
        int total = rows * cols;
        int occupied = 0;
        emptyCount = 0;
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                if (board[r, c] != -1) occupied++;
                else emptyCount++;
            }
        }
        return ((float)occupied+ addedNotOnBoard) / total; // fraction that is occupied
    }

    /// <summary>
    /// Attempts to find a shape (any group that is allowed) + color that,
    /// if placed on the board (with help from queueData), completes a row or column.
    /// We'll do a bounded search (some max attempts).
    /// 
    /// If we succeed, returns the shape. Otherwise returns null.
    /// </summary>
    private static ShapeInfo FindShapeThatCompletesLine(
        int[,] board,
        bool[] groupsAvailability,
        List<List<int>> queueData,
        float fullnessFactor,
        int randomSeed,
        List<string> debugReasons, int difficulty)
    {
        // We'll define a maximum iteration to avoid infinite loops
        const int MAX_ITER = 500;
        for (int attempt = 0; attempt < MAX_ITER; attempt++)
        {
            // 1) pick a random group that is allowed
            int group = PickRandomGroup(groupsAvailability, fullnessFactor);
            // 2) pick a shape name from that group
            string shapeName = PickRandomShapeNameFromGroup(group);
            // 3) pick a color (maybe we bias color based on queue?)
            int colorIndex = PickColorSynergyWithQueue(queueData,difficulty, out string debugReason2);

            // 4) Check if there's a guaranteed line completion:
            //    We'll do a brute force check if this shape can be placed to fill a row/col
            //    possibly with help from queue. (Implementation is left to your logic.)
            bool canComplete = CheckLineCompletionWithShape(board, shapeName, colorIndex, queueData, out string debugReason);

            if (canComplete)
            {
                return new ShapeInfo(shapeName, colorIndex);
            }
        }
        // If we never found a shape that can definitely complete a line
        return null;
    }

    /// <summary>
    /// Picks a shape randomly, ignoring synergy checks. Just chooses from available groups,
    /// weighting bigger groups if fullnessFactor < 0.3, for example.
    /// 
    /// Returns a shape with color (possibly random or queue synergy).
    /// </summary>
    private static ShapeInfo PickRandomShape(
        int[,] board,
        bool[] groupsAvailability,
        float fullnessFactor,
        List<List<int>> queueData,
        List<string> debugReasons, int levelNumColors)
    {
        int group = PickRandomGroup(groupsAvailability, fullnessFactor);
        string shapeName = PickRandomShapeNameFromGroup(group);
        int colorIndex = LevelRNG.Rng.Next(0, levelNumColors);// or synergy approach



        return new ShapeInfo(shapeName, colorIndex);
    }

    /// <summary>
    /// Picks a group randomly from the allowed ones, weighting group5 more if fullnessFactor < 0.3
    /// or weighting smaller groups if fullnessFactor > 0.7, etc. 
    /// This is just an example approach.
    /// </summary>
    private static int PickRandomGroup(bool[] groupsAvailability, float fullnessFactor)
    {
        // We'll build a small weighting logic:
        // if fullnessFactor < 0.3 => board is quite empty => prefer bigger shapes
        // if fullnessFactor > 0.7 => board is quite full => prefer smaller shapes
        // else normal weighting

        List<int> groups = new List<int>();     // store group indices that are allowed
        List<float> weights = new List<float>();

        // groupsAvailability => [0]->group1_2, [1]->3, [2]->4, [3]->5
        // We'll define a base weight for each group index
        float[] baseWeight = new float[] { 0f, 0f, 0f, 0f };
        
        if(fullnessFactor < 0.15f)
        {
            // prefer group5
            baseWeight[3] = 10f;
            baseWeight[2] = 6f;
            baseWeight[1] = 0f;
            baseWeight[0] = 0f;
        }
        else if (fullnessFactor < 0.3f)
        {
            baseWeight[3] = 8f;
            baseWeight[2] = 4f;
            baseWeight[1] = 1f;
            baseWeight[0] = 0f;
        }
        else if(fullnessFactor < 0.5f)
        {
            baseWeight[3] = 5f;
            baseWeight[2] = 5f;
            baseWeight[1] = 2f;
            baseWeight[0] = 0f;
        }
        else if (fullnessFactor < 0.7f)
        {
            baseWeight[3] = 2f;
            baseWeight[2] = 6f;
            baseWeight[1] = 3f;
            baseWeight[0] = 1f;
        }
        else if (fullnessFactor < 0.85f)
        {
            baseWeight[3] = 1f;
            baseWeight[2] = 4f;
            baseWeight[1] = 5f;
            baseWeight[0] = 3f;
        }
        else
        {
            baseWeight[3] = 0f;
            baseWeight[2] = 2f;
            baseWeight[1] = 3f;
            baseWeight[0] = 5f;
        }
        // Build list of (group, weight)
        for (int g = 0; g < 4; g++)
        {
            if (groupsAvailability[g])
            {
                groups.Add(g);
                weights.Add(baseWeight[g]);
            }
        }

        if (groups.Count == 0)
        {
            // fallback: none allowed => force group1_2
            return 0;
        }

        // Weighted pick
        float sum = 0f;
        for (int i = 0; i < weights.Count; i++) sum += weights[i];

        double rv = LevelRNG.Rng.NextDouble();
        double rnd = rv * sum;
        //Debug.Log($"Random value = {rv}, sum = {sum}, final = {rnd}");

        float cumulative = 0f;
        for (int i = 0; i < groups.Count; i++)
        {
            cumulative += weights[i];
            if (rnd <= cumulative)
            {
                return groups[i];
            }
        }
        return groups[groups.Count - 1];
    }

    /// <summary>
    /// Example function that picks a random shape name from a group. 
    /// In practice you'd have a real DB of shapeName strings for each group.
    /// </summary>
    private static string PickRandomShapeNameFromGroup(int group)
    {
        return GameManager.Instance.GetRandomShapeNameByGroup(group);
    }

    private static int PickColorSynergyWithQueue(
    List<List<int>> queueData,
    int difficulty,
    out string debugReason)
    {
        debugReason = "";

        // 1) Gather all distinct colors in queues
        HashSet<int> distinctColors = new HashSet<int>();
        for (int i = 0; i < queueData.Count; i++)
        {
            for (int j = 0; j < queueData[i].Count; j++)
            {
                distinctColors.Add(queueData[i][j]);
            }
        }

        // If no color in queue => fallback to color=0
        if (distinctColors.Count == 0)
        {
            debugReason = "No colors in queue -> fallback color=0";
            return 0;
        }

        // Convert to a list for random picking if needed
        List<int> allQueueColors = new List<int>(distinctColors);

        // Define an approach to difficulty:
        //   difficulty <= 30 => "easy" => pick from queue HEAD(s) only
        //   31..70 => "medium" => pick from HEAD or second item
        //   71..100 => "hard" => pick from any color in queue
        if (difficulty <= 30)
        {
            // Easy => gather top items of all queues
            List<int> possibleHeads = new List<int>();
            for (int i = 0; i < queueData.Count; i++)
            {
                if (queueData[i].Count > 0)
                {
                    possibleHeads.Add(queueData[i][0]); // the top color
                }
            }
            if (possibleHeads.Count > 0)
            {
                int c = possibleHeads[LevelRNG.Rng.Next(0, possibleHeads.Count)];
                debugReason = $"Easy => picking from heads => color={c}";
                return c;
            }
            else
            {
                int idx = LevelRNG.Rng.Next(0, allQueueColors.Count);
                int c = allQueueColors[idx];
                debugReason = $"Easy fallback => any queue color => color={c}";
                return c;
            }
        }
        else if (difficulty <= 70)
        {
            // Medium => pick from HEAD or second if present
            List<int> possibleHeadsSeconds = new List<int>();
            for (int i = 0; i < queueData.Count; i++)
            {
                if (queueData[i].Count > 0)
                {
                    possibleHeadsSeconds.Add(queueData[i][0]);
                }
                if (queueData[i].Count > 1)
                {
                    possibleHeadsSeconds.Add(queueData[i][1]);
                }
            }
            if (possibleHeadsSeconds.Count > 0)
            {
                int c = possibleHeadsSeconds[LevelRNG.Rng.Next(0, possibleHeadsSeconds.Count)];
                debugReason = $"Medium => head/second => color={c}";
                return c;
            }
            else
            {
                int idx = LevelRNG.Rng.Next(0, allQueueColors.Count);
                int c = allQueueColors[idx];
                debugReason = $"Medium fallback => any queue color => color={c}";
                return c;
            }
        }
        else
        {
            // Hard => pick from any color present in any queue
            int idx = LevelRNG.Rng.Next(0, allQueueColors.Count);
            int c = allQueueColors[idx];
            debugReason = $"Hard => any queue color => color={c}";
            return c;
        }
    }


    private static bool CheckLineCompletionWithShape(
    int[,] board,
    string shapeName,
    int colorIndex,
    List<List<int>> queueData,
    out string debugReason)
    {
        debugReason = "";

        // We'll define how many "unitsInColor" we can use from the queue for line completion
        // For simplicity, let's say we see how many of colorIndex exist at the HEAD of any queue 
        // or HEAD+1 if you want. This is just an example.
        int unitsInColor = CountColorUnitsInQueueHeads(queueData, colorIndex);

        // 1) Get shape offsets
        var shapeOffsets = GridUtils.GetShapeOffsets(new ShapeInfo(shapeName, colorIndex));
        if (shapeOffsets == null || shapeOffsets.Count == 0)
        {
            debugReason = $"Shape {shapeName} has no offsets -> cannot place.";
            return false;
        }

        // 2) Attempt to place shape in each board position
        int rows = board.GetLength(0);
        int cols = board.GetLength(1);

        // We'll define a maximum iteration for trying positions to avoid huge overhead
        int triesCount = 0;
        const int TRIES_LIMIT = 20000;

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                triesCount++;
                if (triesCount > TRIES_LIMIT)
                {
                    debugReason = $"Hit tries limit {TRIES_LIMIT}, giving up. Possibly no line completion found.";
                    return false;
                }

                // 2a) Check if shape can fit
                if (!GridUtils.ShapeCanFit(board, r, c, shapeOffsets))
                {
                    continue;
                }

                // 2b) Temporarily place shape on a copy of the board
                int[,] tempBoard = (int[,])board.Clone();
                foreach (var off in shapeOffsets)
                {
                    int rr = r + off.rowOffset;
                    int cc = c + off.colOffset;
                    tempBoard[rr, cc] = colorIndex;
                }

                // 2c) Check if that yields line completion using IsLineCompletion
                //     We pass colorIndex & unitsInColor. If IsLineCompletion => done.
                bool lineCompleted = GridUtils.IsLineCompletion(tempBoard, colorIndex, unitsInColor);
                if (lineCompleted)
                {
                    debugReason = $"Placed {shapeName} color={colorIndex} at (r={r}, c={c}), line completed!";
                    return true;
                }
            }
        }

        debugReason = $"No valid placement found for shape {shapeName} color={colorIndex} that completes a line.";
        return false;
    }

    /// <summary>
    /// Counts how many units of `colorIndex` can be used by 
    /// scanning each queue from the head (index 0) onward.
    /// For each queue, if [0] matches colorIndex, increment total 
    /// and check [1], etc., until you hit a mismatch or end of queue.
    /// </summary>
    private static int CountColorUnitsInQueueHeads(List<List<int>> queueData, int colorIndex)
    {
        int total = 0;
        for (int i = 0; i < queueData.Count; i++)
        {
            // Start from the head of this queue
            List<int> thisQueue = queueData[i];

            for (int j = 0; j < thisQueue.Count; j++)
            {
                // If the color matches, keep counting
                if (thisQueue[j] == colorIndex)
                {
                    total++;
                }
                else
                {
                    // As soon as we find a mismatch, stop checking this queue
                    break;
                }
            }
        }
        return total;
    }


    // Example helper that checks if i-th shape + previously chosen colors all match
    private static bool AllSameColorSoFar(List<int> chosenColors, int newColor, int shapeIndex)
    {
        // shapeIndex is 0..2, so if shapeIndex=0 or 1, we don't have 3 shapes yet, 
        // so there's no risk of "all 3 are same" at that point. 
        // but let's be robust:
        if (shapeIndex < 2) return false;

        // check if the first shape color == second shape color == newColor
        // or if all chosen so far are the same.
        if (chosenColors.Count == 2)
        {
            return (chosenColors[0] == chosenColors[1] && chosenColors[0] == newColor);
        }
        return false;
    }

    /// <summary>
    /// Re-picks a color for the shape if needed, while preserving shapeName.
    /// e.g. shapeName stays the same, but we change color synergy approach.
    /// </summary>
    private static ShapeInfo RePickColor(ShapeInfo oldShape, List<List<int>> queueData, int difficulty, out string reason)
    {
        // Let’s keep the same shapeName, but pick a new color synergy from the queue
        int newColor = PickColorSynergyWithQueue(queueData, difficulty, out reason);

        // Return the shape with new color
        return new ShapeInfo(oldShape.ShapeName, newColor);
    }

    private static ShapeInfo PickSmartRandomShape(
    int[,] board,
    bool[] groupsAvailability,
    float fullnessFactor,
    List<List<int>> queueData,
    List<string> debugReasons,
    int levelNumColors)
    {
        // We do up to some max tries to find a shape that fits
        const int MAX_TRIES = 100;
        int rows = board.GetLength(0);
        int cols = board.GetLength(1);

        // 1) Gather possible colors from queue heads 
        //    (if ALL heads have the same color, also gather second-row colors)
        HashSet<int> candidateColors = new HashSet<int>();
        CollectCandidateColorsFromQueue(queueData, candidateColors, debugReasons);

        // If there's still nothing, fallback to all [0..levelNumColors)
        if (candidateColors.Count == 0)
        {
            debugReasons.Add("No candidate colors from queue, fallback to all level colors.");
            for (int c = 0; c < levelNumColors; c++)
                candidateColors.Add(c);
        }

        // Convert to list for easy random picking
        List<int> colorList = new List<int>(candidateColors);

        for (int attempt = 0; attempt < MAX_TRIES; attempt++)
        {
            // 2) pick a random group based on board fullness
            int group = PickRandomGroup(groupsAvailability, fullnessFactor);


            // 3) pick a random shape name from that group
            string shapeName = PickRandomShapeNameFromGroup(group);

            //Debug.Log("FullnessFactor: " + fullnessFactor + " Group Picked:" + group + "shapePicked: " + shapeName);
            //Debug.Log("FullnessFactor: " + fullnessFactor + " Group Picked:" + group + "shapePicked: " + shapeName);
            
            // 4) pick a random color from candidate colors
            int colorIndex = colorList[LevelRNG.Rng.Next(0, colorList.Count)];

            // 5) check if shape can fit on the board
            //    - we just do a quick attempt to see if there's ANY position for this shape
            if (ShapeCanFitSomewhere(board, shapeName, colorIndex))
            {
                // success => build shape info, return
                debugReasons.Add($"PickSmartRandomShape: Found {shapeName} color={colorIndex} that can fit.");
                return new ShapeInfo(shapeName, colorIndex);
            }
            else
            {
                debugReasons.Add($"PickSmartRandomShape: Attempt={attempt} => {shapeName} color={colorIndex} cannot fit. Retrying...");
            }
        }

        // if all tries fail => fallback shape
        debugReasons.Add("PickSmartRandomShape: No shape fits after max tries. Fallback to '1Cube' color=0.");
        return new ShapeInfo("SingleCube", 0);
    }

    /// <summary>
    /// Collects candidate colors from the queue heads. If ALL heads are the same color, 
    /// we also gather second-row colors (the second item in each queue) to have a second color option.
    /// </summary>
    private static void CollectCandidateColorsFromQueue(
        List<List<int>> queueData,
        HashSet<int> candidateColors,
        List<string> debugReasons)
    {
        // gather top colors from each queue
        HashSet<int> topColors = new HashSet<int>();
        for (int i = 0; i < queueData.Count; i++)
        {
            if (queueData[i].Count > 0)
            {
                topColors.Add(queueData[i][0]);
            }
        }
        // if all heads share the same color => topColors.Count == 1
        if (topColors.Count == 1)
        {
            // also gather second-row colors
            int singleTopColor = -1;
            foreach (int c in topColors) singleTopColor = c;

            bool anySecond = false;
            for (int i = 0; i < queueData.Count; i++)
            {
                if (queueData[i].Count > 1)
                {
                    anySecond = true;
                    candidateColors.Add(queueData[i][1]);
                }
            }
            // always add the single top color
            candidateColors.Add(singleTopColor);

            string msg = (anySecond)
                ? $"CollectCandidateColorsFromQueue: All heads are color={singleTopColor}, also took second-row colors."
                : $"CollectCandidateColorsFromQueue: All heads color={singleTopColor}, no second row found.";
            debugReasons.Add(msg);
        }
        else
        {
            // we just add all topColors
            foreach (int c in topColors)
            {
                candidateColors.Add(c);
            }
            debugReasons.Add($"CollectCandidateColorsFromQueue: multiple top colors => {topColors.Count} distinct added.");
        }
    }

    /// <summary>
    /// Checks if shapeName with colorIndex can fit somewhere on the board. 
    /// We do a simple brute force for all positions, 
    /// using GridUtils.GetShapeOffsets(...) and GridUtils.ShapeCanFit(...).
    /// </summary>
    private static bool ShapeCanFitSomewhere(int[,] board, string shapeName, int colorIndex)
    {
        var offsets = GridUtils.GetShapeOffsets(new ShapeInfo(shapeName, colorIndex));
        if (offsets == null || offsets.Count == 0) return false;

        int rows = board.GetLength(0);
        int cols = board.GetLength(1);

        // we brute force each (r,c)
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                if (GridUtils.ShapeCanFit(board, r, c, offsets))
                {
                    // if it can fit here, we are done
                    return true;
                }
            }
        }
        return false;
    }
}
