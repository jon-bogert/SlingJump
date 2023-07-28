using UnityEngine;

public class Lava : MonoBehaviour
{
    bool _dewit = false;
    [SerializeField] float _offScreenBuffer = 2f;
    [SerializeField] float _moveSpeed = 1f;
    [SerializeField] float _speedAccel = 0.01f;

    private void Update()
    {
        if (!_dewit) return;

        //check off screen -> Rubber-Band
        if (Camera.main.transform.position.y - transform.position.y > Camera.main.orthographicSize + _offScreenBuffer)
        {
            transform.position = new Vector2(transform.position.x, Camera.main.transform.position.y - Camera.main.orthographicSize - _offScreenBuffer);
        }
        else
        {
            transform.Translate(Vector2.up * _moveSpeed * Time.deltaTime);
        }

        //increase move speed;
        _moveSpeed += _speedAccel * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();
        if (player)
            player.GameOver();
    }

    //For Testing On Device only
    public void Dewit()
    {
        _dewit = true;
    }
}
