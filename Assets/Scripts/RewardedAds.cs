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
        var adsInitializer = FindFirstObjectByType<AdsInitializer>();
        if (adsInitializer != null)
        {
            adsInitializer.onAdsInitialized += OnAdsInitialized;
        }
    }

    private void OnDisable()
    {
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
        LoadAd();
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
        Debug.Log($"🟢 Rewarded ad loaded: {adUnitId}");

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


    // -----------------------------
    //   REKLĀMA SĀKAS — PALĒNINI
    // -----------------------------
    public void OnUnityAdsShowStart(string adUnitId)
    {
        if (SceneManager.GetActiveScene().name == "CityScene")
        {
            Debug.Log("🐌 Reklāma sākās — palēninu laiku!");
            Time.timeScale = 0.3f;  // Te nosaki, cik stipri palēninās spēle
        }
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

    // -----------------------------
    //   REKLĀMA BEIDZAS — REWARD
    // -----------------------------
    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        if (adUnitId.Equals(_adUnitId) && showCompletionState == UnityAdsShowCompletionState.COMPLETED)
        {
            Debug.Log("🟢 Rewarded ad completed - granting reward!");

            // TIKAI CITYSCENE – Iznīcina lidobjektus
            if (SceneManager.GetActiveScene().name == "CityScene" && flyingObjectManager != null)
            {
                Debug.Log("✨ CityScene reward – Destroying all flying objects!");
                flyingObjectManager.DestroyAllFlyingObjects();
            }

            _rewardedAdButton.interactable = false;
            StartCoroutine(WaitAndLoad(10f));
        }

        // 10 sekundes palēninājums pēc reklāmas
        if (SceneManager.GetActiveScene().name == "CityScene")
            StartCoroutine(RestoreTimeAfterDelay());
    }


    // -----------------------------
    //  Atgriež laiku normālu pēc 10s
    // -----------------------------
    private IEnumerator RestoreTimeAfterDelay()
    {
        Debug.Log("⏳ Palēnināts režīms vēl 10 sekundes...");
        yield return new WaitForSecondsRealtime(10f);
        Time.timeScale = 1f;
        Debug.Log("⏱️ Laiks atjaunots normāls!");
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
