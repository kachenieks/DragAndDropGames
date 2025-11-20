using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;

public class BannerAd : MonoBehaviour
{
    [SerializeField] string _androidAdUnitId = "Banner_Android"; // Testa reklāma
    string _adUnitId;

    [SerializeField] Button _bannerButton;
    public bool IsBannerVisible = false;

    [SerializeField] BannerPosition _bannerPosition = BannerPosition.BOTTOM_CENTER;

    void Awake()
    {
        _adUnitId = _androidAdUnitId;
        Advertisement.Banner.SetPosition(_bannerPosition);
    }

    public void LoadBanner()
    {
        if (!Advertisement.isInitialized)
        {
            Debug.Log("Reklāmas nav inicializētas");
            return;
        }

        Debug.Log("Ielādē banner reklāmu: " + _adUnitId);

        // ✔ PAREIZĀ klase banner ielādei
        BannerLoadOptions loadOptions = new BannerLoadOptions
        {
            loadCallback = OnBannerLoaded,
            errorCallback = OnBannerError
        };

        Advertisement.Banner.Load(_adUnitId, loadOptions);
    }

    void OnBannerLoaded()
    {
        Debug.Log("Banner reklāma ielādēta");
        _bannerButton.interactable = true;
    }

    void OnBannerError(string message)
    {
        Debug.Log("Banner reklāmas ielādes kļūda: " + message);
        LoadBanner();
    }

    public void ShowBannerAd()
    {
        if (IsBannerVisible)
        {
            HideBannerAd();
        }
        else
        {
            // ✔ PAREIZĀ klase banner rādīšanai
            BannerOptions showOptions = new BannerOptions
            {
                showCallback = OnBannerShown,
                hideCallback = OnBannerHidden,
                clickCallback = OnBannerClicked
            };

            Advertisement.Banner.Show(_adUnitId, showOptions);
        }
    }

    public void HideBannerAd()
    {
        Advertisement.Banner.Hide();
    }

    void OnBannerClicked()
    {
        Debug.Log("Banner reklāma noklikšķināta");
    }

    void OnBannerHidden()
    {
        Debug.Log("Banner reklāma paslēpta");
        IsBannerVisible = false;
    }

    void OnBannerShown()
    {
        Debug.Log("Banner reklāma parādīta");
        IsBannerVisible = true;
    }

    public void SetButton(Button button)
    {
        if (button == null)
        {
            Debug.LogWarning("BannerAd: Poga ir null");
            return;
        }

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(ShowBannerAd);
        _bannerButton = button;
        _bannerButton.interactable = false;
    }
}
