using System.Collections.Generic;
using UnityEngine;
using static ShapesDB; // for Vector2Int

public static class GridUtils
{
    /// <summary>
    /// Recursively collects all squares connected to the given (row, col)
    /// that have the same color. Squares are considered "touching" in the
    /// four orthogonal directions (up, down, left, right).
    /// </summary>
    public static List<Vector2Int> GetConnectedSameColor(int[,] grid, int row, int col)
    {
        int rows = grid.GetLength(0);
        int cols = grid.GetLength(1);

        // Basic bounds check
        if (row < 0 || row >= rows || col < 0 || col >= cols)
            return new List<Vector2Int>();

        // The color we want to match
        int targetColor = grid[row, col];
        bool[,] visited = new bool[rows, cols];
        List<Vector2Int> connected = new List<Vector2Int>();

        // Call our recursive DFS helper
        DFS(grid, row, col, targetColor, visited, connected);
        return connected;
    }

    /// <summary>
    /// Recursive DFS to add squares of matching color to 'connected'.
    /// </summary>
    private static void DFS(int[,] grid,
                            int row,
                            int col,
                            int targetColor,
                            bool[,] visited,
                            List<Vector2Int> connected)
    {
        // Bounds check
        if (row < 0 || row >= grid.GetLength(0) ||
            col < 0 || col >= grid.GetLength(1))
        {
            return;
        }

        // Already visited or not matching color
        if (visited[row, col] || grid[row, col] != targetColor)
        {
            return;
        }

        // Mark visited and add this square
        visited[row, col] = true;
        connected.Add(new Vector2Int(row, col));

        // Explore neighbors
        DFS(grid, row - 1, col, targetColor, visited, connected); // up
        DFS(grid, row + 1, col, targetColor, visited, connected); // down
        DFS(grid, row, col - 1, targetColor, visited, connected); // left
        DFS(grid, row, col + 1, targetColor, visited, connected); // right
    }

    /// <summary>
    /// Scans the grid to find the first completed row or column
    /// (i.e., all cells in that row/column share the same color).
    /// Once found, it calls GetConnectedSameColor on one of the squares
    /// in that row/column, returning the connected squares of the same color.
    /// If no completed line is found, returns an empty list.
    /// </summary>
    public static List<Vector2Int> FindCompletedRowOrColumnTouchings(int[,] grid)
    {
        int rows = grid.GetLength(0);
        int cols = grid.GetLength(1);

        // 1) Check rows
        for (int row = 0; row < rows; row++)
        {
            // We'll check the color of the first cell in this row
            int color = grid[row, 0];
            if (color < 0) // or if you want to skip empty squares marked as -1 or something
                continue;

            bool rowCompleted = true;
            for (int col = 1; col < cols; col++)
            {
                if (grid[row, col] != color)
                {
                    rowCompleted = false;
                    break;
                }
            }

            if (rowCompleted)
            {
                // Found a completed row. Start from (row, 0) for instance.
                return GetConnectedSameColor(grid, row, 0);
            }
        }

        // 2) Check columns
        for (int col = 0; col < cols; col++)
        {
            // We'll check the color of the first cell in this column
            int color = grid[0, col];
            if (color < 0)
                continue;

            bool colCompleted = true;
            for (int row = 1; row < rows; row++)
            {
                if (grid[row, col] != color)
                {
                    colCompleted = false;
                    break;
                }
            }

            if (colCompleted)
            {
                // Found a completed column. Start from (0, col).
                return GetConnectedSameColor(grid, 0, col);
            }
        }

        // No completed row or column found
        return new List<Vector2Int>();
    }

    /// <summary>
    /// Given a ShapeInfo, retrieves the corresponding ShapeData via GameManager
    /// and converts its occupied cells (true) into a list of (rowOffset, colOffset).
    /// This method normalizes them so that the top-left cell of the shape is (0,0).
    /// </summary>
    /// <param name="shapeInfo">The shape info containing the name of the shape.</param>
    /// <returns>A list of (rowOffset, colOffset) for all occupied cells of the shape.</returns>
    

    public static List<(int rowOffset, int colOffset)> GetShapeOffsets(ShapeInfo shapeInfo)
    {
        ShapeDefinition shapeDefinition = GameManager.Instance.GetShapeDataByShapeInfo(shapeInfo);

        // 1) Validate the input
        if (shapeDefinition == null || shapeDefinition.Cells == null || shapeDefinition.Cells.Count == 0)
        {
            // No valid shape definition found, return an empty list
            return new List<(int, int)>();
        }

        // 2) Collect all offsets in (row, col) form
        List<(int r, int c)> rawOffsets = new List<(int r, int c)>();
        foreach (var cell in shapeDefinition.Cells)
        {
            rawOffsets.Add((cell.x, cell.y));
        }

        // 3) Normalize so the top-left cell becomes (0, 0)
        int minRow = int.MaxValue;
        int minCol = int.MaxValue;

        foreach (var cell in rawOffsets)
        {
            if (cell.r < minRow) minRow = cell.r;
            if (cell.c < minCol) minCol = cell.c;
        }

        // Shift everything so that minRow/minCol => 0/0
        List<(int rowOffset, int colOffset)> finalOffsets = new List<(int rowOffset, int colOffset)>();
        foreach (var cell in rawOffsets)
        {
            finalOffsets.Add((cell.r - minRow, cell.c - minCol));
        }

        return finalOffsets;
    }


    /// <summary>
    /// Checks if at least one shape from the given list can be placed on the current board.
    /// Returns true if at least one shape is placable, otherwise false.
    /// </summary>
    /// <param name="board">2D array of ints representing the board</param>
    /// <param name="shapes">A list of ShapeInfo objects with shape name and color</param>
    /// <returns>True if at least one shape can be placed, false otherwise</returns>
    public static bool CanPlaceAtLeastOneShape(int[,] board, List<ShapeInfo> shapes)
    {
        int rows = board.GetLength(0);
        int cols = board.GetLength(1);

        foreach (var shapeInfo in shapes)
        {
            // 1) Convert shapeInfo into a list of offsets
            List<(int rOffset, int cOffset)> shapeOffsets = GetShapeOffsets(shapeInfo);

            // 2) Try placing the shape at every position on the board
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    if (ShapeCanFit(board, r, c, shapeOffsets))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Checks if the given shapeOffsets can fit at board[r,c], 
    /// assuming -1 indicates an empty cell.
    /// </summary>
    public static bool ShapeCanFit(int[,] board, int r, int c, List<(int rOffset, int cOffset)> shapeOffsets)
    {
        int rows = board.GetLength(0);
        int cols = board.GetLength(1);

        foreach (var (rOff, cOff) in shapeOffsets)
        {
            int rr = r + rOff;
            int cc = c + cOff;

            // Out of bounds => can't fit
            if (rr < 0 || rr >= rows || cc < 0 || cc >= cols)
                return false;

            // Cell not empty => can't fit
            if (board[rr, cc] != -1)
                return false;
        }

        return true;
    }


    /// <summary>
    /// Finds the row or column in the board that requires at most 'unitsInColor' to complete
    /// (i.e., all cells are either colorIndex or -1). Among multiple candidates, it chooses
    /// the one with the largest emptyCount. If there's a tie in emptyCount, it picks the one
    /// with the highest adjacency score (prefers positions next to existing same-color pieces).
    /// 
    /// Returns the board positions (row,col) where you should place new color pieces.
    /// If no line is completable, returns an empty list.
    /// </summary>
    public static (List<Vector2Int>,int) FindLineCompletionPositions(
        int[,] board,
        int colorIndex,
        int unitsInColor)
    {
        int rows = board.GetLength(0);
        int cols = board.GetLength(1);

        // We track the best line found and its metrics
        int bestEmptyCount = 0;
        int bestAdjacencyScore = 0;
        List<Vector2Int> bestPositions = new List<Vector2Int>();

        // 1) Check all rows
        for (int r = 0; r < rows; r++)
        {
            bool validRow = true;
            int emptyCount = 0;
            List<Vector2Int> emptyCells = new List<Vector2Int>();

            for (int c = 0; c < cols; c++)
            {
                int cellValue = board[r, c];
                if (cellValue == colorIndex)
                {
                    // Cell already matches colorIndex, so it's fine
                }
                else if (cellValue == -1)
                {
                    emptyCount++;
                    emptyCells.Add(new Vector2Int(r, c));
                }
                else
                {
                    // This cell has a different color => row not valid
                    validRow = false;
                    break;
                }
            }

            // If row is valid and can be filled with available units
            if (validRow && emptyCount <= unitsInColor && emptyCount > 0)
            {
                // Compute adjacency score
                int adjacencyScore = ComputeAdjacencyScore(board, emptyCells, colorIndex);

                // Decide if this row is better than our current best
                // 1) prefer bigger emptyCount
                // 2) if tie, prefer bigger adjacencyScore
                if ((emptyCount > bestEmptyCount)
                    || (emptyCount == bestEmptyCount && adjacencyScore > bestAdjacencyScore))
                {
                    bestEmptyCount = emptyCount;
                    bestAdjacencyScore = adjacencyScore;
                    bestPositions = emptyCells;
                }
            }
        }

        // 2) Check all columns
        for (int c = 0; c < cols; c++)
        {
            bool validCol = true;
            int emptyCount = 0;
            List<Vector2Int> emptyCells = new List<Vector2Int>();

            for (int r = 0; r < rows; r++)
            {
                int cellValue = board[r, c];
                if (cellValue == colorIndex)
                {
                    // Cell already matches colorIndex
                }
                else if (cellValue == -1)
                {
                    emptyCount++;
                    emptyCells.Add(new Vector2Int(r, c));
                }
                else
                {
                    validCol = false;
                    break;
                }
            }

            if (validCol && emptyCount <= unitsInColor && emptyCount > 0)
            {
                int adjacencyScore = ComputeAdjacencyScore(board, emptyCells, colorIndex);

                if ((emptyCount > bestEmptyCount)
                    || (emptyCount == bestEmptyCount && adjacencyScore > bestAdjacencyScore))
                {
                    bestEmptyCount = emptyCount;
                    bestAdjacencyScore = adjacencyScore;
                    bestPositions = emptyCells;
                }
            }
        }

        return (bestPositions,bestEmptyCount);
    }

    /// <summary>
    /// Computes how many neighbors of 'emptyCells' are already of 'colorIndex'.
    /// Higher score => more adjacency to existing same-color pieces.
    /// </summary>
    private static int ComputeAdjacencyScore(int[,] board, List<Vector2Int> emptyCells, int colorIndex)
    {
        int score = 0;
        foreach (var cell in emptyCells)
        {
            // check up, down, left, right
            score += IsSameColor(board, cell.x - 1, cell.y, colorIndex) ? 1 : 0;
            score += IsSameColor(board, cell.x + 1, cell.y, colorIndex) ? 1 : 0;
            score += IsSameColor(board, cell.x, cell.y - 1, colorIndex) ? 1 : 0;
            score += IsSameColor(board, cell.x, cell.y + 1, colorIndex) ? 1 : 0;
        }
        return score;
    }

    private static bool IsSameColor(int[,] board, int r, int c, int colorIndex)
    {
        if (r < 0 || r >= board.GetLength(0) || c < 0 || c >= board.GetLength(1))
            return false;
        return board[r, c] == colorIndex;
    }

    public static (List<Vector2Int>,int) SimulatePlacementAndCheckCompletions(
    int[,] board,
    int[] queueTops,
    int colorIndex)
    {
        // 1) For each color in the queue, see if we can complete a row/column
        //for (int colorIndex = 0; colorIndex < queueTops.Length; colorIndex++)
        //{
            int availableUnits = queueTops[colorIndex];
            if (availableUnits <= 0)
               return (null,0); // No pieces of this color to use

            // 2) See if there's a row or column that can be completed 
            //    with 'availableUnits' of 'colorIndex'
            (List<Vector2Int> positionsToFill, int peopleUsed) = FindLineCompletionPositions(board, colorIndex, availableUnits);

            if (positionsToFill.Count > 0 || true)
            {
                // We can complete a line by filling these positions
                // 3) "Simulate" placing those pieces
                foreach (var pos in positionsToFill)
                {
                    board[pos.x, pos.y] = colorIndex;
                }

                // 4) Check which squares are now fully completed 
                //    (i.e., an entire row or column of the same color).
                List<Vector2Int> completedSquares = FindCompletedRowOrColumnTouchings(board);

                // 5) Return that list so the caller knows which squares
                //    should be removed or exploded, etc.
                return (completedSquares,peopleUsed);
            }
        //}

        // If no line can be completed with the available queue pieces, return empty
        return (new List<Vector2Int>(),0);
    }

    public static Vector2 GetShapeCenter(List<Vector2> linePositions)
    {
        if (linePositions == null || linePositions.Count == 0)
        {
            Debug.LogWarning("The list of positions is empty or null.");
            return Vector2.zero;
        }

        float minX = int.MaxValue;
        float maxX = int.MinValue;
        float minY = int.MaxValue;
        float maxY = int.MinValue;

        // Calculate bounds
        foreach (var position in linePositions)
        {
            minX = Mathf.Min(minX, position.x);
            maxX = Mathf.Max(maxX, position.x);
            minY = Mathf.Min(minY, position.y);
            maxY = Mathf.Max(maxY, position.y);
        }

        // Calculate center
        float centerX = (minX + maxX) / 2f;
        float centerY = (minY + maxY) / 2f;

        return new Vector2(centerX, centerY);
    }

    

    public static bool IsLineCompletion(
        int[,] board,
        int colorIndex,
        int unitsInColor)
    {
        int rows = board.GetLength(0);
        int cols = board.GetLength(1);

        // We track the best line found and its metrics
        int bestEmptyCount = 0;
        int bestAdjacencyScore = 0;
        List<Vector2Int> bestPositions = new List<Vector2Int>();

        // 1) Check all rows
        for (int r = 0; r < rows; r++)
        {
            bool validRow = true;
            int emptyCount = 0;
            List<Vector2Int> emptyCells = new List<Vector2Int>();

            for (int c = 0; c < cols; c++)
            {
                int cellValue = board[r, c];
                if (cellValue == colorIndex)
                {
                    // Cell already matches colorIndex, so it's fine
                }
                else if (cellValue == -1)
                {
                    emptyCount++;
                    emptyCells.Add(new Vector2Int(r, c));
                }
                else
                {
                    // This cell has a different color => row not valid
                    validRow = false;
                    break;
                }
            }

            // If row is valid and can be filled with available units
            if (validRow && emptyCount <= unitsInColor && emptyCount > 0)
            {
                return true;
            }
        }

        // 2) Check all columns
        for (int c = 0; c < cols; c++)
        {
            bool validCol = true;
            int emptyCount = 0;
            List<Vector2Int> emptyCells = new List<Vector2Int>();

            for (int r = 0; r < rows; r++)
            {
                int cellValue = board[r, c];
                if (cellValue == colorIndex)
                {
                    // Cell already matches colorIndex
                }
                else if (cellValue == -1)
                {
                    emptyCount++;
                    emptyCells.Add(new Vector2Int(r, c));
                }
                else
                {
                    validCol = false;
                    break;
                }
            }

            if (validCol && emptyCount <= unitsInColor && emptyCount > 0)
            {
                return true;
            }
        }

        return false;
    }



    /// <summary>
    /// Suggests a best move among the given 'shapes'.
    /// 
    /// 1) If any shape can complete a line, pick the shape/placement that completes 
    ///    the *most squares* (i.e. biggest line completion).
    /// 2) If no shape can complete a line:
    ///    - If the board has some pieces (not empty), do your usual fallback (omitted here).
    ///    - If the board is empty, pick the shape/placement that yields the largest 
    ///      "minimum empty region" (i.e. we want to avoid splitting the empty space 
    ///      into tiny pockets).
    /// 
    /// Returns (positions, shapeView). 
    /// If no move found, returns (empty list, null).
    /// 
    /// Depends on:
    ///   - GridUtils.ShapeCanFit(...)
    ///   - GridUtils.SimulatePlacementAndCheckCompletions(...)
    /// </summary>
    public static (List<Vector2Int> positions, ShapeView shapeView)
        SuggestBestMove(int[,] board, int[] queue, List<ShapeView> shapes)
    {
        int rows = board.GetLength(0);
        int cols = board.GetLength(1);

        // ---------------------------------------------------------------
        // PASS 1: Try to complete a line
        // ---------------------------------------------------------------
        int[,] tempBoard = new int[rows, cols];
        int bestCompletionScore = 0;
        List<Vector2Int> bestPositions = new List<Vector2Int>();
        ShapeView bestShapeView = null;

        foreach (var shapeView in shapes)
        {
            // Convert shape offsets from Vector2Int to (int,int) tuples
            var offsets = shapeView.GetShapeRelativeGridPositions();
            var tupleOffsets = new List<(int, int)>();
            foreach (var cell in offsets)
                tupleOffsets.Add((cell.x, cell.y));

            int colorIndex = shapeView.ColorIndex;

            // Try placing this shape at all (r,c) positions
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    // Can it fit here?
                    if (!GridUtils.ShapeCanFit(board, r, c, tupleOffsets))
                        continue;

                    // Copy board, place shape
                    CopyBoard(board, tempBoard);
                    var placedCells = PlaceShape(tempBoard, r, c, tupleOffsets, colorIndex);

                    // Attempt line completions using queue
                    (var completedSquares, int peopleUsed) = GridUtils.SimulatePlacementAndCheckCompletions(tempBoard, queue, colorIndex);

                    int completionScore = 0;

                    if(completedSquares!= null)
                        completionScore = completedSquares.Count;

                    // Keep track of best
                    if (completionScore > bestCompletionScore)
                    {
                        bestCompletionScore = completionScore;
                        bestPositions = placedCells;
                        bestShapeView = shapeView;
                    }
                }
            }
        }

        // If we found any line completion, return it
        if (bestCompletionScore > 0)
        {
            return (bestPositions, bestShapeView);
        }

        // ---------------------------------------------------------------
        // PASS 2: No line can be completed => fallback logic
        // ---------------------------------------------------------------
        if (!IsBoardCompletelyEmpty(board))
        {
            // Board is NOT empty => 
            // we pick the shape/placement that yields the longest single-color 
            // horizontal or vertical line of shape color.

            int bestLineLength = 0;
            List<Vector2Int> fallbackPositions = new List<Vector2Int>();
            ShapeView fallbackShape = null;

            foreach (var shapeView in shapes)
            {
                // Convert shape offsets
                var offsets = shapeView.GetShapeRelativeGridPositions();
                var tupleOffsets = new List<(int, int)>();
                foreach (var cell in offsets)
                    tupleOffsets.Add((cell.x, cell.y));

                int colorIndex = shapeView.ColorIndex;

                for (int r = 0; r < rows; r++)
                {
                    for (int c = 0; c < cols; c++)
                    {
                        if (!GridUtils.ShapeCanFit(board, r, c, tupleOffsets))
                            continue;

                        // Copy board
                        CopyBoard(board, tempBoard);

                        // Place shape
                        var placedCells = PlaceShape(tempBoard, r, c, tupleOffsets, colorIndex);

                        // Measure the longest single-color line of colorIndex
                        int lineLen = GetLongestSingleColorLine(tempBoard, colorIndex);

                        if (lineLen > bestLineLength)
                        {
                            bestLineLength = lineLen;
                            fallbackPositions = placedCells;
                            fallbackShape = shapeView;
                        }
                    }
                }
            }

            if (fallbackShape != null)
            {
                return (fallbackPositions, fallbackShape);
            }
        }
        else
        {
            // Board IS empty => 
            // pick shape placement that maximizes the smallest empty region
            // of -1 squares (we want the "min region size" to be as big as possible)
            int bestMinRegionSize = int.MinValue;
            List<Vector2Int> fallbackPositions = new List<Vector2Int>();
            ShapeView fallbackShape = null;

            foreach (var shapeView in shapes)
            {
                var offsets = shapeView.GetShapeRelativeGridPositions();
                var tupleOffsets = new List<(int, int)>();
                foreach (var cell in offsets)
                    tupleOffsets.Add((cell.x, cell.y));

                int colorIndex = shapeView.ColorIndex;

                for (int r = 0; r < rows; r++)
                {
                    for (int c = 0; c < cols; c++)
                    {
                        if (!GridUtils.ShapeCanFit(board, r, c, tupleOffsets))
                            continue;

                        // Copy board, place shape
                        CopyBoard(board, tempBoard);
                        PlaceShape(tempBoard, r, c, tupleOffsets, colorIndex);

                        // Get the size of the smallest empty region
                        int minRegionSize = GetMinEmptyRegionSize(tempBoard);

                        if (minRegionSize > bestMinRegionSize)
                        {
                            bestMinRegionSize = minRegionSize;
                            fallbackPositions = new List<Vector2Int>();
                            foreach (var (rOff, cOff) in tupleOffsets)
                                fallbackPositions.Add(new Vector2Int(r + rOff, c + cOff));
                            fallbackShape = shapeView;
                        }
                    }
                }
            }

            if (fallbackShape != null)
            {
                return (fallbackPositions, fallbackShape);
            }
        }

        // If we reach here, no good move was found
        return (new List<Vector2Int>(), null);
    }

    // ------------------------------------------------------------------------
    // HELPER METHODS (no duplicates of existing ones)
    // ------------------------------------------------------------------------

    /// <summary>
    /// Copies 'source' into 'destination'.
    /// </summary>
    private static void CopyBoard(int[,] source, int[,] destination)
    {
        int rows = source.GetLength(0);
        int cols = source.GetLength(1);
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                destination[r, c] = source[r, c];
            }
        }
    }

    /// <summary>
    /// Places a shape in 'board' at (r,c) using the given offsets 
    /// and colorIndex. Returns the list of absolute positions placed.
    /// </summary>
    private static List<Vector2Int> PlaceShape(int[,] board, int r, int c,
        List<(int rOff, int cOff)> offsets, int colorIndex)
    {
        var placed = new List<Vector2Int>();
        foreach (var (rOff, cOff) in offsets)
        {
            int rr = r + rOff;
            int cc = c + cOff;
            board[rr, cc] = colorIndex;
            placed.Add(new Vector2Int(rr, cc));
        }
        return placed;
    }

    /// <summary>
    /// Returns true if every cell in 'board' is -1.
    /// </summary>
    private static bool IsBoardCompletelyEmpty(int[,] board)
    {
        int rows = board.GetLength(0);
        int cols = board.GetLength(1);
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                if (board[r, c] != -1)
                    return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Computes the size of the smallest connected region of empty squares (-1).
    /// We find every connected region (via BFS or DFS) of -1 squares, 
    /// collect their sizes, and return the smallest. 
    /// If no empty squares, returns 0.
    /// </summary>
    private static int GetMinEmptyRegionSize(int[,] board)
    {
        int rows = board.GetLength(0);
        int cols = board.GetLength(1);
        bool[,] visited = new bool[rows, cols];
        List<int> regionSizes = new List<int>();

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                if (board[r, c] == -1 && !visited[r, c])
                {
                    int size = GetConnectedRegionSize(board, r, c, visited);
                    regionSizes.Add(size);
                }
            }
        }

        if (regionSizes.Count == 0)
            return 0; // No empty squares

        regionSizes.Sort();
        return regionSizes[0]; // smallest region
    }

    /// <summary>
    /// BFS to find connected region of empty squares (-1) 
    /// starting at (startR, startC). Returns the region size.
    /// </summary>
    private static int GetConnectedRegionSize(int[,] board, int startR, int startC, bool[,] visited)
    {
        int rows = board.GetLength(0);
        int cols = board.GetLength(1);

        Queue<(int rr, int cc)> queue = new Queue<(int rr, int cc)>();
        queue.Enqueue((startR, startC));
        visited[startR, startC] = true;

        int regionSize = 0;

        while (queue.Count > 0)
        {
            var (rr, cc) = queue.Dequeue();
            regionSize++;

            // Explore up/down/left/right
            var neighbors = new (int nr, int nc)[]
            {
                (rr-1, cc),
                (rr+1, cc),
                (rr, cc-1),
                (rr, cc+1),
            };

            foreach (var (nr, nc) in neighbors)
            {
                if (nr >= 0 && nr < rows && nc >= 0 && nc < cols)
                {
                    if (!visited[nr, nc] && board[nr, nc] == -1)
                    {
                        visited[nr, nc] = true;
                        queue.Enqueue((nr, nc));
                    }
                }
            }
        }

        return regionSize;
    }

    /// <summary>
    /// Scans every row and column in 'board' to find the longest consecutive run
    /// of 'colorIndex'. Returns the length of that run.
    /// </summary>
    private static int GetLongestSingleColorLine(int[,] board, int colorIndex)
    {
        int rows = board.GetLength(0);
        int cols = board.GetLength(1);
        int longest = 0;

        // 1) Check rows
        for (int r = 0; r < rows; r++)
        {
            int currentStreak = 0;
            for (int c = 0; c < cols; c++)
            {
                if (board[r, c] == colorIndex)
                {
                    currentStreak++;
                    if (currentStreak > longest)
                        longest = currentStreak;
                }
                else
                {
                    currentStreak = 0;
                }
            }
        }

        // 2) Check columns
        for (int c = 0; c < cols; c++)
        {
            int currentStreak = 0;
            for (int r = 0; r < rows; r++)
            {
                if (board[r, c] == colorIndex)
                {
                    currentStreak++;
                    if (currentStreak > longest)
                        longest = currentStreak;
                }
                else
                {
                    currentStreak = 0;
                }
            }
        }

        return longest;
    }



    /// <summary>
    /// Returns true if it's guaranteed IMPOSSIBLE to fill all remaining
    /// empty cells in 'board' using:
    ///   - The single-cell blocks available in 'queueAvailability' 
    ///     (i.e. queueAvailability[color] = how many 1x1 blocks you can place).
    ///   - Any multi-cell shapes from 'shapes' (with their own color constraints).
    /// 
    /// If 'count of empty cells' > minimalEmpty, we skip the check 
    /// and assume "not impossible" (return false).
    /// 
    /// If we find at least one way to fill all empties, 
    /// we return false (meaning it's NOT impossible).
    /// If we exhaust the search, return true (it is indeed impossible).
    /// </summary>
    public static bool IsImpossibleToComplete(
        int[,] board,
        int[] queueAvailability,
        List<ShapeDefinition> shapes,
        int minimalEmpty)
    {
        int rows = board.GetLength(0);
        int cols = board.GetLength(1);

        // 1) Count how many empty cells we have
        int emptyCount = 0;
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                if (board[r, c] == -1)
                    emptyCount++;
            }
        }

        // 2) If empties > minimalEmpty => skip check
        if (emptyCount > minimalEmpty)
        {
            // We won't claim it's impossible, so we return false
            return false;
        }

        // 3) If no empties => already filled => not impossible
        if (emptyCount == 0)
        {
            return false;
        }

        // 4) Possibly do quick checks, like:
        //    - If sum(queueAvailability) + total shape coverage < emptyCount => impossible
        //    - If emptyCount is not multiple of shape sizes and single blocks are not enough, etc.
        if (!BasicFeasibilityChecks(board, queueAvailability, shapes))
        {
            return true; // definitely impossible if failing these checks
        }

        // 5) Attempt a backtracking cover
        // If we find at least one arrangement that fills the board, 
        // => we return false (NOT impossible).
        bool canCover = TryCoverAll(board, queueAvailability, shapes);
        return !canCover;
    }

    /// <summary>
    /// Some quick & dirty checks that might rule out obvious impossibilities.
    /// For example, if we have fewer total single-cell pieces + shape coverage 
    /// than needed, or some color constraints that are obviously not satisfiable.
    /// 
    /// Returns false if we detect it's obviously impossible, 
    /// otherwise true (meaning "maybe" possible, so keep checking).
    /// </summary>
    private static bool BasicFeasibilityChecks(
        int[,] board,
        int[] queueAvailability,
        List<ShapeDefinition> shapes)
    {
        // Example: check total empty squares vs total coverage possible
        int emptyCount = 0;
        int rows = board.GetLength(0);
        int cols = board.GetLength(1);
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                if (board[r, c] == -1)
                    emptyCount++;
            }
        }

        // Sum of all single blocks:
        int sumSingleBlocks = 0;
        for (int color = 0; color < queueAvailability.Length; color++)
        {
            sumSingleBlocks += queueAvailability[color];
        }

        // Maximum coverage from multi-cell shapes? 
        // If your game has unlimited usage of shapes, it's unlimited coverage. 
        // If each shape is "one-time" usage, you'd sum shapes.Count * shapeSize, etc.
        // For demonstration, let's assume shapes can be used unlimited times => no upper bound.
        // If you have a limited shape set, you'd compute that total coverage.

        // A trivial check: if we have 0 shapes and sumSingleBlocks < emptyCount => impossible
        // or if shapes are limited in number...
        if (shapes.Count == 0 && sumSingleBlocks < emptyCount)
        {
            return false; // meaning "obviously impossible"
        }

        return true;
    }

    /// <summary>
    /// Core backtracking that tries to fill all empty cells 
    /// using (1) single-cell pieces from 'queueAvailability'
    /// and (2) multi-cell shapes from 'shapes'.
    /// 
    /// Returns true if it finds at least one arrangement 
    /// that fills the board, false if not.
    /// 
    /// This can be expensive for bigger boards but might be OK 
    /// for small # empties.
    /// </summary>
    private static bool TryCoverAll(
    int[,] board,
    int[] queueAvailability,
    List<ShapeDefinition> shapes)
    {
        // 1) Find the next empty cell
        Vector2Int? nextEmpty = FindNextEmptyCell(board);
        if (nextEmpty == null)
        {
            // No empty => board fully covered
            return true;
        }
        int baseR = nextEmpty.Value.x;
        int baseC = nextEmpty.Value.y;

        // 2) Try single-cell piece from each color that we have > 0
        for (int color = 0; color < queueAvailability.Length; color++)
        {
            if (queueAvailability[color] > 0)
            {
                // Place single cell
                board[baseR, baseC] = color;
                queueAvailability[color]--;

                bool canCover = TryCoverAll(board, queueAvailability, shapes);
                if (canCover) return true;

                // Backtrack
                board[baseR, baseC] = -1;
                queueAvailability[color]++;
            }
        }

        // 3) Try each multi-cell shape in 'shapes'
        //    and place it in every possible color.
        foreach (var shape in shapes)
        {
            // Check if shape can fit at (baseR, baseC)
            if (CanPlaceShape(board, baseR, baseC, shape))
            {
                // We'll try every color for this shape
                for (int color = 0; color < queueAvailability.Length; color++)
                {
                    // Place the shape with 'color'
                    PlaceShape(board, baseR, baseC, shape, color);

                    bool canCover = TryCoverAll(board, queueAvailability, shapes);
                    if (canCover) return true;

                    // Backtrack
                    UnplaceShape(board, baseR, baseC, shape);
                }
            }
        }

        // If none of the single-cells or shapes+colors worked, fail
        return false;
    }

    private static Vector2Int? FindNextEmptyCell(int[,] board)
    {
        int rows = board.GetLength(0);
        int cols = board.GetLength(1);
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                if (board[r, c] == -1)
                    return new Vector2Int(r, c);
            }
        }
        return null;
    }

    /// <summary>
    /// Checks if shape can be placed at board[baseR, baseC] 
    /// by shifting shape.Cells so that the shape's "anchor" 
    /// or first cell covers (baseR, baseC).
    /// 
    /// If your shapes are absolute offsets or you want to 
    /// place them so that (0,0) is the anchor, you might do:
    ///   for each cell in shape.Cells => 
    ///       int rr = baseR + cell.x, cc = baseC + cell.y
    /// </summary>
    
    private static bool CanPlaceShape(int[,] board, int baseR, int baseC, ShapeDefinition shape)
    {
        int rows = board.GetLength(0);
        int cols = board.GetLength(1);

        foreach (var offset in shape.Cells)
        {
            int rr = baseR + offset.x;
            int cc = baseC + offset.y;
            // Out of bounds?
            if (rr < 0 || rr >= rows || cc < 0 || cc >= cols)
                return false;

            // Not empty?
            if (board[rr, cc] != -1)
                return false;
        }
        return true;
    }

    /// <summary>
    /// Places 'shape' at board[baseR, baseC] using 'colorIndex'. 
    /// (ShapeDefinition has only geometry; color must be provided.)
    /// </summary>
    private static void PlaceShape(int[,] board, int baseR, int baseC,
                                   ShapeDefinition shape, int colorIndex)
    {
        foreach (var offset in shape.Cells)
        {
            int rr = baseR + offset.x;
            int cc = baseC + offset.y;
            board[rr, cc] = colorIndex;
        }
    }

    /// <summary>
    /// Unplaces (removes) 'shape' from the board by restoring 
    /// its squares to -1 (empty).
    /// </summary>
    private static void UnplaceShape(int[,] board, int baseR, int baseC, ShapeDefinition shape)
    {
        foreach (var offset in shape.Cells)
        {
            int rr = baseR + offset.x;
            int cc = baseC + offset.y;
            board[rr, cc] = -1;
        }
    }

    

}
