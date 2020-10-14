using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FrySpringRollsScene : MonoBehaviour {
	 
	public Animator animButtonNext;
	public GameObject ButtonReplay;
	public GameObject ButtonNext;

	public Animator animFryerNet;
	public Animator animTemperatureInd;
	public Animator animProgress;
	public Animator animEndMenu;

	public Animator animTray;

 
	public Image[] ImgRawSpringRolls;
	public Image[] ImgFriedSpringRolls;
	public Color BurnedColor;
 

	public Transform[] SpringRollsEndPos;
 
	public Button ButtonFryer;

	 
	int springRollsColectedCount = 0;
	public ParticleSystem psLevelCompleted;

	public ParticleSystem psFrying1;
	public ParticleSystem psFrying2;
	public ParticleSystem psFrying3;

	[HideInInspector]
	public bool bFrying = false;
 
	float fryingSpeed= .1f;
	float normalisedFryingTime = 0;
	float normalisedHeatingTime = 0;

	bool bChangeImage = false;
	 
	float colorShift;

	 
	public Transform Cable;

	bool bFryerOn = false;

	public Sprite[] endMessagesSprtes;
	public Image imageEndMessage;

	//public ProgressBar progressBar;

	IEnumerator Start () {

		GlobalVariables.OnPauseGame +=FLPauseGame;
		GlobalVariables.OnUIContinueGame +=PauseUI_ContinueGame;
		GlobalVariables.OnFLContinueGame +=PauseFakeLoading_ContinueGame;

	
		 RawSpringRoll.bEnabled = false;
		animEndMenu.gameObject.SetActive(false);
		//BlockClicks.Instance.SetBlockAll(true);
		//BlockClicks.Instance.SetBlockAllDelay(.5f,false);

		ButtonFryer.enabled = false;
		animTray.gameObject.SetActive(false);

		animButtonNext.gameObject.SetActive(false);

		 
	 
		yield return new WaitForSeconds(1);
		while(GlobalVariables.bPauseGame) yield return new WaitForEndOfFrame();
		 
		for(int i = 0; i<3;i++)
		{
			ImgRawSpringRolls[i].transform.GetComponent<RawSpringRoll>().TargetPoint = SpringRollsEndPos;
		}
 
		//progressBar.gameObject.SetActive(false);
		animTemperatureInd.speed = 0;
		animProgress .speed = 0;

		//LevelTransition.Instance.ShowScene();
		EscapeButtonManager.AddEscapeButonFunction("ButtonHomeClicked" );
		// yield return new WaitForSeconds(1);
		Tutorial.Instance.ShowTutorial(0);
		 
		 
	}


 
	public void ButtonFryerClicked()
	{
		if(!bFryerOn)
		{
			ButtonFryer.transform.GetChild(1).gameObject.SetActive(false);
			ButtonFryer.transform.GetChild(0).gameObject.SetActive(true);
 
			ButtonFryer.enabled = false;
			bFryerOn = true;
			if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound(SoundManager.Instance.OnOffSound);	
			if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound(SoundManager.Instance.FryingSound);

		}
		else
		{
			ButtonFryer.transform.GetChild(1).gameObject.SetActive(true);
			ButtonFryer.transform.GetChild(0).gameObject.SetActive(false);
			ButtonFryer.enabled = false;
			bFryerOn = false;
			 if(bFrying)
			{
				StartCoroutine("FryingDone");
			}

			psFrying1.Stop();
			psFrying2.Stop();
			psFrying3.Stop();
			if(SoundManager.Instance!=null)
			{
				SoundManager.Instance.Play_Sound(SoundManager.Instance.OnOffSound);
				SoundManager.Instance.Stop_Sound(SoundManager.Instance.FryingSound);
				SoundManager.Instance.Stop_Sound(SoundManager.Instance.FryingSound2);
			}
		}
		Tutorial.Instance.StopTutorial();
	}

	IEnumerator InsertInOil()
	{
		ButtonFryer.enabled = false;
		yield return new WaitForSeconds(1);
		while(GlobalVariables.bPauseGame) yield return new WaitForEndOfFrame();
	 
		animFryerNet.Play("Show",-1,0);
		yield return new WaitForSeconds(2);
		while(GlobalVariables.bPauseGame) yield return new WaitForEndOfFrame();
		bFrying = true;
		ButtonFryer.enabled = true;
		psFrying3.gameObject.SetActive(true);

		if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound(SoundManager.Instance.FryingSound2);
	}

	IEnumerator FryingDone()
	{
        animProgress.enabled = false;
		bFrying = false;
		ButtonFryer.enabled = false;
		yield return new WaitForSeconds(1);
		while(GlobalVariables.bPauseGame) yield return new WaitForEndOfFrame();
		animFryerNet.Play("Remove",-1,0);
		yield return new WaitForSeconds(1);
		while(GlobalVariables.bPauseGame) yield return new WaitForEndOfFrame();
		animTray.gameObject.SetActive(true);

		if(normalisedFryingTime>.3 && normalisedFryingTime<.75f)
		{
			if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound(SoundManager.Instance.ActionCompleted);	
			animTray.Play("ShowTray2");
			for(int i = 0; i<3;i++)
			{
				ImgRawSpringRolls[i].transform.GetComponent<RawSpringRoll>().SetFryerStartPostionAndScale();
			}
			RawSpringRoll.bEnabled = true;
			animEndMenu.gameObject.SetActive(true); 
			animEndMenu.Play("default");
			imageEndMessage.sprite = endMessagesSprtes[Random.Range(1, 3)];

			Tutorial.Instance.ShowTutorial(3);
			yield return new WaitForSeconds(2.5f);
			while(GlobalVariables.bPauseGame) yield return new WaitForEndOfFrame();
			animEndMenu.gameObject.SetActive(false); 
		}
		else 	if(normalisedFryingTime>= .75f)
		{

			if(SoundManager.Instance!=null) 
			{
				SoundManager.Instance.Play_Sound(SoundManager.Instance.ActionFailedSound);	
				SoundManager.Instance.Stop_Sound(SoundManager.Instance.FryingSound);	
				SoundManager.Instance.Stop_Sound(SoundManager.Instance.FryingSound2);
			}
			Tutorial.Instance.StopTutorial();
			Debug.Log("ZAGORELO");
			animEndMenu.gameObject.SetActive(true); 
			animEndMenu.Play("default");
			imageEndMessage.sprite = endMessagesSprtes[0];
			yield return new WaitForSeconds(2.5f);
			while(GlobalVariables.bPauseGame) yield return new WaitForEndOfFrame();
			animEndMenu.gameObject.SetActive(false); 

			animButtonNext.gameObject.SetActive(true);
			ButtonReplay.SetActive(true);
			ButtonNext.SetActive(false);

			Tutorial.Instance.ShowTutorial(4);
		}
		else  
		{
 			if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound(SoundManager.Instance.ActionFailedSound);	
			Debug.Log("NIJE PECENO");
			animEndMenu.gameObject.SetActive(true); 
			imageEndMessage.sprite = endMessagesSprtes[0];
			animEndMenu.Play("default");
			yield return new WaitForSeconds(2.5f);
			while(GlobalVariables.bPauseGame) yield return new WaitForEndOfFrame();
			animEndMenu.gameObject.SetActive(false); 

			animButtonNext.gameObject.SetActive(true);
			psLevelCompleted.gameObject.SetActive(true);
			psLevelCompleted.Play();
			if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound(SoundManager.Instance.ActionCompleted);	

			ButtonReplay.SetActive(true);
			ButtonNext.SetActive(false);
		}

	}



	void Update () 
	{
		if( !GlobalVariables.bPauseGame)
		{
			if(bFryerOn)
			{
				if(normalisedHeatingTime<1 )
				{
					animTemperatureInd.Play("RiseTemperature",-1,normalisedHeatingTime);
					normalisedHeatingTime +=Time.deltaTime*.2f;
				 
					if(normalisedHeatingTime>=1)
						StartCoroutine("InsertInOil");

					if(!psFrying1.gameObject.activeSelf && normalisedHeatingTime>.3f)  psFrying1.gameObject.SetActive(true);
					if(!psFrying2.gameObject.activeSelf && normalisedHeatingTime>.7f)  psFrying2.gameObject.SetActive(true);
					 

				}
			}
			else if(normalisedHeatingTime>0  )
			{
				normalisedHeatingTime -=Time.deltaTime*.1f;
				if(normalisedHeatingTime<0) normalisedHeatingTime = 0;
				animTemperatureInd.Play("RiseTemperature",-1,normalisedHeatingTime);
			}

			if(bFrying  )
			{
				normalisedFryingTime += Time.deltaTime*fryingSpeed;
				//progressBar.SetProgress(normalisedFryingTime,true);
				animProgress.Play("FryingProgress", -1, normalisedFryingTime);
				if(!bChangeImage && normalisedFryingTime>=.3f &&  normalisedFryingTime <.75f)
				{
					bChangeImage = true;
					StartCoroutine("ChangeSpritesFried");
					Tutorial.Instance.ShowTutorial(2);
				}
				else if( bChangeImage &&  normalisedFryingTime>=.75f)
				{
					bChangeImage = false;
					StartCoroutine("ChangeSpritesBurned");
				}

				if(  normalisedFryingTime>=1)
				{
					Debug.Log("BURNED");
					ButtonFryer.transform.GetChild(1).gameObject.SetActive(true);
					ButtonFryer.transform.GetChild(0).gameObject.SetActive(false);
					ButtonFryer.enabled = false;
					bFryerOn = false;
					if(bFrying)
					{
						StartCoroutine("FryingDone");
					}

					psFrying1.Stop();
					psFrying2.Stop();
					psFrying3.Stop();

//					animButtonNext.gameObject.SetActive(true);
//					ButtonReplay.SetActive(true);
//					ButtonNext.SetActive(false);
				}
			}
		}
	}


	IEnumerator ChangeSpritesFried()
	{
		for(int i = 0; i<3;i++)
		{
			ImgFriedSpringRolls[i].gameObject.SetActive(true);
			ImgFriedSpringRolls[i].color = new Color(1,1,1,0);
		}
		float pom = 0;
		while (pom < 1)
		{
			pom += Time.deltaTime*2;
			for(int i = 0; i<3;i++)
			{
				ImgFriedSpringRolls[i].color = new Color(1,1,1,pom);
				ImgRawSpringRolls[i].color = new Color(1,1,1,1-pom);
			}
			yield return new WaitForEndOfFrame();
		}


		yield return new WaitForEndOfFrame();
		for(int i = 0; i<3;i++)
		{
			ImgRawSpringRolls[i].enabled = false;
			ImgFriedSpringRolls[i].color = Color.white;
		}
	}

	IEnumerator ChangeSpritesBurned()
	{
		 
		float pom = 0;
		while (pom < 1)
		{
			pom += Time.deltaTime*2;
			for(int i = 0; i<3;i++)
			{
				ImgFriedSpringRolls[i].color =   Color.Lerp (Color.white,BurnedColor,pom);
			}
			yield return new WaitForEndOfFrame();
		}


		yield return new WaitForEndOfFrame();
		for(int i = 0; i<3;i++)
		{
			ImgFriedSpringRolls[i].color =  BurnedColor;
		}
	}


	public void NextPhase(string gameStatePhase)
	{

		if(gameStatePhase == "Plug")
		{
			StartCoroutine("CNextPhase",1);
		}
		else if(gameStatePhase == "SpringRoll")
		{
			springRollsColectedCount++;
			if(springRollsColectedCount == 3)
			{
				//Debug.Log("Kraj");
				StartCoroutine("CNextPhase",10);
			}
		}

	}

 

	IEnumerator CNextPhase(int phase)
	{
		if(phase == 1)
		{
			Debug.Log("PLUG");
			 Tutorial.Instance.StopTutorial();
			float pom = 0;
			Vector3 cStartPos = Cable.localPosition; 
			while(pom<1)
			{
				yield return new WaitForEndOfFrame();
				pom+=Time.deltaTime*2;
				Cable.localPosition = Vector3.Lerp( cStartPos, Vector3.zero, pom);
			}
//			if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound(SoundManager.Instance.PlugInSound);
			yield return new WaitForSeconds(.1f);
			while(GlobalVariables.bPauseGame) yield return new WaitForEndOfFrame();
			ButtonFryer.enabled = true;


			 Tutorial.Instance.ShowTutorial(1);
		}
		else if(phase == 10)
		{
			animTray.Play("EndAnim");

			yield return new WaitForSeconds(.5f);
			while(GlobalVariables.bPauseGame) yield return new WaitForEndOfFrame();

		 

			animFryerNet.Play("Hide",-1,0);
			yield return new WaitForSeconds(1);
			while(GlobalVariables.bPauseGame) yield return new WaitForEndOfFrame();

			psLevelCompleted.gameObject.SetActive(true);
			psLevelCompleted.Play();
			if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound(SoundManager.Instance.ActionCompleted);	 
			yield return new WaitForSeconds(2);
			while(GlobalVariables.bPauseGame) yield return new WaitForEndOfFrame();
	 
			animButtonNext.gameObject.SetActive(true);

		}

	} 



	 
	 

 
	public void ButtonReplayClicked()
	{
        //SDKManager.Instance.ShowAd(ShowAdType.VideoAD, 2, "重吃春卷");
        Tutorial.Instance.StopTutorial();
		//BlockClicks.Instance.SetBlockAll(true);
		 
		 SceneManager.LoadScene("FrySpringRolls") ;
		if(SoundManager.Instance!=null) SoundManager.Instance.Play_ButtonClick();
	}


	IEnumerator SetTimeScale(float timeScale, float waitTime)
	{
		yield return new WaitForSecondsRealtime(waitTime);
		Time.timeScale = timeScale;
		 
	}



	void FLPauseGame()
	{
		Debug.Log("FLPauseGame");
		animButtonNext.enabled = false;
		animFryerNet.enabled = false;
		animTemperatureInd.enabled = false;
		animProgress.enabled = false;
		animEndMenu.enabled = false;
		animTray.enabled = false;

		if(psFrying1.isPlaying) psFrying1.Pause();
		if(psFrying2.isPlaying) psFrying2.Pause();
		if(psFrying3.isPlaying) psFrying3.Pause();

		Tutorial.Instance.animTutorial.enabled = false;
		Tutorial.Instance.animTutorialHolder.enabled = false;

		if( bFrying  )
		{
			if(!GlobalVariables.bPauseUI)
			{
				PopUpPause.transform.parent.parent.GetComponent<MenuManager>().ShowPopUpMenu(PopUpPause);
				GlobalVariables.bPauseUI = true;
			}
		}
	}

	void PauseFakeLoading_ContinueGame()
	{
		Debug.Log("PauseFakeLoading_ContinueGame");
		if(!bFrying)
		{
			animButtonNext.enabled = true;
			animFryerNet.enabled = true;
			animTemperatureInd.enabled = true;
			animProgress.enabled = true;
			animEndMenu.enabled = true;
			animTray.enabled = true;

			if(psFrying1.isPaused) psFrying1.Play();
			if(psFrying2.isPaused) psFrying2.Play();
			if(psFrying3.isPaused) psFrying3.Play();

			Tutorial.Instance.animTutorial.enabled = true;
			Tutorial.Instance.animTutorialHolder.enabled = true;
		}
	}

	void PauseUI_ContinueGame()
	{
		Debug.Log("PauseUI_ContinueGame");
		if(bFrying)
		{
			animButtonNext.enabled = true;
			animFryerNet.enabled = true;
			animTemperatureInd.enabled = true;
			animProgress.enabled = true;
			animEndMenu.enabled = true;
			animTray.enabled = true;

			if(psFrying1.isPaused) psFrying1.Play();
			if(psFrying2.isPaused) psFrying2.Play();
			if(psFrying3.isPaused) psFrying3.Play();

			Tutorial.Instance.animTutorial.enabled = true;
			Tutorial.Instance.animTutorialHolder.enabled = true;
		}
	}


	public GameObject PopUpPause;
 
	public void ButtonUnpauseGameClicked( )
	{
		if(SoundManager.Instance!=null) SoundManager.Instance.Play_ButtonClick();

		PopUpPause.transform.parent.parent.GetComponent<MenuManager>().ClosePopUpMenu(PopUpPause);
		StopCoroutine("CUnpause");
		StartCoroutine("CUnpause");
	}

	IEnumerator CUnpause()
	{
		yield return new WaitForSecondsRealtime(1f);
		GlobalVariables.UnpauseGame(GlobalVariables.PauseSource.UI);
	}

	//-------------------------------------------------------------------------------------------------------------------

	public void ButtonNextClicked()
	{
         SDKManager.Instance.ShowAd(ShowAdType.ChaPing, 1, "炸春卷界面完成后点下一步");
        //BlockClicks.Instance.SetBlockAll(true);
		if(SoundManager.Instance!=null) SoundManager.Instance.Play_ButtonClick();
		StartCoroutine ("LoadNextScene");

		//TODO:ADS  INTERSTITIAL_NEXT
		//AdsManager.Instance.ShowInterstitial(AdsManager.INTERSTITIAL_NEXT);
       // GlobalVariables.ShowHomeNextInterstitial("next");
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
            SceneManager.LoadScene("EatSpringRolls");
        }
    }
	public void ButtonHomeClicked()
	{
       
        //BlockClicks.Instance.SetBlockAll(true);
		//BlockClicks.Instance.SetBlockAllDelay(.5f,false);
		if(SoundManager.Instance!=null) SoundManager.Instance.Play_ButtonClick();

		PopupAreYouSure.parent.parent .GetComponent<MenuManager>().ShowPopUpMenu( PopupAreYouSure.gameObject);
		//animNoodleMachine.speed = 0;
	}

	public Transform PopupAreYouSure;
	public void ButtonHomeYesClicked()
	{
        SDKManager.Instance.ShowAd(ShowAdType.ChaPing, 1, "返回首页");
        DragItem.OneItemEnabledNo = -1;
		//BlockClicks.Instance.SetBlockAll(true);
		if(SoundManager.Instance!=null) SoundManager.Instance.Play_ButtonClick();
		 SceneManager.LoadScene("HomeScene");
		//TODO:ADS  INTERSTITIAL_HOME
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
		//animNoodleMachine.speed = 1;
	}
}
