using UnityEngine;

public class DisableGameobject : MonoBehaviour
{
    [SerializeField] private bool disableOnStart;

    private void Awake()
    {
        if (disableOnStart)
            Disable();
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }
}
