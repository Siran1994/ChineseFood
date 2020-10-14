using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollMenu : MonoBehaviour {
 
	public ScrollRect scrollRect;
	public RectTransform content;
	 //lista menija koji mogu da se prilazu
	public ScrollMenuGroup[] menus;
	 

	public Image[] btnImage;
//	public FacebookNativeAdMask fbNativeAdMask;
	bool bTutorialOver = false;
	public Animator animMenu;


	//pomeranje menija
	float speed = .1f;
	bool bNext = false;
	bool bPrev = false;
	float inertia = 1;

 
	public int activeMenu = 0;

	void Start () {
		if(Shop.RemoveAds == 2)
		{
		}
		else
		{
		}
		CalculateSpeed();

		bNext = false;
		bPrev = false;
		inertia = 0;
	}

	 
	void Update () {
		

		if((bPrev || inertia<-0.05f ) && scrollRect.horizontalNormalizedPosition > 0 ) 
		{
			if(bPrev) 
			{
				scrollRect.horizontalNormalizedPosition -= Time.deltaTime *speed;
			}
			else
			{
				scrollRect.horizontalNormalizedPosition += Time.deltaTime *  inertia;
				inertia *=.95f;
			}
		}
		else if((bNext || inertia>0.05f ) && scrollRect.horizontalNormalizedPosition < 1 )
		{
			if(bNext) 
			{
				scrollRect.horizontalNormalizedPosition += Time.deltaTime*speed;
			}
			else
			{
				scrollRect.horizontalNormalizedPosition += Time.deltaTime *  inertia;
				inertia *=.95f;
			}
		}

	}

	public void ChangeMenu( int menuNo)
	{
		StartCoroutine("CChangeMenu",menuNo);
	}

	 IEnumerator CChangeMenu( int menuNo)
	{
		if(animMenu!=null) animMenu.Play("HideMenu");
		yield return new WaitForSeconds(1f);


		scrollRect.horizontalNormalizedPosition = 0; 
		ShowMenu(menuNo);

		yield return new WaitForEndOfFrame();
		CalculateSpeed();

	}

	 
	public void ShowMenu(int  menuNo)
	{
		menus[menuNo].GetUnlockedItems();
		activeMenu = menuNo;

		//AKO SE MENJAJU SPRAJTOVI
		if(menus[menuNo].bChangeImage)
		{
			//zameni slicice
			for (int i = 0;i<menus[menuNo].MenuGroupSprites.Length;i++)
			{
				SetItemActiveInContent(true, i);//btnImage[i].transform.parent.gameObject.SetActive(true);
				btnImage[i].sprite =   menus[menuNo].MenuGroupSprites[i];
				btnImage[i].transform.parent.GetChild(1).gameObject.SetActive( !menus[menuNo].UnlockedItems[i] );
			}
			//gasenje neaktivnih dugmica
			for(int i =  menus[menuNo].MenuGroupSprites.Length; i<btnImage.Length; i++)
			{
				SetItemActiveInContent(false, i);//btnImage[i].transform.parent.gameObject.SetActive(false);
			}
		}
		//AKO SE MENJA BOJA SLICICA
		if(menus[menuNo].bChangeColor)
		{
			//zameni boje
			for (int i = 0;i<menus[menuNo].itemColors.colors.Length;i++)
			{
				SetItemActiveInContent(true, i); 
				if(btnImage[i].transform.CompareTag("ScrollMenuColor")) btnImage[i].color =   menus[menuNo].itemColors.colors[i];
				foreach(Transform t in btnImage[i].transform)
				{
					if(t.CompareTag("ScrollMenuColor") ) t.GetComponent<Image>().color = menus[menuNo].itemColors.colors[i];
				}

				btnImage[i].transform.parent.GetChild(1).gameObject.SetActive( !menus[menuNo].UnlockedItems[i] );
			}
			//gasenje neaktivnih dugmica
			for(int i = menus[menuNo].itemColors.colors.Length; i<btnImage.Length; i++)
			{
				SetItemActiveInContent(false, i); //btnImage[i].transform.parent.gameObject.SetActive(false);
			}
		}

		if(animMenu!=null) animMenu.Play("ShowMenu");

		CalculateSpeed();
//		if(buttonNativeAd !=null && fbNativeAdMask == null ) fbNativeAdMask = buttonNativeAd.transform.GetChild(1).GetComponent<FacebookNativeAdMask>();
//		if(fbNativeAdMask !=null)
//		{
//			 
//			fbNativeAdMask.StartCheckingVisibility();
//		}
		//else Debug.Log("fbNativeAdMask  NULL");
	}

	//podesiti da je item holder aktivan ili neaktivan
	void SetItemActiveInContent(bool _active, int i)
	{
		Transform parent = btnImage[i].transform.parent;
		Transform child =  btnImage[i].transform;
		while(parent != content)
		{
			child = parent;
			parent = child.parent;
		}
		child.gameObject.SetActive(_active);
	}


	public void HideMenu()
	{
		if(animMenu!=null) animMenu.Play("HideMenu");
//		if(fbNativeAdMask !=null) 
//		{
//			fbNativeAdMask.StopCheckingVisibility();
//			fbNativeAdMask.RemoveAdCompletly();
//		}
	}


	public void ButtonPrevDown()
	{
		if(speed == 0) CalculateSpeed();
		if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound( SoundManager.Instance.ButtonClick2);
		bNext = false;
		bPrev = true;
		inertia = 0;

	}

	public void ButtonPrevUp()
	{
		bNext = false;
		bPrev = false;
		inertia = -speed;
	}


	int videoBtnNo;
	int videoActiveMenu;                            //菜单 炸鸡面 灌汤包 福饼 蒸饺 春卷
                                                    //面团
                                                    //肉类
                                                    //配菜 蘑菇 西红柿 洋葱
                                                    //油 
                                                    //做福饼用的罐子彩色油
                                                    //巧克力
                                                    //糖果
                                                    public static bool IsToUnLockDough = false;
    public void MenuButtonClick(int btnNo)
	{
        if (Tutorial.Instance !=null) Tutorial.Instance.StopTutorial();
		if(  menus[activeMenu].UnlockedItems[btnNo-1]) 
		{
			Camera.main.SendMessage("ScrollMenuButtonClicked" , btnNo-1, SendMessageOptions.DontRequireReceiver );
			if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound( SoundManager.Instance.ButtonClick2);

			videoActiveMenu = -1;
			videoBtnNo = -1;
		}
		else
		{
			 //ZAKLJUCANO
			//Watch video
			videoBtnNo = btnNo-1;
			videoActiveMenu =  activeMenu;

           // UnlockItem();
           // SDKManager.Instance.MakeToast();
             SDKManager.Instance.ShowAd(ShowAdType.Reward,1,"解锁面团");
             IsToUnLockDough = true;

            

            // WatchVideoPopUp.instance.ShowPopUpWatchVideo(gameObject);
            // 
            //			Shop.Instance.MenuItemName =  GetFullPath(gameObject);
            //			Shop.Instance.WatchVideo();
            //			if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound( SoundManager.Instance.Locked);

        }
 
	}

	public void UnlockItem()
	{
		//  OTKLJUCAVANJE ITEMA
		//Debug.Log( videoActiveMenu + "  " + videoBtnNo);

		menus[videoActiveMenu].UnlockedItems[videoBtnNo] = true;
		menus[videoActiveMenu].SetUnlockedItems();
		btnImage[videoBtnNo].transform.parent.GetChild(1).gameObject.SetActive( !menus[videoActiveMenu].UnlockedItems[videoBtnNo] );
		//if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound( SoundManager.Instance.Coins);
	}



	public void ButtonNextDown()
	{
		if(speed == 0) CalculateSpeed();
		if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound( SoundManager.Instance.ButtonClick2);
		 
		bNext = true;
		bPrev = false;
		inertia = 0;
	}

	public void ButtonNextUp()
	{
		bNext = false;
		bPrev = false;
		inertia = speed;
	}

	public void CalculateSpeed()
	{
		speed =  .2f*  content.rect.width / (1+ (content.rect.width  - scrollRect.GetComponent<RectTransform>().rect.width ) )   ;
	}
 
	private string GetFullPath(GameObject go)
	{
		return go.transform.parent == null
			? go.name
				: GetFullPath(go.transform.parent.gameObject) + "/" + go.name;
	}
}
