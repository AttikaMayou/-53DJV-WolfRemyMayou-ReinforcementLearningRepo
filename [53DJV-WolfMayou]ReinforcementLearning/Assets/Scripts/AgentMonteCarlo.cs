using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using Intent = TicTacToeController.TicTacToeIntent;

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
        }

        foreach (var state in _allStates)
        {
            state.stateValue = state.returnS / state.nS;
        }
    }

    private void GetBestIntent(State currentState)
    {
        /*float max = float.MinValue;
        GridWorldController.GridWorldIntent bestGridWorldIntent = currentState.gridWorldPolicy;
        for (int i = 0; i < 4; ++i)
        {
            if (CheckIntent(currentState, (GridWorldController.GridWorldIntent) i))
            {
                State tempState = GetNextState(currentState, (GridWorldController.GridWorldIntent) i);
                if ( tempState.stateValue > max)
                {
                    max = tempState.stateValue;
                    bestGridWorldIntent = (GridWorldController.GridWorldIntent) i;
                }
            }
        }
        
        return bestGridWorldIntent;*/
    }

    private void InitializeMonteCarlo()
    {
        _allStates = new List<State>();
    }

    private void SimulateGame(State currentState)
    {
        bool gameOver = false;
        do
        {
            bool intentValid = false;

            SAR newSar = new SAR();
            newSar.state = GetNextState(currentState, currentState.ticTacToePolicy, true);
            newSar.intent = newSar.state.ticTacToePolicy;
            newSar.reward = GetReward(Cell.CellTicTacToeType.Cross,newSar.state);
            _simulatedSARs.Add(newSar);
            if (newSar.reward >= 1000 || newSar.reward <= -500)
            {
                gameOver = true;
            }
        } while (!gameOver);
    }

    private float GetReward(Cell.CellTicTacToeType type, State currentState)
    {
        int reward = 0;
        if (ticTacToeController.CheckDiagonal(Cell.CellTicTacToeType.Circle, currentState.currentGrid) ||
            ticTacToeController.CheckVerticalRows(Cell.CellTicTacToeType.Circle, currentState.currentGrid) ||
            ticTacToeController.CheckHorizontalRows(Cell.CellTicTacToeType.Circle, currentState.currentGrid))
        {
            if (type == Cell.CellTicTacToeType.Circle)
            {
                reward += 1000;
            }
            else
            {
                reward -= 1000;
            }
        } else if (ticTacToeController.CheckDiagonal(Cell.CellTicTacToeType.Cross, currentState.currentGrid) ||
                    ticTacToeController.CheckVerticalRows(Cell.CellTicTacToeType.Cross, currentState.currentGrid) ||
                    ticTacToeController.CheckHorizontalRows(Cell.CellTicTacToeType.Cross, currentState.currentGrid))
        {
            if (type == Cell.CellTicTacToeType.Cross)
            {
                reward += 1000;
            }
            else
            {
                reward -= 1000;
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
        Cell[][] grid = currentState.currentGrid;
        switch (intent)
        {
            case Intent.Tile0:
                grid[0][0].cellTicTacToeType = player1 ? Cell.CellTicTacToeType.Cross : Cell.CellTicTacToeType.Circle; 
                break;
            case Intent.Tile1:
                grid[0][1].cellTicTacToeType = player1 ? Cell.CellTicTacToeType.Cross : Cell.CellTicTacToeType.Circle;
                break;
            case Intent.Tile2:
                grid[0][2].cellTicTacToeType = player1 ? Cell.CellTicTacToeType.Cross : Cell.CellTicTacToeType.Circle;
                break;
            case Intent.Tile3:
                grid[1][0].cellTicTacToeType = player1 ? Cell.CellTicTacToeType.Cross : Cell.CellTicTacToeType.Circle;
                break;
            case Intent.Tile4:
                grid[1][1].cellTicTacToeType = player1 ? Cell.CellTicTacToeType.Cross : Cell.CellTicTacToeType.Circle;
                break;
            case Intent.Tile5:
                grid[1][2].cellTicTacToeType = player1 ? Cell.CellTicTacToeType.Cross : Cell.CellTicTacToeType.Circle;
                break;
            case Intent.Tile6:
                grid[2][0].cellTicTacToeType = player1 ? Cell.CellTicTacToeType.Cross : Cell.CellTicTacToeType.Circle;
                break;
            case Intent.Tile7:
                grid[2][1].cellTicTacToeType = player1 ? Cell.CellTicTacToeType.Cross : Cell.CellTicTacToeType.Circle;
                break;
            case Intent.Tile8:
                grid[2][2].cellTicTacToeType = player1 ? Cell.CellTicTacToeType.Cross : Cell.CellTicTacToeType.Circle;
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
                intentValid = ticTacToeController.ProcessIntent(rdmIntent, true);
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
            if (state.currentGrid == grid)
                return state;
        }
        return null;
    }
}
