#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using GoogleMobileAds.Api;
#pragma warning disable 0618
#pragma warning disable 0414
/*****************************************
	 文件:   SDKManager.cs
	 作者:   漠白
	 日期:   2020.7.25
	 功能:   Google广告接入管理类 
 *****************************************/
public enum PackageType  //出包类型
{
    PingCe,   //评测包(无广告)
    AD,       //广告包
}
public enum ShowAdType  //展示广告类型插屏
{
    Banner = 1,   //条幅广告
    ChaPing,      //插屏广告
    Reward,       //激励视频
}
public class SDKManager : MonoBehaviour
{
    public static SDKManager Instance; //单例类

    #region 全局设置
    public static string AppName = "模拟中餐制作"; //项目名称
    public static string ChannelName = "GooglePlay"; //渠道名称
    public static string VersionNum = "1.0"; //版本号

    [Header("APK类型")]
    public PackageType PT; //包类型

    [Header("是否展示广告(非AD包:false/Auto)")]
    public bool IsShowAd = true; //是否展示广告

    [Header("BannerAdId")]
    public string BannerAdId = "ca-app-pub-4624835792442570/3292348992"; //Banner测试参数

    [Header("ChaPingAdId")]
    public string ChaPingAdId = "ca-app-pub-4624835792442570/1458847985"; //插屏测试参数

    [Header("RewardAdId")]
    public string RewardAdId; //插屏测试参数

    private BannerView bannerView;  //banner广告
    private InterstitialAd interstitial;//插屏广告
    private RewardedAd rewardedAd;   //激励视屏

    private AdRequest request;
    //private AdRequest request()
    //{
    //    return new AdRequest.Builder()
    //        .AddTestDevice("4835597D8B9409233D31325459340479")
    //        .Build();
    //}

    #endregion

    #region Unity方法
    void Awake()
    {
        this.gameObject.name = "SDKManager";
        Application.targetFrameRate = 60; //控制update帧率
        if (Instance){
            DestroyImmediate(this);
        }else{
            Instance = this;
            DontDestroyOnLoad(this);
        }
        if (PT != PackageType.AD || Application.platform != RuntimePlatform.Android)  //非安卓和非广告包不展示广告
            IsShowAd = false;
      
    }
    void Start() //广告初始化
    {
        if (IsShowAd == false)
            return;
        BannerRequest(BannerAdId, AdPosition.Bottom);
        ChaPingInit(ChaPingAdId);
      //  RewardInit(RewardAdId);
        RepeatShowBan(2, 30, "默认启动后2s开始弹,30s循环"); //重复调用Banner 2s后展示 30秒刷新
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
        }
    }
    #endregion

    #region 公共方法
    public void ShowAd(ShowAdType ADType, int index = 0, string Log = "Unity日志展示")
    {
        switch (ADType)
        {
            case ShowAdType.Banner:
                bannerView.Show();
                Debug.Log(Log + "展示Banner");
                break;
            case ShowAdType.ChaPing:
                if (interstitial.IsLoaded())
                {
                    Debug.Log("插屏广告展示成功");
                    interstitial.Show();
                }
                else
                {
                    MakeToast("Interstitial ad is not ready yet");
                }
                Debug.Log(Log + "展示插屏");
                break;
            case ShowAdType.Reward:
              //  if (rewardedAd != null)
              //  {
              //   ///   Debug.Log("激励视频展示成功");
              //     // rewardedAd.Show();
              //  }
              //  else
              //  {
              //     // MakeToast("Rewarded ad is not ready yet.");
              //  }
              ////  Debug.Log(Log + "展示激励视屏");
                break;
        }
    }
    public void RepeatShowBan(float time, int rate, string Log = "Unity日志展示") //重复调用Banner
    {
        Debug.Log(Log + "展示Banner");
        if (IsShowAd == false)
            return;
        InvokeRepeating("ShowBanner", time, rate);
    }
    public void CloseBanner(string Log = "Unity日志展示") //关闭banner
    {
        Debug.Log(Log + "关闭Banner");
        if (IsShowAd == false)
            return;
        CancelInvoke("ShowBanner");
        bannerView.Hide();
    }
    public void MakeToast(string str = "暂无广告!!!")//安卓手机的提示信息(如安装软件时的提示)
    {
        AndroidJavaObject currentActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaClass Toast = new AndroidJavaClass("android.widget.Toast");
        currentActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
        {
            Toast.CallStatic<AndroidJavaObject>("makeText", currentActivity, str, Toast.GetStatic<int>("LENGTH_LONG")).Call("show");
        }));
    }
    #endregion

    #region  私有方法
    private void ShowBanner()//Banner展示
    {
        bannerView.Show();
    }
    #endregion

    #region 广告初始化
    void BannerRequest(string adId,AdPosition adPosition) //请求Banner
    {
        if (bannerView != null)
        {
            bannerView.Destroy();
        }

        bannerView = new BannerView(adId, AdSize.Banner, adPosition);
        request = new AdRequest.Builder()/*.AddTestDevice("4835597D8B9409233D31325459340479")*/.Build();
      
        //广告事件
        bannerView.OnAdLoaded += (sender, args) =>
        {
            Debug.Log(sender+"广告加载成功" + args);
        };
        bannerView.OnAdFailedToLoad += (sender, args) =>
        {
            Debug.Log(sender+"广告加载失败" + args);
        };
        bannerView.OnAdOpening += (sender, args) => { };
        bannerView.OnAdClosed += (sender, args) => { };
        bannerView.OnAdLeavingApplication += (sender, args) => { };

        bannerView.LoadAd(request);
       // bannerView.Hide();
    }
    void ChaPingInit(string adId) //请求插屏广告
    {
        if (interstitial != null)
        {
            interstitial.Destroy();
        }

        interstitial = new InterstitialAd(adId);
        request = new AdRequest.Builder()/*.AddTestDevice("4835597D8B9409233D31325459340479")*/.Build();
      
        //广告事件
        interstitial.OnAdLoaded += (sender, args) =>
        {
            Debug.Log(sender + "广告加载成功" + args);
        };
        interstitial.OnAdFailedToLoad += (sender, args) =>
        {
            Debug.Log(sender + "广告加载失败" + args);
        };
        interstitial.OnAdOpening += (sender, args) => { };
        interstitial.OnAdClosed += (sender, args) =>
        {
            interstitial.LoadAd(request);
        };
        interstitial.OnAdLeavingApplication += (sender, args) => { };

        interstitial.LoadAd(request);
       
    }
    void RewardInit(string adId)  //请求激励视频
    {
        rewardedAd = new RewardedAd(adId);
        request = new AdRequest.Builder()/*.AddTestDevice("4835597D8B9409233D31325459340479")*/.Build();
        //广告事件
        rewardedAd.OnAdLoaded += (sender, args) =>
        {
            Debug.Log(sender + "广告加载成功" + args);
        };
        rewardedAd.OnAdFailedToLoad += (sender, args) =>
        {
            Debug.Log(sender + "广告加载失败" + args);
        };
        rewardedAd.OnAdOpening += (sender, args) =>
        {
           // rewardedAd.LoadAd(request);
        };
        rewardedAd.OnAdClosed += (sender, args) =>
        {
           // Invoke("RewardReLoad", 2);
            //Debug.Log("激励视频关闭");
            //rewardedAd = new RewardedAd(adId);
            //request = new AdRequest.Builder().AddTestDevice("4835597D8B9409233D31325459340479").Build();
            //rewardedAd.LoadAd(request);
            //if (rewardedAd != null)
            //{
            //    Debug.Log("激励视频加载成功");
            //}
        };
        rewardedAd.OnAdFailedToShow += (sender, args) =>
        {
           // Debug.Log("激励视频展示失败");
          //  rewardedAd.LoadAd(request());
        };
        rewardedAd.OnUserEarnedReward += (sender, args) =>
        {
            if (ScrollMenu.IsToUnLockDough)//解锁面团
            {
                ScrollMenu.IsToUnLockDough = false;
                FindObjectOfType<ScrollMenu>().UnlockItem();
            }
            if (FortuneCookieMixIngredients.IsToUnLockMixHolder)//解锁搅拌器
            {
                FortuneCookieMixIngredients.IsToUnLockMixHolder = false;
                FindObjectOfType<FortuneCookieMixIngredients>().UnlockItem();
            }
            if (SpringRollsMixIngredients.IsToUnLockMixeder)//解锁搅拌器
            {
                SpringRollsMixIngredients.IsToUnLockMixeder = false;
                FindObjectOfType<SpringRollsMixIngredients>().UnlockItem();
            }
        };
       // rewardedAd.LoadAd(request);
    }
    #endregion

    void RewardReLoad()
    {
       // RewardInit("ca-app-pub-3940256099942544/5224354917");
        Debug.Log("激励视频加载成功");
    }

}
#region 编辑器拓展
public class Menus : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("用户数据/一键清理")]
    static void ClearPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("数据清理成功!");
    }
#endif
}
#endregion





