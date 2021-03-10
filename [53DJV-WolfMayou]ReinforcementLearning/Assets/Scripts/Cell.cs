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

    public enum CellSokobanType
    {
        Start,
        Empty,
        Wall,
        Crate,
        CrateTarget
    }

    public CellTicTacToeType cellTicTacToeType;
    public CellGridWorldType cellGridWorldType;
    public CellSokobanType cellSokobanType;
    public GameObject cellObject;
}