using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using Intent = GridWorldController.GridWorldIntent;
using CellType = Cell.CellGridWorldType;

public class AgentPolicy : MonoBehaviour
{
    [SerializeField] private GridWorldController gridWorldController;
    private List<State> _allStates;
    [SerializeField] private DebuggerManager debugIntentParent;

    public void LaunchAgent(AgentSelector.AgentType algo, bool step = false)
    {
        switch (algo)
        {
            case AgentSelector.AgentType.PolicyIteration:
                if (!step)
                {
                    InitializePolicyIteration();
                    for (int i = 0; i < 10; ++i)
                    {
                        PolicyImprovement();
                    }
                }
                else
                {
                    if (!gridWorldController.grid.hasBeenInitialized)
                    {
                        InitializePolicyIteration();
                        gridWorldController.grid.hasBeenInitialized = true;
                    }
                    PolicyImprovement();
                    
                    gridWorldController.grid.debuggerManager.ClearIntents();
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
    }

    public void FollowPath()
    {
        int iter = 0;
        while (gridWorldController._player.transform.position-Vector3.up*0.5f != gridWorldController.grid.endPos)
        {
            Debug.Log(gridWorldController._player.transform.position);
            State currentState = GetStateFromPos(gridWorldController._player.transform.position-Vector3.up*0.5f);
            switch (currentState.gridWorldPolicy)
            {
                case Intent.Down:
                    gridWorldController.DownIntent();
                    break;
                case Intent.Up:
                    gridWorldController.UpIntent();
                    break;
                case Intent.Left:
                    gridWorldController.LeftIntent();
                    break;
                case Intent.Right:
                    gridWorldController.RightIntent();
                    break;
            }
            ++iter;
            if (iter > 100)
            {
                break;
            }
        }
        Debug.Log("End");
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
            Intent wantedGridWorldIntent= (Intent) Random.Range(0, 3);
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
                if (currentState.currentPlayerPos != gridWorldController.grid.endPos && GetCellType(currentState.currentPlayerPos) != CellType.Obstacle)
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
                    if (GetCellType(currentState.currentPlayerPos) == CellType.Hole)
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
            Intent tempPolicy = currentState.gridWorldPolicy;
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
        if (GetCellType(nextState.currentPlayerPos) == CellType.Obstacle)
        {
            return result - 1000;
        }
        return result;
    }

    public Dictionary<State, float> GetPossibleStatesFromIntent(State currentState, Intent gridWorldIntent)
    {
        Dictionary<State, float> possibleStates = new Dictionary<State, float>();
        possibleStates.Add(GetNextState(currentState, gridWorldIntent),1.0f);
        return possibleStates;
    }
    
    public Intent GetBestIntent(State currentState)
    {
        float max = float.MinValue;
        Intent bestGridWorldIntent = currentState.gridWorldPolicy;
        for (int i = 0; i < 4; ++i)
        {
            if (CheckIntent(currentState, (Intent) i))
            {
                State tempState = GetNextState(currentState, (Intent) i);
                if ( tempState.stateValue > max)
                {
                    max = tempState.stateValue;
                    bestGridWorldIntent = (Intent) i;
                }
            }
        }
        
        return bestGridWorldIntent;
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
                    GetCellType(currentState.currentPlayerPos) != CellType.Obstacle)
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
                    if (GetCellType(currentState.currentPlayerPos) == CellType.Hole)
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
    
    public State GetNextState(State currentState,Intent gridWorldIntent)
    {
        Vector3 nextPlayerPos = Vector3.zero;
        switch (gridWorldIntent)
        {
            case Intent.Down:
                nextPlayerPos = currentState.currentPlayerPos - Vector3.forward;
                break;
            case Intent.Up:
                nextPlayerPos = currentState.currentPlayerPos + Vector3.forward;
                break;
            case Intent.Left:
                nextPlayerPos = currentState.currentPlayerPos + Vector3.left;
                break;
            case Intent.Right:
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

    public CellType GetCellType(Vector3 pos)
    {
        return gridWorldController.grid.grid[(int)pos.x][(int)pos.z].cellGridWorldType;
    }

    public bool CheckIntent(State currentState, Intent wantedGridWorldIntent)
    {
        if (GetNextState(currentState, wantedGridWorldIntent) == null)
        {
            return false;
        }
        if (GetCellType(GetNextState(currentState, wantedGridWorldIntent).currentPlayerPos) == CellType.Obstacle)
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
                case Intent.Down:
                    arrow = Instantiate(gridWorldController.downArrow, state.currentPlayerPos, Quaternion.identity);
                    arrow.transform.SetParent(debugIntentParent.transform);
                    break;
                case Intent.Up:
                    arrow = Instantiate(gridWorldController.upArrow, state.currentPlayerPos, Quaternion.identity);
                    arrow.transform.SetParent(debugIntentParent.transform);
                    break;
                case Intent.Left:
                    arrow = Instantiate(gridWorldController.leftArrow, state.currentPlayerPos, Quaternion.identity);
                    arrow.transform.SetParent(debugIntentParent.transform);
                    break;
                case Intent.Right:
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
