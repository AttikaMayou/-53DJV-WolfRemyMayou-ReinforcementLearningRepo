using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    public enum CellType
    {
        Empty,
        Obstacle,
        Hole,
        Start,
        End
    }

    public enum State
    {
        NEUTRAL = -1,
        CROSS = 0,
        CIRCLE = 1
    }
    public State state;

    public CellType type;
    public GameObject cellObject;
}