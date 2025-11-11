using System.Collections;
using Unity.Cinemachine;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{
    public static SceneManager Instance; // Singleton instance
    public SceneAsset startScene;
    public Animator transitionAnimator;
    public Vector3 portalSpawnIn;
    public CinemachineConfiner2D cameraConfiner;

    public GameObject player;
    private Vector3 lastKnownPosition = Vector3.zero;

    public StoryProgression storyProgression = StoryProgression.Act1;
    public GameObject portalPrefab;

    private bool isPortalTransition = false;

    public enum StoryProgression
    {
        Act1,
        Act2,
        Act3,
        Act4,
        Act5
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        portalSpawnIn = GameObject.FindWithTag("PortalSpawnIn").transform.position;
    }

    void OnDestroy()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void LoadScene(SceneAsset sceneToLoad)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneToLoad.name);
    }

    public void TransitionToScene(SceneAsset sceneToLoad, Vector3? customSpawnPoint = null)
    {
        if (customSpawnPoint != null)
        {
            lastKnownPosition = customSpawnPoint.Value;
        }

        StartCoroutine(TransitionAndLoadScene(sceneToLoad));
    }

    public void Retry()
    {
        StartCoroutine(RetryTransitionAndLoadScene(startScene));
    }

    public void TransitionToPortalScene(SceneAsset sceneToLoad, Vector3? customSpawnPoint = null)
    {
        isPortalTransition = true;
        if (customSpawnPoint != null)
        {
            lastKnownPosition = customSpawnPoint.Value;
        }

        StartCoroutine(TransitionAndLoadScene(sceneToLoad));
    }

    public void SetStoryProgression(StoryProgression progression)
    {
        storyProgression = progression;
    }

    private IEnumerator RetryTransitionAndLoadScene(SceneAsset sceneAsset)
    {
        var playerHealth = player.GetComponent<PlayerHealth>();
        playerHealth.ReviveStats();
        transitionAnimator.ResetTrigger("Death");
        transitionAnimator.SetBool("Transition", true);
        transitionAnimator.Play("Fade_In");

        yield return new WaitForSeconds(1f);

        playerHealth.ReviveAnimation();
        lastKnownPosition = Vector3.zero;

        playerHealth.ReviveStats();
        transitionAnimator.ResetTrigger("Death");
        transitionAnimator.SetBool("Transition", true);
        transitionAnimator.Play("Fade_In");
        LoadScene(sceneAsset);
    }

    private IEnumerator TransitionAndLoadScene(SceneAsset sceneAsset)
    {
        transitionAnimator.SetBool("Transition", true);

        yield return new WaitForSeconds(1f);

        LoadScene(sceneAsset);
    }

    private IEnumerator FadeIn()
    {
        yield return new WaitForSeconds(0.5f);
        transitionAnimator.SetBool("Transition", false);
    }

    private IEnumerator FadeInPortal()
    {
        yield return new WaitForSeconds(0.4f);
        transitionAnimator.SetBool("Transition", false);
        var spawn = lastKnownPosition;
        spawn.y += 1f;
        Instantiate(portalPrefab, spawn, portalPrefab.transform.rotation);
        yield return new WaitForSeconds(1f);
        player.GetComponentInChildren<Renderer>().enabled = true;
        player.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        isPortalTransition = false;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        player = GameObject.FindWithTag("Player");
        cameraConfiner.BoundingShape2D = GameObject.FindWithTag("MapCollider").GetComponent<PolygonCollider2D>();

        if (scene.name == startScene.name)
        {
            if (lastKnownPosition == Vector3.zero)
            {
                var transform = GameObject.FindWithTag("SpawnPoint").transform;
                player.transform.position = transform.position;
                StartCoroutine(FadeIn());
            }
            else
            {
                if (isPortalTransition)
                {
                    player.GetComponentInChildren<Renderer>().enabled = false;
                    player.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
                    player.transform.position = lastKnownPosition;
                    StartCoroutine(FadeInPortal());
                }
                else
                {
                    player.transform.position = lastKnownPosition;
                    StartCoroutine(FadeIn());
                }
            }
        }
        else
        {
            var transform = GameObject.FindWithTag("SpawnPoint").transform;
            player.transform.position = transform.position;
            StartCoroutine(FadeIn());
        }
    }
}
