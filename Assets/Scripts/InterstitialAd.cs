using System;
using System.Collections; // ✅ Nepieciešams priekš IEnumerator
using UnityEngine;
using UnityEngine.UI; // ✅ Nepieciešams priekš Button
using UnityEngine.Advertisements;

public class InterstitialAd : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [SerializeField] string _androidAdUnitId = "Interstitial_Android";
    private string _adUnitId;

    public event Action onInterstitialAdReady;
    public bool isReady = false;

    [SerializeField] private Button _interstitialAdButton;

    private void Awake()
    {
        _adUnitId = _androidAdUnitId;

        if (_interstitialAdButton != null)
            _interstitialAdButton.onClick.AddListener(ShowInterstitial);
    }

    private void Start()
    {
        LoadAd();
    }

    private void Update()
    {
        if (_interstitialAdButton != null)
            _interstitialAdButton.interactable = isReady;
    }

    public void LoadAd()
    {
        if (!Advertisement.isInitialized)
        {
            Debug.LogWarning("❌ Unity Ads is not initialized. Cannot load interstitial ad.");
            return;
        }

        Debug.Log("🟡 Loading interstitial ad...");
        Advertisement.Load(_adUnitId, this);
    }

    public void ShowAd()
{
    if (isReady)
    {
        Debug.Log("🟢 Showing interstitial ad...");
        Advertisement.Show(_adUnitId, this);
        isReady = false;
    }
    else
    {
        Debug.LogWarning("⚠️ Interstitial ad not ready yet. Trying to reload...");
        LoadAd();
    }
}


    public void ShowInterstitial()
    {
        if(AdManager.instance != null && isReady)
        {
        Debug.Log("Showing interstitial ad from button.");
           ShowAd();
        }else
        {
            Debug.LogWarning("⚠️ Cannot show interstitial ad from button. AdManager instance is null or ad not ready.");
            LoadAd();
        }
    }

    // ================================
    // ✅ Load listener
    // ================================
    public void OnUnityAdsAdLoaded(string placementId)
    {
        Debug.Log($"✅ Interstitial ad loaded: {placementId}");
        _interstitialAdButton.interactable = true;
        isReady = true;
        onInterstitialAdReady?.Invoke();
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        Debug.LogWarning($"❌ Failed to load interstitial ad {placementId}: {error} - {message}");
        LoadAd();
    }

    // ================================
    // ✅ Show listener
    // ================================

    public void OnUnityAdsShowClick(string placementId)
    {
        Debug.Log($"🖱️ Interstitial ad clicked: {placementId}");
    }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        if (showCompletionState == UnityAdsShowCompletionState.COMPLETED)
        {
            Debug.Log("✅ Interstitial ad finished successfully.");
            StartCoroutine(SlowDownTimeTemporarily(30f));
            LoadAd();
        }
        else
        {
            Debug.LogWarning($"⚠️ Interstitial ad not completed: {placementId}, state: {showCompletionState}");
            LoadAd();
        }
    }
    
    private IEnumerator SlowDownTimeTemporarily(float seconds)
    {
        Time.timeScale = 0.4f;
        Debug.Log("Time slowed down for ad completion effect.");
        yield return new WaitForSeconds(seconds);

        Time.timeScale = 1f;
        Debug.Log("Time restored to normal.");
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        Debug.LogWarning($"❌ Interstitial ad show failed {placementId}: {error} - {message}");
        LoadAd();
    }

    public void OnUnityAdsShowStart(string placementId)
    {
        Debug.Log($"▶️ Interstitial ad started: {placementId}");
        Time.timeScale = 0f;
    }

    public void SetButton(Button button)
    {
        if (button == null)
            return;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(ShowInterstitial);
        _interstitialAdButton = button;
        _interstitialAdButton.interactable = false;
    }
}
