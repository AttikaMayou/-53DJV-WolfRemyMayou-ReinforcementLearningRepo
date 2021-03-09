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

    private bool isPlayer1Turn = true;

    public void InitTicTacToeGame()
    {
        // Set TicTacToe Grid Size
        grid.gridWidth = 3;
        grid.gridHeight = 3;
        grid.TicTacToe();
    }

    // Update is called once per frame
    void Update()
    {
        // Mouse Left Click
        if (Input.GetMouseButtonDown(0) && isPlayer1Turn)
        {
            onMouseLeftClick();
        }
        // Mouse Right Click
        if (Input.GetMouseButtonDown(1) && !isPlayer1Turn)
        {
            onMouseRightClick();
        }
    }

    private void onMouseLeftClick()
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

    private void onMouseRightClick()
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

    private bool gridIsEmpty(Vector3 position)
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

    private void Place(GameObject prefabSign, Vector3 position, Material team, Cell.CellTicTacToeType cellType)
    {
        if (gridIsEmpty(position))
        {
            // Change Player Turn
            isPlayer1Turn = !isPlayer1Turn;
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
            CheckVictory();
        }   
    }

    private void CheckVictory()
    {
        CheckHorizontalRows();
        //CheckVerticalRows();
        //CheckFirstDiagonal();
        //CheckSecondDiagonal();
    }

    private void CheckHorizontalRows()
    {
        bool isVictory = false;
        int x = 0;
        int z = 0;
        Cell.CellTicTacToeType cellType = grid.grid[z][x].cellTicTacToeType;

        // Horizontal (X Axe)
        for (x = 0; x < grid.gridHeight; x++)
        {
            z = 0;
            cellType = grid.grid[z][x].cellTicTacToeType;

            // Vertical (Z Axe)
            for (z = 0; z < grid.gridWidth; z++)
            {
                Debug.Log("Cell x = " + x + " ; " + "z = " + z + " ; cellTicTacToeType = " + grid.grid[z][x].cellTicTacToeType);
                if (cellType == grid.grid[z][x].cellTicTacToeType)
                {
                    isVictory = true;
                }
                else
                {
                    isVictory = false;
                    break;
                }
                cellType = grid.grid[z][x].cellTicTacToeType;
            } 

            /*if (isVictory)
            {
                break;
            }*/
        }

        if (isVictory)
        {
            Debug.Log("Partie gagnée sur ligne horizontale.");
        }
    }

    private void CheckVerticalRows()
    {
        Cell.CellTicTacToeType cellType = grid.grid[0][0].cellTicTacToeType;
        bool isVictory = false;

        // Vertical (Z Axe)
        for (int i = 0; i < grid.gridWidth; i++)
        {
            // Horizontal (X Axe)
            for (int j = 0; j < grid.gridHeight; j++)
            {
                
            }

            /*if (isVictory)
            {
                break;
            }*/
        }

        if (isVictory)
        {
            Debug.Log("Partie gagnée sur ligne verticale.");
        }
    }

    private void CheckFirstDiagonal()
    {
        // Diagonal 1 (Bas Gauche à Haut Droit)
        for (int i = 0; i < grid.gridHeight; i++)
        {
            for (int j = 0; j < grid.gridWidth; j++, i++)
            {
            }
        }
    }

    private void CheckSecondDiagonal()
    {
        // Diagonal 2 (Haut Gauche à Bas Droit)
        for (int i = grid.gridHeight; i > 0; i--)
        {
            for (int j = grid.gridWidth; j > 0; j--, i--)
            {
            }
        }
    }
}