using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using Intent = TicTacToeController.TicTacToeIntent;

public class AgentMonteCarlo : MonoBehaviour
{
    private class Episode
    {
        public State state;
        public Intent intent;
        public float reward;
    }
    
    [SerializeField] private TicTacToeController ticTacToeController;
    private List<State> _allStates;
    private List<Episode> _simulatedEpisodes;

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
        //LA ON FAIT TOUT ALEATOIRE MAIS IL FAUT COMMENCER PAR LA POLICY QUI SERAIT UN INTENT, PUIS FAIRE DE L'ALEATOIRE
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

            Episode newEpisode = new Episode();
            newEpisode.state = GetNextState(currentState, rdmIntent, true, false);
            newEpisode.intent = rdmIntent;
            newEpisode.reward = GetReward(Cell.CellTicTacToeType.Cross,newEpisode.state);
            _simulatedEpisodes.Add(newEpisode);
            if (newEpisode.reward >= 1000)
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
        
        return reward;
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
