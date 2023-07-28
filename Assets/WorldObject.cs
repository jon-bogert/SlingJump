using UnityEngine;

public class WorldObject : MonoBehaviour
{
    public delegate void WorldShiftEvent(float amt);

    [HideInInspector]
    public event WorldShiftEvent worldShifted;

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

    public void Invoke(float amt)
    {
        worldShifted?.Invoke(amt);
    }
}
