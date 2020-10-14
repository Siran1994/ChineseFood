using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class FortuneCookieMixIngredients : MonoBehaviour {

	public Animator animButtonNext;
	public Animator animBowl;
 

	float mixingTime;

	public Sprite[] mixingDoughSprites;
	int mixingPhase = 0;
	public Image imgDough;
	public Color imgDoughColor;

	public GameObject MixIngredientdHolder;
	public GameObject DoughHolder;
	Transform MixerHolder;

	 
	public ParticleSystem psLevelCompleted;

	int CompletedActionNo = 0;

	public CanvasGroup cgMixerHolder;
	public ScrollMenu scrollMenu;

	IEnumerator Start () {

		if(GameData.unlockedItems[0]  == 1)
		{
			cgMixerHolder.gameObject.SetActive(true);
			GameObject go =GameObject.Find("Canvas/MixerHolder/HandMixer/Lock");
			go.transform.parent.GetComponent<Mixer>().enabled = true;
			GameObject.Destroy(go.transform.parent.GetComponent<EventTrigger>()); 
			go.SetActive(false);
		}

		animButtonNext.gameObject.SetActive(false);
		scrollMenu.gameObject.SetActive(false);
		cgMixerHolder.gameObject.SetActive(false);

		//BlockClicks.Instance.SetBlockAll(true);
		//BlockClicks.Instance.SetBlockAllDelay(.5f,false);

		Mixer.bEnabled = false;
		Mixer.bMixBowl = false;

		DragItem.OneItemEnabledNo = 0;
		yield return new WaitForSeconds(1);
		DragItem.OneItemEnabledNo = 1; 

		DoughHolder.SetActive(false);
		MixerHolder = GameObject.Find("BowlHolder/BowlAnimationHolder/MixerHolder").transform;
 
		//LevelTransition.Instance.ShowScene();
		EscapeButtonManager.AddEscapeButonFunction("ButtonHomeClicked" );
		yield return new WaitForSeconds(1);
		Tutorial.Instance.ShowTutorial(0);
 
	}
	
 
	void Update () {
		if(Mixer.bMixBowl )
		{
			Debug.Log("bMixBowl   " + mixingPhase);
			if(mixingPhase == 1)
			{
				imgDough.color = new Color(imgDoughColor.r,imgDoughColor.g,imgDoughColor.b,mixingTime);
			}
			if(Mixer.bHandMixer)
				mixingTime+=Time.deltaTime*.3f;
			else mixingTime+=Time.deltaTime*.5f;

			imgDough.transform.Rotate(new Vector3(0,0,Time.deltaTime*120) );

			 
			if(SoundManager.Instance!=null) 
			{
				if(Mixer.bHandMixer)
					SoundManager.Instance.Play_Sound(SoundManager.Instance.MixerSound2);	
				else
					SoundManager.Instance.Play_Sound(SoundManager.Instance.MixerSound);	
			}

			if(mixingTime>mixingPhase)
			{
				imgDough.sprite = mixingDoughSprites[mixingPhase];
				mixingPhase = Mathf.FloorToInt(mixingTime);
				if(mixingPhase == 2) MixIngredientdHolder.SetActive(false);
				if(mixingPhase == 5) 
				{
					CompletedActionNo++;
					if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound(SoundManager.Instance.ActionCompleted);	
					 
					mixingPhase = -1;
					Debug.Log("Kraj");
					StartCoroutine("EndMixing");
				}
			}
			if(!Mixer.bHandMixer) Mixer.bMixBowl = false;
		}
		else if( (mixingPhase >= 1 || 	mixingPhase == -1) && !Mixer.bMixBowl)
		{
			
			if(SoundManager.Instance!=null) 
			{
				if(Mixer.bHandMixer)
					SoundManager.Instance.Stop_Sound(SoundManager.Instance.MixerSound2);	
				else
					SoundManager.Instance.Stop_Sound(SoundManager.Instance.MixerSound);	
			}
		}
	}


	IEnumerator EndMixing()
	{
		//BlockClicks.Instance.SetBlockAll(true);
		Mixer m  = MixerHolder.GetChild(0).GetComponent<Mixer>();
		m.EndMixing();
		yield return new WaitForSeconds(1);
		animBowl.Play("EndMixing",-1,0);

		psLevelCompleted.gameObject.SetActive(true);
		psLevelCompleted.Play();

		yield return new WaitForSeconds(1);
		animButtonNext.gameObject.SetActive(true);
		psLevelCompleted.gameObject.SetActive(true);
		psLevelCompleted.Play();
		if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound(SoundManager.Instance.ActionCompleted);	
		yield return new WaitForSeconds(.5f);
		//BlockClicks.Instance.SetBlockAll(false);
	}




	//-------------locekd item----------

    public static bool IsToUnLockMixHolder = false;
	public void LockedItemClick( )
	{
        if (Mixer.bEnabled && GameData.unlockedItems[0] == 0)
        {
            //WatchVideoPopUp.instance.ShowPopUpWatchVideo();
           // UnlockItem();
           // SDKManager.Instance.MakeToast();
           //  SDKManager.Instance.ShowAd(ShowAdType.Reward,1,"解锁搅拌器");
            // IsToUnLockMixHolder = true;
        }


    }

	public void UnlockItem()
	{
		GameData.unlockedItems[0]  = 1;
		GameObject go =GameObject.Find("Canvas/MixerHolder/HandMixer/Lock");
		go.transform.parent.GetComponent<Mixer>().enabled = true;
		GameObject.Destroy(go.transform.parent.GetComponent<EventTrigger>()); 
		go.SetActive(false);
		//GameData.SaveUnlocekedItemsToPP();
	}





	 




	public void NextPhase(string gameStatePhase)
	{
		 
		if(gameStatePhase == "PitcherFillMold")
		{
			
			StartCoroutine("WaitNextPhase",1);
		}
		else if(gameStatePhase == "PitcherFillMoldEnd")
		{
 
		 
			DragItem.OneItemEnabledNo = 2;
			 Tutorial.Instance.ShowTutorial(1);
		}
 

		else if(gameStatePhase == "Flour")
		{
			
			StartCoroutine("WaitNextPhase",2);
		}
		else if(gameStatePhase == "FlourEnd")
		{
 
			DragItem.OneItemEnabledNo = 3;
			 Tutorial.Instance.ShowTutorial(2);
		}
		else if(gameStatePhase == "Sugar")
		{
			
			StartCoroutine("WaitNextPhase",3);
		}
		else if(gameStatePhase == "SugarEnd")
		{
			 
			DragItem.OneItemEnabledNo = 4;
			 Tutorial.Instance.ShowTutorial(3);
		}
 

		else if(gameStatePhase == "ButterEnd")
		{
			 
			DragItem.OneItemEnabledNo = -1;
			Mixer.bEnabled = true;
			mixingPhase = 1;
			DoughHolder.SetActive(true);

			StartCoroutine("WaitNextPhase",4);

			Tutorial.Instance.ShowTutorial(4);
			//Tutorial.Instance.StartCoroutine("ShowMixerTut");
		}

		else if( gameStatePhase.StartsWith("JF") )
		{
			GameData.selectedFlavor = int.Parse(gameStatePhase.Substring(2,1)) -1;
			imgDoughColor  = scrollMenu.menus[scrollMenu.activeMenu].itemColors.colors[GameData.selectedFlavor];
			StartCoroutine("WaitNextPhase",5);
		}

	}

	IEnumerator WaitNextPhase(int phase)
	{
		Tutorial.Instance.StopTutorial();
		yield return new WaitForEndOfFrame();
		if(phase == 1)
		{
			yield return new WaitForSeconds(.5f);
			if(SoundManager.Instance!=null) SoundManager.Instance.StopAndPlay_Sound(SoundManager.Instance.WaterSound);
			yield return new WaitForSeconds(.5f);
			animBowl.Play("Water");
		}
		 
		else if(phase == 2)
		{
			yield return new WaitForSeconds(1.3f);
			animBowl.Play("InsertFlour");
			 
			if(SoundManager.Instance!=null) SoundManager.Instance.StopAndPlay_Sound(SoundManager.Instance.SugarSound);
		}
		else if(phase == 3)
		{
			yield return new WaitForSeconds(.8f);
			animBowl.Play("InsertSugar");
			 
			if(SoundManager.Instance!=null) SoundManager.Instance.StopAndPlay_Sound(SoundManager.Instance.SugarSound);
		}
		else if(phase == 4)
		{
			scrollMenu.gameObject.SetActive(true);
			yield return new WaitForEndOfFrame();
			scrollMenu.ShowMenu(0);
			if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound( SoundManager.Instance.ShowItemSound);

		}
		else if(phase == 5)
		{
			yield return new WaitForSeconds(5f);
			scrollMenu.HideMenu();
			Debug.Log("HIDE");
			yield return new WaitForSeconds(1);
			if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound( SoundManager.Instance.ShowItemSound);
			cgMixerHolder.gameObject.SetActive(true);
			cgMixerHolder.alpha = 0;
			float pom = 0;
			while(pom <1)
			{
				pom+=Time.deltaTime*3;
				yield return new WaitForEndOfFrame();
				cgMixerHolder.alpha = pom;
			}
			scrollMenu.gameObject.SetActive(false);
			Tutorial.Instance.ShowTutorial(5);
			//Tutorial.Instance.StartCoroutine("ShowMixerTut");
		}
	}


    public GameObject LoadPanel;
    public void ButtonNextClicked()
	{
         SDKManager.Instance.ShowAd(ShowAdType.ChaPing, 1, "加幸运饼干配料界面点下一步");
        //BlockClicks.Instance.SetBlockAll(true);
        if (SoundManager.Instance != null) SoundManager.Instance.Play_ButtonClick();
        if (LoadPanel != null /*&& SDKManager.Instance.IsCanShowAd*/)
        {
            LoadPanel.SetActive(true);
        }
        else
        {
            SceneManager.LoadScene("StretchAndCutDough");
        }
       
	}



	 


	public Transform  PopupAreYouSure;
	public void ButtonHomeClicked()
	{
      
		if(SoundManager.Instance!=null) SoundManager.Instance.Play_ButtonClick();

		PopupAreYouSure.parent.parent .GetComponent<MenuManager>().ShowPopUpMenu( PopupAreYouSure.gameObject);

	}

	public void ButtonHomeYesClicked()
	{
        SDKManager.Instance.ShowAd(ShowAdType.ChaPing, 1, "返回首页");
        //BlockClicks.Instance.SetBlockAll(true);
        if (SoundManager.Instance!=null) SoundManager.Instance.Play_ButtonClick();

		//TODO:ADS -  INTERSTITIAL_HOME
		//AdsManager.Instance.ShowInterstitial(AdsManager.INTERSTITIAL_HOME);
        GlobalVariables.ShowHomeNextInterstitial("home");

		 SceneManager.LoadScene("HomeScene");
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
