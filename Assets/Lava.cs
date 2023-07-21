using UnityEngine;

public class Lava : MonoBehaviour
{
    [SerializeField] bool dewit = true;
    [SerializeField] float _moveSpeed = 1f;

    private void Update()
    {
        if (!dewit) return;
        transform.Translate(Vector2.up * _moveSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();
        if (player)
            player.GameOver();
    }
}
