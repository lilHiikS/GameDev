using UnityEditor;
using UnityEngine;

public class DoorInteractable : MonoBehaviour, IInteractable
{
    public GameObject customSpawnPoint;
    public SceneAsset sceneToLoad;
    public GameObject lightSource;
    public GameObject romanNumber;
    public SceneManager.StoryProgression act;

    [SerializeField] private AudioSource interactionSound;

    void Start()
    {
        if ((int)SceneManager.Instance.storyProgression > (int)act)
        {
            if (lightSource != null && romanNumber != null)
            {
                lightSource.SetActive(false);
                romanNumber.GetComponent<SpriteRenderer>().material.DisableKeyword("_ISGLOWING");
            }
        }
        else if ((int)SceneManager.Instance.storyProgression == (int)SceneManager.StoryProgression.Act5)
        {
            if (lightSource != null)
            {
                lightSource.SetActive(true);
            }
        }
    }

    public void Interact()
    {
        var sceneManager = SceneManager.Instance;

        if ((int)SceneManager.Instance.storyProgression != (int)act)
            return;

        // Play interaction sound if assigned
        if (interactionSound != null)
        {
            interactionSound.Play();
        }

        if (sceneManager != null)
        {
            sceneManager.TransitionToScene(
                sceneToLoad, 
                customSpawnPoint != null ? customSpawnPoint.transform.position : null
            );
        }
    }
}
