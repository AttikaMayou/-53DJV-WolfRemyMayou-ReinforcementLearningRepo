using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SokobanController : MonoBehaviour
{
    [SerializeField] public Grid grid;

    public void InitSokobanGrid()
    {
        grid.Sokoban();
    }
}
