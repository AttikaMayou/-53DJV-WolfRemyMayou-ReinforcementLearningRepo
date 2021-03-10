using System;
using System.Collections.Generic;
using System.Linq;
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

    public float Epsilon = 0.8f;
    private bool monteCarloInitialized;

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
        if (!monteCarloInitialized)
        {
            monteCarloInitialized = true;
            InitializeMonteCarlo();
        }
        Vector3 mouse = Input.mousePosition;
        Ray castPoint = Camera.main.ScreenPointToRay(mouse);
        RaycastHit hit;
        Debug.Log(!ticTacToeController._gameIsOver);
        if (!ticTacToeController._gameIsOver && Physics.Raycast(castPoint, out hit, Mathf.Infinity))
        {
            float x = Mathf.Round(hit.point.x);
            float y = 0.5f;
            float z = Mathf.Round(hit.point.z);
            

            if (!ticTacToeController._gameIsOver && ticTacToeController.Place(ticTacToeController.cross, new Vector3(x, y, z), ticTacToeController.crossGridMaterial, CellType.Cross))
            {

                State playedState = GetStateFromGrid(ticTacToeController.grid.grid);
                
                if (playedState == null)
                {
                    playedState = new State();
                    playedState.currentGrid = ticTacToeController.grid.grid;
                    playedState.ticTacToePolicy = GetRandomValidIntent(ticTacToeController.grid.grid);
                    _allStates.Add(playedState);
                }
                MonteCarloPrediction(playedState, 10, false, false);
                ticTacToeController.Place(ticTacToeController.circle,
                    ticTacToeController.GetPositionFromIntent(playedState.ticTacToePolicy),
                    ticTacToeController.circleGridMaterial, CellType.Circle);
            }
        }
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
            float G = 0;
            for (int j = _simulatedSARs.Count-2; j >= 0; --j)
            {
                G += _simulatedSARs[j + 1].reward;
                
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
                    _simulatedSARs[j].state.returnS += G;
                    ++_simulatedSARs[j].state.nS;
                }
            }

            if (onPolicy)
            {
                foreach (var state in _allStates)
                {
                    state.stateValue = state.returnS / state.nS;
                }

                if (Random.Range(0.0f, 1.0f) < Epsilon)
                {
                    currentState.ticTacToePolicy = GetRandomValidIntent(currentState.currentGrid);
                }
                else
                {
                    currentState.ticTacToePolicy = GetBestIntent(currentState);
                }
            }
        }
        
        if (!onPolicy)
        { 
            foreach (var state in _allStates)
            {
                state.stateValue = state.returnS / state.nS;
            }
            if (Random.Range(0.0f, 1.0f) < Epsilon)
            {
                currentState.ticTacToePolicy = GetRandomValidIntent(currentState.currentGrid);
            }
            else
            {
                currentState.ticTacToePolicy = GetBestIntent(currentState);
            }
        }
    }

    private Intent GetBestIntent(State currentState)
    {
        float max = float.MinValue;
        Intent bestIntent = currentState.ticTacToePolicy;
        for (int i = 0; i < 9; ++i)
        {
           if (ticTacToeController.GridIsEmpty(ticTacToeController.GetPositionFromIntent((Intent)i), currentState.currentGrid))
           {
               State tempState = GetNextState(currentState, (Intent) i);
               if ( tempState.stateValue > max)
               {
                   max = tempState.stateValue;
                   bestIntent = (Intent) i;
               }
           }
        }

        return bestIntent;
    }

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
                
                newState.currentGrid[i][j] = new Cell();
                newState.currentGrid[i][j].cellTicTacToeType = CellType.Neutral;
            }
        }

        newState.currentGrid[Random.Range(0, 3)][Random.Range(0, 3)].cellTicTacToeType = CellType.Cross; 
        newState.nS = 0;
        newState.returnS = 0;
        newState.ticTacToePolicy = (Intent) Random.Range(0, 9);
        newState.stateValue = 0;
        _allStates.Add(newState);
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
            if (ticTacToeController.GridIsEmpty(ticTacToeController.GetPositionFromIntent(tmpState.ticTacToePolicy), tmpState.currentGrid))
            {
                newSar.state = GetNextState(tmpState, tmpState.ticTacToePolicy);
            }
            else
            {
                Intent rdmIntent;
                bool intentValid;
                int iter = 0;
                do
                {
                    ++iter;
                    rdmIntent = (Intent) Random.Range(0, 9);
                    intentValid = ticTacToeController.GridIsEmpty(ticTacToeController.GetPositionFromIntent(rdmIntent),tmpState.currentGrid);
                } while (!intentValid && iter < 20);
                tmpState.ticTacToePolicy = rdmIntent;
                newSar.state = GetNextState(tmpState, tmpState.ticTacToePolicy);
            }

            newSar.intent = newSar.state.ticTacToePolicy;
            newSar.reward = GetReward(newSar.state);
            Debug.Log(newSar.reward);
            _simulatedSARs.Add(newSar);
            if (newSar.reward >= 1000 || newSar.reward <= -500)
            {
                gameOver = true;
            }

            tmpState = newSar.state;
        } while (!gameOver && security < 100);
    }

    private float GetReward(State currentState)
    {
        int reward = 0;
        if (ticTacToeController.CheckDiagonal(CellType.Circle, currentState.currentGrid) ||
            ticTacToeController.CheckVerticalRows(CellType.Circle, currentState.currentGrid) ||
            ticTacToeController.CheckHorizontalRows(CellType.Circle, currentState.currentGrid))
        {
            reward += 1000;
        } else if (ticTacToeController.CheckDiagonal(CellType.Cross, currentState.currentGrid) ||
                    ticTacToeController.CheckVerticalRows(CellType.Cross, currentState.currentGrid) ||
                    ticTacToeController.CheckHorizontalRows(CellType.Cross, currentState.currentGrid))
        {
            reward -= 1000;
        }
        else
        {
            reward = 1;
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
            Debug.Log("Match nul !!! Bande de nullos hihihih");
            reward -= 500;
        }
        
        return reward;
    }

    public State GetNextState(State currentState, Intent intent)
    {
        Cell[][] grid = new Cell[currentState.currentGrid.Length][];
        for (int i = 0; i < currentState.currentGrid.Length; ++i)
        {
            grid[i] = new Cell[currentState.currentGrid[0].Length];
            for (int j = 0; j < currentState.currentGrid[0].Length; j++)
            {
                grid[i][j] = new Cell();
                grid[i][j].cellTicTacToeType = currentState.currentGrid[i][j].cellTicTacToeType;
            }
        }
        
        switch (intent)
        {
            case Intent.Tile0:
                grid[0][0].cellTicTacToeType = CellType.Circle; 
                break;
            case Intent.Tile1:
                grid[0][1].cellTicTacToeType = CellType.Circle;
                break;
            case Intent.Tile2:
                grid[0][2].cellTicTacToeType = CellType.Circle;
                break;
            case Intent.Tile3:
                grid[1][0].cellTicTacToeType =  CellType.Circle;
                break;
            case Intent.Tile4:
                grid[1][1].cellTicTacToeType = CellType.Circle;
                break;
            case Intent.Tile5:
                grid[1][2].cellTicTacToeType = CellType.Circle;
                break;
            case Intent.Tile6:
                grid[2][0].cellTicTacToeType = CellType.Circle;
                break;
            case Intent.Tile7:
                grid[2][1].cellTicTacToeType = CellType.Circle;
                break;
            case Intent.Tile8:
                grid[2][2].cellTicTacToeType = CellType.Circle;
                break;
        }

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
                Debug.Log("cross added");
            }

            State newState = new State();
            newState.currentGrid = grid;
            newState.ticTacToePolicy = GetRandomValidIntent(grid);
            _allStates.Add(newState);
            return newState;
        }
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
}
