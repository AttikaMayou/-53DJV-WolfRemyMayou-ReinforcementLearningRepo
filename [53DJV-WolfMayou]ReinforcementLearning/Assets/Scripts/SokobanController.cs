using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SokobanController : MonoBehaviour
{
    [SerializeField] public Grid grid;

    public void InitSokobanGrid()
    {
        grid.Sokoban();
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

    // Crate hit a target box ?
    private bool IsCrateHitTargetBox(GameObject gameObject)
    {
        int x = (int)gameObject.transform.position.x;
        int z = (int)gameObject.transform.position.z;
        if (grid.grid[z][x].cellSokobanType == Cell.CellSokobanType.CrateTarget)
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
    private void CheckVictory()
    {
        if (_filledTargetBox == totalTargetBox)
        {
            Debug.Log("Partie gagnée en " + _playerStrokeNumber + " coups.");
            _gameIsFinished = true;
        }
    }

    // Check Collision With Player
    private bool CheckCollision(Vector3 currentPosition, Vector3 direction)
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
                        IsCrateHitTargetBox(crate);
                        return true;
                    }
                }
                else
                {
                    Debug.Log("Pas d'obstacle je déplace la caisse.");
                    crate.transform.position += direction;
                    Debug.Log("Vector3 : " + crate.transform.position);
                    IsCrateHitTargetBox(crate);
                    return true;
                }
            }
        }

        return true;
    }

    public void UpIntent()
    {
        if (player.transform.position.z < grid.gridHeight - 1 && CheckCollision(player.transform.position, Vector3.forward))
        {
            player.transform.position += Vector3.forward;
            _playerStrokeNumber++;
        }
    }

    public void DownIntent()
    {
        if (player.transform.position.z > 0 && CheckCollision(player.transform.position, - Vector3.forward))
        {
            player.transform.position -= Vector3.forward;
            _playerStrokeNumber++;
        }
    }

    public void LeftIntent()
    {
        if (player.transform.position.x > 0 && CheckCollision(player.transform.position, Vector3.left))
        {
            player.transform.position += Vector3.left;
            _playerStrokeNumber++;
        }
    }

    public void RightIntent()
    {
        if (player.transform.position.x < grid.gridWidth - 1 && CheckCollision(player.transform.position, - Vector3.left))
        {
            player.transform.position -= Vector3.left;
            _playerStrokeNumber++;
        }
    }
}
