using UnityEditor;
using UnityEngine;

public class PortalInteractable : MonoBehaviour
{
    public GameObject portalEffect;
    public SceneAsset sceneToLoad;
    public SceneManager.StoryProgression nextAct;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        var sceneManager = SceneManager.Instance;
        var customSpawnPoint = sceneManager.portalSpawnIn;

        if (sceneManager != null)
        {
            SceneManager.Instance.storyProgression = nextAct;
            sceneManager.TransitionToPortalScene(sceneToLoad, customSpawnPoint != null ? customSpawnPoint : null);
        }

    }

    public void OpenPortal()
    {
        if (portalEffect != null)
        {
            portalEffect.SetActive(true);
        }
    }
}
