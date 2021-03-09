using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Grid : MonoBehaviour
{
    public enum GameType
    {
        GridWorld,
        TicTacToe,
        Sokoban
    }

    [SerializeField] public GameType type;
    public Cell[][] grid;

    [SerializeField] public int gridWidth;
    [SerializeField] public int gridHeight;
    [SerializeField] private GameObject gridPrefab;
    [SerializeField] private List<Material> gridMaterials;
    public GameObject downArrow;
    public GameObject upArrow;
    public GameObject leftArrow;
    public GameObject rightArrow;

    [SerializeField] public GameObject valueObject;
    [SerializeField] private DebuggerManager debuggerManager;

    public Vector3 startPos;
    public Vector3 endPos;
    private void Start()
    {
    }

    public void InitGridWorld()
    {
        if (type == GameType.GridWorld)
        {
            GridWorld();
        }
        else if (type == GameType.TicTacToe)
        {
            TicTacToe();
        }
    }

    private void GridWorld()
    {
        debuggerManager.ClearIntents();
        foreach (Transform child in this.transform)
        {
            Destroy(child.gameObject);
        }
        
        grid = new Cell[gridHeight][];

        for (int i = 0; i < gridHeight; ++i)
        {
            grid[i] = new Cell[gridWidth];
            for (int j = 0; j < gridWidth; ++j)
            {
                grid[i][j] = new Cell();
                
                grid[i][j].cellObject = Instantiate(gridPrefab,new Vector3(i,0,j),Quaternion.identity);
                grid[i][j].cellObject.transform.SetParent(this.transform);
                
                float rdm = Random.Range(0.0f, 1.0f);
                if (rdm < 0.8f)
                {
                    grid[i][j].type = Cell.CellType.Empty;
                }
                else
                {
                    if ((i > 0 && j > 0 && j < gridWidth - 1) && (grid[i - 1][j].type == Cell.CellType.Obstacle ||
                        grid[i - 1][j - 1].type == Cell.CellType.Obstacle ||
                        grid[i][j - 1].type == Cell.CellType.Obstacle ||
                        grid[i - 1][j + 1].type == Cell.CellType.Obstacle))
                    {
                        grid[i][j].type = Cell.CellType.Hole;
                    }
                    else
                    {
                        grid[i][j].type = Cell.CellType.Obstacle;
                    }
                }

                grid[i][j].cellObject.GetComponent<MeshRenderer>().material = gridMaterials[(int)grid[i][j].type];
            }
        }

        int startX, startY;
        int endX, endY;
        startX = Random.Range(0, gridWidth);
        startY = Random.Range(0, gridHeight);
        startPos = new Vector3(startX,0.0f,startY);
        do
        {
            endX = Random.Range(0, gridWidth);
            endY = Random.Range(0, gridHeight);
        } while (endX == startX && endY == startY);

        endPos = new Vector3(endX,0.0f,endY);
        grid[startX][startY].type = Cell.CellType.Start;
        grid[startX][startY].cellObject.GetComponent<MeshRenderer>().material = gridMaterials[(int)Cell.CellType.Start];
        grid[endX][endY].type = Cell.CellType.End;
        grid[endX][endY].cellObject.GetComponent<MeshRenderer>().material = gridMaterials[(int)Cell.CellType.End];
    }

    private void TicTacToe()
    {
        grid = new Cell[gridHeight][];

        for (int i = 0; i < gridHeight; ++i)
        {
            grid[i] = new Cell[gridWidth];
            for (int j = 0; j < gridWidth; ++j)
            {
                grid[i][j] = new Cell();
                grid[i][j].cellObject = Instantiate(gridPrefab, new Vector3(j, 0, i), Quaternion.identity);
            }
        }
    }
}