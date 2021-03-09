using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TicTacToeController : MonoBehaviour
{
    [SerializeField] public Grid grid;

    [Header("Team GameObject")]
    [SerializeField] public GameObject cross;
    [SerializeField] public GameObject circle;

    [Header("Team Grid Color")]
    [SerializeField] public Material crossGridMaterial;
    [SerializeField] public Material circleGridMaterial;

    public enum TicTacToeIntent
    {
        Tile0, 
        Tile1, 
        Tile2, 
        Tile3, 
        Tile4, 
        Tile5, 
        Tile6, 
        Tile7, 
        Tile8
    }
    
    private bool _isPlayer1Turn = true;
    private bool _gameIsOver = false;

    public void InitTicTacToeGame()
    {
        // Set TicTacToe Grid Size
        grid.gridWidth = 3;
        grid.gridHeight = 3;
        grid.TicTacToe();
    }
    
    public bool ProcessIntent(TicTacToeIntent wantedIntent, bool simulation = false)
    {
        if (_isPlayer1Turn) return false;
        switch (wantedIntent)
        {
            case TicTacToeIntent.Tile0:
                return Place(circle, new Vector3(0, 0.5f, 0), circleGridMaterial, Cell.CellTicTacToeType.Circle, simulation);
                break;
            case TicTacToeIntent.Tile1:
                return Place(circle, new Vector3(0, 0.5f, 1), circleGridMaterial, Cell.CellTicTacToeType.Circle, simulation);
                break;
            case TicTacToeIntent.Tile2:
                return Place(circle, new Vector3(0, 0.5f, 2), circleGridMaterial, Cell.CellTicTacToeType.Circle, simulation);
                break;
            case TicTacToeIntent.Tile3:
                return Place(circle, new Vector3(1, 0.5f, 0), circleGridMaterial, Cell.CellTicTacToeType.Circle, simulation);
                break;
            case TicTacToeIntent.Tile4:
                return Place(circle, new Vector3(1, 0.5f, 1), circleGridMaterial, Cell.CellTicTacToeType.Circle, simulation);
                break;
            case TicTacToeIntent.Tile5:
                return Place(circle, new Vector3(1, 0.5f, 2), circleGridMaterial, Cell.CellTicTacToeType.Circle, simulation);
                break;
            case TicTacToeIntent.Tile6:
                return Place(circle, new Vector3(2, 0.5f, 0), circleGridMaterial, Cell.CellTicTacToeType.Circle, simulation);
                break;
            case TicTacToeIntent.Tile7:
                return Place(circle, new Vector3(2, 0.5f, 1), circleGridMaterial, Cell.CellTicTacToeType.Circle, simulation);
                break;
            case TicTacToeIntent.Tile8:
                return Place(circle, new Vector3(2, 0.5f, 2), circleGridMaterial, Cell.CellTicTacToeType.Circle, simulation);
                break;
        }

        return false;
    }

    // Update is called once per frame
    private void Update()
    {
        // Mouse Left Click
        if (Input.GetMouseButtonDown(0) && _isPlayer1Turn)
        {
            OnMouseLeftClick();
        }
        // Mouse Right Click
        if (Input.GetMouseButtonDown(1) && !_isPlayer1Turn)
        {
            OnMouseRightClick();
        }
    }

    private void OnMouseLeftClick()
    {
        Vector3 mouse = Input.mousePosition;
        Ray castPoint = Camera.main.ScreenPointToRay(mouse);
        RaycastHit hit;
        if (Physics.Raycast(castPoint, out hit, Mathf.Infinity))
        {
            float x = Mathf.Round(hit.point.x);
            float y = 0.5f;
            float z = Mathf.Round(hit.point.z);
            Place(cross, new Vector3(x, y, z), crossGridMaterial, Cell.CellTicTacToeType.Cross);
        }
    }

    private void OnMouseRightClick()
    {
        Vector3 mouse = Input.mousePosition;
        Ray castPoint = Camera.main.ScreenPointToRay(mouse);
        RaycastHit hit;
        if (Physics.Raycast(castPoint, out hit, Mathf.Infinity))
        {
            float x = Mathf.Round(hit.point.x);
            float y = 0.5f;
            float z = Mathf.Round(hit.point.z);
            Place(circle, new Vector3(x, y, z), circleGridMaterial, Cell.CellTicTacToeType.Circle);
        }
    }

    private bool GridIsEmpty(Vector3 position)
    {
        int x = (int)position.x;
        int z = (int)position.z;
        if (grid.grid[z][x].cellTicTacToeType == Cell.CellTicTacToeType.Neutral)
        {
            return true;
        }
        Debug.Log("Place prise.");
        return false;
    }

    private bool Place(GameObject prefabSign, Vector3 position, Material team, Cell.CellTicTacToeType cellType, bool simulation = false)
    {
        if (GridIsEmpty(position))
        {
            if (!simulation)
            { 
                // Change Player Turn
                _isPlayer1Turn = !_isPlayer1Turn;
                
                int x = (int)position.x;
                int z = (int)position.z;
                // Instantiate Team GameObject
                GameObject sign = Instantiate(prefabSign, position, transform.rotation * Quaternion.Euler(90f, 0f, 0f));
                sign.transform.SetParent(this.transform);
                
                // Change Grid Material
                grid.grid[z][x].cellObject.GetComponent<MeshRenderer>().material = team;
                
                // Set Cell State
                grid.grid[z][x].cellTicTacToeType = cellType;
                
                //Debug.Log("Set State : " + grid.grid[x][z].state);
                CheckVictory(cellType);
            }
            return true;
        }
        return false;
    }

    private void CheckVictory(Cell.CellTicTacToeType cellType)
    {
        CheckHorizontalRows(cellType);
        CheckVerticalRows(cellType);
        CheckDiagonal(cellType);
        
        //Check match null
        int count = 0;
        for (int i = 0; i < 3; ++i)
        {
            for (int j = 0; j < 3; ++j)
            {
                if (!GridIsEmpty(new Vector3(i, .5f, j)))
                {
                    ++count;
                }
            }
        }

        if (count == 9)
        {
            Debug.Log("Match nul !!! Bande de nullos hihihih");
            _gameIsOver = true;
        }
    }

    private void CheckVerticalRows(Cell.CellTicTacToeType cellType)
    {
        bool isVictory = false;

        for (int x = 0; x < grid.gridHeight; x++)
        {
            if (grid.grid[0][x].cellTicTacToeType == cellType
            && grid.grid[1][x].cellTicTacToeType == cellType 
            && grid.grid[2][x].cellTicTacToeType == cellType)
            {
                isVictory = true;
            }
        }

        if (isVictory)
        {
            Debug.Log("Partie gagnée sur ligne verticale par " + cellType);
        }
    }

    private void CheckHorizontalRows(Cell.CellTicTacToeType cellType)
    {
        bool isVictory = false;

        for (int x = 0; x < grid.gridWidth; x++)
        {
            if (grid.grid[x][0].cellTicTacToeType == cellType
                && grid.grid[x][1].cellTicTacToeType == cellType 
                && grid.grid[x][2].cellTicTacToeType == cellType)
            {
                isVictory = true;
            }
        }

        if (isVictory)
        {
            Debug.Log("Partie gagnée sur ligne horizontale par " + cellType);
        }
    }

    private void CheckDiagonal(Cell.CellTicTacToeType cellType)
    {
        bool isVictory = grid.grid[0][0].cellTicTacToeType == cellType
                         && grid.grid[1][1].cellTicTacToeType == cellType 
                         && grid.grid[2][2].cellTicTacToeType == cellType
                         || grid.grid[0][2].cellTicTacToeType == cellType
                         && grid.grid[1][1].cellTicTacToeType == cellType 
                         && grid.grid[2][0].cellTicTacToeType == cellType;
        
        if (isVictory)
        {
            Debug.Log("Partie gagnée sur une diagonale par " + cellType);
        }
    }

}