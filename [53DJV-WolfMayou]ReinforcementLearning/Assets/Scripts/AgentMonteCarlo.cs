using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using Intent = TicTacToeController.TicTacToeIntent;
using CellType = Cell.CellTicTacToeType;

public class AgentMonteCarlo : MonoBehaviour
{
    private class SAR
    {
        public State state;
        public Intent intent;
        public float reward;
    }
    
    [SerializeField] private TicTacToeController ticTacToeController;
    private List<State> _allStates;
    private List<SAR> _simulatedSARs;
    [SerializeField] private DebuggerManager debuggerManager;

    public float epsilon;
    private bool _monteCarloInitialized;
    public bool everyVisit;
    public bool onPolicy;
    public int episodesNumber;
    
    public void LaunchAgent(AgentSelector.AgentType algo)
    {
        //InitializeMonteCarlo();
        switch (algo)
        {
            case AgentSelector.AgentType.MonteCarloOffPolicy:
                break;
            case AgentSelector.AgentType.MonteCarloOffPolicyEvery:
                break;
            case AgentSelector.AgentType.MonteCarloOnPolicy:
                break;
            case AgentSelector.AgentType.MonteCarloOnPolicyEvery:
                break;
        }
    }

    /*public void LaunchMonteCarlo(bool everyVisit = false, bool onPolicy = false)
    {
        InitializeMonteCarlo();
        for (int i = 0; i < 10; i++)
        {
            /*if (Random.Range(0.0f, 1.0f) < Epsilon)
            {
                MonteCarloPrediction(_allStates[Random.Range(0, _allStates.Count)], 100, everyVisit, onPolicy);
            }
            else
            {
                MonteCarloPrediction(GetBestState(), 100, everyVisit, onPolicy);
            }#1#
        }
        
    }*/
    
    private void Update()
    {
        // Mouse Left Click
        if (Input.GetMouseButtonDown(0))
        {
            OnMouseLeftClick();
        }
    }
    
    private void OnMouseLeftClick()
    {
        if (!_monteCarloInitialized)
        {
            InitializeMonteCarlo();
        }

        Vector3 mouse = Input.mousePosition;
        Ray castPoint = Camera.main.ScreenPointToRay(mouse);
        RaycastHit hit;
        if (!ticTacToeController.gameIsOver && Physics.Raycast(castPoint, out hit, Mathf.Infinity))
        {
            float x = Mathf.Round(hit.point.x);
            float z = Mathf.Round(hit.point.z);
            
            if (ticTacToeController.Place(ticTacToeController.cross, new Vector3(x, 0.5f, z), ticTacToeController.crossGridMaterial, CellType.Cross) && !ticTacToeController.gameIsOver)
            {
                State playedState = GetStateFromGrid(ticTacToeController.grid.grid);
                
                if (playedState == null)
                {
                    playedState = new State
                    {
                        currentGrid = ticTacToeController.grid.grid,
                        ticTacToePolicy = GetRandomValidIntent(ticTacToeController.grid.grid)
                    };
                    _allStates.Add(playedState);
                }
                
                if (!_monteCarloInitialized)
                {
                    _monteCarloInitialized = true;
            
                    MonteCarloPrediction(playedState, episodesNumber, everyVisit, onPolicy);

                    //Improvement
                    int security = 0;
                    bool policyStable = true;
                    do
                    {
                        ++security;
                        policyStable = MonteCarloImprovement(playedState);
                    } while (!policyStable && security < 100);
                }

                ticTacToeController.ProcessIntent(playedState.ticTacToePolicy);
            }
        }
    }
    

    #region MonteCarloAlgorithm
    
    private void InitializeMonteCarlo()
    {
        _allStates = new List<State>();
        State newState = new State();
        newState.currentGrid = new Cell[3][];
        for (int i = 0; i < 3; ++i)
        {
            newState.currentGrid[i] = new Cell[3];
            for (int j = 0; j < 3; j++)
            {

                newState.currentGrid[i][j] = new Cell {cellTicTacToeType = CellType.Neutral};
            }
        }

        //newState.currentGrid[Random.Range(0, 3)][Random.Range(0, 3)].cellTicTacToeType = CellType.Cross; 
        newState.nS = 0;
        newState.returnS = 0;
        newState.ticTacToePolicy = (Intent) Random.Range(0, 9);
        newState.stateValue = 0;
        _allStates.Add(newState);
    }
    
    private void MonteCarloPrediction(State currentState, int iteration, bool everyVisit = false, bool onPolicy = false)
    {
        //initialisation
        foreach (var state in _allStates)
        {
            state.nS = 0;
            state.returnS = 0;
        }
        
        //simulation
        for (int i = 0; i < iteration; ++i)
        {
            _simulatedSARs = new List<SAR>();
            SimulateGame(currentState);
            float g = 0;
            for (int j = _simulatedSARs.Count-2; j >= 0; --j)
            {
                g += _simulatedSARs[j + 1].reward;
                
                bool isContained = false;
                if (!everyVisit)
                {
                    for (int k = 0; k < j; k++)
                    {
                        if (_simulatedSARs[k] == _simulatedSARs[j])
                        {
                            isContained = true;
                            break;
                        }
                    }
                }

                if (!everyVisit && !isContained  || everyVisit)
                {
                    _simulatedSARs[j].state.returnS += g;
                    ++_simulatedSARs[j].state.nS;
                }
            }

            if (onPolicy)
            {
                foreach (var state in _allStates)
                {
                    state.stateValue = state.returnS / state.nS;
                }

                foreach (var state in _allStates)
                {
                    state.ticTacToePolicy = GetBestIntent(state);
                }
            }
        }
        
        if (!onPolicy)
        { 
            foreach (var state in _allStates)
            {
                state.stateValue = state.returnS / state.nS;
            }
           
            foreach (var state in _allStates)
            {
                state.ticTacToePolicy = GetBestIntent(state);
            }
            
        }
    }
    
    private void SimulateGame(State currentState)
    {
        bool gameOver = false;
        int security = 0;
        State tmpState = currentState;
        do
        {
            ++security;
            SAR newSar = new SAR();
            Intent chosenIntent;
            
            if (Random.Range(0.0f, 1.0f) < epsilon)
            {
                chosenIntent = GetRandomValidIntent(tmpState.currentGrid);
            }
            else
            {
                chosenIntent = tmpState.ticTacToePolicy;
            }
            
            if (ticTacToeController.GridIsEmpty(ticTacToeController.GetPositionFromIntent(chosenIntent), tmpState.currentGrid))
            {
                newSar.state = GetNextState(tmpState, chosenIntent);
            }
            else
            {
                tmpState.ticTacToePolicy = GetRandomValidIntent(tmpState.currentGrid);
                newSar.state = GetNextState(tmpState, tmpState.ticTacToePolicy);
            }

            newSar.intent = newSar.state.ticTacToePolicy;
            newSar.reward = GetReward(newSar.state);
            
            _simulatedSARs.Add(newSar);
            if (newSar.reward >= 1000 || newSar.reward <= -500)
            {
                gameOver = true;
            }

            tmpState = newSar.state;
        } while (!gameOver && security < 100);
    }

    private bool MonteCarloImprovement(State currentState)
    {
        bool policyStable = true;
        foreach (var state in _allStates)
        {
            Intent tempPolicy = state.ticTacToePolicy;
            state.ticTacToePolicy = GetBestIntent(state);
            if (tempPolicy != state.ticTacToePolicy)
            {
                policyStable = false;
            }
        }

        if (!policyStable)
        {
            MonteCarloPrediction(currentState, episodesNumber, everyVisit, onPolicy);
        }

        return policyStable;
    }
    
    private float GetReward(State currentState)
    {
        int reward = 0;
        if (ticTacToeController.CheckDiagonal(CellType.Circle, currentState.currentGrid) ||
            ticTacToeController.CheckVerticalRows(CellType.Circle, currentState.currentGrid) ||
            ticTacToeController.CheckHorizontalRows(CellType.Circle, currentState.currentGrid))
        {
            reward += 1;
        } /*else if (ticTacToeController.CheckDiagonal(CellType.Cross, currentState.currentGrid) ||
                    ticTacToeController.CheckVerticalRows(CellType.Cross, currentState.currentGrid) ||
                    ticTacToeController.CheckHorizontalRows(CellType.Cross, currentState.currentGrid))
        {
            reward -= 1000;
        }
        else
        {
            reward -= 1;
        }
        
        int count = 0;
        for (int i = 0; i < 3; ++i)
        {
            for (int j = 0; j < 3; ++j)
            {
                if (!ticTacToeController.GridIsEmpty(new Vector3(i, .5f, j),currentState.currentGrid))
                {
                    ++count;
                }
            }
        }

        if (count == 9)
        {
            //Debug.Log("Match nul !!! Bande de nullos hihihih");
            reward -= 500;
        }*/
        
        return reward;
    }

    #endregion
    
    
    #region MonteCarloUtils

    private State GetNextState(State currentState, Intent intent)
    {
        Cell[][] grid = CopyGrid(currentState.currentGrid);

        grid = ApplyIntentToGrid(intent, grid, CellType.Circle);

        /*for (int i = 0; i < grid.Length; i++)
        {
            for (int j = 0; j < grid[0].Length; j++)
            {
                Debug.Log(grid[i][j].cellTicTacToeType);
            }
        }*/

        State nextState = GetStateFromGrid(grid);
        if (nextState!=null)
        {
            //Debug.Log("pasnew");
            return nextState;
        }
        else
        {
            //Debug.Log("new");

            Intent rdmIntent = GetRandomValidIntent(grid);
            if (ticTacToeController.GridIsEmpty(ticTacToeController.GetPositionFromIntent(rdmIntent),grid))
            {
                Vector3 player1Pos = ticTacToeController.GetPositionFromIntent(rdmIntent);
                grid[(int) player1Pos.x][(int) player1Pos.z].cellTicTacToeType = CellType.Cross;
            }

            State newState = new State();
            newState.currentGrid = grid;
            newState.ticTacToePolicy = GetRandomValidIntent(grid);
            _allStates.Add(newState);
            return newState;
        }
    }
    
    private Dictionary<State, float> GetPossibleStatesFromIntent(State currentState, Intent intent)
    {
        Dictionary<State, float> possibleStates = new Dictionary<State, float>();

        Cell[][] simulatedGrid = CopyGrid(currentState.currentGrid);
        simulatedGrid = ApplyIntentToGrid(intent, simulatedGrid, CellType.Circle);
        
        int i = 0;
        int possibilities = 0;
        for (i = 0; i < 9; i++)
        {
            if (ticTacToeController.GridIsEmpty(ticTacToeController.GetPositionFromIntent((Intent) i), simulatedGrid))
            {
                ++possibilities;
            }
        }
        
        for (i = 0; i < 9; i++)
        {
            if (ticTacToeController.GridIsEmpty(ticTacToeController.GetPositionFromIntent((Intent) i), simulatedGrid))
            {
                
                Cell[][] copyGrid = CopyGrid(simulatedGrid);
                copyGrid = ApplyIntentToGrid((Intent) i, copyGrid, CellType.Cross);
                
                State possibleState = GetStateFromGrid(copyGrid);
                if (possibleState == null)
                {
                    possibleState = new State()
                    {
                        stateValue = 0.0f,
                        currentGrid = copyGrid,
                        ticTacToePolicy = GetRandomValidIntent(copyGrid),
                        nS = 0.0f,
                        returnS = 0.0f
                    };
                }

                possibleStates.Add(possibleState, 1.0f / possibilities);
            }
        }

        return possibleStates;
    }
    
    private Intent GetBestIntent(State currentState)
    {
        float max = float.MinValue;
        Intent bestIntent = currentState.ticTacToePolicy;
        for (int i = 0; i < 9; ++i)
        {
            if (ticTacToeController.GridIsEmpty(ticTacToeController.GetPositionFromIntent((Intent)i), currentState.currentGrid))
            {
                Dictionary<State, float> possibleStates = GetPossibleStatesFromIntent(currentState, (Intent) i);

                float totalValue = 0;
                foreach (var possibleState in possibleStates)
                {
                    totalValue += possibleState.Key.stateValue * possibleState.Value;
                }
                
                if ( totalValue > max)
                {
                    max = totalValue;
                    bestIntent = (Intent) i;
                }
            }
        }

        return bestIntent;
    }
    
    private Intent GetRandomValidIntent(Cell[][] currentGrid)
    {
        Intent rdmIntent = Intent.Tile0 ;
        bool intentValid;
        int iter = 0;
        do
        {
            ++iter;
            rdmIntent = (Intent) Random.Range(0, 9);
            intentValid = ticTacToeController.GridIsEmpty(ticTacToeController.GetPositionFromIntent(rdmIntent), currentGrid);
        } while (!intentValid && iter < 20);
        return rdmIntent;
    }
    
    private Cell[][] ApplyIntentToGrid(Intent intent, Cell[][] grid, CellType type)
    {
        switch (intent)
        {
            case Intent.Tile0:
                grid[0][2].cellTicTacToeType = type; 
                break;
            case Intent.Tile1:
                grid[1][2].cellTicTacToeType = type;
                break;
            case Intent.Tile2:
                grid[2][2].cellTicTacToeType = type;
                break;
            case Intent.Tile3:
                grid[0][1].cellTicTacToeType =  type;
                break;
            case Intent.Tile4:
                grid[1][1].cellTicTacToeType = type;
                break;
            case Intent.Tile5:
                grid[2][1].cellTicTacToeType = type;
                break;
            case Intent.Tile6:
                grid[0][0].cellTicTacToeType = type;
                break;
            case Intent.Tile7:
                grid[1][0].cellTicTacToeType = type;
                break;
            case Intent.Tile8:
                grid[2][0].cellTicTacToeType = type;
                break;
        }
        
        return grid;
    }
    
    private State GetStateFromGrid(Cell[][] grid)
    {
        foreach (var state in _allStates)
        {
            int count = 0;
            int same = 0;
            for (int i = 0; i < grid.Length; i++)
            {
                for (int j = 0; j < grid[i].Length; j++)
                {
                    if(state.currentGrid[i][j].cellTicTacToeType == grid[i][j].cellTicTacToeType)
                        ++same;
                    
                    ++count;
                }
            }

            if (count == same)
                return state;
        }
        return null;
    }
    
    private Cell[][] CopyGrid(Cell[][] grid)
    {
        Cell[][] copyGrid = new Cell[grid.Length][];
        for (int i = 0; i < grid.Length; ++i)
        {
            copyGrid[i] = new Cell[grid[0].Length];
            for (int j = 0; j < grid[0].Length; j++)
            {
                copyGrid[i][j] = new Cell {cellTicTacToeType = grid[i][j].cellTicTacToeType};
            }
        }

        return copyGrid;
    }

    
    #endregion
    
    
    #region UnityUtils
    
    private void DebugGrid(Cell[][] grid)
    {
        for (int i = 0; i < 3; i++)
        {
            string debug = "";
            for (int j = 0; j < 3; j++)
            {
                debug += grid[i][j].cellTicTacToeType + " ";
            }
            Debug.Log(debug);
        }
    }
    
    #endregion
}
