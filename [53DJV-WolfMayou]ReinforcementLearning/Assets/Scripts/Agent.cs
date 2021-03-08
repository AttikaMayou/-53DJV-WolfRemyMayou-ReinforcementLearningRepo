using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Intents = GridWorldController.Intents;
using Random = UnityEngine.Random;

public class Agent : MonoBehaviour
{
    [SerializeField] private GridWorldController gridWorldController;
    private List<State> allStates;
    
    public void Initialize()
    {
        allStates = new List<State>();
        for (int i = 0; i < gridWorldController.grid.gridHeight; ++i)
        {
            for (int j = 0; j < gridWorldController.grid.gridWidth; ++j)
            {
                State currentState = new State();
                currentState.stateValue = Random.Range(-100.0f, 100.0f);
            }
        }

        foreach (var currentState in allStates)
        {
            Intents wantedIntent;
            do
            {
                wantedIntent = (Intents) Random.Range(0, 3);
            } while (!CheckIntent(currentState,wantedIntent));
            currentState.statePolicy = wantedIntent;
        }
    }

    public void PolicyEvaluation()
    {
        float delta;
        float theta = 0.01f;
        float gamma = 0.7f;
        do
        {
            delta = 0;
            foreach (var currentState in allStates)
            {
                float temp = currentState.stateValue; 
                float v_S = 0;
                foreach (var possibleStateDict in GetPossibleStatesFromIntent(currentState, currentState.statePolicy))
                {
                    v_S += possibleStateDict.Value * (Reward(possibleStateDict.Key) + gamma * possibleStateDict.Key.stateValue);
                }
                currentState.stateValue = v_S;
                delta = Mathf.Max(delta, Mathf.Abs(temp - currentState.stateValue));
            }
        } while (delta >= theta);
    }

    public void PublicyImprovement()
    {
        bool policyStable = true;
        foreach (var currentState in allStates)
        {
            Intents tempPolicy = currentState.statePolicy;
            currentState.statePolicy = GetBestIntent(currentState);
            if (tempPolicy != currentState.statePolicy)
            {
                policyStable = false;
            }
        }

        if (!policyStable)
        {
            PolicyEvaluation();
        }
    }

    public float Reward(State nextState)
    {
        float result = gridWorldController.grid.gridHeight * gridWorldController.grid.gridWidth - (Vector3.Distance(nextState.currentPlayerPos, gridWorldController.grid.endPos));
        if (GetCellType(nextState.currentPlayerPos) == Cell.CellType.Obstacle)
        {
            return result - 1000;
        }
        return result;
    }

    public Dictionary<State, float> GetPossibleStatesFromIntent(State currentState, Intents intent)
    {
        Dictionary<State, float> possibleStates = new Dictionary<State, float>();
        possibleStates.Add(GetNextState(currentState,intent),1.0f);
        return possibleStates;
    }
    
    public Intents GetBestIntent(State currentState)
    {
        List<State> possibleStates = new List<State>();
        float max = float.MinValue;
        Intents bestIntent = currentState.statePolicy;
        for (int i = 0; i < 4; ++i)
        {
            if (CheckIntent(currentState, (Intents) i))
            {
                State tempState = GetNextState(currentState, (Intents) i);
                possibleStates.Add(tempState);
                if ( tempState.stateValue > max)
                {
                    max = tempState.stateValue;
                    bestIntent = (Intents) i;
                }
            }
        }
        return bestIntent;
    }
    
    public State GetNextState(State currentState, Intents intent)
    {
        Vector3 nextPlayerPos = Vector3.zero;
        switch (intent)
        {
            case Intents.Down:
                nextPlayerPos = currentState.currentPlayerPos - Vector3.forward;
                break;
            case Intents.Up:
                nextPlayerPos = currentState.currentPlayerPos + Vector3.forward;
                break;
            case Intents.Left:
                nextPlayerPos = currentState.currentPlayerPos + Vector3.left;
                break;
            case Intents.Right:
                nextPlayerPos = currentState.currentPlayerPos - Vector3.left;
                break;
        }
        return GetStateFromPos(nextPlayerPos);
    }

    public State GetStateFromPos(Vector3 pos)
    {
        foreach (var state in allStates)
        {
            if (state.currentPlayerPos == pos)
            {
                return state;
            }
        }

        return null;
    }

    public Cell.CellType GetCellType(Vector3 pos)
    {
        return gridWorldController.grid.grid[(int)pos.x][(int)pos.z].type;
    }

    public bool CheckIntent(State currentState, Intents wantedIntent)
    {
        if (GetNextState(currentState, wantedIntent) == null)
        {
            return false;
        }
        if (GetCellType(GetNextState(currentState, wantedIntent).currentPlayerPos) == Cell.CellType.Obstacle)
        {
            return false;
        }
        return true;
    }
}
