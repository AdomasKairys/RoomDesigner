using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this.gameObject);
        else
            Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public void QuitGame() => Application.Quit();
    public void StartGame() => SceneManager.LoadSceneAsync("GameScene");
}
