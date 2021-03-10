using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SokobanController : MonoBehaviour
{
    [SerializeField] public Grid grid;

    [SerializeField] public GameObject playerPrefab;
    public GameObject _player;

    [Header("Grid Color")]
    [SerializeField] public Material emptyGridMaterial;
    [SerializeField] public Material wallGridMaterial;
    [SerializeField] public Material crateGridMaterial;
    [SerializeField] public Material cratePlacedGridMaterial;
    [SerializeField] public Material targetBoxGridMaterial;

    [Header("GameObject")]
    [SerializeField] public GameObject crate;
    [SerializeField] public GameObject wall;

    private int playerStrokeNumber = 0;
    private bool gameIsFinished = false;

    public enum SokobanIntent
    {
        Down,
        Up,
        Left,
        Right
    }

    public void InitSokobanGrid()
    {
        // Set Sokoban Grid Size
        grid.gridWidth = 4;
        grid.gridHeight = 4;
        grid.Sokoban();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            UpIntent();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            DownIntent();
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            LeftIntent();
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            RightIntent();
        }
    }

    // Check Collision With Player
    private bool checkCollision(Vector3 targetPosition)
    {
        int x = (int)targetPosition.x;
        int z = (int)targetPosition.z;

        // If Wall
        if (grid.grid[z][x].cellSokobanType == Cell.CellSokobanType.Wall)
        {
            Debug.Log("Un mur bloque le chemin pour avancer.");
            return false;
        }

        // If Crate

        return true;
    }

    public void UpIntent()
    {
        Vector3 targetPosition = _player.transform.position + Vector3.forward;
        if (_player.transform.position.z < grid.gridHeight - 1 && checkCollision(targetPosition))
        {
            _player.transform.position += Vector3.forward;
        }
    }

    public void DownIntent()
    {
        Vector3 targetPosition = _player.transform.position - Vector3.forward;
        if (_player.transform.position.z > 0 && checkCollision(targetPosition))
        {
            _player.transform.position -= Vector3.forward;
        }
    }

    public void LeftIntent()
    {
        Vector3 targetPosition = _player.transform.position + Vector3.left;
        if (_player.transform.position.x > 0 && checkCollision(targetPosition))
        {
            _player.transform.position += Vector3.left;
        }
    }

    public void RightIntent()
    {
        Vector3 targetPosition = _player.transform.position - Vector3.left;
        if (_player.transform.position.x < grid.gridWidth - 1 && checkCollision(targetPosition))
        {
            _player.transform.position -= Vector3.left;
        }
    }
}