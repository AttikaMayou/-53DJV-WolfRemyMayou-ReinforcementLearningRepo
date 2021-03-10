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
    public bool _gameIsOver = false;

    public void InitTicTacToeGame()
    {
        // Set TicTacToe Grid Size
        grid.gridWidth = 3;
        grid.gridHeight = 3;
        grid.TicTacToe();
        _gameIsOver = false;
    }
    
    public bool ProcessIntent(TicTacToeIntent wantedIntent)
    {
        if (_isPlayer1Turn) return false;
       
        return Place(circle, GetPositionFromIntent(wantedIntent), circleGridMaterial, Cell.CellTicTacToeType.Circle);
         
    }



    /*private void OnMouseRightClick()
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
    }*/

    public bool GridIsEmpty(Vector3 position, Cell[][] currentGrid)
    {
        int x = (int)position.x;
        int z = (int)position.z;
        if (currentGrid[z][x].cellTicTacToeType == Cell.CellTicTacToeType.Neutral)
        {
            return true;
        }
        //Debug.Log("Place prise.");
        return false;
    }

    public Vector3 GetPositionFromIntent(TicTacToeIntent intent)
    {
        Vector3 position = new Vector3(0, 0.5f, 0);
        
        switch (intent)
        {
            case TicTacToeIntent.Tile0:
                position.x = 0;
                position.z = 0;
                break;
            case TicTacToeIntent.Tile1:
                position.x = 0;
                position.z = 1;
                break;
            case TicTacToeIntent.Tile2:
                position.x = 0;
                position.z = 2;
                break;
            case TicTacToeIntent.Tile3:
                position.x = 1;
                position.z = 0;
                break;
            case TicTacToeIntent.Tile4:
                position.x = 1;
                position.z = 1;
                break;
            case TicTacToeIntent.Tile5:
                position.x = 1;
                position.z = 2;
                break;
            case TicTacToeIntent.Tile6:
                position.x = 2;
                position.z = 0;
                break;
            case TicTacToeIntent.Tile7:
                position.x = 2;
                position.z = 1;
                break;
            case TicTacToeIntent.Tile8:
                position.x = 2;
                position.z = 2;
                break;
        }

        return position;
    }
    
    public bool Place(GameObject prefabSign, Vector3 position, Material team, Cell.CellTicTacToeType cellType)
    {
        if (GridIsEmpty(position,grid.grid))
        {
            // Change Player Turn
            //_isPlayer1Turn = !_isPlayer1Turn;
            
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
            CheckVictory(cellType, grid.grid);
            
            return true;
        }
        return false;
    }

    private void CheckVictory(Cell.CellTicTacToeType cellType, Cell[][] currentGrid)
    {
        if (CheckHorizontalRows(cellType, currentGrid) ||
            CheckVerticalRows(cellType, currentGrid) ||
            CheckDiagonal(cellType, currentGrid))
        {
            _gameIsOver = true;
        }
        
        //Check match null
        int count = 0;
        for (int i = 0; i < 3; ++i)
        {
            for (int j = 0; j < 3; ++j)
            {
                if (!GridIsEmpty(new Vector3(i, .5f, j),grid.grid))
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

    public bool CheckVerticalRows(Cell.CellTicTacToeType cellType, Cell[][] currentGrid)
    {
        bool isVictory = false;

        for (int x = 0; x < grid.gridHeight; x++)
        {
            if (currentGrid[0][x].cellTicTacToeType == cellType
            && currentGrid[1][x].cellTicTacToeType == cellType 
            && currentGrid[2][x].cellTicTacToeType == cellType)
            {
                isVictory = true;
            }
        }

        if (isVictory)
        {
            Debug.Log("Partie gagnée sur ligne verticale par " + cellType);
        }

        return isVictory;
    }

    public bool CheckHorizontalRows(Cell.CellTicTacToeType cellType, Cell[][] currentGrid)
    {
        bool isVictory = false;

        for (int x = 0; x < grid.gridWidth; x++)
        {
            if (currentGrid[x][0].cellTicTacToeType == cellType
            && currentGrid[x][1].cellTicTacToeType == cellType
            && currentGrid[x][2].cellTicTacToeType == cellType)
            {
                isVictory = true;
            }
        }

        if (isVictory)
        {
            Debug.Log("Partie gagnée sur ligne horizontale par " + cellType);
        }
        return isVictory;
    }

    public bool CheckDiagonal(Cell.CellTicTacToeType cellType, Cell[][] currentGrid)
    {
        bool isVictory = currentGrid[0][0].cellTicTacToeType == cellType
                         && currentGrid[1][1].cellTicTacToeType == cellType 
                         && currentGrid[2][2].cellTicTacToeType == cellType
                         || currentGrid[0][2].cellTicTacToeType == cellType
                         && currentGrid[1][1].cellTicTacToeType == cellType 
                         && currentGrid[2][0].cellTicTacToeType == cellType;
        
        if (isVictory)
        {
            Debug.Log("Partie gagnée sur une diagonale par " + cellType);
        }
        return isVictory;
    }

}