using UnityEngine;

public class WorldObject : MonoBehaviour
{
    private void OnEnable()
    {
        FindObjectOfType<CameraController>().worldObjects.Add(this);
    }

    private void OnDisable()
    {
        FindObjectOfType<CameraController>().worldObjects.Remove(this);
    }
}
