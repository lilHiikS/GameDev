using UnityEditor;
using UnityEngine;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    public SceneAsset sceneToLoad;

    [Header("Story Settings")]
    [Tooltip("Panel (GameObject) that contains the story UI, typically a Canvas child.")]
    public GameObject storyPanel;

    [Tooltip("Text component to show story lines (TextMeshProUGUI).")]
    public TextMeshProUGUI storyText;

    [Tooltip("Story lines to show before starting the game.")]
    public string[] storyLines;

    [Tooltip("Seconds each line is shown before advancing.")]
    public float secondsPerLine = 3f;

    private bool isShowingStory;
    private bool skipRequested;

    private void Awake()
    {
        if (storyPanel != null)
        {
            storyPanel.SetActive(false);
        }
    }

    public void Play()
    {
        // If story is configured, show it first; otherwise load immediately
        if (storyPanel != null && storyText != null && storyLines != null && storyLines.Length > 0)
        {
            StartCoroutine(ShowStoryThenLoad());
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneToLoad.name);
        }
    }

    private System.Collections.IEnumerator ShowStoryThenLoad()
    {
        isShowingStory = true;
        skipRequested = false;

        storyPanel.SetActive(true);

        for (int i = 0; i < storyLines.Length; i++)
        {
            storyText.text = storyLines[i];
            float elapsed = 0f;
            while (elapsed < secondsPerLine)
            {
                elapsed += Time.deltaTime;
                yield return null;
            }

            if (skipRequested)
            {
                break;
            }
        }

        isShowingStory = false;
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneToLoad.name);
    }
}
