using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    public enum CellGridWorldType
    {
        Empty,
        Obstacle,
        Hole,
        Start,
        End
    }

    public enum CellTicTacToeType
    {
        Neutral = -1,
        Cross = 0,
        Circle = 1
    }
    
    public CellTicTacToeType cellTicTacToeType;
    public CellGridWorldType gridWorldType;
    public GameObject cellObject;
}