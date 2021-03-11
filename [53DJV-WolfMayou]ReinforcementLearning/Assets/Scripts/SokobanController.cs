using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class SokobanController : MonoBehaviour
{
    [SerializeField] public Grid grid;

    [SerializeField] public GameObject playerPrefab;
    public GameObject player;

    [Header("Material Color")]
    [SerializeField] public Material emptyMaterial;
    [SerializeField] public Material wallMaterial;
    [SerializeField] public Material crateMaterial;
    [SerializeField] public Material cratePlacedMaterial;
    [SerializeField] public Material targetBoxMaterial;

    [Header("GameObject")]
    [SerializeField] public GameObject crate;
    [SerializeField] public GameObject wall;
    [SerializeField] public GameObject worldLimit;

    private int _playerStrokeNumber;
    [Header("Game Data")]
    [SerializeField] private int _playerStrokeLimitNumber = 10;
    public int totalTargetBox;
    private int _filledTargetBox;
    private bool _gameIsFinished;

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
        ResetSokobanGame();
        grid.Sokoban();
        //Debug.Log("totalTargetBox = " + totalTargetBox);
    }

    private void ResetSokobanGame()
    {
        _playerStrokeNumber = 0;
        totalTargetBox = 0;
        _filledTargetBox = 0;
        _gameIsFinished = false;
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
    private bool IsCrateHitTargetBox(GameObject gameObject)
    {
        int x = (int)gameObject.transform.position.x;
        int z = (int)gameObject.transform.position.z;
        if (grid.grid[x][z].cellSokobanType == Cell.CellSokobanType.CrateTarget)
        {
            Debug.Log("La caisse à touché une cible.");
            // Update Crate Material
            gameObject.GetComponent<MeshRenderer>().material = cratePlacedMaterial;
            _filledTargetBox++;
            CheckVictory();
            return true;
        }
        return false;
    }

    // Check if all targets boxes have a crate
    private bool CheckVictory()
    {
        if ((_filledTargetBox == totalTargetBox) && !_gameIsFinished)
        {
            Debug.Log("Partie gagnée en " + _playerStrokeNumber + " coups.");
            _gameIsFinished = true;
            return true;
        }
        return false;
    }

    // Check if the stroke limit is reached
    private bool CheckDefeat()
    {
        if ((_playerStrokeNumber == _playerStrokeLimitNumber) && !_gameIsFinished)
        {
            Debug.Log("Partie perdue. Limite de coups (" + _playerStrokeLimitNumber + ") atteinte.");
            _gameIsFinished = true;
            return true;
        }
        return false;
    }

    // Move the crate
    private void MoveCrate(GameObject crate, Vector3 direction)
    {
        Debug.Log("Pas d'obstacle je déplace la caisse.");

        Vector3 currentPosition = crate.transform.position;
        Vector3 newPosition = currentPosition + direction;
        crate.transform.position = newPosition;
        Debug.Log("Vector3 : " + crate.transform.position);

        IsCrateHitTargetBox(crate);

        int x = (int)currentPosition.x;
        int z = (int)currentPosition.z;
        // Update Current Crate Grid
        grid.grid[x][z].cellSokobanType = Cell.CellSokobanType.Empty;

        x = (int)newPosition.x;
        z = (int)newPosition.z;
        // Update New Crate Grid
        grid.grid[x][z].cellSokobanType = Cell.CellSokobanType.Crate;
    }

    // Check Grid State Collision With Player
    private bool checkCollisionWithGridState(Vector3 currentPosition, Vector3 direction)
    {
        Vector3 targetPosition = currentPosition + direction;
        int x = (int)targetPosition.x;
        int z = (int)targetPosition.z;

        // If Wall
        if (grid.grid[x][z].cellSokobanType == Cell.CellSokobanType.Wall)
        {
            Debug.Log("Un mur bloque le chemin pour avancer.");
            return false;
        }

        // If Crate
        if (grid.grid[x][z].cellSokobanType == Cell.CellSokobanType.Crate)
        {
            Debug.Log("Un mur bloque le chemin pour avancer.");
            return false;
        }

        return true;
    }

    // Check Raycast Collision With Player
    private bool checkCollisionWithRaycast(Vector3 currentPosition, Vector3 direction)
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
                    else if (hit.collider.gameObject.tag == "Crate")
                    {
                        Debug.Log("Une caisse bloque le chemin pour avancer.");
                        return false;
                    }
                    else
                    {
                        MoveCrate(crate, direction);
                        return true;
                    }
                }
                else
                {
                    MoveCrate(crate, direction);
                    return true;
                }
            }
        }

        return true;
    }

    public void UpIntent()
    {
        if (player.transform.position.z < grid.gridHeight - 1 && checkCollisionWithRaycast(player.transform.position, Vector3.forward))
        {
            player.transform.position += Vector3.forward;
            _playerStrokeNumber++;
            CheckDefeat();
        }
    }

    public void DownIntent()
    {
        if (player.transform.position.z > 0 && checkCollisionWithRaycast(player.transform.position, - Vector3.forward))
        {
            player.transform.position -= Vector3.forward;
            _playerStrokeNumber++;
            CheckDefeat();
        }
    }

    public void LeftIntent()
    {
        if (player.transform.position.x > 0 && checkCollisionWithRaycast(player.transform.position, Vector3.left))
        {
            player.transform.position += Vector3.left;
            _playerStrokeNumber++;
            CheckDefeat();
        }
    }

    public void RightIntent()
    {
        if (player.transform.position.x < grid.gridWidth - 1 && checkCollisionWithRaycast(player.transform.position, - Vector3.left))
        {
            player.transform.position -= Vector3.left;
            _playerStrokeNumber++;
            CheckDefeat();
        }
    }
}