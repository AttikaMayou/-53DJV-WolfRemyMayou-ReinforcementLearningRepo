using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                currentState.statePolicy = new List<GridWorldController.Intents>();
                for(int k = 0; k < gridWorldController.grid.gridHeight * gridWorldController.grid.gridWidth - 1; ++k)
                {
                    currentState.statePolicy.Add((GridWorldController.Intents)Random.Range(0, 3));
                }
            }
        }
    }
}
