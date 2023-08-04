using UnityEngine;

public class AddLivesScreen : MonoBehaviour
{
    private void Awake()
    {
        Disable();
    }

    public void Enable()
    {
        gameObject.SetActive(true);
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }

    public void WatchAd()
    {
        AdsManager.instance.ShowRewardAd();
    }

    public void OnWatchAdComplete()
    {
        Debug.Log("Add Watch Complete and adding lives");
        ScoreManager.instance.AddLives(5);
        FindObjectOfType<GameOverScore>().UpdateLivesText();
        Disable();
    }
}
