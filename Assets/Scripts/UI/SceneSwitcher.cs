using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    [Tooltip("Exact name of the scene you want to switch to")]
    [SerializeField] private string sceneName;


    public void swapScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}
