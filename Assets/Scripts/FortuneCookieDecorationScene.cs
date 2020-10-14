using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FortuneCookieDecorationScene : MonoBehaviour {

	public ScrollMenu scrollMenu;

	int phase = 0;
	public Image imgFortuneCookie;
	public Image imgFortuneCookieChocolate;
	public ItemsColors doughColors;
	public ItemsColors chocolateColors;

 
	int selectedDecotartionMenu;


	//dekoracije
	bool bEnableCreateDec = false;
	Sprite sNewDecoration;
	int decorationMaxCount = 20;
	int decCounter = 0;
	public Transform decorationsHolder;
	public ParticleSystem psLevelCompleted;
	public Collider2D colider2D; 
	 

	//SCREEN CAPTURE
	public Transform SC_decorationsHolder ;
	public Image SC_imgFortuneCookie;
	public Image SC_imgFortuneCookieChocolate;

 
  



	IEnumerator Start()
	{
		if(GameData.selectedFlavor == -1) GameData.selectedFlavor = 2; 
		if(GameData.chocolateFillColor == -1) GameData.chocolateFillColor = 0; 



		//BlockClicks.Instance.SetBlockAll(true);
		 
		imgFortuneCookie.color  =  doughColors.colors[GameData.selectedFlavor];
		imgFortuneCookieChocolate.color  =  chocolateColors.colors[GameData.chocolateFillColor];
		SC_imgFortuneCookie.color  =  doughColors.colors[GameData.selectedFlavor];
		SC_imgFortuneCookieChocolate.color  =  chocolateColors.colors[GameData.chocolateFillColor];

		colider2D.gameObject.SetActive(false);
 
		yield return new WaitForEndOfFrame();
		//scrollMenu.HideMenu();

		yield return new WaitForSeconds(.1f);

		//LevelTransition.Instance.ShowScene();

		yield return new WaitForSeconds(.3f);


		Tutorial.Instance.ShowTutorial(0);
		EscapeButtonManager.AddEscapeButonFunction("ButtonHomeClicked" );

		scrollMenu.ShowMenu(0);
		scrollMenu.scrollRect.horizontalNormalizedPosition = 0;
		yield return new WaitForSeconds(.5f);
		//BlockClicks.Instance.SetBlockAll(false);
	}

	 
	void Update () {
		if(bEnableCreateDec && decCounter< decorationMaxCount && MenuManager.activeMenu == "")
		{
			
			if(Input.GetMouseButtonDown(0))
			{
				RaycastHit2D hit = Physics2D.Raycast (Camera.main.ScreenToWorldPoint (Input.mousePosition), Vector3.zero);

				//Collider2D[] hitColliders = Physics2D.Raycast(TestPoint.position, testDistance  , 1 << LayerMask.NameToLayer("Tool"+ToolNo.ToString()+"Interact")); //layermask to filter the varius colliders
				if(hit.transform !=null)
				{
					GameObject go = new GameObject("dec"+decCounter.ToString().PadLeft(2,'0'));
					go.transform.parent = decorationsHolder;
					RectTransform rt = go.AddComponent<RectTransform>();
					rt.sizeDelta = new Vector2(100,100);
					Image img =  go.AddComponent<Image>();
					img.sprite = sNewDecoration;
					img.preserveAspect = true;

					go.transform.localScale = Vector3.one;
					Vector3 pos =  Camera.main.ScreenToWorldPoint (Input.mousePosition);
					pos =  new Vector3( pos.x,pos.y,0);
					go.transform.position = pos;
					int angle  = Random.Range(0,360);
					float scale  = Random.Range(.6f,1f);
					go.transform.localRotation = Quaternion.Euler(0,0,angle);
					go.transform.localScale = scale*Vector3.one;


					//kreiranje mirror-a
					 
					 
					go = new GameObject("dec"+decCounter.ToString().PadLeft(2,'0'));
					go.transform.parent = SC_decorationsHolder;
					rt = go.AddComponent<RectTransform>();
					rt.sizeDelta = new Vector2(100,100);
					go.transform.localScale = Vector3.one;
					pos = pos +  SC_decorationsHolder.position - decorationsHolder.position;
					go.transform.position =  pos;
					go.transform.localRotation = Quaternion.Euler(0,0,angle);
					go.transform.localScale = scale*Vector3.one;
					img =  go.AddComponent<Image>();
					img.sprite = sNewDecoration;
					img.preserveAspect = true;

					decCounter++;

					Tutorial.Instance.StopTutorial();

					if(SoundManager.Instance!=null) SoundManager.Instance.StopAndPlay_Sound( SoundManager.Instance.Decoration);
				}

			}
			//if(bEnableCreateDec && decCounter>= decorationMaxCount && MenuManager.activeMenu == "")
			//{

			//	if(Input.GetMouseButtonDown(0))
			//	{
			//		MenuManager.Instance.ShowPopUpDialogTitleText("超出限制");
			//		MenuManager.Instance.ShowPopUpDialogCustomMessageText("不要过度装饰你的福饼!"); 
			//		Tutorial.Instance.StopTutorial();
			//		bEnableCreateDec = false;
			//	}
			//}
		}
	}


	public void	SelectDecorationsMenuButtonClicked(int itemIndex)
	{
		 
		if(SoundManager.Instance!=null) SoundManager.Instance.Play_ButtonClick();
		if(selectedDecotartionMenu == itemIndex)
		{
			//BlockClicks.Instance.SetBlockAll(true);
			//BlockClicks.Instance.SetBlockAllDelay(.5f,false);
			return;
		}
		else
		{
			bEnableCreateDec = false;
		 
			//BlockClicks.Instance.SetBlockAll(true);
			//BlockClicks.Instance.SetBlockAllDelay(.5f,false);
			scrollMenu.ChangeMenu(itemIndex);
			selectedDecotartionMenu= itemIndex;
		}
	}

	public void	ScrollMenuButtonClicked(int itemIndex)
	{
		//BlockClicks.Instance.SetBlockAll(true);
		if(phase == 0)
		{
//			//Zamena oblika slatkisa
//			BlockClicks.Instance.SetBlockAllDelay(1.5f,false);
//			//imgDecorationPhase1.color = Color.white;
//			//imgDecorationPhase1.sprite = scrollMenu.menus[1].MenuGroupSprites[itemIndex];
//			StartCoroutine(FadeOutAndInImage( imgJellyGum, scrollMenu.menus[0].MenuGroupSprites[itemIndex])) ; 
//			selectedJellyGum = itemIndex;
//			Tutorial.Instance.StopTutorial();
//		}
//		else if(phase == 1)
//		{
			Tutorial.Instance.StopTutorial();
			if(bEnableCreateDec)
			{
		 		Tutorial.Instance.ShowTutorial(1);
			}

			//KREIRANJE DEKORACIJA DODIROM PRSTA NA KUGLU
			 
			bEnableCreateDec = true;
			sNewDecoration = scrollMenu.menus[selectedDecotartionMenu].MenuGroupSprites[itemIndex];

			colider2D.gameObject.SetActive(true);
			 
			//BlockClicks.Instance.SetBlockAllDelay(0.3f,false);

		}

	}
 
	public void ButtonResetClicked()
	{
       // SDKManager.Instance.ShowAd(ShowAdType.VideoAD, 2, "烤幸运饼糖果重做");
        if (SoundManager.Instance!=null) SoundManager.Instance.Play_ButtonClick();
		Tutorial.Instance.StopTutorial();
		//BlockClicks.Instance.SetBlockAll(true);
		//BlockClicks.Instance.SetBlockAllDelay(.5f,false);

		if(phase == 1)
		{
			//obrisati dekoracije
			for(int i= decorationsHolder.childCount-1; i>=0; i--)
			{
				GameObject.Destroy(decorationsHolder.GetChild(i).gameObject);
			}
			for(int i= decorationsHolder.childCount-1; i>=0; i--)
			{
				GameObject.Destroy(SC_decorationsHolder.GetChild(i).gameObject);
			}
			decCounter = 0;
		}
	}

	public void ButtonNextClicked()
	{
        SDKManager.Instance.ShowAd(ShowAdType.ChaPing, 1, "幸运饼干加糖果界面点下一步");
        StartCoroutine("CNextPhase");
	}


	IEnumerator CNextPhase()
	{
		if(SoundManager.Instance!=null) SoundManager.Instance.Play_ButtonClick();
		//BlockClicks.Instance.SetBlockAll(true);
		if(phase == 0)
		{
			 
			phase = 1;
			scrollMenu.HideMenu();
			psLevelCompleted.gameObject.SetActive(true);
			psLevelCompleted.Play();
			yield return new WaitForSeconds(1);
			//SNIMANJE 


			CaptureImage  captureImage = transform.GetComponent<CaptureImage>();
 
			SC_imgFortuneCookie.transform.parent.localScale = 1.8f*Vector3.one;
			captureImage.ScreenshotMeal();

			yield return new WaitForSeconds(1);

			//load next
			StartCoroutine ("LoadNextScene");
			//TODO: MINJA  INTERSTITIAL_NEXT
			//AdsManager.Instance.ShowInterstitial(AdsManager.INTERSTITIAL_NEXT);
            GlobalVariables.ShowHomeNextInterstitial("next");
		}

		yield return new WaitForEndOfFrame();

	}

    public GameObject LoadPanel;
    IEnumerator LoadNextScene()
    {
        Debug.Log("Load Next");
        yield return new WaitForEndOfFrame();
        if (LoadPanel != null /*&& SDKManager.Instance.IsCanShowAd*/)
        {
            LoadPanel.SetActive(true);
        }
        else
        {
            SceneManager.LoadScene("EatFortuneCookie");
        }
    }
	public void ButtonHomeClicked()
	{
       
        //BlockClicks.Instance.SetBlockAll(true);
		//BlockClicks.Instance.SetBlockAllDelay(.5f,false);
		if(SoundManager.Instance!=null) SoundManager.Instance.Play_ButtonClick();

		PopupAreYouSure.parent.parent .GetComponent<MenuManager>().ShowPopUpMenu( PopupAreYouSure.gameObject);
	}

	public Transform PopupAreYouSure;
	public void ButtonHomeYesClicked()
    {
        SDKManager.Instance.ShowAd(ShowAdType.ChaPing, 1, "返回首页");
        //BlockClicks.Instance.SetBlockAll(true);
        if (SoundManager.Instance!=null) SoundManager.Instance.Play_ButtonClick();
		 SceneManager.LoadScene("HomeScene");
		//TODO: MINJA  INTERSTITIAL_HOME
		//AdsManager.Instance.ShowInterstitial(AdsManager.INTERSTITIAL_HOME);
        GlobalVariables.ShowHomeNextInterstitial("home");
	}

	public void ButtonHomeNoClicked()
	{
        //BlockClicks.Instance.SetBlockAll(true);
		//BlockClicks.Instance.SetBlockAllDelay(.5f,false);
		if(SoundManager.Instance!=null) SoundManager.Instance.Play_ButtonClick();
		PopupAreYouSure.parent.parent .GetComponent<MenuManager>().ClosePopUpMenu( PopupAreYouSure.gameObject);
		if(EscapeButtonManager.EscapeButonFunctionStack.Count == 0)  EscapeButtonManager.AddEscapeButonFunction("ButtonHomeClicked" );
	}



}
