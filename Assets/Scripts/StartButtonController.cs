using UnityEngine;
using UnityEngine.UI;

public class StartButtonController : MonoBehaviour
{
    [SerializeField] Button startButton;
    void Awake()
    {
        startButton.onClick.AddListener(() => GameManager.Instance.StartGame());
    }
}
