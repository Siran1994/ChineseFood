using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class LoadPannel : MonoBehaviour
{
    public Image bgimg;
    public Sprite[] imgs;
  //  public GameObject bg;

   // public Button Nextbtn;
    public String NextSceneName;
    void Awake()
    {
       
    }

    void Start()
    {
       // SDKManager.Instance.CloseBanner();
        bgimg.sprite = imgs[Random.Range(0, 2)];

        Invoke("LoadNext",1);
        //Nextbtn.onClick.AddListener(delegate
        //{
        //    SceneManager.LoadScene(NextSceneName);
        //});
    }

    void LoadNext()
    {
        SceneManager.LoadScene(NextSceneName);
    }

    //private void OnDisable()
    //{
    //    bg.SetActive(false);
    //}
}
