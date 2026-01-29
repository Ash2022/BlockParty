using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlayerSavedData 
{
    public int lastCompletedLevel;
    public bool allLevelsComplete = false;

    public PlayerSavedData()
    {
        lastCompletedLevel = -1;
        
        allLevelsComplete = false ;
    }

    // Save to JSON
    public static void SaveToJson(string filePath,PlayerSavedData playerSavedData)
    {
        string json = JsonUtility.ToJson(playerSavedData, true); // Serialize object to JSON string (pretty print)
        File.WriteAllText(filePath, json); // Write JSON string to file
    }

    // Load from JSON
    public static PlayerSavedData LoadFromJson(string filePath)
    {
        if (File.Exists(filePath)) // Check if file exists
        {
            string json = File.ReadAllText(filePath); // Read JSON string from file
            return JsonUtility.FromJson<PlayerSavedData>(json); // Deserialize JSON string to object
        }
        else
        {
            Debug.LogWarning("Save file not found: " + filePath);
            return new PlayerSavedData(); // Return default data if file not found
        }
    }
}
