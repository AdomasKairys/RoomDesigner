using UnityEngine;
using UnityEngine.UI;

public class QuitButtonController : MonoBehaviour
{
    [SerializeField] Button quitButton;
    void Awake()
    {
        quitButton.onClick.AddListener(() => GameManager.Instance.QuitGame());
    }
}
