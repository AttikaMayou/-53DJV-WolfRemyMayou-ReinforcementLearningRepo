using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AgentPolicy : MonoBehaviour
{
    [SerializeField] private GridWorldController gridWorldController;
    private List<State> _allStates;
    [SerializeField] private DebuggerManager debugIntentParent;

    public void LaunchAgent(AgentSelector.AgentType algo)
    {
        switch (algo)
        {
            case AgentSelector.AgentType.PolicyIteration:
                InitializePolicyIteration();
                for (int i = 0; i < 10; ++i)
                {
                    PolicyImprovement();
                }
                break;
            case AgentSelector.AgentType.ValueIteration:
                InitializeValueIteration();
                for (int i = 0; i < 1; ++i)
                {
                    ValueIteration();
                }
                break;
        }

        DebugIntents();
        
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
        _allStates = new List<State>();
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
                _allStates.Add(currentState);
            }
        }
        Debug.Log("Number of states : " + _allStates.Count);

        foreach (var currentState in _allStates)
        {
            GridWorldController.GridWorldIntent wantedGridWorldIntent= (GridWorldController.GridWorldIntent) Random.Range(0, 3);
            currentState.gridWorldPolicy = wantedGridWorldIntent;
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
            foreach (var currentState in _allStates)
            {
                if (currentState.currentPlayerPos != gridWorldController.grid.endPos && GetCellType(currentState.currentPlayerPos) != Cell.CellGridWorldType.Obstacle)
                {
                    float temp = currentState.stateValue;
                    float v_S = 0;
                    foreach (var possibleStateDict in GetPossibleStatesFromIntent(currentState,
                        currentState.gridWorldPolicy))
                    {
                        v_S += possibleStateDict.Value *
                               (Reward(possibleStateDict.Key) + gamma * possibleStateDict.Key.stateValue);
                    }
                    
                    currentState.stateValue = v_S;
                    if (GetCellType(currentState.currentPlayerPos) == Cell.CellGridWorldType.Hole)
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
        foreach (var currentState in _allStates)
        {
            GridWorldController.GridWorldIntent tempPolicy = currentState.gridWorldPolicy;
            currentState.gridWorldPolicy = GetBestIntent(currentState);
            if (tempPolicy != currentState.gridWorldPolicy)
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
        if (GetCellType(nextState.currentPlayerPos) == Cell.CellGridWorldType.Obstacle)
        {
            return result - 1000;
        }
        return result;
    }

    public Dictionary<State, float> GetPossibleStatesFromIntent(State currentState, GridWorldController.GridWorldIntent gridWorldIntent)
    {
        Dictionary<State, float> possibleStates = new Dictionary<State, float>();
        possibleStates.Add(GetNextState(currentState, gridWorldIntent),1.0f);
        return possibleStates;
    }
    
    public GridWorldController.GridWorldIntent GetBestIntent(State currentState)
    {
        float max = float.MinValue;
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
        
        Debug.Log("maxValue : " + max);
        return bestGridWorldIntent;
    }

    public float GetBestValue(State currentState)
    {
        float max = float.MinValue;
        for (int i = 0; i < 4; ++i)
        {
            if (CheckIntent(currentState, (GridWorldController.GridWorldIntent) i))
            {
                State tempState = GetNextState(currentState, (GridWorldController.GridWorldIntent) i);
                if ( tempState.stateValue > max)
                {
                    max = tempState.stateValue;
                }
            }
        }
        Debug.Log("maxValue : " + max);
        return max;
    }

    public void InitializeValueIteration()
    {
        _allStates = new List<State>();
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
                _allStates.Add(currentState);
            }
        }
    }
    public void ValueIteration()
    {
        float delta;
        float theta = 0.01f;
        float gamma = 0.7f;
        do
        {
            delta = 0;
            foreach (var currentState in _allStates)
            {
                if (currentState.currentPlayerPos != gridWorldController.grid.endPos &&
                    GetCellType(currentState.currentPlayerPos) != Cell.CellGridWorldType.Obstacle)
                {
                    float temp = currentState.stateValue;
                    //currentState.stateValue = GetBestValue(currentState);
                    
                    float v_S = 0;
                    foreach (var possibleStateDict in GetPossibleStatesFromIntent(currentState,GetBestIntent(currentState)))
                    {
                        v_S += possibleStateDict.Value *
                               (Reward(possibleStateDict.Key) + gamma * possibleStateDict.Key.stateValue);
                    }

                    currentState.stateValue = v_S;
                    if (GetCellType(currentState.currentPlayerPos) == Cell.CellGridWorldType.Hole)
                    {
                        currentState.stateValue *= -1;
                    }
                    delta = Mathf.Max(delta, Mathf.Abs(temp - currentState.stateValue));
                }
            }
        } while(delta >= theta);

        foreach (var currentState in _allStates)
        {
            currentState.gridWorldPolicy = GetBestIntent(currentState);
        }
    }
    
    public State GetNextState(State currentState, GridWorldController.GridWorldIntent gridWorldIntent)
    {
        Vector3 nextPlayerPos = Vector3.zero;
        switch (gridWorldIntent)
        {
            case GridWorldController.GridWorldIntent.Down:
                nextPlayerPos = currentState.currentPlayerPos - Vector3.forward;
                break;
            case GridWorldController.GridWorldIntent.Up:
                nextPlayerPos = currentState.currentPlayerPos + Vector3.forward;
                break;
            case GridWorldController.GridWorldIntent.Left:
                nextPlayerPos = currentState.currentPlayerPos + Vector3.left;
                break;
            case GridWorldController.GridWorldIntent.Right:
                nextPlayerPos = currentState.currentPlayerPos - Vector3.left;
                break;
        }
        return GetStateFromPos(nextPlayerPos);
    }

    public State GetStateFromPos(Vector3 pos)
    {
        foreach (var state in _allStates)
        {
            if (state.currentPlayerPos == pos)
            {
                //Debug.Log("found");
                return state;
            }
        }
        return null;
    }

    public Cell.CellGridWorldType GetCellType(Vector3 pos)
    {
        return gridWorldController.grid.grid[(int)pos.x][(int)pos.z].cellGridWorldType;
    }

    public bool CheckIntent(State currentState, GridWorldController.GridWorldIntent wantedGridWorldIntent)
    {
        if (GetNextState(currentState, wantedGridWorldIntent) == null)
        {
            return false;
        }
        if (GetCellType(GetNextState(currentState, wantedGridWorldIntent).currentPlayerPos) == Cell.CellGridWorldType.Obstacle)
        {
            return false;
        }
        return true;
    }

    public void DebugIntents()
    {
        debugIntentParent.ClearIntents();

        foreach (var state in _allStates)
        {
            GameObject arrow;
            switch (state.gridWorldPolicy)
            {
                case GridWorldController.GridWorldIntent.Down:
                    arrow = Instantiate(gridWorldController.downArrow, state.currentPlayerPos, Quaternion.identity);
                    arrow.transform.SetParent(debugIntentParent.transform);
                    break;
                case GridWorldController.GridWorldIntent.Up:
                    arrow = Instantiate(gridWorldController.upArrow, state.currentPlayerPos, Quaternion.identity);
                    arrow.transform.SetParent(debugIntentParent.transform);
                    break;
                case GridWorldController.GridWorldIntent.Left:
                    arrow = Instantiate(gridWorldController.leftArrow, state.currentPlayerPos, Quaternion.identity);
                    arrow.transform.SetParent(debugIntentParent.transform);
                    break;
                case GridWorldController.GridWorldIntent.Right:
                    arrow = Instantiate(gridWorldController.rightArrow, state.currentPlayerPos,
                        Quaternion.identity);
                    arrow.transform.SetParent(debugIntentParent.transform);
                    break;
            }
            
            GameObject valueText = Instantiate(gridWorldController.grid.valueObject, (state.currentPlayerPos + Vector3.up) - new Vector3(0.1f, 0, 0.3f), Quaternion.Euler(90,0,0));
            valueText.GetComponent<TextMesh>().text = (Mathf.Floor(state.stateValue*100)/100).ToString();
            valueText.transform.SetParent(debugIntentParent.transform);
        }
    }
}
