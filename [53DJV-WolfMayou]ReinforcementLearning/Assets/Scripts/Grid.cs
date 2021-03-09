using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Grid : MonoBehaviour
{
    public Cell[][] grid;

    [HideInInspector] public int gridWidth;
    [HideInInspector] public int gridHeight;
    [SerializeField] private GameObject gridPrefab;
    [SerializeField] private List<Material> gridMaterials;

    public GameObject valueObject;
    [SerializeField] private DebuggerManager debuggerManager;

    public Vector3 startPos;
    public Vector3 endPos;
    
    public void GridWorld()
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
                    grid[i][j].gridWorldType = Cell.CellGridWorldType.Empty;
                }
                else
                {
                    if ((i > 0 && j > 0 && j < gridWidth - 1) && (grid[i - 1][j].gridWorldType == Cell.CellGridWorldType.Obstacle ||
                        grid[i - 1][j - 1].gridWorldType == Cell.CellGridWorldType.Obstacle ||
                        grid[i][j - 1].gridWorldType == Cell.CellGridWorldType.Obstacle ||
                        grid[i - 1][j + 1].gridWorldType == Cell.CellGridWorldType.Obstacle))
                    {
                        grid[i][j].gridWorldType = Cell.CellGridWorldType.Hole;
                    }
                    else
                    {
                        grid[i][j].gridWorldType = Cell.CellGridWorldType.Obstacle;
                    }
                }

                grid[i][j].cellObject.GetComponent<MeshRenderer>().material = gridMaterials[(int)grid[i][j].gridWorldType];
            }
        }

        int endX, endY;
        var startX = Random.Range(0, gridWidth);
        var startY = Random.Range(0, gridHeight);
        startPos = new Vector3(startX,0.0f,startY);
        do
        {
            endX = Random.Range(0, gridWidth);
            endY = Random.Range(0, gridHeight);
        } while (endX == startX && endY == startY);

        endPos = new Vector3(endX,0.0f,endY);
        grid[startX][startY].gridWorldType = Cell.CellGridWorldType.Start;
        grid[startX][startY].cellObject.GetComponent<MeshRenderer>().material = gridMaterials[(int)Cell.CellGridWorldType.Start];
        grid[endX][endY].gridWorldType = Cell.CellGridWorldType.End;
        grid[endX][endY].cellObject.GetComponent<MeshRenderer>().material = gridMaterials[(int)Cell.CellGridWorldType.End];
    }

    
    public void TicTacToe()
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
                grid[i][j].cellObject = Instantiate(gridPrefab, new Vector3(j, 0, i), Quaternion.identity);
                grid[i][j].cellObject.transform.SetParent(this.transform);
            }
        }
    }


    public void Sokoban()
    {
        debuggerManager.ClearIntents();
        foreach (Transform child in this.transform)
        {
            Destroy(child.gameObject);
        }
    }
}