using UnityEngine;

public class State
{
    public float stateValue;
    public GridWorldController.GridWorldIntent gridWorldPolicy;

    public float nS;
    public float returnS;
    public TicTacToeController.TicTacToeIntent ticTacToePolicy;

    public Vector3 currentPlayerPos;
    public Cell[][] currentGrid;
}