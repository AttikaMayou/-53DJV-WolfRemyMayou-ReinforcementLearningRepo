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

    public CellType type;
    public GameObject cellObject;
}
