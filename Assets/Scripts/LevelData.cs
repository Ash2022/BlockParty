using System;
using System.Collections.Generic;
using Newtonsoft.Json;


/// <summary>
/// Represents the data structure of a game level as defined in the JSON.
/// </summary>
public class LevelData
{
    public LevelData(int rows, int columns)
    {
        Rows = rows;
        Columns = columns;
    }

    [JsonProperty("cols")]
    public int Columns { get; set; }

    [JsonProperty("rows")]
    public int Rows { get; set; }

    [JsonProperty("peopleQueues")]
    public List<List<int>> PeopleQueues { get; set; } = new List<List<int>>();

    [JsonProperty("shapesLines")]
    public List<ShapeInfo> ShapesLines { get; set; } = new List<ShapeInfo>();

    [JsonProperty("randomSeed")]
    public int RandomSeed { get; set; }

    [JsonProperty("tutorial")]
    public bool Tutorial { get; set; }

    [JsonProperty("ExcludeShape5")]
    public bool ExcludeShape5 { get; set; }

    [JsonIgnore]
    public int NumberOfColors { get; set; }

    [JsonProperty("board")]
    public List<List<int>> Board { get; set; } = new List<List<int>>();


    /// <summary>
    /// Parses a JSON string and converts it into a LevelData object.
    /// </summary>
    /// <param name="jsonString">The JSON string representing the level.</param>
    /// <returns>An instance of LevelData populated with data from the JSON string.</returns>
    public static LevelData ParseLevelDataFromString(string jsonString)
    {
        if (string.IsNullOrWhiteSpace(jsonString))
        {
            throw new ArgumentException("The JSON string provided is null or empty.", nameof(jsonString));
        }

        LevelData levelData;
        try
        {
            levelData = JsonConvert.DeserializeObject<LevelData>(jsonString);
        }
        catch (JsonException ex)
        {
            throw new JsonException("Failed to deserialize the level JSON string. Please ensure the JSON format is correct.", ex);
        }

        if (levelData == null)
        {
            throw new Exception("Deserialization resulted in a null LevelData object.");
        }

        // Calculate NumberOfColors based on PeopleQueues
        levelData.NumberOfColors = CalculateNumberOfColors(levelData.PeopleQueues);

        return levelData;
    }

    /// </summary>
    /// <param name="jsonString">The JSON string containing multiple levels.</param>
    /// <returns>A List of LevelData populated with data from the JSON string.</returns>
    public static List<LevelData> ParseMultipleLevelsDataFromString(string jsonString)
    {
        if (string.IsNullOrWhiteSpace(jsonString))
        {
            throw new ArgumentException("The JSON string provided is null or empty.", nameof(jsonString));
        }

        // We expect a wrapper object containing an array called "levels".
        // e.g. { "levels": [ {...}, {...}, ... ] }
        MultipleLevelsWrapper wrapper;
        try
        {
            wrapper = JsonConvert.DeserializeObject<MultipleLevelsWrapper>(jsonString);
        }
        catch (JsonException ex)
        {
            throw new JsonException("Failed to deserialize the levels JSON string. Please ensure the JSON format is correct.", ex);
        }

        if (wrapper?.Levels == null)
        {
            throw new Exception("Deserialization resulted in null or missing 'levels' array.");
        }

        // Calculate NumberOfColors for each level
        foreach (var levelData in wrapper.Levels)
        {
            levelData.NumberOfColors = CalculateNumberOfColors(levelData.PeopleQueues);
        }

        return wrapper.Levels;
    }

    /// <summary>
    /// Calculates the number of distinct colors from the people queues.
    /// </summary>
    /// <param name="peopleQueues">The list of people queues.</param>
    /// <returns>The count of distinct color indices.</returns>
    private static int CalculateNumberOfColors(List<List<int>> peopleQueues)
    {

        int highestColorIndex = 0;

        foreach (var queue in peopleQueues)
        {
            foreach (var color in queue)
            {
                if (color > highestColorIndex)
                    highestColorIndex = color;
            }
        }

        return highestColorIndex + 1;
        /*
            HashSet<int> distinctColors = new HashSet<int>();

            foreach (var queue in peopleQueues)
            {
                foreach (var color in queue)
                {
                    distinctColors.Add(color);
                }
            }

            return distinctColors.Count;
        }
        */

    }
}

public class MultipleLevelsWrapper
{
    [JsonProperty("levels")]
    public List<LevelData> Levels { get; set; }
}

/// <summary>
/// Information about each shape line used in the level.
/// </summary>

/*

public class ShapeInfo
{
    [JsonProperty("shapeName")]
    public string ShapeName { get; set; }
    [JsonProperty("shapeColor")]
    public int ShapeColor { get; set; }

    public ShapeInfo(string name, int color)
    {
        ShapeName = name;
        ShapeColor = color;
    }
}

*/



