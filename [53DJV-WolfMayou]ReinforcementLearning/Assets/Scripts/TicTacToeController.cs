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

    public void InitTicTacToeGame()
    {
        // Set TicTacToe Grid Size
        grid.gridWidth = 3;
        grid.gridHeight = 3;
        grid.TicTacToe();
    }

    public void Place(GameObject prefabSign, Vector3 position, Material team, Cell.CellTicTacToeType cellTicTacToeType)
    {
        int x = (int)position.z;
        int z = (int)position.x;
        // Instantiate Team GameObject
        GameObject sign = Instantiate(prefabSign, position, transform.rotation * Quaternion.Euler(90f, 0f, 0f));
        sign.transform.SetParent(this.transform);
        // Change Grid Material
        grid.grid[x][z].cellObject.GetComponent<MeshRenderer>().material = team;
        // Set Cell State
        grid.grid[x][z].cellTicTacToeType = cellTicTacToeType;
        Debug.Log("Set State : " + grid.grid[x][z].cellTicTacToeType);
    }
}