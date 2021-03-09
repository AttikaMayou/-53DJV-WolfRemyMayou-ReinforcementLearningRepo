using System;
using System.Collections.Generic;
using UnityEngine;
using Intents = GridWorldController.Intents;
using Random = UnityEngine.Random;

public class Agent : MonoBehaviour
{
    [SerializeField] private GridWorldController gridWorldController;
    private List<State> allStates;
    [SerializeField] private DebuggerManager debugIntentParent;

    public void LaunchAgent()
    {
        //with policy iteration
        InitializePolicyIteration();
        Debug.Log("endinit");
        for (int i = 0; i < 100; ++i)
        {
            PolicyImprovement();
        }
        DebugIntents();
        Debug.Log("endImprovement");
        /*int iter = 0;
        while (gridWorldController.player.transform.position != gridWorldController.grid.endPos)
        {
            Debug.Log(gridWorldController.player.transform.position);
            State currentState = GetStateFromPos(gridWorldController.player.transform.position);
            Debug.Log(currentState);
            Debug.Log(currentState.statePolicy);
            switch (currentState.statePolicy)
            {
                case Intents.Down:
                    gridWorldController.DownIntent();
                    break;
                case Intents.Up:
                    gridWorldController.UpIntent();
                    break;
                case Intents.Left:
                    gridWorldController.LeftIntent();
                    break;
                case Intents.Right:
                    gridWorldController.RightIntent();
                    break;
            }
            ++iter;
            if (iter > 1000)
            {
                break;
            }
        }
        Debug.Log("End");*/
    }
    
    public void InitializePolicyIteration()
    {
        Debug.Log("Initialization");
        allStates = new List<State>();
        for (int i = 0; i < gridWorldController.grid.gridHeight; ++i)
        {
            for (int j = 0; j < gridWorldController.grid.gridWidth; ++j)
            {
                State currentState = new State();
                currentState.currentPlayerPos = new Vector3(i, 0, j);
                if (currentState.currentPlayerPos == gridWorldController.grid.endPos)
                {
                    currentState.stateValue = 1000.0f;
                }
                else
                {
                    currentState.stateValue = 0;
                }
                allStates.Add(currentState);
            }
        }
        Debug.Log("Number of states : " + allStates.Count);

        foreach (var currentState in allStates)
        {
            Intents wantedIntent= (Intents) Random.Range(0, 3);
            currentState.statePolicy = wantedIntent;
        }
    }
    
    public void PolicyEvaluation()
    {
        float delta;
        float theta = 0.1f;
        float gamma = 0.7f;
        do
        {
            delta = 0;
            foreach (var currentState in allStates)
            {
                if (currentState.currentPlayerPos != gridWorldController.grid.endPos && GetCellType(currentState.currentPlayerPos) != Cell.CellType.Obstacle)
                {
                    float temp = currentState.stateValue;
                    float v_S = 0;
                    foreach (var possibleStateDict in GetPossibleStatesFromIntent(currentState,
                        currentState.statePolicy))
                    {
                        v_S += possibleStateDict.Value *
                               (Reward(possibleStateDict.Key) + gamma * possibleStateDict.Key.stateValue);
                    }
                    
                    currentState.stateValue = v_S;
                    if (GetCellType(currentState.currentPlayerPos) == Cell.CellType.Hole)
                    {
                        currentState.stateValue *= -1;
                    }
                    delta = Mathf.Max(delta, Mathf.Abs(temp - currentState.stateValue));
                } 
            }
        } while (delta >= theta);
    }

    public bool PolicyImprovement()
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
            Debug.Log("unstable");
            PolicyEvaluation();
        }

        return policyStable;
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
        float max = float.MinValue;
        Intents bestIntent = currentState.statePolicy;
        for (int i = 0; i < 4; ++i)
        {
            if (CheckIntent(currentState, (Intents) i))
            {
                State tempState = GetNextState(currentState, (Intents) i);
                if ( tempState.stateValue > max)
                {
                    max = tempState.stateValue;
                    bestIntent = (Intents) i;
                }
            }
        }
        
        Debug.Log("maxValue : " + max);
        return bestIntent;
    }

    public float GetBestValue(State currentState)
    {
        float max = float.MinValue;
        for (int i = 0; i < 4; ++i)
        {
            if (CheckIntent(currentState, (Intents) i))
            {
                State tempState = GetNextState(currentState, (Intents) i);
                if ( tempState.stateValue > max)
                {
                    max = tempState.stateValue;
                }
            }
        }
        Debug.Log("maxValue : " + max);
        return max;
    }

    public void ValueIteration()
    {
        float delta;
        float theta = 0.01f;
        float gamma = 0.7f;
        allStates = new List<State>();
        for (int i = 0; i < gridWorldController.grid.gridHeight; ++i)
        {
            for (int j = 0; j < gridWorldController.grid.gridWidth; ++j)
            {
                State currentState = new State();
                currentState.stateValue = 0;
            }
        }

        do
        {
            delta = 0;
            foreach (var currentState in allStates)
            {
                float temp = currentState.stateValue;
                currentState.stateValue = GetBestValue(currentState);
                delta = Mathf.Max(delta, Mathf.Abs(temp - currentState.stateValue));
            }
        } while(delta >= theta);

        foreach (var currentState in allStates)
        {
            currentState.statePolicy = GetBestIntent(currentState);
        }
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
                //Debug.Log("found");
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

    public void DebugIntents()
    {
        debugIntentParent.ClearIntents();

        foreach (var state in allStates)
        {
            GameObject arrow;
            switch (state.statePolicy)
            {
                case Intents.Down:
                    arrow = Instantiate(gridWorldController.grid.downArrow, state.currentPlayerPos, Quaternion.identity);
                    arrow.transform.SetParent(debugIntentParent.transform);
                    break;
                case Intents.Up:
                    arrow = Instantiate(gridWorldController.grid.upArrow, state.currentPlayerPos, Quaternion.identity);
                    arrow.transform.SetParent(debugIntentParent.transform);
                    break;
                case Intents.Left:
                    arrow = Instantiate(gridWorldController.grid.leftArrow, state.currentPlayerPos, Quaternion.identity);
                    arrow.transform.SetParent(debugIntentParent.transform);
                    break;
                case Intents.Right:
                    arrow = Instantiate(gridWorldController.grid.rightArrow, state.currentPlayerPos,
                        Quaternion.identity);
                    arrow.transform.SetParent(debugIntentParent.transform);
                    break;
            }
            
            GameObject valueText = Instantiate(gridWorldController.grid.valueObject, state.currentPlayerPos + Vector3.up, Quaternion.Euler(90,0,0));
            valueText.GetComponent<TextMesh>().text = (Mathf.Floor(state.stateValue*100)/100).ToString();
            valueText.transform.SetParent(debugIntentParent.transform);
        }
    }
}
