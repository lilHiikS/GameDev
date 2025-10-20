using UnityEditor;
using UnityEngine;

public class DoorInteractable : MonoBehaviour, IInteractable
{
    public string doorId;
    public string targetDoorId;
    public GameObject spawnPoint;
    public SceneAsset sceneToLoad;

    public void Interact()
    {
        var sceneManager = FindFirstObjectByType<SceneManager>();
        if (sceneManager != null)
        {
            sceneManager.TransitionToScene(sceneToLoad, targetDoorId);
        }
    }
}