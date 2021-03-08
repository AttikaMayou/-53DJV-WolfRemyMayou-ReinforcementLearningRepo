using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Grid : MonoBehaviour
{
    public enum GameType
    {
        GridWorld,
        TicTacToe,
        Sokoban
    }

    [SerializeField] private GameType type;
    public Cell[][] grid;

    [SerializeField] private int gridWidth;
    [SerializeField] private int gridHeight;
    [SerializeField] private GameObject gridPrefab;
    [SerializeField] private List<Material> gridMaterials; 
    private void Start()
    {
        
    }

    private void InitGridWorld()
    {
        grid = new Cell[gridHeight][];

        for (int i = 0; i < gridHeight; ++i)
        {
            grid[i] = new Cell[gridWidth];
            for (int j = 0; j < gridWidth; ++j)
            {
                grid[i][j].cellObject = Instantiate(gridPrefab,new Vector3(j,0,i),Quaternion.identity);
                grid[i][j].type = (Cell.CellType)Random.Range(0, 6);
                grid[i][j].cellObject.GetComponent<MeshRenderer>().material = gridMaterials[(int)grid[i][j].type];
            }
        }
    }
}
