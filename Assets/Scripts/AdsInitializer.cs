using UnityEngine;
using UnityEngine.Advertisements;
using System;

public class AdsInitializer : MonoBehaviour, IUnityAdsInitializationListener
{
    [SerializeField] string _androidGameId;
    [SerializeField] bool _testMode = true;
    private string _gameId;

    // ✅ Notikums, uz kuru pieslēdzas RewardedAds un InterstitialAd
    public event Action onAdsInitialized;

    private void Awake()
    {
        InitializeAds();
    }

    public void InitializeAds()
    {
#if UNITY_ANDROID || UNITY_EDITOR
        _gameId = _androidGameId;
#endif

        if (!Advertisement.isInitialized && Advertisement.isSupported)
        {
            Debug.Log("🟡 Initializing Unity Ads...");
            Advertisement.Initialize(_gameId, _testMode, this);
        }
    }

    public void OnInitializationComplete()
    {
        Debug.Log("✅ Unity Ads initialization complete.");
        onAdsInitialized?.Invoke(); // Izsauc eventu, kas aktivizē reklāmu ielādi
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.LogWarning($"❌ Unity Ads initialization failed: {error} - {message}");
    }
}
