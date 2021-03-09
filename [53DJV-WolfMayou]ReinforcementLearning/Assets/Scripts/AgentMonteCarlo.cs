using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using Intent = TicTacToeController.TicTacToeIntent;

public class AgentMonteCarlo : MonoBehaviour
{
    [SerializeField] private TicTacToeController ticTacToeController;
    private List<State> _allStates;
    private List<State> _simulatedStates;

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

    private float MonteCarloPrediction(State currentState, int iteration, bool everyVisit = false)
    {
        float v_S= 0.0f;

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
        }

        return v_S;
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
            Intent rdmIntent;
            do
            {
                rdmIntent = (Intent) Random.Range(0, 9);
                intentValid = ticTacToeController.ProcessIntent(rdmIntent, true);
            } while (!intentValid);

            _simulatedStates.Add(GetNextState(currentState, rdmIntent, true, false));
            
            gameOver = true;
        } while (!gameOver);
    }

    public State GetNextState(State currentState, Intent intent, bool player1 = true, bool reference = true)
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

        if (reference)
        {
            return GetStateFromGrid(grid);
        }
        else
        {
            State newState = new State();
            newState.currentGrid = grid;
            newState.ticTacToePolicy = intent;
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
