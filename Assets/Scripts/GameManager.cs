using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using static ComboEffectView;
using static ShapesDB;

using Random = System.Random;


public class GameManager : MonoBehaviour
{
    const int LEVEL_LOOP_SIZE = 20;
    const float SUGGESTED_MOVE_TIME = 5f;

    const int BASE_SQUARE_CLEAR_SCORE = 50;
    const int DROP_SHAPE_SCORE = 100;
    const int CLEAR_BOARD_SCORE = 500;

    const string FTUE_KEY = "FTUE";
    const float EXPLOSION_DELAY = 0.11f;
    const float DELAY_DECAY = 0.965f;
    const int MIN_COMBO_AMOUNT = 5;

    public static GameManager Instance;

    public enum ScoreEventTypes
    {
        DropShape,
        ClearBoard,
        ClearGroup
    }

    public enum GameStates
    {
        Loading,
        Building,
        ShowingMenu,
        ShowingEndScreen,
        Playing,
        ShowingUnlock,
        ShowingTutorial
    }

    [SerializeField] ShapeStorage _shapeStorage;
    [SerializeField] Grid _gameGrid;
    [SerializeField] Lines _queueLines;

    [SerializeField] GameObject _shapePrefab;

    [SerializeField] RectTransform _canvasRect;
    [SerializeField] TMP_Text _levelText;
    [SerializeField] TMP_Text _scoreText;
    [SerializeField] TMP_Text _addedScoreText;

    [SerializeField] RectTransform _addedScoreRect;
    [SerializeField] CanvasGroup _addedScoreCanvas;

    //[SerializeField] TMP_Text _addedScoreReasonText;
    //[SerializeField] CanvasGroup _addedScoreReasonCanvas;

    [SerializeField] EndGameView _endGameView;
    [SerializeField] GameObject comboPrefab;
    [SerializeField] Transform dynamicHolder;

    [SerializeField]FeatureUnlockView _featureColorUnlockView;
    //[SerializeField] TutorialView _tutorialView;
    [SerializeField]SimpleTutorialView _simpleTutorialView;
    [SerializeField]RectTransform canvas;
    [SerializeField] Texture2D _handTexture;

    List<Vector2Int> simulationResults;
    int[,] gameBoard;

    public int currLevelIndex = 0;
    public int currCombo;
    int shapeCompeltedCounter = 0;
    bool levelComplete = false;
    public bool lockDrag = false;
    int score = 0;
    float _scoreTextY = 0;
    float _addedScoreY = 0;
    int numRevivesUsedInLevel = 0;
    Sequence scoreSequence;

    GameStates _gameState;

    DateTime _gameStartTime;

    [SerializeField] DebugView _debugView;

    Coroutine _suggestedMoveRoutine;
    
    LevelData currLevelData;
    public int[,] GameBoard { get => gameBoard; set => gameBoard = value; }
    public RectTransform CanvasRect { get => _canvasRect; set => _canvasRect = value; }
    public Grid GameGrid { get => _gameGrid; set => _gameGrid = value; }
    public GameStates GameState { get => _gameState; set => _gameState = value; }

    private void Awake()
    {
        Instance = this;
        _gameState = GameStates.Loading;
    }

    // Start is called before the first frame update
    void Start()
    {
#if UNITY_EDITOR && false
        Cursor.SetCursor(_handTexture, Vector2.zero, CursorMode.ForceSoftware);
#endif
        Application.targetFrameRate = 60;
        DOTween.SetTweensCapacity(500, 150);

        PlayerPrefs.DeleteAll();

        bool isNewUser = PlayerPrefs.GetInt(FTUE_KEY, 1) == 1;

        if (isNewUser)
            PlayerPrefs.SetInt(FTUE_KEY, 0);

        //isNewUser = true;

        ModelManager.Instance.BuildLevels(isNewUser);
        _shapeStorage.InitStorageParams();
        //_tutorialView.SetAspectRatio(canvas.rect.width/canvas.rect.height);

        currLevelIndex = ModelManager.Instance.GetCurrentLevelIndex();

        //currLevelIndex = 24;

        _scoreTextY = _scoreText.gameObject.GetComponent<RectTransform>().localPosition.y;
        _addedScoreY = _addedScoreRect.localPosition.y;

        //TinySauce.SubscribeOnInitFinishedEvent((param1,param2)=>
        //{
            BuildLevel(currLevelIndex);
        //});

        
    }

    public int GetCurrNumRows()
    {
        return currLevelData.Rows;
    }

    public int GetCurrNumCols()
    {
        return currLevelData.Columns;
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.UpArrow))
            CheatLevels(true);

        if (Input.GetKeyDown(KeyCode.DownArrow))
            CheatLevels(false);


        if (Input.GetKeyDown(KeyCode.W))
            GameOver(true);

        if (Input.GetKeyDown(KeyCode.F))
            GameOver(false);

        if (Input.GetKeyDown(KeyCode.T))
            ShowComboVisual(ComboTypes.ClearGroup,UnityEngine.Random.Range(9,40),0, UnityEngine.Random.Range(0, 7), Vector3.zero);

        if (Input.GetKeyDown(KeyCode.Y))
            ShowComboVisual(ComboTypes.ClearBoard, UnityEngine.Random.Range(9, 40), 0, UnityEngine.Random.Range(0, 7), Vector3.zero);

        if (Input.GetKeyDown(KeyCode.U))
            ShowComboVisual(ComboTypes.ComboIncreased, UnityEngine.Random.Range(9, 40), UnityEngine.Random.Range(2, 6), UnityEngine.Random.Range(0, 7), Vector3.zero);


        /*
    if (Input.GetKeyDown(KeyCode.O))

        */
        if (Input.GetKeyDown(KeyCode.M))
        IsNoMoreMoves();

    }

    private void GenerateSuggestedMove()
    {
        (List<Vector2Int> positions, ShapeView shapeView) = GridUtils.SuggestBestMove(gameBoard, _queueLines.GetQueueTop(),
                _shapeStorage.GetShapesList());

        /*
        Debug.Log("ShapeIndex: " + shapeView.name);

        foreach (var position in positions)
            Debug.Log(position.ToString());

        */

        if(shapeView == null)
        {
            Debug.Log("No Suggestion Returned");
            return;
        }

        List<Vector2> gridSquareBlocksPositions = new List<Vector2>();

        foreach (Vector2Int item in positions)
            gridSquareBlocksPositions.Add(_gameGrid.GridSquareViews[item.x, item.y].RectTransform.position);

        Vector2 center = GridUtils.GetShapeCenter(gridSquareBlocksPositions);

        _simpleTutorialView.GenerateSuggestion(center, shapeView);
    }

    public void RestartLevel()
    {
        //if (_gameState == GameStates.ShowingTutorial)
        //    return;

        BuildLevel(currLevelIndex);
    }

    private void ResetParams()
    {
        StopSuggestedMoveRoutine();
        HideTutorialOrSuggestions();
        score = 0;
        _scoreText.text = "0";
        _addedScoreText.text = "";
        shapeCompeltedCounter = 0;
        currCombo = 1;
        numRevivesUsedInLevel = 0;


    }

    private void BuildLevel(int levelIndex)
    {
        ResetParams();

        _gameStartTime = DateTime.Now;

        LevelStartEvent(levelIndex);

        _gameState = GameStates.Building;

        _levelText.text = "LEVEL " + (levelIndex + 1);

        int tempLevelIndex = currLevelIndex;

        //for looping levels at the end
        while (tempLevelIndex >= ModelManager.Instance.levelsData.Count)
            tempLevelIndex -= LEVEL_LOOP_SIZE;

        currLevelData = ModelManager.Instance.levelsData[tempLevelIndex];

        // Initialize random with the provided seed
        LevelRNG.InitLevelRNG(currLevelData.RandomSeed);

        gameBoard = new int[currLevelData.Rows, currLevelData.Columns];

        for (int i = 0; i < currLevelData.Rows; i++)
        {
            for (int j = 0; j < currLevelData.Columns; j++)
                gameBoard[i, j] = currLevelData.Board[i][j];
        }

        //init grid
        //float gridTopY = _gameGrid.CreateGrid(currLevelData.Rows, currLevelData.Columns);

        float gridTopY = _gameGrid.CreateGridFixed1000(currLevelData.Rows, currLevelData.Columns,currLevelData.Board);


        List<List<int>> linesModelCopy = new List<List<int>>();

        int highestColorIndex = 0;

        //List<int> numColorsInQueue = new List<int>();

        for (int i = 0; i < currLevelData.PeopleQueues.Count; i++)
        {
            List<int> singleLine = new List<int>();

            for (int j = 0; j < currLevelData.PeopleQueues[i].Count; j++)
            {
                int personColorValue = currLevelData.PeopleQueues[i][j];

                singleLine.Add(personColorValue);

                if(personColorValue>highestColorIndex)
                    highestColorIndex = personColorValue;

                //if (!numColorsInQueue.Contains(personColorValue))
                //    numColorsInQueue.Add(personColorValue);
            }
            

            linesModelCopy.Add(singleLine);
        }

        //count how many colors are in the lines 

        currLevelData.NumberOfColors = highestColorIndex+1;
        //init lines
        gridTopY = 325f;

        _queueLines.BuildLines(linesModelCopy, _gameGrid, currLevelData.NumberOfColors, gridTopY);
        //init storage

        //0.35 is the scale to fit a square size of 100 for all the shapes in their squares
        //float shapeNativeStorageScale = 0.35f;// (0.35f * _gameGrid.squareScale / 100f)/1.75f;

        float shapeNativeStorageScale =  100 / _gameGrid.GetSquareScale() * 0.35f;

        List<ShapeInfo> shapeInfo = new List<ShapeInfo>();

        shapeInfo = currLevelData.ShapesLines.GetRange(0, 3);//take first 6 shapes

        _shapeStorage.SetRandomSeed(currLevelData.RandomSeed);
        _shapeStorage.GenerateNewShapes(shapeInfo,true,true, shapeNativeStorageScale);

        ShowLevelBuilding();

        int needToShowUnlock = ModelManager.Instance.GetUnlock(currLevelIndex);

        //check if need to show unlock

        

        if(currLevelData.Tutorial)
        {
            _gameState = GameStates.ShowingTutorial;
            ShowTutorial();
        }
        else if (needToShowUnlock>-1)
        {
            _gameState = GameStates.ShowingUnlock;
            ShowFeatureUnlock(needToShowUnlock);
        }
        else
        {
            StartPlaying();
        }
    }


    private void ShowLevelBuilding()
    {
        SoundsController.Instance.PlayBuildLevel();
        _gameGrid.GridCanvas.alpha = 0;
        _gameGrid.GridCanvas.DOFade(1, 0.75f);
        _queueLines.QueueLinesEnterAnimation();
        _shapeStorage.ShowShapesEnterAnimation();
    }

    public void HideLevelVisuals()
    {
        _gameGrid.GridCanvas.DOFade(0, 0.25f);
        _queueLines.QueueLinesExitAnimation();
        _shapeStorage.ShowShapesExitAnimation();
    }

    public void FeatureUnlockClosed()
    {
        StartPlaying();
    }

    public void HideTutorialOrSuggestions()
    {
        _simpleTutorialView.CloseTutorial();
       StopSuggestedMoveRoutine();
    }

    private void StartPlaying()
    {
        _gameState = GameStates.Playing;
        lockDrag = false;

        StartSuggestedMoveTimer();
    }

    private void StopSuggestedMoveRoutine()
    {
        if (_suggestedMoveRoutine != null)
        {
            StopCoroutine(_suggestedMoveRoutine);            
        }       

    }

    public void StartSuggestedMoveTimer()
    {
        if (GameState == GameStates.ShowingTutorial)
            return;

        StopSuggestedMoveRoutine();
        _suggestedMoveRoutine = StartCoroutine(SuggestedMoveRoutine()); 
    }

    private IEnumerator SuggestedMoveRoutine()
    {
        yield return new WaitForSeconds(SUGGESTED_MOVE_TIME);

        GenerateSuggestedMove();
                
    }

    private void ShowFeatureUnlock(int indexToUnlock)
    {
        //2 first colors are unlcoked at game start
        indexToUnlock += 3;

        _featureColorUnlockView.ShowColorUnlocked(indexToUnlock);

    }

    private void ShowTutorial()
    {
        //take the current shape in position 1 and send to tutorial

        (GameObject tempShapeCopy, int colorIndex) = _shapeStorage.GetTutorialShape();

        GameObject shapeCopy = Instantiate(tempShapeCopy);

        ShapeView shapeView = shapeCopy.GetComponent<ShapeView>();

        shapeView.TutorialMode(colorIndex);
        lockDrag = false;
        _simpleTutorialView.ShowTutorial(shapeCopy);

    }

    public void RequestNewShapes()
    {

        shapeCompeltedCounter++;
        List<ShapeInfo> shapeInfo = new List<ShapeInfo>();

        if (currLevelData.ShapesLines.Count >= (shapeCompeltedCounter + 1) * 3)
        {
            shapeInfo = currLevelData.ShapesLines.GetRange(shapeCompeltedCounter * 3, 3);
            _shapeStorage.GenerateNewShapes(shapeInfo,false,true);
        }
        else
        {
            //no more defained shapes - take random ones instead

            bool[] shapesToUse = new bool[4];
            shapesToUse[0] = true;
            shapesToUse[1] = true;
            shapesToUse[2] = true;
            shapesToUse[3] = !currLevelData.ExcludeShape5;
          

            shapeInfo = ShapeGenerator.GenerateNextThreeShapes(gameBoard, shapesToUse, _queueLines.CurrentLines, 30, out List<string> debugReasons, currLevelData.NumberOfColors);


            _shapeStorage.GenerateNewShapes(shapeInfo, false, false);
        }
    }

    public string GetRandomShapeNameByGroup(int groupIndex)
    {
        return _shapeStorage.ShapesDB.GetRandomShapeNameFromGroup(groupIndex);
    }

    public async void GameOver(bool allLinesEmpty)
    {
        StopSuggestedMoveRoutine();
        HideTutorialOrSuggestions(); 

        _gameState = GameStates.ShowingEndScreen;

        //show end game

        levelComplete = allLinesEmpty;

        LevelCompelteEvent(currLevelIndex, allLinesEmpty, score);

        if(currLevelData.Tutorial)
        {
            await Task.Delay(900 + currLevelIndex*250);
            NextButtonClicked();
        }
        else
        {
            _endGameView.InitEndScreen(allLinesEmpty,currLevelIndex,score, () =>
            {
                //end screen done
                NextButtonClicked();
            });
        }
       
    }

    public void NextButtonClicked()
    {        
        if (levelComplete)
        {
            ModelManager.Instance.SetLevelComplete(currLevelIndex);
            currLevelIndex++;
            BuildLevel(currLevelIndex);
        }
        else
        {
            //if i didnt lose this level ever - i clear the board and resume

            numRevivesUsedInLevel++;

            //TinySauce.OnUpgradeEvent("Revive",currLevelIndex);

            float delay = 0;

            foreach (GridSquareView gridSquareView in _gameGrid.GridSquareViews)
            {
                if(gridSquareView.IsFull)
                {
                    gridSquareView.ExplodeSquere(delay);
                    delay += 0.005f;
                }
            }

            //clear the board and resume
            for (int i = 0; i < currLevelData.Rows; i++)
            {
                for (int j = 0; j < currLevelData.Columns; j++)
                    gameBoard[i, j] = -1;
            }

            


            _gameState = GameStates.Playing;
            lockDrag = false;
            
        }

    }

    public void CheatLevels(bool cheatUp)
    {
        currLevelIndex += cheatUp ? 1 : -1;

        if (currLevelIndex == ModelManager.Instance.levelsData.Count)
            currLevelIndex = 0;

        if (currLevelIndex == -1)
            currLevelIndex = ModelManager.Instance.levelsData.Count - 1;

        BuildLevel(currLevelIndex);
    }

    //checking on the grid how many shapes squares are over it - compare it to the number of squares in the shape
    internal int GetNumValidAttemptsToPosition()
    {
        int counter = 0;
        foreach (GridSquareView gridSquareView in _gameGrid.GridSquareViews)
            if (gridSquareView.ValidAttempt)
                counter++;

        return counter;
    }

    public List<Vector2Int> GetValidAttemptedPositions()
    {
        List<Vector2Int> result = new List<Vector2Int>();

        foreach (GridSquareView gridSquareView in _gameGrid.GridSquareViews)
            if (gridSquareView.ValidAttempt)
                result.Add(new Vector2Int(gridSquareView.row,gridSquareView.col));

        return result;
    }

    public void ClearSimulationIndications()
    {
        for (int i = 0; i < currLevelData.Rows; i++)
        {
            for (int j = 0; j < currLevelData.Columns; j++)
                _gameGrid.GridSquareViews[i, j].SimulateIndication.HideAllIndications();
        }

        _queueLines.UnHighLightAllPeople();
    }

  

    public void SimulatePlacingShape(List<Vector2Int> shape, int colorIndex)
    {
        int[,] tempBoard = new int[currLevelData.Rows, currLevelData.Columns];

        for (int i = 0; i < currLevelData.Rows; i++)
        {
            for (int j = 0; j < currLevelData.Columns; j++)
                tempBoard[i,j] = GameBoard[i,j];
        }

        foreach (Vector2Int position in shape)
            tempBoard[position.x, position.y] = colorIndex;

        int numPeopleUsed = 0;

        (simulationResults,numPeopleUsed) = GridUtils.SimulatePlacementAndCheckCompletions(tempBoard, _queueLines.GetQueueTop(), colorIndex);

       ClearSimulationIndications();

        if (simulationResults != null && simulationResults.Count > 0)
        {
            foreach (Vector2Int square in simulationResults)
            {
                _gameGrid.GridSquareViews[square.x, square.y].SimulateIndication.ShowIndication(square.x, square.y, simulationResults, colorIndex);
            }

            _queueLines.HighLightPeople(numPeopleUsed,colorIndex);
        }
    }

    public void PositionCurrentShape(ShapeView shape, int colorIndex)
    {        
        foreach (GridSquareView gridSquareView in _gameGrid.GridSquareViews)
            if (gridSquareView.ValidAttempt)
            {
                gridSquareView.SetIndicationToFull(colorIndex);
                gridSquareView.SetColorToFull(colorIndex);
                GameBoard[gridSquareView.row, gridSquareView.col] = colorIndex;
            }

        _queueLines.UnHighLightAllPeople();

        SoundsController.Instance.PlayPiecePlaced();

        bool shapesListEmpty = _shapeStorage.ShapeWasPositioned(shape);

        PostShapePositionedFlow(shapesListEmpty);

    }

    internal void ResumePlacingShape()
    {
        PostShapePositionedFlow(false);        
    }

    public void TutorialComplete()
    {
        GameOver(true);
    }

    private async Task<bool> CheckPersonAssist(bool anylineAlreadyComplete)
    {
        bool personAssist = true;
        bool anyLineComplete = anylineAlreadyComplete;
        while (personAssist)
        {
            int colorIndex;
            (personAssist, colorIndex) = await CheckPotentialLines();

            if (personAssist)
            {
                anyLineComplete = true;
            }

            
        }

        return anyLineComplete;
    }

    

    private void RemoveCompletedLine(List<Vector2Int> linePositions,int colorIndex,float initialExplosionDelay)
    {
        LineCompleteVisuals();

        ScoreEvent(ScoreEventTypes.ClearGroup, linePositions.Count);

        List<Vector2> gridSquareBlocksPositions = new List<Vector2>();

        foreach (Vector2Int item in linePositions)
            gridSquareBlocksPositions.Add(_gameGrid.GridSquareViews[item.x, item.y].RectTransform.position);

        Vector2 center = GridUtils.GetShapeCenter(gridSquareBlocksPositions);

        if (linePositions.Count > MIN_COMBO_AMOUNT)
            ShowComboVisual(ComboTypes.ClearGroup, linePositions.Count, currCombo, colorIndex, center);

        //clear the extended completed line
        for (int i = 0; i < linePositions.Count; i++)
        {
            gameBoard[linePositions[i].x, linePositions[i].y] = -1;
            initialExplosionDelay *= DELAY_DECAY;
            _gameGrid.GridSquareViews[linePositions[i].x, linePositions[i].y].ExplodeSquere(i * initialExplosionDelay);
        }

        Taptic.Heavy();
        SoundsController.Instance.PlayLineComplete();
        IncreaseCombo(colorIndex);
    }

    private void LineCompleteVisuals()
    {
        if(simulationResults!=null &&  simulationResults.Count > 0)
        {
            foreach (Vector2Int square in simulationResults)
            {
                _gameGrid.GridSquareViews[square.x, square.y].SimulateIndication.ShowComplete();
            }
        }

    }

    private async void PostShapePositionedFlow(bool shapeListEmpty)
    {
        ScoreEvent(ScoreEventTypes.DropShape, 0);

        lockDrag = true;

        bool anyLineComplete = false;

        //check line completion with people assist

        anyLineComplete = await CheckPersonAssist(false);


        bool lineComplete = true;

        while(lineComplete)
        {
            int lineColor = 0;
            List<Vector2Int> linePositions;
            lineComplete = false;
            float initialExplosionDelay = EXPLOSION_DELAY;

            linePositions = GridUtils.FindCompletedRowOrColumnTouchings(GameBoard);

            if(linePositions == null || linePositions.Count >0)
            {
                lineComplete = true;
                lineColor = GameBoard[linePositions[0].x, linePositions[0].y];

                RemoveCompletedLine(linePositions, lineColor, initialExplosionDelay);

                await Task.Delay(100);

                _queueLines.FillAllLineGaps();

                anyLineComplete = true;
            }

        }

        anyLineComplete = await CheckPersonAssist(anyLineComplete);

        //check game over complete or fail

        bool allQueueLinesEmpty = _queueLines.AllLinesEmpty();

        if (allQueueLinesEmpty)
            Debug.Log("GameOver - WIN");

        bool noMoreMoves = IsNoMoreMoves();

        if (noMoreMoves)
            Debug.Log("GameOver - NO MORE MOVES");

        bool boardStuck = GridUtils.IsImpossibleToComplete(gameBoard, _queueLines.GetQueueTop(), _shapeStorage.ShapesDB.AllShapes,12);

        if(boardStuck)
        {
            Debug.Log("Board cannot be completed");
            noMoreMoves = true;
        }

        //check if board is empty - give combo
        if (IsBoardEmpty() && currLevelData.Tutorial == false)
        {
            ShowComboVisual(ComboTypes.ClearBoard,0,0,0,Vector3.zero);
            ScoreEvent(ScoreEventTypes.ClearBoard,0);

        }

        if (allQueueLinesEmpty || noMoreMoves)
        {
            
            lockDrag = true;
            GameOver(allQueueLinesEmpty);
        }
        else
        {
            if (shapeListEmpty)
                RequestNewShapes();

            lockDrag = false;
            StartSuggestedMoveTimer();
        }

        if (anyLineComplete == false)
            currCombo = 1;

        

    }

    //returns if any line was completed by the people, and if so the line color as well
    private async Task<(bool,int)> CheckPotentialLines()
    {
        int completedColorIndex = -1;

        //Debug.Log("Checking potentialLines");

        int[] currentLineTopByIndex = _queueLines.GetQueueTop();

        //the array contains the amount of each color (based on color index in the array) 

        //for each entry in the array - if the entry is bigger than 0 - see if there is a row that can be completed with this help

        int indexWeTookFrom = -1;
        int numQueues = _queueLines.GetNumQueues();

        for (int i = 0; i < currentLineTopByIndex.Length; i++)
        {
            if (currentLineTopByIndex[i] > 0)
            {               
                (List<Vector2Int> resultingIndexes,int peopleUsed) = GridUtils.FindLineCompletionPositions(GameBoard, i, currentLineTopByIndex[i]);

                int totalPeopleToMove = resultingIndexes.Count;

                if (resultingIndexes.Count > 0)
                {
                    if(completedColorIndex == -1)
                        completedColorIndex = i;
                    

                    for (int j = 0; j < resultingIndexes.Count; j++)
                    {
                        _gameGrid.GridSquareViews[resultingIndexes[j].x, resultingIndexes[j].y].SetIndicationToFull(i);
                        gameBoard[resultingIndexes[j].x, resultingIndexes[j].y] = i;

                        int betweenPeopleMoveDelay = j * 35;

                        //limit max delay
                        betweenPeopleMoveDelay = Math.Min(100,betweenPeopleMoveDelay);

                        await Task.Delay(betweenPeopleMoveDelay);

                        SoundsController.Instance.PlayStartWalking();

                        indexWeTookFrom++;
                        indexWeTookFrom %= numQueues;

                        //we have a match - need to pick person of the i color and move to the first index
                        indexWeTookFrom = _queueLines.MoveColorPersonToIndexInQueuesOrder(indexWeTookFrom, i, resultingIndexes[j], (person,colorIndex,gridPosition) =>
                        {
                            _gameGrid.GridSquareViews[gridPosition.x, gridPosition.y].SetColorToFull(colorIndex);
                            //move completed
                            totalPeopleToMove--;

                            Destroy(person);                            

                            if (totalPeopleToMove == 0)
                            {
                                //Debug.Log("Move Complete");

                                bool complete = false;
                                int completeColorIndex = 0;
                                float initialExplosionDelay = EXPLOSION_DELAY;
                                List<Vector2Int> linePositions;

                                linePositions = GridUtils.FindCompletedRowOrColumnTouchings(GameBoard);

                                if (linePositions == null || linePositions.Count > 0)
                                {
                                    complete = true;
                                    completeColorIndex = GameBoard[linePositions[0].x, linePositions[0].y];

                                    RemoveCompletedLine(linePositions, completeColorIndex, initialExplosionDelay);
                                }                                    
                            }
                        });

                    }

                    await Task.Delay(1000);

                    return (true, completedColorIndex);
                }
            }

        }

        return (false,0);
    }

    public bool IsBoardEmpty()
    {
        for (int i = 0; i < gameBoard.GetLength(0); i++)
        {
            for (int j = 0; j < gameBoard.GetLength(1); j++)
            {
                if (gameBoard[i, j] != -1)
                {
                    return false; // Found a non-empty cell
                }
            }
        }
        return true; // All cells are empty
    }

    private bool IsNoMoreMoves()
    {
        //no shapes
        if (_shapeStorage.shapeList.Count == 0)
            return false;

        int numberOfShapesInWaitingArea = 0;
        int numberOfShapesThatCantBePlaced = 0;

        //need to loop over all the active shapes and see if none of them can fit anywhere
        foreach (ShapeView shape in _shapeStorage.shapeList)
        {            
            bool foundPositionForShape = false;

            numberOfShapesInWaitingArea++;

            for (int row = 0; row < currLevelData.Rows; row++)
            {
                for (int col = 0;col<currLevelData.Columns; col++)
                {
                    if (foundPositionForShape == false)
                    {
                        List<Vector2Int> shapesPositions = shape.GetShapeRelativeGridPositions();

                        bool shapeCanBePlacedInThisIndex = true;

                        //check if all the shape positions can be placed

                        //convert the vector positions to the array index

                        for (int j = 0; j < shapesPositions.Count; j++)
                        {
                            Vector2Int combinedPosition = new Vector2Int(row + shapesPositions[j].x, col + shapesPositions[j].y);

                            bool indexInGrid = true;

                            if (combinedPosition.x < 0 || combinedPosition.x > currLevelData.Rows-1)
                                indexInGrid = false;

                            if (combinedPosition.y < 0 || combinedPosition.y > currLevelData.Columns - 1)
                                indexInGrid = false;

                            if (indexInGrid == false || gameBoard[combinedPosition.x,combinedPosition.y] != -1)
                                shapeCanBePlacedInThisIndex = false;
                            
                        }

                        if (shapeCanBePlacedInThisIndex)
                        {
                            //found a valid place for the shape
                            foundPositionForShape = true;
                        }
                    }
                }
            }
            if (foundPositionForShape == false)
                numberOfShapesThatCantBePlaced++;            
        }

        return numberOfShapesInWaitingArea == numberOfShapesThatCantBePlaced;
    }

    //this checks if the color index i sent exists in the line - if it doesn - method returns a color that does exist
    public int CheckIfMyColorExistsInQueues(int colorIndex,Random random)
    {
        return _queueLines.CheckIfMyColorExistsInQueuesHead(colorIndex, random);
    }

    private void IncreaseCombo(int colorIndex)
    {
        if (currLevelData.Tutorial)
            return;

        currCombo++;

        if(currCombo>=3)
            ShowComboVisual(ComboTypes.ComboIncreased,0,currCombo,colorIndex,Vector3.zero);
    }

    public void ShowComboVisual(ComboTypes comboType, int amountCombo,int currCombo, int colorIndex, Vector3 rectPosition)
    {
        _gameGrid.DoComboEffect(colorIndex);

        //show combo object
        GameObject comboEffect = Instantiate(comboPrefab, dynamicHolder);

        ComboEffectView comboEffectView = comboEffect.GetComponent<ComboEffectView>();

        comboEffectView.InitComboEffect(comboType,amountCombo, currCombo-1, rectPosition, ModelManager.Instance.GetColorByColorIndex(colorIndex));

    }

    public void ShowHideDebugClicked()
    {
        if (_debugView.gameObject.activeInHierarchy)
            _debugView.gameObject.SetActive(false);
        else
            _debugView.InitDebug();//, _removeFromQueueBonus,_queueOptimization);
    }

   

    public ShapeDefinition GetShapeDataByShapeInfo(ShapeInfo shapeInfo)
    {
        return _shapeStorage.GetShapeByName(shapeInfo.ShapeName); 
    }

    private void ScoreEvent(ScoreEventTypes scoreEventType, int numberOfClearedSquares)
    {
        int startScore = score;
        int deltaScore=0;
        if (scoreEventType == ScoreEventTypes.DropShape)
        {
             deltaScore = DROP_SHAPE_SCORE * currCombo;
            
        }
        else if(scoreEventType == ScoreEventTypes.ClearGroup)
        {
            deltaScore = BASE_SQUARE_CLEAR_SCORE * numberOfClearedSquares * currCombo;
            
        }
        else if(scoreEventType == ScoreEventTypes.ClearBoard)
        {
            deltaScore = CLEAR_BOARD_SCORE * currCombo;
        }

        _addedScoreText.text = "+" + deltaScore;

        score += deltaScore;

        if (scoreSequence != null)
            scoreSequence.Kill();

        scoreSequence = DOTween.Sequence();

        _addedScoreCanvas.alpha = 0;
        

        _addedScoreRect.localPosition = new Vector3(0, _addedScoreY);

        SoundsController.Instance.PlayProgressBar();

        scoreSequence.Join(DOVirtual.Int(startScore, score, 1, (newValue) =>
        {
            _scoreText.text = newValue.ToString("N0");
        }));

        scoreSequence.Join(_addedScoreRect.DOLocalMoveY(_scoreTextY, 1).OnComplete(()=>
        {
            _addedScoreText.text = "";
        }));

        scoreSequence.Join(_addedScoreCanvas.DOFade(1, 0.25f));
                
        scoreSequence.Join(_addedScoreCanvas.DOFade(0, 0.25f).SetDelay(0.75f));
        
        scoreSequence.Play();

    }

    private void LevelStartEvent(int levelIndex)
    {
        //TinySauce.OnGameStarted(levelIndex);
    }

    private void LevelCompelteEvent(int levelIndex,bool success,float score)
    {
        Dictionary<string,object> completeParams = new Dictionary<string,object>();

        TimeSpan levelTime = DateTime.Now-_gameStartTime;

        Debug.Log("Level- " + (levelIndex + 1) + " took " + levelTime.TotalSeconds);

        completeParams.Add("LevelTime", levelTime.TotalSeconds);

        //TinySauce.OnGameFinished( success,score,levelIndex, completeParams);
    }

}
