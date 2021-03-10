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
                grid[i][j] = new Cell();
                grid[i][j].cellObject = Instantiate(gridPrefab, new Vector3(j, 0, i), Quaternion.identity);
                grid[i][j].cellObject.transform.SetParent(this.transform);
                grid[i][j].cellTicTacToeType = Cell.CellTicTacToeType.Neutral;
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
                grid[i][j] = new Cell();
                grid[i][j].cellObject = Instantiate(gridPrefab, new Vector3(j, 0, i), Quaternion.identity);
                grid[i][j].cellObject.transform.SetParent(this.transform);
                grid[i][j].cellSokobanType = Cell.CellSokobanType.Empty;
                grid[i][j].cellObject.GetComponent<MeshRenderer>().material = sokobanController.emptyGridMaterial;
            }
        }

        // Set Player Start
        grid[0][0].cellSokobanType = Cell.CellSokobanType.Start;
        sokobanController._player = Instantiate(sokobanController.playerPrefab, startPos + new Vector3(0, 0.5f, 0), Quaternion.identity);
        sokobanController._player.transform.SetParent(this.transform);

        // Set Crates
        createCrate(3, 1);

        // Set Targets Boxes
        createTargetBox(3, 3);

        // Set Walls
        createWall(0, 1);
        createWall(1, 1);
    }

    private void createCrate(int x, int z)
    {
        grid[z][x].cellSokobanType = Cell.CellSokobanType.Crate;
        GameObject crate = Instantiate(sokobanController.crate, new Vector3(x, 0.5f, z), Quaternion.identity);
        crate.transform.SetParent(this.transform);
        grid[z][x].cellObject.GetComponent<MeshRenderer>().material = sokobanController.crateGridMaterial;
    }

    private void createTargetBox(int x, int z)
    {
        grid[z][x].cellSokobanType = Cell.CellSokobanType.CrateTarget;
        grid[z][x].cellObject.GetComponent<MeshRenderer>().material = sokobanController.targetBoxGridMaterial;
    }

    private void createWall(int x, int z)
    {
        grid[z][x].cellSokobanType = Cell.CellSokobanType.Wall;
        GameObject wall = Instantiate(sokobanController.wall, new Vector3(x, 0.5f, z), Quaternion.identity);
        wall.transform.SetParent(this.transform);
        grid[z][x].cellObject.GetComponent<MeshRenderer>().material = sokobanController.wallGridMaterial;
    }
}