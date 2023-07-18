using UnityEngine;
using UnityEngine.InputSystem;

public class InputActionManager : MonoBehaviour
{
    [SerializeField] InputActionAsset asset;
    bool initialized = false;
    private void Awake()
    {
        if (FindObjectsOfType<InputActionManager>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }
        
        DontDestroyOnLoad(this);
        asset.Enable();
        initialized = true;
    }

    private void OnDestroy()
    {
        if (initialized)
        {
            asset.Disable();
        }
    }
}
