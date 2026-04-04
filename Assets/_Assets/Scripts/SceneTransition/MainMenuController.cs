using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private Button playButton;

    private void Awake()
    {
        playButton.onClick.AddListener(OnPlayButtonClicked);
    }

    public void OnPlayButtonClicked()
    {
        SceneTransitioner.Instance.LoadSceneWithIndex(SceneTransitioner.GAMEPLAY_INDEX);
    }
}
