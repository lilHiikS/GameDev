using UnityEngine;

public class DoorInteractable : MonoBehaviour, IInteractable
{
    public string doorId;
    public GameObject spawnPoint;
    public string targetDoorId;
    private SceneManager sceneManager;
    public GameObject icon;

    void Start()
    {
        sceneManager = FindFirstObjectByType<SceneManager>();
    }

    public void Interact()
    {
        if (sceneManager != null)
        {
            sceneManager.TeleportPlayerToDoor(targetDoorId);
        }
    }
}