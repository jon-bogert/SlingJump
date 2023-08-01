using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdsManager : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsInitializationListener, IUnityAdsShowListener
{
#if UNITY_IPHONE
    private string _adID = "5367609"; // replace with apple ad id
    private string _interstitialID = "Interstitial_iOS";
    private string _rewardedID = "Rewarded_iOS";
    private string _bannerID = "Banner_iOS";
#elif UNITY_ANDROID
    private string _adID = "5367608"; // replace with android ad id
    private string _interstitialID = "Interstitial_Android";
    private string _rewardedID = "Rewarded_Android";
    private string _bannerID = "Banner_Android";
#endif
    [SerializeField] bool _testMode = false;
    public static AdsManager instance = null;

    private ushort _deathToPlayCouter = 4;
    private const ushort _deathToPlayCouterReset = 4;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        Advertisement.Initialize(_adID, _testMode, this);
    }

    public void ShowInterstitialAd()
    {
        if (_deathToPlayCouter > 0)
        {
            _deathToPlayCouter--;
            return;
        }

        _deathToPlayCouter = 4;
        Advertisement.Show(_interstitialID, this);
    }

    public void ShowRewardAd()
    {
        Advertisement.Show(_rewardedID, this);
    }

    public void ShowBannerAd()
    {
        Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
        Advertisement.Banner.Show(_bannerID);
    }

    public void HideBanner()
    {
        Advertisement.Banner.Hide();
        Advertisement.Banner.Load();
    }

    public void OnInitializationComplete()
    {
        Debug.Log("Ads Init Complete");
        Advertisement.Load(_interstitialID, this);
        Advertisement.Load(_rewardedID, this);
        Advertisement.Load(_bannerID, this);
    }
    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.LogError("Could not initialize ads -> " + message);
    }
    public void OnUnityAdsAdLoaded(string placementId)
    {
        Debug.Log(placementId + " Ad Loaded successfully");
    }
    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        Debug.LogError("Failed to load ad " + placementId + " -> " + message);
    }
    public void OnUnityAdsShowClick(string placementId)
    {
        Debug.Log("Ad Click through");
    }
    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        if (placementId == _rewardedID && showCompletionState == UnityAdsShowCompletionState.COMPLETED)
        {
            Debug.Log("Reward Ad Complete");
        }
        else if (showCompletionState == UnityAdsShowCompletionState.COMPLETED)
        {
            Debug.Log("Ad Watched through");
        }
        else if (showCompletionState == UnityAdsShowCompletionState.SKIPPED)
        {
            Debug.Log("Ad Skipped");
        }
        else
        {
            Debug.Log("Unknown Complete state");
        }
        Advertisement.Load(_bannerID, this); // Make sure to load a new Ad
    }
    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    { 
        Debug.LogError("Could not show Ad " + placementId + " -> " + message);
    }
    public void OnUnityAdsShowStart(string placementId)
    {
        Debug.Log("Showing Ad " + placementId);
    }
}
