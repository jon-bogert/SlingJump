using UnityEngine;
using UnityEngine.InputSystem;

public class ResetScores : MonoBehaviour
{
    [SerializeField] InputActionReference resetTap;

    private void Awake()
    {
        resetTap.action.performed += ResetScore;
    }

    private void OnDestroy()
    {
        resetTap.action.performed -= ResetScore;
    }

    void ResetScore(InputAction.CallbackContext ctx)
    {
        ScoreManager.instance.ResetAll();
        FindObjectOfType<GameOverScore>().UpdateHighScoreText();
    }
}
