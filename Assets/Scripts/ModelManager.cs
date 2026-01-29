using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class ModelManager : MonoBehaviour
{
    public Color fullColor = new Color(0.75f,0,0,1);
    public Color aviableColor = new Color(0, 0.75f, 0, 1);

    public static ModelManager Instance;

    public List<LevelData> levelsData = new List<LevelData>();

    public List<TextAsset> levelsFromFiles;

    public TextAsset mutipleLevelsAsset;

    [SerializeField] Transform _levelsHolderTransform;

    [SerializeField]List<Color> colors = new List<Color>();

    [SerializeField] List<Color> secondaryParticleColor = new List<Color>();

    [SerializeField] List<Sprite> blockImages = new List<Sprite>();

    [SerializeField] List<Sprite> person0Images = new List<Sprite>();
    [SerializeField] List<Sprite> person1Images = new List<Sprite>();
    [SerializeField] List<Sprite> person2Images = new List<Sprite>();
    [SerializeField] List<Sprite> person3Images = new List<Sprite>();
    [SerializeField] List<Sprite> person4Images = new List<Sprite>();
    [SerializeField] List<Sprite> person5Images = new List<Sprite>();
    [SerializeField] List<Sprite> person6Images = new List<Sprite>();

    [SerializeField]List<Sprite> comboImages = new List<Sprite>();
    

    List<int> unlocksIndexList = new List<int>();

    public bool _useMultiLevelFile;

    public List<int> UnlocksIndexList { get => unlocksIndexList; set => unlocksIndexList = value; }

    PlayerSavedData playerSavedData;
    string filePath;

    private void Awake()
    {
        Instance = this;
    }

    public void BuildLevels(bool newUser)
    {
        filePath = Path.Combine(Application.persistentDataPath, "PlayerData.json");

        //BuildLevel2();
        /*
        foreach (Transform levelTrans in _levelsHolderTransform)
        {
            LevelData levelData = levelTrans.gameObject.GetComponent<LevelEditorView>().BuildLevelData();
            levelsData.Add(levelData);
        }*/

        unlocksIndexList.Add(10);//4 colors
        unlocksIndexList.Add(15);//5 colors
        //unlocksIndexList.Add(26);//6 colors

        if (_useMultiLevelFile)
        {
            levelsData.AddRange(LevelData.ParseMultipleLevelsDataFromString(mutipleLevelsAsset.ToString()));

        }
        else
        {
            foreach (var levelData in levelsFromFiles)
                levelsData.Add(LevelData.ParseLevelDataFromString(levelData.ToString()));
        }

        if (newUser)
        {
            playerSavedData = new PlayerSavedData();
            PlayerSavedData.SaveToJson(filePath, playerSavedData);
        }
        else
        {
            playerSavedData = PlayerSavedData.LoadFromJson(filePath);
        }

        //playerSavedData.allLevelsComplete = false;
        //playerSavedData.lastCompletedLevel = 0;

    }

    public bool AllLevelsComplete()
    {
        return playerSavedData.allLevelsComplete;
    }

    public int GetCurrentLevelIndex()
    {
        return playerSavedData.lastCompletedLevel + 1;

        //return Math.Min(playerSavedData.lastCompletedLevel+1,levelsData.Count-1);
    }

    public void SetLevelComplete(int levelIndex)
    {
        if (levelIndex == levelsData.Count - 1)
            playerSavedData.allLevelsComplete = true;

        playerSavedData.lastCompletedLevel = levelIndex;

        PlayerSavedData.SaveToJson(filePath, playerSavedData);

    }

    
    public Color GetColorByColorIndex(int colorIndex)
    {
        return colors[colorIndex];       
    }

    public Sprite GetImageByColorIndex(int colorIndex)
    {
        return blockImages[colorIndex];
    }

    public List<Sprite> GetPersonImagesByColorIndex(int colorIndex)
    {
        switch (colorIndex)
        {
            case 0:
                return person0Images;
            case 1:
                return person1Images;
            case 2:
                return person2Images;
            case 3:
                return person3Images;
            case 4:
                return person4Images;
            case 5:
                return person5Images;
            case 6:
                return person6Images;

            default:
                return person0Images;
        }
    }

    public Color GetSecondaryParticleColor(int colorIndex)
    {
        return secondaryParticleColor[colorIndex];
    }

    public string GetColorNameByIndex(int colorIndex)
    {
        switch (colorIndex)
        {
            case 0:
                return "GREEN";
                case 1:
                return "BLUE";
                case 2:
                return "YELLOW";
                case 3:
                return "RED";
                case 4:
                return "PURPLE";
                case 5:
                return "ORANGE";


            default:
                return "BLANK";
        }
    }

    internal int GetUnlock(int currLevelIndex)
    {
        int index = -1;

        if (unlocksIndexList.Contains(currLevelIndex))
            index = unlocksIndexList.FindIndex(x=>x.Equals(currLevelIndex));
        
        return index;
    }

    public Sprite GetComboImageByIndex(int index)
    {
        return comboImages[index];
    }

}
