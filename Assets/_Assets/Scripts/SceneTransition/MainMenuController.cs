using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] Animator howToPlayPopup;

    private static string ANIM_PARAM_STATUS = "Status";

    public void Start()
    {
        howToPlayPopup.SetBool(ANIM_PARAM_STATUS, false);
    }

    public void Update()
    {
        if (Input.anyKey)
        {
            howToPlayPopup.SetBool(ANIM_PARAM_STATUS, false);
        }
    }

    public void OnPlayButtonClicked()
    {
        SceneTransitioner.Instance.LoadSceneWithIndex(SceneTransitioner.GAMEPLAY_INDEX);
    }

    public void OnHowToPlayClicked()
    {
        howToPlayPopup.SetBool(ANIM_PARAM_STATUS, true);
    }

    public void OnHowToPlayDismissed()
    {
        howToPlayPopup.SetBool(ANIM_PARAM_STATUS, false);
    }
}
