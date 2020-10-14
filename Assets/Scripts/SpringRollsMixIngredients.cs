using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class SpringRollsMixIngredients : MonoBehaviour {

	public Animator animButtonNext;
	public Animator animBowl;
 
	float mixingTime;

	public Sprite[] mixingDoughSprites;
	int mixingPhase = 0;
	public Image imgDough;
	 

	public GameObject MixIngredientdHolder;
	public GameObject DoughHolder;
	Transform MixerHolder;
	 
	public ParticleSystem psLevelCompleted;

	int CompletedActionNo = 0;

	public CanvasGroup cgMixerHolder;
	 
	bool bShowMixerTutorial = true;
	IEnumerator Start () {
		animButtonNext.gameObject.SetActive(false);


		//BlockClicks.Instance.SetBlockAll(true);
		//BlockClicks.Instance.SetBlockAllDelay(.5f,false);

		Mixer.bEnabled = false;
		Mixer.bMixBowl = false;

		DragItem.OneItemEnabledNo = 0;
		yield return new WaitForSeconds(1);
		DragItem.OneItemEnabledNo = 1; 

		DoughHolder.SetActive(false);
		MixerHolder = GameObject.Find("BowlHolder/BowlAnimationHolder/MixerHolder").transform;

		if( GameData.unlockedItems[1]  == 1)
		{
			GameObject go =GameObject.Find("Canvas/MixerHolder/HandMixer/Lock");
			go.transform.parent.GetComponent<Mixer>().enabled = true;
			GameObject.Destroy(go.transform.parent.GetComponent<EventTrigger>()); 
			go.SetActive(false);
		}

		cgMixerHolder.gameObject.SetActive(false);

		//LevelTransition.Instance.ShowScene();
		EscapeButtonManager.AddEscapeButonFunction("ButtonBackClicked" );
		yield return new WaitForSeconds(1);
		Tutorial.Instance.ShowTutorial(0);
		//scrollMenu.ShowMenu(0);

	}


	void Update () {
		if(Mixer.bMixBowl )
		{
			Debug.Log("bMixBowl   " + mixingPhase);
			if(mixingPhase == 1)
			{
				imgDough.color = new Color(1,1,1,mixingTime);
			}
			if(Mixer.bHandMixer)
				mixingTime+=Time.deltaTime*.3f;
			else mixingTime+=Time.deltaTime*.5f;
			 
			imgDough.transform.Rotate(new Vector3(0,0,Time.deltaTime*120) );

			if(bShowMixerTutorial)
			{
				Tutorial.Instance.StopTutorial();
				bShowMixerTutorial = false;
			}
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
		if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound(SoundManager.Instance.ActionCompleted);	 
		yield return new WaitForSeconds(1);
		animButtonNext.gameObject.SetActive(true);
	 
		yield return new WaitForSeconds(.5f);
		//BlockClicks.Instance.SetBlockAll(false);
	}




	//-------------locekd item----------

    public static bool IsToUnLockMixeder = false;
	public void LockedItemClick( )
	{
        if (Mixer.bEnabled && GameData.unlockedItems[1] == 0)
        {
           // SDKManager.Instance.MakeToast();
           // SDKManager.Instance.ShowAd(ShowAdType.Reward,1,"点击解锁搅拌器");
           // IsToUnLockMixeder = true;
        }

        //WatchVideoPopUp.instance.ShowPopUpWatchVideo();	
	 
	}

	public void UnlockItem()
	{
		GameData.unlockedItems[1]  = 1;
		GameObject go =GameObject.Find("Canvas/MixerHolder/HandMixer/Lock");
		go.transform.parent.GetComponent<Mixer>().enabled = true;
		GameObject.Destroy(go.transform.parent.GetComponent<EventTrigger>()); 
		go.SetActive(false);
		//GameData.SaveUnlocekedItemsToPP();
	}
 


	public void NextPhase(string gameStatePhase)
	{
		Tutorial.Instance.StopTutorial();
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
		else if(gameStatePhase == "Salt")
		{
			Tutorial.Instance.ShowTutorial(3);
			StartCoroutine("WaitNextPhase",3);
		}
 
	}

	IEnumerator WaitNextPhase(int phase)
	{
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
			//animBowl.Play("InsertSalt");
			yield return new WaitForSeconds( 1.5f);

 
			DragItem.OneItemEnabledNo = 0;
			Mixer.bEnabled = true;
			mixingPhase = 1;
			DoughHolder.SetActive(true);
			 
			cgMixerHolder.gameObject.SetActive(true);
			cgMixerHolder.alpha = 0;
			float pom = 0;
			while(pom <1)
			{
				pom+=Time.deltaTime*3;
				yield return new WaitForEndOfFrame();
				cgMixerHolder.alpha = pom;
			}
		}
	}




    public GameObject LoadPanel;
    public void ButtonNextClicked()
	{
        SDKManager.Instance.ShowAd(ShowAdType.ChaPing, 1, "春卷搅拌界面返回首页点下一步");
        //BlockClicks.Instance.SetBlockAll(true);
		
		if(SoundManager.Instance!=null) SoundManager.Instance.Play_ButtonClick();
        if (LoadPanel != null /*&& SDKManager.Instance.IsCanShowAd*/)
        {
            LoadPanel.SetActive(true);
        }
        else
        {
            SceneManager.LoadScene("SpringRollsBakeWrapper");
        }
    }






	public Transform  PopupAreYouSure;
	public void ButtonHomeClicked()
	{
        
        //BlockClicks.Instance.SetBlockAll(true);
		//BlockClicks.Instance.SetBlockAllDelay(.5f,false);
		if(SoundManager.Instance!=null) SoundManager.Instance.Play_ButtonClick();

		PopupAreYouSure.parent.parent .GetComponent<MenuManager>().ShowPopUpMenu( PopupAreYouSure.gameObject);

	}

	public void ButtonHomeYesClicked()
	{
        SDKManager.Instance.ShowAd(ShowAdType.ChaPing, 1, "返回首页");
        //BlockClicks.Instance.SetBlockAll(true);
        if (SoundManager.Instance!=null) SoundManager.Instance.Play_ButtonClick();

		//TODO:ADS  INTERSTITIAL_HOME
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
