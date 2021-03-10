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
    [SerializeField] public GameObject worldLimit;

    private int playerStrokeNumber;
    [Header("Game Data")]
    [SerializeField] public int totalTargetBox;
    private int filledTargetBox;
    private bool gameIsFinished;

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
        resetSokobanGame();
        grid.Sokoban();
        Debug.Log("totalTargetBox = " + totalTargetBox);
    }

    private void resetSokobanGame()
    {
        playerStrokeNumber = 0;
        totalTargetBox = 0;
        filledTargetBox = 0;
        gameIsFinished = false;
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

    // Crate hit a target box ?
    private bool crateHitTargetBox(Vector3 position)
    {
        int x = (int)position.x;
        int z = (int)position.z;
        if (grid.grid[z][x].cellSokobanType == Cell.CellSokobanType.CrateTarget)
        {
            Debug.Log("La caisse à touché une cible.");
            filledTargetBox++;
            CheckVictory();
            return true;
        }
        return false;
    }

    // Check if all targets boxes have a crate
    private void CheckVictory()
    {
        if (filledTargetBox == totalTargetBox)
        {
            Debug.Log("Partie gagnée en " + playerStrokeNumber + " coups.");
            gameIsFinished = true;
        }
    }

    // Check Collision With Player
    private bool checkCollision(Vector3 currentPosition, Vector3 direction)
    {
        /*Vector3 targetPosition = currentPosition + direction;
        int x = (int)targetPosition.x;
        int z = (int)targetPosition.z;*/
        float maxDistance = 1.0f;
        RaycastHit hit;

        // What we hit
        if (Physics.Raycast(currentPosition, direction, out hit, maxDistance))
        {
            Debug.Log("Hit Something.");

            // If Wall
            if (hit.collider.gameObject.tag == "Wall")
            {
                Debug.Log("Un mur bloque le chemin pour avancer.");
                return false;
            }

            // If Crate
            if (hit.collider.gameObject.tag == "Crate")
            {
                Debug.Log("Une caisse est touché.");
                GameObject crate = hit.collider.gameObject;

                // Can i move the crate
                if (Physics.Raycast(crate.transform.position, direction, out hit, maxDistance))
                {
                    if (hit.collider.gameObject.tag == "Wall")
                    {
                        Debug.Log("Un mur bloque le chemin pour avancer.");
                        return false;
                    }
                    else
                    {
                        Debug.Log("Pas d'obstacle je déplace la caisse.");
                        crate.transform.position += direction;
                        Debug.Log("Vector3 : " + crate.transform.position);
                        crateHitTargetBox(crate.transform.position);
                        return true;
                    }
                }
                else
                {
                    Debug.Log("Pas d'obstacle je déplace la caisse.");
                    crate.transform.position += direction;
                    Debug.Log("Vector3 : " + crate.transform.position);
                    crateHitTargetBox(crate.transform.position);
                    return true;
                }
            }
        }

        return true;
    }

    public void UpIntent()
    {
        if (_player.transform.position.z < grid.gridHeight - 1 && checkCollision(_player.transform.position, Vector3.forward))
        {
            _player.transform.position += Vector3.forward;
            playerStrokeNumber++;
        }
    }

    public void DownIntent()
    {
        if (_player.transform.position.z > 0 && checkCollision(_player.transform.position, - Vector3.forward))
        {
            _player.transform.position -= Vector3.forward;
            playerStrokeNumber++;
        }
    }

    public void LeftIntent()
    {
        if (_player.transform.position.x > 0 && checkCollision(_player.transform.position, Vector3.left))
        {
            _player.transform.position += Vector3.left;
            playerStrokeNumber++;
        }
    }

    public void RightIntent()
    {
        if (_player.transform.position.x < grid.gridWidth - 1 && checkCollision(_player.transform.position, -Vector3.left))
        {
            _player.transform.position -= Vector3.left;
            playerStrokeNumber++;
        }
    }
}