using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State
{
    public float stateValue;
    public GridWorldController.Intents statePolicy;

    public Vector3 currentPlayerPos;
    public Cell[][] currentGrid;

}