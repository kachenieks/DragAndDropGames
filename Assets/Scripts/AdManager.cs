using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AdManager : MonoBehaviour
{
    public AdsInitializer adsInitializer;
    public InterstitialAd interstitialAd;
    [SerializeField] bool turnOffInterstitialAd = false;

    // tavs esošais flags — paturam, bet vairs nebalstāmies uz to
    private bool firstAdShown = false;

    public RewardedAds rewardedAds;
    [SerializeField] bool turnOffRewardedAds = false;

    // 🔸 Ja OnSceneLoaded atnāk, bet reklāma vēl nav gatava, atzīmējam,
    // ka pēc ielādes to vajag parādīt.
    private bool pendingShowAfterLoad = false;

    public static AdManager instance { get; private set; }


    public BannerAd bannerAd;
    [SerializeField] bool turnOffBannerAd = false;

    private void Awake()
    {
        if (adsInitializer == null)
            adsInitializer = FindFirstObjectByType<AdsInitializer>();

        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        if (adsInitializer != null)
            adsInitializer.onAdsInitialized += HandleAdsInitialized;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        if (adsInitializer != null)
            adsInitializer.onAdsInitialized -= HandleAdsInitialized;

        // drošībai — noņemam subskripciju, ja bija
        if (interstitialAd != null)
            interstitialAd.onInterstitialAdReady -= HandleInterstitialReady;
    }

    private void HandleAdsInitialized()
    {
        if (turnOffInterstitialAd) return;

        // Ja references nav, mēģinām atrast
        if (interstitialAd == null)
            interstitialAd = FindFirstObjectByType<InterstitialAd>();

        if (interstitialAd == null)
        {
            Debug.LogWarning("AdManager: InterstitialAd nav atrasts scenā. Pievieno to Ads objektam.");
            return;
        }

        // Piereģistrējamies tikai vienu reizi
        interstitialAd.onInterstitialAdReady -= HandleInterstitialReady;
        interstitialAd.onInterstitialAdReady += HandleInterstitialReady;

        // Ja nav gatavs — ielādē
        if (!interstitialAd.isReady)
            interstitialAd.LoadAd();

        if (!turnOffRewardedAds) 
        {
            rewardedAds.LoadAd();
        }

        if (!turnOffBannerAd)
        {
            bannerAd.LoadBanner();
        }
    }

    private void HandleInterstitialReady()
    {
        // Ja sagaidām rādīšanu pēc ainas ielādes — rādām tūlīt
        if (pendingShowAfterLoad && interstitialAd != null && interstitialAd.isReady && !turnOffInterstitialAd)
        {
            Debug.Log("AdManager: Interstitial became ready — showing now (pending from scene load).");
            pendingShowAfterLoad = false;
            interstitialAd.ShowAd();
            firstAdShown = true; // saglabājam tavu esošo flag lietošanai, ja vajag
            return;
        }

        // Pretējā gadījumā vienkārši piefiksējam, ka gatavs nākamajai pārejai
        if (!firstAdShown)
        {
            Debug.Log("Showing first interstitial ad.");
            // Ja gribi rādīt uzreiz pie pirmās ielādes, atkomentē nākamo rindu:
            // interstitialAd.ShowAd();
            firstAdShown = true;
        }
        else
        {
            Debug.Log("Interstitial ad is ready for later use.");
        }
    }

    // ⚠️ Tavu sākotnējo pirmās ainas skip loģiku noņemam,
    // jo tu prasīji rādīt reklāmu pie KATRAS ainas ielādes.
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Atsvaidzinām references
        if (interstitialAd == null)
            interstitialAd = FindFirstObjectByType<InterstitialAd>();

        // Ja ir UI poga ar tagu — piesienam (nav obligāti)
        Button interstitialButton = null;
        var go = GameObject.FindGameObjectWithTag("InterstitialAdButton");
        if (go != null) interstitialButton = go.GetComponent<Button>();
        if (interstitialAd != null && interstitialButton != null)
            interstitialAd.SetButton(interstitialButton);

        if (adsInitializer == null)
            adsInitializer = FindFirstObjectByType<AdsInitializer>();

        if (bannerAd == null)
            bannerAd = FindFirstObjectByType<BannerAd>();

        if (turnOffInterstitialAd || interstitialAd == null)
        {
            Debug.Log("AdManager: ads off vai InterstitialAd nav pieejams šajā brīdī.");
            return;
        }

        // MĒRĶIS: parādīt reklāmu katru reizi, kad iekļūst jaunā ainā
        if (interstitialAd.isReady)
        {
            Debug.Log($"Showing interstitial ad on scene load → {scene.name}");
            pendingShowAfterLoad = false;
            interstitialAd.ShowAd();
        }
        else
        {
            Debug.Log($"Ad not ready on scene load → {scene.name}, loading and will show when ready.");
            pendingShowAfterLoad = true;
            interstitialAd.LoadAd(); // HandleInterstitialReady parādīs, tiklīdz būs gatava
        }

        if (rewardedAds == null)
            rewardedAds = FindFirstObjectByType<RewardedAds>();

        Button bannerAdButton = GameObject.FindGameObjectWithTag("BannerAdButton").GetComponent<Button>();
        if (bannerAd != null && bannerAdButton != null)
            bannerAd.SetButton(bannerAdButton);

        Button rewardedAdButton = GameObject.FindGameObjectWithTag("RewardedAdButton").GetComponent<Button>();

        if (rewardedAds != null && rewardedAdButton != null)
            rewardedAds.SetButton(rewardedAdButton);
    }
}
