using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEditorView : MonoBehaviour
{
    [SerializeField] int rows;
    [SerializeField] int columns;

    [SerializeField] Transform pplQueuesHolder;
    [SerializeField] Transform shapesQueuesHolder;

    [SerializeField] bool shapes_1_2 = true;
    [SerializeField] bool shapes_3 = true;
    [SerializeField] bool shapes_4 = true;
    [SerializeField] bool shapes_5 = true;


    public LevelData BuildLevelData()
    {
        LevelData level1 = new LevelData(rows, columns);


        foreach (Transform queue in pplQueuesHolder)
        {
            List<int> pplQueue = new List<int>();

            foreach (Transform queuePosHolders in queue)
            {
                if (queuePosHolders.childCount > 0)
                    pplQueue.Add((int)queuePosHolders.GetChild(0).gameObject.GetComponent<ColorSelector>().CurrentColor);

            }
            if(pplQueue.Count > 0)
                level1.PeopleQueues.Add(pplQueue);
        }

        List<ShapeInfo> shapeQueue = new List<ShapeInfo>();
        
        foreach (Transform queue in shapesQueuesHolder)
        {
            //currently its hard coded that shapes arrives in packs of 3
            for (int i = 0; i < 3; i++)
            {
                if (queue.GetChild(i).childCount > 0)
                {
                    ShapeInfo shapeInfo = new ShapeInfo();
                    shapeInfo.ShapeName = queue.GetChild(i).GetChild(0).name;
                    shapeInfo.ShapeColor = (int)queue.GetChild(i).GetChild(0).gameObject.GetComponent<ColorSelector>().CurrentColor;
                    shapeQueue.Add(shapeInfo);
                }
            }
        }      

        
        level1.ShapesLines = shapeQueue;

        

        return level1;
    }

} 

