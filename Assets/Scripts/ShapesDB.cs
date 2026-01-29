using System.Collections.Generic;
using UnityEngine;

public class ShapesDB
{
    /// <summary>
    /// A simple structure holding the shape's name and its list of cell offsets.
    /// </summary>
    public class ShapeDefinition
    {
        public string Name;                     // e.g. "Line3_Horizontal"
        public List<Vector2Int> Cells;          // The list of (row,col) offsets

        public ShapeDefinition(string name, List<Vector2Int> cells)
        {
            Name = name;
            Cells = cells;
        }

        public int GetNumberOfRows()
        {
            int minRow = int.MaxValue;
            int maxRow = int.MinValue;

            foreach (var cell in Cells)
            {
                minRow = Mathf.Min(minRow, cell.x);
                maxRow = Mathf.Max(maxRow, cell.x);
            }

            return maxRow - minRow + 1;
        }

        // Method to calculate the number of columns in the shape
        public int GetNumberOfColumns()
        {
            int minCol = int.MaxValue;
            int maxCol = int.MinValue;

            foreach (var cell in Cells)
            {
                minCol = Mathf.Min(minCol, cell.y);
                maxCol = Mathf.Max(maxCol, cell.y);
            }

            return maxCol - minCol + 1;
        }
    }

    // We store shapes in a dictionary keyed by group ID (number of units).
    // For example: shapeGroups[2] => all shapes made of 2 units, etc.
    public Dictionary<int, List<ShapeDefinition>> shapeGroups
        = new Dictionary<int, List<ShapeDefinition>>();

    /// <summary>
    /// Public accessor for shape groups.
    /// Key is the unit count, value is the list of shape definitions.
    /// </summary>
    public Dictionary<int, List<ShapeDefinition>> ShapeGroups => shapeGroups;

    public List<ShapeDefinition> AllShapes { get => allShapes; set => allShapes = value; }

    List<ShapeDefinition> allShapes = new List<ShapeDefinition>();

    /// <summary>
    /// Call this once to build the shape database and print out summaries.
    /// </summary>
    public void Init()
    {
        shapeGroups.Clear();

        // We'll create group entries for 1_2 => use group ID=2 to store them,
        // or you can store them with separate IDs (like group ID=12?). 
        // For clarity, let's store them under 1 and 2:
        shapeGroups[1] = new List<ShapeDefinition>();
        shapeGroups[2] = new List<ShapeDefinition>();
        shapeGroups[3] = new List<ShapeDefinition>();
        shapeGroups[4] = new List<ShapeDefinition>();
        shapeGroups[5] = new List<ShapeDefinition>();

        // 1) Build 1-cube shapes
        BuildGroup1();
        // 2) Build 2-cube shapes
        BuildGroup2();
        // 3) Build 3-unit shapes
        BuildGroup3();

        BuildGroup4();

        BuildGroup5();

        string allShapesList="";

        // Print a summary of each group
        foreach (var kvp in shapeGroups)
        {
            int groupCount = kvp.Key;         // e.g. 1,2,3,4,5
            List<ShapeDefinition> shapes = kvp.Value;

            // Count how many total shapes in that group
            Debug.Log($"Group {groupCount} => {shapes.Count} shapes");
            // Also print each shape name
            foreach (var shape in shapes)
            {
                Debug.Log($"   {shape.Name} => {OffsetsToString(shape.Cells)}");

                allShapesList += shape.Name + ",";

                allShapes.Add( shape );
            }
        }

        Debug.Log(allShapesList);
    }

    /// <summary>
    /// Helper for printing cell offsets in logs
    /// </summary>
    private string OffsetsToString(List<Vector2Int> cells)
    {
        // e.g. "(0,0)(0,1)"
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        foreach (var c in cells)
        {
            sb.Append($"({c.x},{c.y})");
        }
        return sb.ToString();
    }

    #region Build Group 1 & 2
    private void BuildGroup1()
    {
        // Single cube => (0,0)
        // We'll name it "SingleCube"
        var single = new ShapeDefinition("SingleCube", new List<Vector2Int>
        {
            new Vector2Int(0,0)
        });
        shapeGroups[1].Add(single);
    }

    private void BuildGroup2()
    {
        // 2 cubes. Base shape: horizontal => (0,0), (0,1)
        // Rotate 90 => (0,0), (1,0)
        // So we have 2 permutations plus the single-cube shape is in group1.
        // But you asked to combine "1_2" as a group that has 3 elements total:
        //   1-cube shape + 2-cube horizontal + 2-cube vertical.
        // For clarity, let's store the 2-cube shapes here in group=2.

        // horizontal
        var shapeH = new ShapeDefinition("2Line_Hor", new List<Vector2Int>
        {
            new Vector2Int(0,0), new Vector2Int(0,1)
        });
        shapeGroups[2].Add(shapeH);

        // vertical
        var shapeV = new ShapeDefinition("2Line_Ver", new List<Vector2Int>
        {
            new Vector2Int(0,0), new Vector2Int(1,0)
        });
        shapeGroups[2].Add(shapeV);
    }
    #endregion

    #region Build Group 3
    private void BuildGroup3()
    {
        // You said:
        //   2 base shapes => line of 3, and L shape of 3
        //   line of 3 => 2 permutations (horizontal, vertical)
        //   L shape => 4 permutations

        // 1) 3-line horizontal => (0,0),(0,1),(0,2)
        //    3-line vertical   => (0,0),(1,0),(2,0)
        var lineH3 = new ShapeDefinition("3Line_Hor", new List<Vector2Int>
        {
            new Vector2Int(0,0),
            new Vector2Int(0,1),
            new Vector2Int(0,2)
        });
        shapeGroups[3].Add(lineH3);

        var lineV3 = new ShapeDefinition("3Line_Ver", new List<Vector2Int>
        {
            new Vector2Int(0,0),
            new Vector2Int(1,0),
            new Vector2Int(2,0)
        });
        shapeGroups[3].Add(lineV3);

        // 2) L shape => base shape (0,0),(1,0),(0,1)
        // We'll generate the 4 rotations by rotating around (0,0) or
        // more systematically by a rotation function.

        // base shape
        var baseL = new List<Vector2Int>
        {
            new Vector2Int(0,0),
            new Vector2Int(1,0),
            new Vector2Int(0,1)
        };

        // create 4 rotated versions
        // We can define a helper method => Rotate90
        var shapesL = GenerateAllRotationsAndMirror(baseL, "3LShape");
        // shapesL has up to 4 distinct shape offsets

        // Add them to group 3
        foreach (var sdef in shapesL)
        {
            shapeGroups[3].Add(sdef);
        }
    }
    #endregion

    private void BuildGroup4()
    {

        //   line of 4 => 2 permutations (horizontal, vertical)

        //   Cube of 4 => 1 permutaion

        //   Plus shape => 4 permutations
        //   Z shape => 4 permutations
        //   L shape => 4 permutations
        //   Mirror L shape => 4 permutations

        // 1) 3-line horizontal => (0,0),(0,1),(0,2)
        //    3-line vertical   => (0,0),(1,0),(2,0)
        var lineH4 = new ShapeDefinition("4Line_Hor", new List<Vector2Int>
        {
            new Vector2Int(0,0),
            new Vector2Int(0,1),
            new Vector2Int(0,2),
            new Vector2Int(0,3)
        });
        shapeGroups[4].Add(lineH4);

        var lineV4 = new ShapeDefinition("4Line_Ver", new List<Vector2Int>
        {
            new Vector2Int(0,0),
            new Vector2Int(1,0),
            new Vector2Int(2,0),
            new Vector2Int(3,0)
        });
        shapeGroups[4].Add(lineV4);

        //cube 
        var Cube = new ShapeDefinition("4Cube", new List<Vector2Int>
        {
            new Vector2Int(0,0),
            new Vector2Int(1,0),
            new Vector2Int(0,1),
            new Vector2Int(1,1)
        });
        shapeGroups[4].Add(Cube);

        // 2) Plus shape => base shape (0,0),(1,0),(2,0),(1,1)
        // We'll generate the 4 rotations by rotating around (0,0) or
        // more systematically by a rotation function.

        // base shape - PLUS
        var basePlus = new List<Vector2Int>
        {
            new Vector2Int(0,0),
            new Vector2Int(1,0),
            new Vector2Int(1,1),
            new Vector2Int(2,0),
        };

        // create 4 rotated versions
        // We can define a helper method => Rotate90
        var basePlusShape = GenerateAllRotationsAndMirror(basePlus, "4basePlus");
        // shapesL has up to 4 distinct shape offsets

        // Add them to group 4
        foreach (var sdef in basePlusShape)
            shapeGroups[4].Add(sdef);

        // base shape - L
        var baseL = new List<Vector2Int>
        {
            new Vector2Int(0,0),
            new Vector2Int(0,1),
            new Vector2Int(0,2),
            new Vector2Int(1,0),
        };

        // create 4 rotated versions
        // We can define a helper method => Rotate90
        var baseLShape = GenerateAllRotationsAndMirror(baseL, "4baseL");
        // shapesL has up to 4 distinct shape offsets

        // Add them to group 3
        foreach (var sdef in baseLShape)
            shapeGroups[4].Add(sdef);

        // base shape - L mirror
        var baseLMirror = new List<Vector2Int>
        {
            new Vector2Int(1,0),
            new Vector2Int(1,1),
            new Vector2Int(1,2),
            new Vector2Int(0,0),
        };

        // create 4 rotated versions
        // We can define a helper method => Rotate90
        var baseLMirrorShape = GenerateAllRotationsAndMirror(baseLMirror, "4baseLMirror");
        // shapesL has up to 4 distinct shape offsets

        // Add them to group 3
        foreach (var sdef in baseLMirrorShape)
            shapeGroups[4].Add(sdef);


        // base shape - Z 
        var baseZ = new List<Vector2Int>
        {
            new Vector2Int(0,0),
            new Vector2Int(1,0),
            new Vector2Int(1,1),
            new Vector2Int(2,1),
        };

        // create 4 rotated versions
        // We can define a helper method => Rotate90
        var baseZShape = GenerateAllRotationsAndMirror(baseZ, "4baseZ");
        // shapesL has up to 4 distinct shape offsets

        // Add them to group 3
        foreach (var sdef in baseZShape)
            shapeGroups[4].Add(sdef);

    }

    #region Possibly Build Group 4 & 5
    // TODO: Similarly define BuildGroup4() and BuildGroup5(), 
    // enumerating base shapes (like squares, T-shapes, L-shapes, lines, S-shapes, etc.)
    #endregion

    private void BuildGroup5()
    {
        // 1) F => base shape horizontal
        //    (0, 0), (1, 0), (1, 1), (2, 1), (1, 2)
        // Rotations will yield vertical, etc.
        var F = new List<Vector2Int>
        {
            new Vector2Int(0,0),
            new Vector2Int(1,0),
            new Vector2Int(1,1),
            new Vector2Int(2,1),
            new Vector2Int(1,2)
        };
        var fShape = GenerateAllRotationsAndMirror(F, "5F");
        shapeGroups[5].AddRange(fShape);

        // 1) I => base shape horizontal
        //   (0, 0), (0, 1), (0, 2), (0, 3), (0, 4)
        // Rotations will yield vertical, etc.
        var I = new List<Vector2Int>
        {
            new Vector2Int(0,0),
            new Vector2Int(0,1),
            new Vector2Int(0,2),
            new Vector2Int(0,3),
            new Vector2Int(0,4)
        };
        var iShape = GenerateAllRotationsAndMirror(I, "5I");
        shapeGroups[5].AddRange(iShape);

        // 1) L => base shape horizontal
        //   (0, 0), (0, 1), (0, 2), (0, 3), (1, 3)
        // Rotations will yield vertical, etc.
        var L = new List<Vector2Int>
        {
            new Vector2Int(0,0),
            new Vector2Int(0,1),
            new Vector2Int(0,2),
            new Vector2Int(0,3),
            new Vector2Int(1,3)
        };
        var lShape = GenerateAllRotationsAndMirror(L, "5L");
        shapeGroups[5].AddRange(lShape);

        // 1) N => base shape horizontal
        //   (0, 0), (0, 1), (1, 1), (1, 2), (1, 3)
        // Rotations will yield vertical, etc.
        var N = new List<Vector2Int>
        {
            new Vector2Int(0,0),
            new Vector2Int(0,1),
            new Vector2Int(1,1),
            new Vector2Int(1,2),
            new Vector2Int(1,3)
        };
        var nShape = GenerateAllRotationsAndMirror(N, "5N");
        shapeGroups[5].AddRange(nShape);

        // 1) P => base shape horizontal
        //   (0, 0), (1, 0), (0, 1), (1, 1), (0, 2)
        // Rotations will yield vertical, etc.
        var P = new List<Vector2Int>
        {
            new Vector2Int(0,0),
            new Vector2Int(1,0),
            new Vector2Int(0,1),
            new Vector2Int(1,1),
            new Vector2Int(0,2)
        };
        var pShape = GenerateAllRotationsAndMirror(P, "5P");
        shapeGroups[5].AddRange(pShape);

        // 1) T => base shape horizontal
        //   (0, 0), (1, 0), (2, 0), (1, 1), (1, 2)
        // Rotations will yield vertical, etc.
        var T = new List<Vector2Int>
        {
            new Vector2Int(0,0),
            new Vector2Int(1,0),
            new Vector2Int(2,0),
            new Vector2Int(1,1),
            new Vector2Int(1,2)
        };
        var tShape = GenerateAllRotationsAndMirror(T, "5T");
        shapeGroups[5].AddRange(tShape);

        // 1) U => base shape horizontal
        //   (0, 0), (1, 0), (2, 0), (0, 1), (2, 1)
        // Rotations will yield vertical, etc.
        var U = new List<Vector2Int>
        {
            new Vector2Int(0,0),
            new Vector2Int(1,0),
            new Vector2Int(2,0),
            new Vector2Int(0,1),
            new Vector2Int(2,1)
        };
        var uShape = GenerateAllRotationsAndMirror(U, "5U");
        shapeGroups[5].AddRange(uShape);

        // 1) V => base shape horizontal
        //   (0, 0), (0, 1), (0, 2), (1, 2), (2, 2)
        // Rotations will yield vertical, etc.
        var V = new List<Vector2Int>
        {
            new Vector2Int(0,0),
            new Vector2Int(0,1),
            new Vector2Int(0,2),
            new Vector2Int(1,2),
            new Vector2Int(2,2)
        };
        var vShape = GenerateAllRotationsAndMirror(V, "5V");
        shapeGroups[5].AddRange(vShape);

        // 1) W => base shape horizontal
        //   (0, 0), (0, 1), (1, 1), (1, 2), (2, 2)
        // Rotations will yield vertical, etc.
        var W = new List<Vector2Int>
        {
            new Vector2Int(0,0),
            new Vector2Int(0,1),
            new Vector2Int(1,1),
            new Vector2Int(1,2),
            new Vector2Int(2,2)
        };
        var wShape = GenerateAllRotationsAndMirror(W, "5W");
        shapeGroups[5].AddRange(wShape);

        // 1) X => base shape horizontal
        //   (1, 0), (0, 1), (1, 1), (2, 1), (1, 2)
        // Rotations will yield vertical, etc.
        var X = new List<Vector2Int>
        {
            new Vector2Int(1,0),
            new Vector2Int(0,1),
            new Vector2Int(1,1),
            new Vector2Int(2,1),
            new Vector2Int(1,2)
        };
        var xShape = GenerateAllRotationsAndMirror(X, "5X");
        shapeGroups[5].AddRange(xShape);

        // 1) Y => base shape horizontal
        //   (0, 0), (1, 0), (2, 0), (3, 0), (2, 1)
        // Rotations will yield vertical, etc.
        var Y = new List<Vector2Int>
        {
            new Vector2Int(0,0),
            new Vector2Int(1,0),
            new Vector2Int(2,0),
            new Vector2Int(3,0),
            new Vector2Int(2,1)
        };
        var yShape = GenerateAllRotationsAndMirror(Y, "5Y");
        shapeGroups[5].AddRange(yShape);

        // 1) Z => base shape horizontal
        //   (0, 0), (1, 0), (1, 1), (2, 1), (2, 2)
        // Rotations will yield vertical, etc.
        var Z = new List<Vector2Int>
        {
            new Vector2Int(0,0),
            new Vector2Int(1,0),
            new Vector2Int(1,1),
            new Vector2Int(2,1),
            new Vector2Int(2,2)
        };
        var zShape = GenerateAllRotationsAndMirror(Z, "5Z");
        shapeGroups[5].AddRange(zShape);
    }


    #region Helper: GenerateAllRotations
    /// <summary>
    /// Generates up to 8 distinct transformations: 4 rotations of the base shape,
    /// plus 4 rotations of its mirrored version. Each is normalized and deduplicated.
    /// </summary>
    private List<ShapeDefinition> GenerateAllRotationsAndMirror(List<Vector2Int> baseShape, string namePrefix)
    {
        List<ShapeDefinition> result = new List<ShapeDefinition>();

        // Used to detect and skip duplicates
        HashSet<string> usedSignatures = new HashSet<string>();

        // Generate original shape's rotations
        for (int i = 0; i < 4; i++)
        {
            int angle = 90 * i;
            var rotated = RotateShape(baseShape, angle);
            var normalized = NormalizeShape(rotated);

            string signature = MakeSignature(normalized);
            if (!usedSignatures.Contains(signature))
            {
                usedSignatures.Add(signature);
                string shapeName = $"{namePrefix}_R{angle}";
                result.Add(new ShapeDefinition(shapeName, normalized));
            }
        }

        // Generate mirrored shape's rotations
        // We'll define mirror as flipping X => (x,y)->(-x,y) around x=0
        var mirroredBase = MirrorShapeX(baseShape);

        for (int i = 0; i < 4; i++)
        {
            int angle = 90 * i;
            var rotated = RotateShape(mirroredBase, angle);
            var normalized = NormalizeShape(rotated);

            string signature = MakeSignature(normalized);
            if (!usedSignatures.Contains(signature))
            {
                usedSignatures.Add(signature);
                string shapeName = $"{namePrefix}_M_R{angle}";
                result.Add(new ShapeDefinition(shapeName, normalized));
            }
        }

        return result;
    }

    /// <summary>
    /// Mirrors the shape horizontally around x=0. 
    /// i.e. each (x,y) => (-x,y).
    /// </summary>
    private List<Vector2Int> MirrorShapeX(List<Vector2Int> shape)
    {
        List<Vector2Int> mirrored = new List<Vector2Int>();
        foreach (var c in shape)
        {
            // flip x
            mirrored.Add(new Vector2Int(-c.x, c.y));
        }
        return mirrored;
    }

    /// <summary>
    /// Rotate shape by 'deg' degrees around (0,0).
    ///  deg=90 => (x,y) => (y, -x)
    ///  deg=180=> (x,y) => (-x, -y)
    ///  deg=270=> (x,y) => (-y, x)
    ///  deg=0  => no change
    /// </summary>
    private List<Vector2Int> RotateShape(List<Vector2Int> cells, int deg)
    {
        List<Vector2Int> result = new List<Vector2Int>();
        foreach (var c in cells)
        {
            int x = c.x;
            int y = c.y;
            if (deg == 90)
                result.Add(new Vector2Int(y, -x));
            else if (deg == 180)
                result.Add(new Vector2Int(-x, -y));
            else if (deg == 270)
                result.Add(new Vector2Int(-y, x));
            else // deg=0
                result.Add(new Vector2Int(x, y));
        }
        return result;
    }

    /// <summary>
    /// Normalize shape so that top-left bounding box corner becomes (0,0).
    /// i.e. find minX,minY among all cells, subtract them from each cell.
    /// Then we can sort for a stable ordering.
    /// </summary>
    private List<Vector2Int> NormalizeShape(List<Vector2Int> shape)
    {
        if (shape.Count == 0) return shape;

        int minX = int.MaxValue;
        int minY = int.MaxValue;
        foreach (var c in shape)
        {
            if (c.x < minX) minX = c.x;
            if (c.y < minY) minY = c.y;
        }

        List<Vector2Int> result = new List<Vector2Int>();
        foreach (var c in shape)
        {
            result.Add(new Vector2Int(c.x - minX, c.y - minY));
        }

        // sort for consistent signature
        result.Sort((a, b) => a.x == b.x ? a.y.CompareTo(b.y) : a.x.CompareTo(b.x));
        return result;
    }

    /// <summary>
    /// Creates a string signature from shape cells, e.g. "(0,0)(0,1)..."
    /// for deduplication.
    /// </summary>
    private string MakeSignature(List<Vector2Int> shapeCells)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        foreach (var c in shapeCells)
        {
            sb.Append($"({c.x},{c.y})");
        }
        return sb.ToString();
    }
    #endregion

    public ShapeDefinition GetShapeDefinitionByName(string name)
    {
        // Print a summary of each group
        foreach (var kvp in shapeGroups)
        {
            int groupCount = kvp.Key;         // e.g. 1,2,3,4,5
            List<ShapeDefinition> shapes = kvp.Value;

            // Count how many total shapes in that group
            //Debug.Log($"Group {groupCount} => {shapes.Count} shapes");
            // Also print each shape name
            foreach (var shape in shapes)
            {
                if(shape.Name == name)
                    return shape;
            }
        }

        return null;
    }

    public string GetRandomShapeNameFromGroup(int groupIndex)
    {
        switch (groupIndex)
        {
            case 0:
                {
                    //return random shape from group 1 or 2
                    //LevelRNG.Rng.Next(0, group1_2.Count)

                    int index = LevelRNG.Rng.Next(0, 3);

                    if (index == 0)
                        return "SingleCube";
                    if (index == 1)
                        return "2Line_Hor";
                    if (index == 2)
                        return "2Line_Ver";

                    return "SingleCube";
                }

            case 1:
                {
                    //return random shape from group 1 or 2
                    //LevelRNG.Rng.Next(0, group1_2.Count)

                    int index = LevelRNG.Rng.Next(0, shapeGroups[3].Count);

                    return shapeGroups[3][index].Name;
                }
            case 2:
                {
                    //return random shape from group 1 or 2
                    //LevelRNG.Rng.Next(0, group1_2.Count)

                    int index = LevelRNG.Rng.Next(0, shapeGroups[4].Count);

                    return shapeGroups[4][index].Name;
                }
            case 3:
                {
                    //return random shape from group 1 or 2
                    //LevelRNG.Rng.Next(0, group1_2.Count)

                    int index = LevelRNG.Rng.Next(0, shapeGroups[5].Count);

                    return shapeGroups[5][index].Name;
                }

            default:
                return "SingleCube";
        }
    }
}
