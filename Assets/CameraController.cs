using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Resets Camera and Objects in world space to keep floating point precision 
/// while using built-in physics
/// </summary>
public class CameraController : MonoBehaviour
{
    [SerializeField] float _resetDistance = 100f;

    [HideInInspector]
    public List<WorldObject> worldObjects;

    Player _player;

    private void Start()
    {
        _player = FindObjectOfType<Player>();
    }

    private void Update()
    {
        Camera.main.transform.position = new Vector3(
            0f,
            _player.transform.position.y,
            Camera.main.transform.position.z);

        if (Camera.main.transform.position.y >= _resetDistance)
        {
            Camera.main.transform.position = new Vector3(
                Camera.main.transform.position.x,
                Camera.main.transform.position.y - _resetDistance * 2f,
                Camera.main.transform.position.z);

            for (int i = 0; i < worldObjects.Count; ++i)
            {
                Vector3 objPos = worldObjects[i].transform.position;
                worldObjects[i].transform.position = new Vector3(
                    objPos.x,
                    objPos.y - _resetDistance * 2f,
                    objPos.z);
            }
            _player.TeleportY(-_resetDistance * 2f);
        }
    }
}
