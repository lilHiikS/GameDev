using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    public List<SceneAsset> scenes;
    public List<DoorInteractable> doorInteractables;
    private GameObject player;
    public Animator transitionAnimator;

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        doorInteractables = new List<DoorInteractable>(FindObjectsByType<DoorInteractable>(FindObjectsSortMode.None));
    }

    public void LoadScene(int index)
    {
        if (index < 0 || index >= scenes.Count)
        {
            Debug.LogError("Invalid scene index");
            return;
        }

        UnityEngine.SceneManagement.SceneManager.LoadScene(scenes[index].name);
    }

    public void Play()
    {
        LoadScene(1);
    }

    public void TeleportPlayerToDoor(string targetDoorId)
    {
        if (player != null)
        {
            foreach (var door in doorInteractables)
            {
                if (door.doorId == targetDoorId)
                {
                    StartCoroutine(TransitionAndLoadScene(door));
                    break;
                }
            }
        }
    }

    private IEnumerator TransitionAndLoadScene(DoorInteractable door)
    {
        transitionAnimator.SetBool("Transition", true);

        yield return new WaitForSeconds(1f);

        player.transform.position = door.spawnPoint.transform.position;

        yield return new WaitForSeconds(0.4f);

        transitionAnimator.SetBool("Transition", false);
    }
}
