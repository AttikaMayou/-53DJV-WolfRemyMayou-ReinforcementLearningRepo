using System;
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

    public void LaunchAgent(AgentSelector.AgentType algo)
    {
        switch (algo)
        {
           case AgentSelector.AgentType.MonteCarloES:
                break;
            case AgentSelector.AgentType.MonteCarloESEvery:
                break;
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
                currentState.ticTacToePolicy = GetBestIntent(currentState);
            }
        }
        
        if (!onPolicy)
        { 
            foreach (var state in _allStates)
            {
                state.stateValue = state.returnS / state.nS;
            }
            currentState.ticTacToePolicy = GetBestIntent(currentState);
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
    }

    private void SimulateGame(State currentState, bool player1 = true)
    {
        bool gameOver = false;
        do
        {
            SAR newSar = new SAR();
            newSar.state = GetNextState(currentState, currentState.ticTacToePolicy, true);
            newSar.intent = newSar.state.ticTacToePolicy;
            newSar.reward = GetReward(player1 ? CellType.Cross : CellType.Circle, newSar.state);
            _simulatedSARs.Add(newSar);
            if (newSar.reward >= 1000 || newSar.reward <= -500)
            {
                gameOver = true;
            }
            player1 = !player1;
        } while (!gameOver);
    }

    private float GetReward(CellType type, State currentState, bool player1 = true)
    {
        int reward = 0;
        if (ticTacToeController.CheckDiagonal(CellType.Circle, currentState.currentGrid) ||
            ticTacToeController.CheckVerticalRows(CellType.Circle, currentState.currentGrid) ||
            ticTacToeController.CheckHorizontalRows(CellType.Circle, currentState.currentGrid))
        {
            if (type == CellType.Circle)
            {
                if (player1)
                    reward += 1000;
                else
                    reward -= 1000;
            }
            else
            {
                if (player1)
                    reward -= 1000;
                else
                    reward += 1000;
            }
        } else if (ticTacToeController.CheckDiagonal(CellType.Cross, currentState.currentGrid) ||
                    ticTacToeController.CheckVerticalRows(CellType.Cross, currentState.currentGrid) ||
                    ticTacToeController.CheckHorizontalRows(CellType.Cross, currentState.currentGrid))
        {
            if (type == CellType.Cross)
            {
                if (player1)
                    reward += 1000;
                else
                    reward -= 1000;
            }
            else
            {
                if (player1)
                    reward -= 1000;
                else
                    reward += 1000;
            }
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

    public State GetNextState(State currentState, Intent intent, bool player1 = true)
    {
        Cell[][] grid = new Cell[currentState.currentGrid.Length][];
        for (int i = 0; i < currentState.currentGrid.Length; ++i)
        {
            grid[i] = new Cell[currentState.currentGrid[0].Length];
            for (int j = 0; j < currentState.currentGrid[0].Length; j++)
            {
                grid[i][j] = currentState.currentGrid[i][j];
            }
        }

        switch (intent)
        {
            case Intent.Tile0:
                grid[0][0].cellTicTacToeType = player1 ? CellType.Cross : CellType.Circle; 
                break;
            case Intent.Tile1:
                grid[0][1].cellTicTacToeType = player1 ? CellType.Cross : CellType.Circle;
                break;
            case Intent.Tile2:
                grid[0][2].cellTicTacToeType = player1 ? CellType.Cross : CellType.Circle;
                break;
            case Intent.Tile3:
                grid[1][0].cellTicTacToeType = player1 ? CellType.Cross : CellType.Circle;
                break;
            case Intent.Tile4:
                grid[1][1].cellTicTacToeType = player1 ? CellType.Cross : CellType.Circle;
                break;
            case Intent.Tile5:
                grid[1][2].cellTicTacToeType = player1 ? CellType.Cross : CellType.Circle;
                break;
            case Intent.Tile6:
                grid[2][0].cellTicTacToeType = player1 ? CellType.Cross : CellType.Circle;
                break;
            case Intent.Tile7:
                grid[2][1].cellTicTacToeType = player1 ? CellType.Cross : CellType.Circle;
                break;
            case Intent.Tile8:
                grid[2][2].cellTicTacToeType = player1 ? CellType.Cross : CellType.Circle;
                break;
        }

        State nextState = GetStateFromGrid(grid);
        if (nextState!=null)
        {
            return nextState;
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
                intentValid = ticTacToeController.GridIsEmpty(ticTacToeController.GetPositionFromIntent(rdmIntent),grid);
            } while (!intentValid && iter < 20);
            
            State newState = new State();
            newState.currentGrid = grid;
            newState.ticTacToePolicy = rdmIntent;
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
                    if(state.currentGrid[i][j] == grid[i][j])
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
