using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{
    public static SceneManager Instance; // Singleton instance
    public List<DoorInteractable> doorInteractables;
    public SceneAsset startScene;
    public Animator transitionAnimator;

    private GameObject player;
    private string targetDoorId;

    void Awake()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void LoadScene(SceneAsset sceneToLoad)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneToLoad.name);
    }

    public void TransitionToScene(SceneAsset sceneToLoad, string targetDoorId)
    {
        this.targetDoorId = targetDoorId;
        StartCoroutine(TransitionAndLoadScene(sceneToLoad));
    }

    private IEnumerator TransitionAndLoadScene(SceneAsset sceneAsset)
    {
        transitionAnimator.SetBool("Transition", true);

        yield return new WaitForSeconds(1f);

        LoadScene(sceneAsset);
    }

    private IEnumerator FadeIn()
    {
        yield return new WaitForSeconds(0.4f);
        transitionAnimator.SetBool("Transition", false);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        player = GameObject.FindWithTag("Player");
        doorInteractables = new List<DoorInteractable>(FindObjectsByType<DoorInteractable>(FindObjectsSortMode.None));

        if (targetDoorId != null)
        {
            foreach (var door in doorInteractables)
            {
                if (door.doorId == targetDoorId)
                {
                    player.transform.position = door.spawnPoint.transform.position;
                    break;
                }
            }
        }

        StartCoroutine(FadeIn());
        targetDoorId = null; // Clear the target door ID after spawning
    }
}
