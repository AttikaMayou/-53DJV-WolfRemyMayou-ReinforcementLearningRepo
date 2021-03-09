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

    // Start is called before the first frame update
    void Start()
    {
        if (grid.type == Grid.GameType.TicTacToe)
        {
            // Set TicTacToe Grid Size
            grid.gridWidth = 3;
            grid.gridHeight = 3;
            grid.InitGridWorld();

            /*Place(cross, new Vector3(0, 0.5f, 0), crossGridMaterial, Cell.State.CROSS);
            Place(circle, new Vector3(1, 0.5f, 0), circleGridMaterial, Cell.State.CIRCLE);
            Place(cross, new Vector3(2, 0.5f, 0), crossGridMaterial, Cell.State.CROSS);

            Place(circle, new Vector3(0, 0.5f, 1), circleGridMaterial, Cell.State.CIRCLE);
            Place(cross, new Vector3(1, 0.5f, 1), crossGridMaterial, Cell.State.CROSS);
            Place(circle, new Vector3(2, 0.5f, 1), circleGridMaterial, Cell.State.CIRCLE);

            Place(circle, new Vector3(0, 0.5f, 2), circleGridMaterial, Cell.State.CIRCLE);
            Place(cross, new Vector3(1, 0.5f, 2), crossGridMaterial, Cell.State.CROSS);
            Place(cross, new Vector3(2, 0.5f, 2), crossGridMaterial, Cell.State.CROSS);

            for (int i = 0; i < grid.gridHeight; ++i)
            {
                for (int j = 0; j < grid.gridWidth; ++j)
                {
                    Debug.Log("Grid State is : " + grid.grid[i][j].state);
                }
            }*/
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Place(GameObject gameObject, Vector3 position, Material team, Cell.State state)
    {
        int x = (int)position.z;
        int z = (int)position.x;
        // Instantiate Team GameObject
        Instantiate(gameObject, position, transform.rotation * Quaternion.Euler(90f, 0f, 0f));
        // Change Grid Material
        grid.grid[x][z].cellObject.GetComponent<MeshRenderer>().material = team;
        // Set Cell State
        grid.grid[x][z].state = state;
        Debug.Log("Set State : " + grid.grid[x][z].state);
    }
}