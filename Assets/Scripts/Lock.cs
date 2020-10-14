using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Lock : MonoBehaviour
{
    public CustomButton Items;

    public Button AdToUnLock;
    public string ItemName;
    public static bool[] IsItem = {false, false, false};

    void Start()
    {
        AdToUnLock.onClick.AddListener(delegate
        {
            SDKManager.Instance.ShowAd(ShowAdType.Reward,1,"点击解锁菜");
            // PlayerPrefs.SetInt(ItemName, 1);
            Invoke("IsToUnlock",0.5f);
        });
    }

    void IsToUnlock()
    {
        PlayerPrefs.SetInt(ItemName, 1);
    }

    public  void ToUnLock()
    {
        Items.enabled = true;
        AdToUnLock.gameObject.SetActive(false);
       
    }


    void Update()
    {
        if (PlayerPrefs.GetInt(ItemName, 0) != 0)
        {
            Items.enabled = true;
            AdToUnLock.gameObject.SetActive(false);
        }
    }

}
