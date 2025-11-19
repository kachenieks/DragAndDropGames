using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class RewardedAds : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [SerializeField] private string _androidAdUnitId = "Rewarded_Android";
    private string _adUnitId;

    [SerializeField] private Button _rewardedAdButton;
    public FlyingObjectManager flyingObjectManager;

    private void Awake()
    {
        _adUnitId = _androidAdUnitId;

        if (flyingObjectManager == null)
            flyingObjectManager = FindFirstObjectByType<FlyingObjectManager>();
    }

    private void OnEnable()
    {
        // ✅ Piesakāmies AdsInitializer notikumam
        var adsInitializer = FindFirstObjectByType<AdsInitializer>();
        if (adsInitializer != null)
        {
            adsInitializer.onAdsInitialized += OnAdsInitialized;
        }
    }

    private void OnDisable()
    {
        // Atvienojam, kad objekts tiek deaktivizēts
        var adsInitializer = FindFirstObjectByType<AdsInitializer>();
        if (adsInitializer != null)
        {
            adsInitializer.onAdsInitialized -= OnAdsInitialized;
        }
    }

    private void Start()
    {
        if (_rewardedAdButton != null)
        {
            _rewardedAdButton.interactable = false;
            _rewardedAdButton.onClick.RemoveAllListeners();
            _rewardedAdButton.onClick.AddListener(ShowAd);
        }
    }

    private void OnAdsInitialized()
    {
        LoadAd(); // Tikai pēc inicializācijas
    }

    public void LoadAd()
    {
        if (!Advertisement.isInitialized)
        {
            Debug.LogWarning("❌ Unity Ads is not initialized. Cannot load rewarded ad.");
            return;
        }

        Debug.Log("🟡 Loading rewarded ad...");
        Advertisement.Load(_adUnitId, this);
    }

    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        // --- HANOJA REWARD: -1 MOVE ---
if (SceneManager.GetActiveScene().name == "HanojasTornis")
{
    Debug.Log("[ADS] Reward saņemts Hanojā – mēģinu samazināt gājienus!");

    var tm = TowerManager.Instance;

    if (tm != null)
    {
        bool ok = tm.ReduceMoveByOne();

        if (ok)
            Debug.Log("[ADS] ✔ Samazināju gājienu par 1!");
        else
            Debug.Log("[ADS] ✖ Gājieni jau ir 0 – nevar samazināt!");
    }
    else
    {
        Debug.LogWarning("[ADS] TowerManager nav atrasts!");
    }
}



        Debug.Log($"🟢 Rewarded ad loaded: {adUnitId}");

        if (adUnitId.Equals(_adUnitId))
            SetButton(true);
    }

    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        Debug.LogWarning($"❌ Failed to load rewarded ad {adUnitId}: {error} - {message}");
        StartCoroutine(WaitAndLoad(5f));
    }

    private IEnumerator WaitAndLoad(float delay)
    {
        yield return new WaitForSeconds(delay);
        LoadAd();
    }

    public void ShowAd()
{
    if (!Advertisement.isInitialized)
    {
        Debug.LogWarning("❌ Unity Ads not initialized yet.");
        return;
    }

    if (_rewardedAdButton != null)
        _rewardedAdButton.interactable = false;

    Debug.Log("🟢 Trying to show rewarded ad...");
    Advertisement.Show(_adUnitId, this);
}


    public void OnUnityAdsShowStart(string adUnitId)
    {
        if (SceneManager.GetActiveScene().name == "CityScene")
    Time.timeScale = 0f;

    }

    public void OnUnityAdsShowClick(string adUnitId)
    {
        Debug.Log($"🟡 Rewarded ad clicked: {adUnitId}");
    }

    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        Debug.LogWarning($"❌ Failed to show rewarded ad {adUnitId}: {error} - {message}");
        StartCoroutine(WaitAndLoad(5f));
    }

    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
{
    if (adUnitId.Equals(_adUnitId) && showCompletionState == UnityAdsShowCompletionState.COMPLETED)
    {
        Debug.Log("🟢 Rewarded ad completed - granting reward!");

        if (flyingObjectManager != null)
        {
            Debug.Log("✅ Reward granted! Destroying all flying objects...");
            flyingObjectManager.DestroyAllFlyingObjects();
        }
        else
        {
            Debug.LogWarning("⚠️ FlyingObjectManager not found — cannot clear objects.");
        }

        _rewardedAdButton.interactable = false;
        StartCoroutine(WaitAndLoad(10f));
    }

    if (SceneManager.GetActiveScene().name == "CityScene")
    Time.timeScale = 1f;

}


    public void SetButton(bool active)
    {
        if (_rewardedAdButton == null)
        {
            Debug.LogWarning("⚠️ RewardedAds: Rewarded ad button reference is missing.");
            return;
        }

        _rewardedAdButton.interactable = active;
    }
}
