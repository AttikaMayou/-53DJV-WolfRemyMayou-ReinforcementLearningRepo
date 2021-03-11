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
    public DebuggerManager debuggerManager;

    public Vector3 startPos;
    public Vector3 endPos;
    public bool hasBeenInitialized = false;

    private SokobanController sokobanController;

    private void Awake()
    {
        sokobanController = GetComponent<SokobanController>();
    }

    public void GridWorld()
    {
        debuggerManager.ClearIntents();
        foreach (Transform child in this.transform)
        {
            Destroy(child.gameObject);
        }

        hasBeenInitialized = false;

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
                    grid[i][j].cellGridWorldType = Cell.CellGridWorldType.Empty;
                }
                else
                {
                    if ((i > 0 && j > 0 && j < gridWidth - 1) && (grid[i - 1][j].cellGridWorldType == Cell.CellGridWorldType.Obstacle ||
                        grid[i - 1][j - 1].cellGridWorldType == Cell.CellGridWorldType.Obstacle ||
                        grid[i][j - 1].cellGridWorldType == Cell.CellGridWorldType.Obstacle ||
                        grid[i - 1][j + 1].cellGridWorldType == Cell.CellGridWorldType.Obstacle))
                    {
                        grid[i][j].cellGridWorldType = Cell.CellGridWorldType.Hole;
                    }
                    else
                    {
                        grid[i][j].cellGridWorldType = Cell.CellGridWorldType.Obstacle;
                    }
                }

                grid[i][j].cellObject.GetComponent<MeshRenderer>().material = gridMaterials[(int)grid[i][j].cellGridWorldType];
            }
        }

        int endX, endY;
        var startX = Random.Range(0, gridWidth);
        var startY = Random.Range(0, gridHeight);
        startPos = new Vector3(startX, 0.0f, startY);
        do
        {
            endX = Random.Range(0, gridWidth);
            endY = Random.Range(0, gridHeight);
        } while (endX == startX && endY == startY);

        endPos = new Vector3(endX, 0.0f, endY);
        grid[startX][startY].cellGridWorldType = Cell.CellGridWorldType.Start;
        grid[startX][startY].cellObject.GetComponent<MeshRenderer>().material = gridMaterials[(int)Cell.CellGridWorldType.Start];
        grid[endX][endY].cellGridWorldType = Cell.CellGridWorldType.End;
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
                grid[i][j] = new Cell {cellObject = Instantiate(gridPrefab, new Vector3(i, 0, j), Quaternion.identity)};
                grid[i][j].cellObject.transform.SetParent(this.transform);
                grid[i][j].cellTicTacToeType = Cell.CellTicTacToeType.Neutral;
                GameObject debugObj = Instantiate(valueObject, new Vector3(i, 0.5f, j), Quaternion.Euler(90, 0, 0));
                debugObj.GetComponent<TextMesh>().text = "(" + i + " ," + j + ")";
                debugObj.transform.SetParent(debuggerManager.transform);
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

        grid = new Cell[gridHeight][];

        // Create Grid
        for (int i = 0; i < gridHeight; ++i)
        {
            grid[i] = new Cell[gridWidth];
            for (int j = 0; j < gridWidth; ++j)
            {
                grid[i][j] = new Cell {cellObject = Instantiate(gridPrefab, new Vector3(i, 0, j), Quaternion.identity)};
                grid[i][j].cellObject.transform.SetParent(this.transform);
                grid[i][j].cellSokobanType = Cell.CellSokobanType.Empty;
                grid[i][j].cellObject.GetComponent<MeshRenderer>().material = sokobanController.emptyMaterial;
            }
        }

        if (false)
        {
            LoadLevel1();
        }
        else
        {
            LoadLevel2();
        }
    }

    // Sokoban Level 1
    private void LoadLevel1()
    {
        // Set Player Start
        int x = 0;
        int z = 3;
        grid[x][z].cellSokobanType = Cell.CellSokobanType.Start;
        sokobanController.player = Instantiate(sokobanController.playerPrefab, startPos + new Vector3(x, 1.0f, z), Quaternion.identity);
        sokobanController.player.transform.SetParent(this.transform);

        // Set Crates
        CreateCrate(3, 1);

        // Set Targets Boxes
        CreateTargetBox(3, 3);

        // Set Walls
        CreateWall(0, 1);
        CreateWall(1, 1);

        // Set World Limit
        for (int i = 0; i < gridHeight; ++i)
        {
            CreateWorldLimit(gridHeight, i);
            CreateWorldLimit(-1, i);

            CreateWorldLimit(i, gridHeight);
            CreateWorldLimit(i, -1);
        }
    }

    // Sokoban Level 2
    private void LoadLevel2()
    {
        // Set Player Start
        int x = 0;
        int z = 0;
        grid[x][z].cellSokobanType = Cell.CellSokobanType.Start;
        sokobanController.player = Instantiate(sokobanController.playerPrefab, startPos + new Vector3(x, 1.0f, z), Quaternion.identity);
        sokobanController.player.transform.SetParent(this.transform);

        // Set Crates
        CreateCrate(1, 3);
        CreateCrate(3, 1);

        // Set Targets Boxes
        CreateTargetBox(0, 3);
        CreateTargetBox(3, 3);

        // Set Walls
        CreateWall(0, 1);
        CreateWall(1, 1);

        // Set World Limit
        for (int i = 0; i < gridHeight; ++i)
        {
            CreateWorldLimit(gridHeight, i);
            CreateWorldLimit(-1, i);

            CreateWorldLimit(i, gridHeight);
            CreateWorldLimit(i, -1);
        }
    }

    private void CreateCrate(int x, int z)
    {
        grid[x][z].cellSokobanType = Cell.CellSokobanType.Crate;
        GameObject crate = Instantiate(sokobanController.crate, new Vector3(x, 1.0f, z), Quaternion.identity);
        crate.transform.SetParent(this.transform);
        crate.tag = "Crate";
        grid[x][z].cellObject.GetComponent<MeshRenderer>().material = sokobanController.crateMaterial;
    }

    private void CreateTargetBox(int x, int z)
    {
        grid[x][z].cellSokobanType = Cell.CellSokobanType.CrateTarget;
        grid[x][z].cellObject.GetComponent<MeshRenderer>().material = sokobanController.targetBoxMaterial;
        // Increase number of target box
        sokobanController.totalTargetBox++;
    }

    private void CreateWall(int x, int z)
    {
        grid[x][z].cellSokobanType = Cell.CellSokobanType.Wall;
        GameObject wall = Instantiate(sokobanController.wall, new Vector3(x, 1.0f, z), Quaternion.identity);
        wall.transform.SetParent(this.transform);
        wall.tag = "Wall";
        grid[x][z].cellObject.GetComponent<MeshRenderer>().material = sokobanController.wallMaterial;
    }

    private void CreateWorldLimit(int x, int z)
    {
        GameObject wall = Instantiate(sokobanController.worldLimit, new Vector3(x, 1.0f, z), Quaternion.identity);
        wall.transform.SetParent(this.transform);
        wall.tag = "Wall";
    }
}