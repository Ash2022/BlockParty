using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static ShapesDB;

public class ShapeStorage : MonoBehaviour
{
    
    public List<ShapeView> shapeList;
    [SerializeField] GameObject _shapePrefab;
   
    ShapesDB shapesDB;

    [SerializeField]List<ShapeHolderView> lineHolders = new List<ShapeHolderView>();

    float shapeStorageScale = 0;

    // Initialize the seeded random number generator
    System.Random rng;

    public ShapesDB ShapesDB { get => shapesDB; set => shapesDB = value; }

    //this array tells which shapes data can be used for the level
    public void InitStorageParams()
    {


        shapesDB = new ShapesDB();

        shapesDB.Init();

    }

    public List<ShapeView> GetShapesList()
    {
        return shapeList;
    }

    public void GenerateNewShapes(List<ShapeInfo> shapesInfoList, bool isBuildingLevel, bool isPredefinedShapes, float _shapeStorageScale = 0)
    {
        if (_shapeStorageScale != 0)
            shapeStorageScale = _shapeStorageScale;

        foreach (ShapeView shapeView in shapeList)
            Destroy(shapeView.gameObject);

        shapeList.Clear();

        //Debug.Log("Generate New Shapes");

        //generate the current new shapes into the bottom holders

        for (int i = 0; i < shapesInfoList.Count; i++)
        {
            if (shapesInfoList[i].ShapeName == "Null")
            {
                shapeList.Add(null);
                continue;
            }

            GameObject newShape = Instantiate(_shapePrefab, lineHolders[i].transform);
            ShapeView shapeView = newShape.GetComponent<ShapeView>();
            shapeView.ShapeRectTransform.localScale = Vector3.zero;
            shapeList.Add(shapeView);

            lineHolders[i].InitShapeHolder(shapeView.MouseDownUp);

            //make sure the shape color we are creating belongs to a person in the queue that is alive
            //if not - pick a different color

            int confirmedColorIndex = shapesInfoList[i].ShapeColor;

            if (!isPredefinedShapes)
                confirmedColorIndex = GameManager.Instance.CheckIfMyColorExistsInQueues(shapesInfoList[i].ShapeColor, rng);

            shapeList[i].BuildShapeFromDefinition(GetShapeByName(shapesInfoList[i].ShapeName), confirmedColorIndex, shapeStorageScale);

            if (isBuildingLevel)
                shapeView.ShapeRectTransform.localScale = Vector3.zero;
            else
                shapeView.ShapeRectTransform.DOScale(Vector3.one * shapeStorageScale, 0.25f);

        }

        if (shapeList[2] == null)
            shapeList.RemoveAt(2);

        if (shapeList[1] == null)
            shapeList.RemoveAt(1);

        if (shapeList[0] == null)
            shapeList.RemoveAt(0);
    }

    public async void ShowShapesEnterAnimation()
    {
        await Task.Delay(350);

        for (int i = 0; i < shapeList.Count; i++)
        {
            if(shapeList[i] == null) continue;

            shapeList[i].ShapeRectTransform.DOScale(Vector3.one * shapeStorageScale, 0.25f);
            await Task.Delay(150);
        }
    }

    public void ShowShapesExitAnimation()
    {
        for (int i = 0; i < shapeList.Count; i++)
            shapeList[i].ShapeRectTransform.DOScale(Vector3.zero, 0.25f);
    }


    public ShapeDefinition GetShapeByName(string name)
    {
        ShapeDefinition sd =  shapesDB.GetShapeDefinitionByName(name);

        if(sd==null)
            Debug.Log("Cant find shape: " + name);

        return sd;
    }

    //the bool returns that shapes are empty and need new ones
    internal bool ShapeWasPositioned(ShapeView shape)
    {
        shapeList.Remove(shape);
        Destroy(shape.gameObject);

        return shapeList.Count == 0;
    }

    internal void SetRandomSeed(int randomSeed)
    {
        rng = new System.Random(randomSeed);
    }

    internal (GameObject,int) GetTutorialShape()
    {
        return (shapeList[0].gameObject, shapeList[0].ColorIndex);
    }
}
