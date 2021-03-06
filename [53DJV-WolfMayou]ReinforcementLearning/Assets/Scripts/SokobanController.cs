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
    private bool _hitTargetBox;

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
        _hitTargetBox = false;
    }

    private void Update()
    {
        if (GameSelector.type != GameSelector.GameType.Sokoban) return;
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

    // Crate hit a target box ?
    public bool IsCrateHitTargetBox(int x, int z, Cell[][] currentGrid)
    {
        if (currentGrid[x][z].cellSokobanType == Cell.CellSokobanType.CrateTarget)
        {
            Debug.Log("La caisse à touché une cible.");
            _filledTargetBox++;
            _hitTargetBox = true;
            CheckVictory();
            return true;
        }
        return false;
    }

    // Check Grid State Collision With Player
    public bool checkCollisionWithGridState(Vector3 currentPosition, Vector3 direction, Cell[][] currentGrid)
    {
        Vector3 targetPosition = currentPosition + direction;
        int targetX = (int)targetPosition.x;
        int targetZ = (int)targetPosition.z;

        // If Wall In Target Position
        if (currentGrid[targetX][targetZ].cellSokobanType == Cell.CellSokobanType.Wall)
        {
            Debug.Log("Un mur bloque le chemin pour avancer.");
            return false;
        }
        // If Crate Placed In Target Position
        else if (currentGrid[targetX][targetZ].cellSokobanType == Cell.CellSokobanType.CratePlaced)
        {
            Debug.Log("Une caisse placé bloque le chemin pour avancer.");
            return false;
        }
        // If Crate In Target Position
        else if (currentGrid[targetX][targetZ].cellSokobanType == Cell.CellSokobanType.Crate)
        {
            // Check If We Can Move the Crate
            Vector3 nextTargetPosition = targetPosition + direction;
            int nextTargetX = (int)nextTargetPosition.x;
            int nextTargetZ = (int)nextTargetPosition.z;

            // If Map Limit In Next Target Position
            if (nextTargetX >= grid.gridHeight || nextTargetX < 0 || nextTargetZ >= grid.gridWidth || nextTargetZ < 0)
            {
                Debug.Log("Limite de la carte atteinte.");
                return false;
            }
            // If Wall In Next Target Position
            else if (currentGrid[nextTargetX][nextTargetZ].cellSokobanType == Cell.CellSokobanType.Wall)
            {
                Debug.Log("Un mur bloque le chemin pour avancer.");
                return false;
            }
            // If Crate Placed In Next Target Position
            else if (currentGrid[nextTargetX][nextTargetZ].cellSokobanType == Cell.CellSokobanType.CratePlaced)
            {
                Debug.Log("Une caisse placé bloque le chemin pour avancer.");
                return false;
            }
            // If Crate In Next Target Position
            else if (currentGrid[nextTargetX][nextTargetZ].cellSokobanType == Cell.CellSokobanType.Crate)
            {
                Debug.Log("Une caisse bloque le chemin pour avancer.");
                return false;
            }
            // We can move the crate
            else
            {
                // We Hit Target Box ?
                if (IsCrateHitTargetBox(nextTargetX, nextTargetZ,grid.grid))
                {
                    // Update Current Crate Grid
                    currentGrid[targetX][targetZ].cellSokobanType = Cell.CellSokobanType.Empty;
                    // Update New Crate Grid
                    currentGrid[nextTargetX][nextTargetZ].cellSokobanType = Cell.CellSokobanType.CratePlaced;
                }
                else
                {
                    // Update Current Crate Grid
                    currentGrid[targetX][targetZ].cellSokobanType = Cell.CellSokobanType.Empty;
                    // Update New Crate Grid
                    currentGrid[nextTargetX][nextTargetZ].cellSokobanType = Cell.CellSokobanType.Crate;
                }             
                return true;
            }
        }

        return true;
    }

    // Crate hit a target box ?
    private void IsCrateHitTargetBox(GameObject gameObject)
    {
        if (_hitTargetBox)
        {
            // Update Crate Material
            gameObject.GetComponent<MeshRenderer>().material = cratePlacedMaterial;
            _hitTargetBox = false;
        }
    }

    // Move the crate
    private void MoveCrate(GameObject crate, Vector3 direction)
    {
        Debug.Log("Pas d'obstacle je déplace la caisse.");
        Vector3 currentPosition = crate.transform.position;
        Vector3 newPosition = currentPosition + direction;
        crate.transform.position = newPosition;
        Debug.Log("Vector3 : " + crate.transform.position);
    }

    // Check Raycast Collision With Player
    private void checkCollisionWithRaycast(Vector3 currentPosition, Vector3 direction)
    {
        float maxDistance = 1.0f;
        RaycastHit hit;

        // What we hit
        if (Physics.Raycast(currentPosition, direction, out hit, maxDistance))
        {
            // If Crate
            if (hit.collider.gameObject.tag == "Crate")
            {
                Debug.Log("Une caisse est touché.");
                GameObject crate = hit.collider.gameObject;
                MoveCrate(crate, direction);
                IsCrateHitTargetBox(crate);
            }
        }
    }

    public void UpIntent()
    {
        if (player.transform.position.z < grid.gridHeight - 1 && checkCollisionWithGridState(player.transform.position, Vector3.forward,grid.grid))
        {
            checkCollisionWithRaycast(player.transform.position, Vector3.forward);
            player.transform.position += Vector3.forward;
            _playerStrokeNumber++;
            CheckDefeat();
        }
    }

    public void DownIntent()
    {
        if (player.transform.position.z > 0 && checkCollisionWithGridState(player.transform.position, - Vector3.forward,grid.grid))
        {
            checkCollisionWithRaycast(player.transform.position, - Vector3.forward);
            player.transform.position -= Vector3.forward;
            _playerStrokeNumber++;
            CheckDefeat();
        }
    }

    public void LeftIntent()
    {
        if (player.transform.position.x > 0 && checkCollisionWithGridState(player.transform.position, Vector3.left,grid.grid))
        {
            checkCollisionWithRaycast(player.transform.position, Vector3.left);
            player.transform.position += Vector3.left;           
            _playerStrokeNumber++;
            CheckDefeat();
        }
    }

    public void RightIntent()
    {
        if (player.transform.position.x < grid.gridWidth - 1 && checkCollisionWithGridState(player.transform.position, - Vector3.left,grid.grid))
        {
            checkCollisionWithRaycast(player.transform.position, - Vector3.left);
            player.transform.position -= Vector3.left;
            _playerStrokeNumber++;
            CheckDefeat();
        }
    }
}