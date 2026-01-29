using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class Lines : MonoBehaviour
{
    public const float PERSON_MOVE_TO_BOARD_TIME = 0.5f;

    List<List<int>> currentLines  = new List<List<int>>();

    List<LineView> lineViews = new List<LineView> ();

    public GameObject LinePrefab;
    public GameObject LinePersonPrefab;
    public Transform personHolderNotInLine;

    [SerializeField] RectTransform queueTransform;

    [SerializeField] Image queueProgressImage;
    int startLevelNumPeopleInQueues = 0;

    Grid gameGrid;

    int numberOfColors = 0;

    public List<List<int>> CurrentLines { get => currentLines; set => currentLines = value; }

    public void UpdateProgressBar()
    {
        int currNumPeopleInQueues = 0;

        for (int i = 0; i < currentLines.Count; i++)
            currNumPeopleInQueues += currentLines[i].Count;

        float fillAmount = (startLevelNumPeopleInQueues - currNumPeopleInQueues) / (float)startLevelNumPeopleInQueues;

        queueProgressImage.fillAmount = fillAmount;

    }

    public int GetNumQueues()
    {
        return currentLines.Count;
    }

    public void BuildLines(List<List<int>> levelLines, Grid grid, int _numberOfColors, float gridTopY)
    {
        startLevelNumPeopleInQueues = 0;

        for (int i = 0;i<levelLines.Count;i++)
            startLevelNumPeopleInQueues += levelLines[i].Count;

        //Debug.Log("startLevelNumPeopleInQueues " + startLevelNumPeopleInQueues);


        //clear old data if there is any
        foreach (Transform person in personHolderNotInLine) 
            Destroy(person.gameObject);

        foreach (Transform line in transform)
            Destroy(line.gameObject);

        lineViews.Clear();

        numberOfColors = _numberOfColors;
        gameGrid = grid;
        currentLines =  levelLines;

        UpdateProgressBar();

        for (int i = 0; i < currentLines.Count; i++)
        {
            GameObject singleLine = Instantiate(LinePrefab,transform);
            LineView lineView = singleLine.GetComponent<LineView>();
            lineView.InitLine(i,currentLines[i], LinePersonPrefab, personHolderNotInLine,currentLines.Count == 1);
            lineViews.Add(lineView);
        }

        int numLines = currentLines.Count;
        float totalWidth = (numLines-1) * LineView.QUEUE_WIDTH + ((numLines-1) * GameManager.Instance.GameGrid.GetSquareGap());
        queueTransform.localPosition = new Vector2(-totalWidth/2f, gridTopY);
    }

    public async void QueueLinesEnterAnimation()
    {
        SoundsController.Instance.PlayProgressBar();
        for (int i = 0; i < currentLines.Count; i++)
        {
            lineViews[i].ShowLineVisually();
            await Task.Delay(150);
        }        
    }

    public void QueueLinesExitAnimation()
    {
        for (int i = 0; i < currentLines.Count; i++)        
            lineViews[i].HideLineVisually();
    }

    //returns an array saying for each color how many people in the line are avaialble
    public int[] GetQueueTop()
    {
        int[] top = new int[numberOfColors];


        //top[0] - how many people of color index 0 are  

        for (int color = 0; color < numberOfColors; color++)
        {
            foreach (List<int> personInLine in currentLines)
            {
                bool colorChainBroken = false;

                for (int i = 0; i < personInLine.Count; i++)
                {
                    if (personInLine[i] == color && colorChainBroken == false)
                        top[color]++;

                    if(personInLine[i] != color)
                        colorChainBroken = true;
                }
            }
        }

        

        return top;
    }

    internal int CheckIfMyColorExistsInQueuesHead(int colorIndex,Random random)
    {
        List<int> queueColors = new List<int>();

        foreach (List<int> personInLine in currentLines)
        {
            bool colorChainBroken = false;


            //check only in queue head
            for (int i = 0; i < Math.Min(2,personInLine.Count); i++)
            //for (int i = 0; i < personInLine.Count; i++)
            {
                if (!queueColors.Contains(personInLine[i]))
                    queueColors.Add(personInLine[i]);
            }
        }

        if(queueColors.Contains(colorIndex))
            return colorIndex;
        else
        {
            if(queueColors.Count==0) 
                return 0;
            else
                return queueColors[random.Next(0,queueColors.Count-1)];
        }

    }

    //using start index so when i have 2 people of same color in lines 0 and 1 - i will take these 2 people and not line 0 and line 0 again (if same color person)
    //returns the queue index we took person from
    internal int MoveColorPersonToIndexInQueuesOrder(int queueIndexToStartFrom,int colorIndex, Vector2Int indexOnGridToGoTo, Action<GameObject, int, Vector2Int> moveDone)
    {
        float gridScale = GameManager.Instance.GameGrid.GetSquareScale() / 100f;
        Vector3 gridScaleVector = new Vector3(gridScale, gridScale, gridScale);
        bool found = false;
        //need to look in the lines we have on person 0 and see if his color match

        int numQueues = currentLines.Count;

        for (int i = queueIndexToStartFrom % numQueues; i < queueIndexToStartFrom + numQueues; i++)
        {
            int modedI = i % numQueues;

            if (currentLines[modedI].Count > 0 && currentLines[modedI][0] == colorIndex && found == false)
            {
                found = true;
                GameObject personToMove = lineViews[modedI].GetTopPersonFromQueue();
                RectTransform personTrans = personToMove.GetComponent<RectTransform>();

                PersonView personView = personToMove.GetComponent<PersonView>();

                Vector3 targetPosition = gameGrid.GridSquareViews[indexOnGridToGoTo.x, indexOnGridToGoTo.y].RectTransform.position;

                personView.PersonMove(targetPosition, gridScaleVector, PERSON_MOVE_TO_BOARD_TIME, (() =>
                {
                    moveDone?.Invoke(personToMove, colorIndex, indexOnGridToGoTo);
                }));

                
                currentLines[modedI].RemoveAt(0);

                UpdateProgressBar();
                return modedI;
            }

        }
        return -1;
    }

    /// <summary>
    /// Gathers all distinct color indices present in all the queues.
    /// </summary>
    /// <param name="allQueues">A list of queues, where each queue is a list of color indices.</param>
    /// <returns>A list of unique color indices found across all queues.</returns>
    public List<int> GetAllQueueColors()
    {
        HashSet<int> distinctColors = new HashSet<int>();

        foreach (var queue in currentLines)
        {
            foreach (var color in queue)
            {
                distinctColors.Add(color);
            }
        }

        return new List<int>(distinctColors);
    }

   
    internal bool AllLinesEmpty()
     {
        bool allLinesEmpty = true;

        for (int i = 0;i < lineViews.Count;i++)
            if(lineViews[i].GetNumberOfPeopleInLine()>0)
                allLinesEmpty = false;

        return allLinesEmpty;
    }

    //this needs to look over all the queue lines - and look for count people of lineColor
    //if found - they need to be removed from the queues both gameObject and also data
    //this time the loop will go over all the lines from position 0 - and not inside the line 
    internal List<PersonView> RequestPeopleFromLines(int count, int lineColor)
    {
        List <PersonView> personViews = new List <PersonView>();
        List<Vector2Int> positionsToRemove = new List<Vector2Int>();

        int maxLineCount = 0;

        for(int i = 0;i< currentLines.Count; i++)
            if (currentLines[i].Count > maxLineCount)
                maxLineCount = currentLines[i].Count;

        //now we know the max of all the lines
        //start running from line index 0 to end - and inside run on the internal line (as long as it exists)

        for(int i = 0;i<maxLineCount;i++)
        {
            for(int j = 0;j<currentLines.Count;j++)
            {
                //make sure we dont take too many people
                if (currentLines[j].Count > i && positionsToRemove.Count<count)
                {
                    //found a match - remove him from the lines
                    if (currentLines[j][i] == lineColor)
                    {
                        positionsToRemove.Add(new Vector2Int(j, i));
                        //currentLines[j].RemoveAt(i);
                    }

                }
            }
        }

        //remove the position from the data from end to start 
        for(int i = positionsToRemove.Count-1;i>=0;i--)
        {
            int queueIndex = positionsToRemove[i].x;
            int inQueuePosIndex = positionsToRemove[i].y;

            currentLines[queueIndex].RemoveAt(inQueuePosIndex);
            PersonView personView = lineViews[queueIndex].GetPersonViewByIndex(inQueuePosIndex);
            personViews.Add(personView);
        }
        

        return personViews;

    }

    public void FillAllLineGaps()
    {
        foreach (var item in lineViews)
            item.MoveAllPersonsToPosition();
        
    }

    /// <summary>
    /// Tries to collect up to 'numberOfUnits' PersonView objects of 'unitColor'
    /// from each line's personsLine, scanning them in a "column-wise" fashion.
    /// If a line has a mismatch in color at some index, we stop using that line entirely.
    /// </summary>
    public List<PersonView> GetMatchingPersons(int numberOfUnits, int unitColor)
    {
        List<PersonView> result = new List<PersonView>();
        if (numberOfUnits <= 0) return result; // no need to collect anything

        // We'll keep track of which lines are still "active" (not disqualified)
        List<LineView> activeLines = new List<LineView>(lineViews);

        // Find the max possible "column index" among all lines
        // so we don't go out of bounds
        int maxLineLength = 0;
        foreach (var line in lineViews)
        {
            int count = line.PersonsLine.Count;
            if (count > maxLineLength)
                maxLineLength = count;
        }

        // Column by column
        for (int i = 0; i < maxLineLength && numberOfUnits > 0 && activeLines.Count > 0; i++)
        {
            // We may remove some lines if there's a mismatch at this column
            List<LineView> linesToRemove = new List<LineView>();

            // Check each active line at index 'i'
            foreach (var line in activeLines)
            {
                // If this line doesn't have a person at 'i', skip it
                if (i >= line.PersonsLine.Count)
                {
                    // no person at index i
                    continue;
                }

                // Check color match
                PersonView person = line.PersonsLine[i];
                if (person.PersonColorIndex == unitColor)
                {
                    // Good match, add to result
                    result.Add(person);
                    numberOfUnits--;

                    // If we've collected enough, we can stop entirely
                    if (numberOfUnits == 0)
                        break;
                }
                else
                {
                    // Mismatch => remove this line from future consideration
                    linesToRemove.Add(line);
                }
            }

            // Remove all lines that had a mismatch
            foreach (var line in linesToRemove)
            {
                activeLines.Remove(line);
            }

            // If we have collected all needed units, break out
            if (numberOfUnits == 0)
                break;
        }

        return result;
    }


        //highlight the people that will be used 
    internal void HighLightPeople(int numPeopleUsed, int colorIndex)
    {
        List<PersonView> usedPersons = GetMatchingPersons(numPeopleUsed, colorIndex);

        foreach (PersonView person in usedPersons)
        {
            person.ShowHidePersonHighLight(true,colorIndex);
        }
    }

    internal void UnHighLightAllPeople()
    {
        foreach (LineView lineView in lineViews)
        {
            foreach (PersonView person in lineView.PersonsLine)
            {
                person.ShowHidePersonHighLight(false, 0);
            }
        }
    }
}
