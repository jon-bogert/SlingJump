using UnityEngine;

public class WorldObject : MonoBehaviour
{
    private void Start()
    {
        FindObjectOfType<CameraController>().worldObjects.Add(this);
    }

    private void OnDestroy()
    {
        CameraController camCtrl = FindObjectOfType<CameraController>();
        if (camCtrl)
            camCtrl.worldObjects.Remove(this);
    }
}
