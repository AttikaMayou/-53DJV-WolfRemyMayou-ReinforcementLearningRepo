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
        switch (type)
        {
            case GameType.GridWorld:
                InitGridWorld();
                break;
            case GameType.TicTacToe:
                break;
            case GameType.Sokoban:
                break;
        }
    }

    private void InitGridWorld()
    {
        grid = new Cell[gridHeight][];
        bool startDone;
        bool endDone;

        for (int i = 0; i < gridHeight; ++i)
        {
            Debug.Log("iai " + i);
            grid[i] = new Cell[gridWidth];
            for (int j = 0; j < gridWidth; ++j)
            {
                Debug.Log("yay " + j);
                grid[i][j] = new Cell();
                Debug.Log("yay1 " + j);
                grid[i][j].cellObject = Instantiate(gridPrefab,new Vector3(j,0,i),Quaternion.identity);
                Debug.Log("yay2 " + j);
                grid[i][j].type = (Cell.CellType)Random.Range(0, 3);
                Debug.Log("yay3 " + j);
                grid[i][j].cellObject.GetComponent<MeshRenderer>().material = gridMaterials[(int)grid[i][j].type];
                Debug.Log("yay4 " + j);
            }
        }

        int startX, startY;
        int endX, endY;
        startX = Random.Range(0, gridWidth);
        startY = Random.Range(0, gridHeight);
        do
        {
            endX = Random.Range(0, gridWidth);
            endY = Random.Range(0, gridHeight);
        } while (endX == startX && endY == startY);

        grid[startX][startY].type = Cell.CellType.Start;
        grid[startX][startY].cellObject.GetComponent<MeshRenderer>().material = gridMaterials[(int)Cell.CellType.Start];
        grid[endX][endY].type = Cell.CellType.End;
        grid[endX][endY].cellObject.GetComponent<MeshRenderer>().material = gridMaterials[(int)Cell.CellType.End];
    }
}
