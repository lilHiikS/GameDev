using UnityEditor;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public SceneAsset sceneToLoad;

    public void Play()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneToLoad.name);
    }
}
