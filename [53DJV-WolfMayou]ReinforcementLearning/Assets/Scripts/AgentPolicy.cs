using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using GridWorldIntent = GridWorldController.GridWorldIntent;
using SokobanIntent = SokobanController.SokobanIntent;
using CellType = Cell.CellGridWorldType;

public class AgentPolicy : MonoBehaviour
{
    [SerializeField] private GridWorldController gridWorldController;
    [SerializeField] private SokobanController sokobanController;
    private List<State> _allStates;
    [SerializeField] private DebuggerManager debugIntentParent;

    public void LaunchAgent(AgentSelector.AgentType algo, bool step = false)
    {
        switch (algo)
        {
            case AgentSelector.AgentType.PolicyIteration:
                if (GameSelector.type == GameSelector.GameType.GridWorld)
                {
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
                }
                else if(GameSelector.type == GameSelector.GameType.Sokoban)
                {
                    InitializePolicyIteration();
                    for (int i = 0; i < 10; ++i)
                    {
                        PolicyImprovement();
                    }
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
        while (gridWorldController.player.transform.position-Vector3.up*0.5f != gridWorldController.grid.endPos)
        {
            Debug.Log(gridWorldController.player.transform.position);
            State currentState = GetStateFromPos(gridWorldController.player.transform.position-Vector3.up*0.5f);
            switch (currentState.gridWorldPolicy)
            {
                case GridWorldIntent.Down:
                    gridWorldController.DownIntent();
                    break;
                case GridWorldIntent.Up:
                    gridWorldController.UpIntent();
                    break;
                case GridWorldIntent.Left:
                    gridWorldController.LeftIntent();
                    break;
                case GridWorldIntent.Right:
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
        if (GameSelector.type == GameSelector.GameType.GridWorld)
        {
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
                GridWorldIntent wantedGridWorldIntent = (GridWorldIntent) Random.Range(0, 3);
                currentState.gridWorldPolicy = wantedGridWorldIntent;
            }
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
            if (GameSelector.type == GameSelector.GameType.GridWorld)
            {
                GridWorldIntent tempPolicy = currentState.gridWorldPolicy;
                currentState.gridWorldPolicy = GetBestIntent(currentState);
                if (tempPolicy != currentState.gridWorldPolicy)
                {
                    policyStable = false;
                }
            }
            else if (GameSelector.type == GameSelector.GameType.Sokoban)
            {
                SokobanIntent tempPolicy = currentState.sokobanPolicy;
                currentState.sokobanPolicy = GetBestIntentSokoban(currentState);
                if (tempPolicy != currentState.sokobanPolicy)
                {
                    policyStable = false;
                }
            }
        }

        if (!policyStable)
        {
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

    public Dictionary<State, float> GetPossibleStatesFromIntent(State currentState, GridWorldIntent gridWorldIntent)
    {
        Dictionary<State, float> possibleStates = new Dictionary<State, float>();
        possibleStates.Add(GetNextState(currentState, gridWorldIntent),1.0f);
        return possibleStates;
    }
    
    public GridWorldIntent GetBestIntent(State currentState)
    {
        float max = float.MinValue;
        GridWorldIntent bestGridWorldIntent = currentState.gridWorldPolicy;
        for (int i = 0; i < 4; ++i)
        {
            if (CheckIntent(currentState, (GridWorldIntent) i))
            {
                State tempState = GetNextState(currentState, (GridWorldIntent) i);
                if ( tempState.stateValue > max)
                {
                    max = tempState.stateValue;
                    bestGridWorldIntent = (GridWorldIntent) i;
                }
            }
        }
        
        return bestGridWorldIntent;
    }
    
    public SokobanIntent GetBestIntentSokoban(State currentState)
    {
        float max = float.MinValue;
        SokobanIntent bestSokobanIntent = currentState.sokobanPolicy;
        for (int i = 0; i < 4; ++i)
        {
            if (sokobanController.checkCollisionWithGridState(currentState.currentPlayerPos, GetDirectionFromIntent((SokobanIntent) i),currentState.currentGrid))
            {
                State tempState = GetNextStateSokoban(currentState, (SokobanIntent) i);
                if ( tempState.stateValue > max)
                {
                    max = tempState.stateValue;
                    bestSokobanIntent = (SokobanIntent) i;
                }
            }
        }
        
        return bestSokobanIntent;
    }

    private Vector3 GetDirectionFromIntent(SokobanIntent intent)
    {
        switch (intent)
        {
            case SokobanIntent.Down:
                return -Vector3.forward;
            case SokobanIntent.Up:
                return Vector3.forward;
            case SokobanIntent.Left:
                return Vector3.left;
            case SokobanIntent.Right:
                return -Vector3.left;
        }
        return Vector3.zero;
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
    
    public State GetNextState(State currentState,GridWorldIntent gridWorldIntent)
    {
        Vector3 nextPlayerPos = Vector3.zero;
        switch (gridWorldIntent)
        {
            case GridWorldIntent.Down:
                nextPlayerPos = currentState.currentPlayerPos - Vector3.forward;
                break;
            case GridWorldIntent.Up:
                nextPlayerPos = currentState.currentPlayerPos + Vector3.forward;
                break;
            case GridWorldIntent.Left:
                nextPlayerPos = currentState.currentPlayerPos + Vector3.left;
                break;
            case GridWorldIntent.Right:
                nextPlayerPos = currentState.currentPlayerPos - Vector3.left;
                break;
        }
        return GetStateFromPos(nextPlayerPos);
    }
    
    public State GetNextStateSokoban(State currentState,SokobanIntent sokobanIntent)
    {
        Cell[][] nextGrid = CopyGrid(currentState.currentGrid);
        Vector3 nextPlayerPos = currentState.currentPlayerPos;
        if (currentState.currentPlayerPos.z < currentState.currentGrid.Length - 1 && sokobanController.checkCollisionWithGridState(currentState.currentPlayerPos, GetDirectionFromIntent(sokobanIntent),currentState.currentGrid))
        {
            switch (sokobanIntent)
            {
                case SokobanIntent.Up :
                    if (currentState.currentGrid[(int) currentState.currentPlayerPos.x][(int) currentState.currentPlayerPos.z+1].cellSokobanType == Cell.CellSokobanType.Crate)
                    {
                        nextGrid[(int) currentState.currentPlayerPos.x][(int) currentState.currentPlayerPos.z + 1].cellSokobanType = Cell.CellSokobanType.Empty;
                        if (sokobanController.IsCrateHitTargetBox((int) currentState.currentPlayerPos.x, (int) currentState.currentPlayerPos.z + 2, nextGrid))
                        {
                            nextGrid[(int) currentState.currentPlayerPos.x][(int) currentState.currentPlayerPos.z + 2].cellSokobanType = Cell.CellSokobanType.CratePlaced;
                        }
                        else
                        {
                            nextGrid[(int) currentState.currentPlayerPos.x][(int) currentState.currentPlayerPos.z + 2].cellSokobanType = Cell.CellSokobanType.Crate;
                        }
                    }
                    nextPlayerPos += Vector3.forward;
                    break;
                case SokobanIntent.Down :
                    if (currentState.currentGrid[(int) currentState.currentPlayerPos.x][(int) currentState.currentPlayerPos.z-1].cellSokobanType == Cell.CellSokobanType.Crate)
                    {
                        nextGrid[(int) currentState.currentPlayerPos.x][(int) currentState.currentPlayerPos.z - 1].cellSokobanType = Cell.CellSokobanType.Empty;
                        if (sokobanController.IsCrateHitTargetBox((int) currentState.currentPlayerPos.x, (int) currentState.currentPlayerPos.z - 2, nextGrid))
                        {
                            nextGrid[(int) currentState.currentPlayerPos.x][(int) currentState.currentPlayerPos.z - 2].cellSokobanType = Cell.CellSokobanType.CratePlaced;
                        }
                        else
                        {
                            nextGrid[(int) currentState.currentPlayerPos.x][(int) currentState.currentPlayerPos.z - 2].cellSokobanType = Cell.CellSokobanType.Crate;
                        }
                    }
                    nextPlayerPos -= Vector3.forward;
                    break;
                case SokobanIntent.Left :
                    if (currentState.currentGrid[(int) currentState.currentPlayerPos.x][(int) currentState.currentPlayerPos.z+1].cellSokobanType == Cell.CellSokobanType.Crate)
                    {
                        nextGrid[(int) currentState.currentPlayerPos.x-1][(int) currentState.currentPlayerPos.z].cellSokobanType = Cell.CellSokobanType.Empty;
                        if (sokobanController.IsCrateHitTargetBox((int) currentState.currentPlayerPos.x-2, (int) currentState.currentPlayerPos.z, nextGrid))
                        {
                            nextGrid[(int) currentState.currentPlayerPos.x-2][(int) currentState.currentPlayerPos.z].cellSokobanType = Cell.CellSokobanType.CratePlaced;
                        }
                        else
                        {
                            nextGrid[(int) currentState.currentPlayerPos.x-2][(int) currentState.currentPlayerPos.z].cellSokobanType = Cell.CellSokobanType.Crate;
                        }
                    }
                    nextPlayerPos += Vector3.left;
                    break;
                case SokobanIntent.Right :
                    if (currentState.currentGrid[(int) currentState.currentPlayerPos.x+1][(int) currentState.currentPlayerPos.z].cellSokobanType == Cell.CellSokobanType.Crate)
                    {
                        nextGrid[(int) currentState.currentPlayerPos.x+1][(int) currentState.currentPlayerPos.z].cellSokobanType = Cell.CellSokobanType.Empty;
                        if (sokobanController.IsCrateHitTargetBox((int) currentState.currentPlayerPos.x+2, (int) currentState.currentPlayerPos.z, nextGrid))
                        {
                            nextGrid[(int) currentState.currentPlayerPos.x+2][(int) currentState.currentPlayerPos.z].cellSokobanType = Cell.CellSokobanType.CratePlaced;
                        }
                        else
                        {
                            nextGrid[(int) currentState.currentPlayerPos.x+2][(int) currentState.currentPlayerPos.z].cellSokobanType = Cell.CellSokobanType.Crate;
                        }
                    }
                    nextPlayerPos -= Vector3.left;
                    break;
            }
        }

        State nextState = GetStateFromGrid(nextGrid,currentState.currentPlayerPos);
        
        if (nextState != null)
        {
            nextState = new State();
            nextState.currentGrid = nextGrid;
            nextState.stateValue = 0;
            nextState.currentPlayerPos = nextPlayerPos;
        }

        return nextState;
    }
    
    private State GetStateFromGrid(Cell[][] grid, Vector3 playerPosition)
    {
        foreach (var state in _allStates)
        {
            int count = 0;
            int same = 0;
            for (int i = 0; i < grid.Length; i++)
            {
                for (int j = 0; j < grid[i].Length; j++)
                {
                    if(state.currentGrid[i][j].cellSokobanType == grid[i][j].cellSokobanType)
                        ++same;
                    
                    ++count;
                }
            }

            if (count == same && state.currentPlayerPos == playerPosition)
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
                copyGrid[i][j] = new Cell {cellSokobanType = grid[i][j].cellSokobanType};
            }
        }

        return copyGrid;
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
    public CellType GetCellTypeSokoban(Vector3 pos)
    {
        return gridWorldController.grid.grid[(int)pos.x][(int)pos.z].cellGridWorldType;
    }

    public bool CheckIntent(State currentState, GridWorldIntent wantedGridWorldIntent)
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
    
    public bool CheckIntentSokoban(State currentState, SokobanIntent wantedSokobanIntent)
    {
        if (GetNextStateSokoban(currentState, wantedSokobanIntent) == null)
        {
            return false;
        }
        if (GetCellType(GetNextStateSokoban(currentState, wantedSokobanIntent).currentPlayerPos) == CellType.Obstacle)
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
                case GridWorldIntent.Down:
                    arrow = Instantiate(gridWorldController.downArrow, state.currentPlayerPos, Quaternion.identity);
                    arrow.transform.SetParent(debugIntentParent.transform);
                    break;
                case GridWorldIntent.Up:
                    arrow = Instantiate(gridWorldController.upArrow, state.currentPlayerPos, Quaternion.identity);
                    arrow.transform.SetParent(debugIntentParent.transform);
                    break;
                case GridWorldIntent.Left:
                    arrow = Instantiate(gridWorldController.leftArrow, state.currentPlayerPos, Quaternion.identity);
                    arrow.transform.SetParent(debugIntentParent.transform);
                    break;
                case GridWorldIntent.Right:
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
