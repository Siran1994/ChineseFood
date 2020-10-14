using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.UI;

public class NativeAd_Banner : MonoBehaviour
{
    [Header("指定渲染的UI")]
    public Image Ui;

    [Header("原生广告的ID")]
    public string AdId;

    [Header("原生广告的标题")]
    public Text Title;

    [Header("原生广告的描述")]
    public Text desc;

    public GameObject Root;
    public Button JumpBtn;
    public Button CloseBtn;

    void Awake()
    {
        //if (SDKManager.Instance.IsCanShowAd)
        //{
        //    AdId = "193994";
        //    Root.gameObject.SetActive(true);
        //    if (SDKManager.Instance.IsNativeAvaiable())
        //    {
        //        SDKManager.Instance.ShowAd(ShowAdType.Native, 1, "首页展示原生", Ui);
        //        Title.text = SDKManager.Instance.adTitle;
        //    }
        //}
    }
    void Start()
    {
        //SDKManager.Instance.NativeInit(193995);

        JumpBtn.onClick.AddListener(delegate
        {
            //SDKManager.Instance.NativeClick(AdId);//原生广告点击回调绑定
        });
        CloseBtn.onClick.AddListener(delegate
        {
            Root.gameObject.SetActive(false);
        });

        if (Ui==null)
        {
            Ui = this.GetComponent<Image>();
        }

        if (AdId=="")
        {
           Debug.LogError("请输入原生广告ID");
        }
        else
        {
            Invoke("ShowNativeAd", 0.0f);
        }
    }
    void ShowNativeAd()
    {
        //if (SDKManager.Instance.IsCanShowAd)
        //{
        //    Debug.Log("888888888888" + SDKManager.Instance.IsNativeAvaiable());

        //    if (SDKManager.Instance.IsNativeAvaiable())
        //    {
        //        SDKManager.Instance.ShowAd(ShowAdType.Native, 2, "首页展示原生Banner", Ui);
        //        Title.text = SDKManager.Instance.adTitle;
        //        desc.text = SDKManager.Instance.adDesc;
        //    }
        //}
    }
}
