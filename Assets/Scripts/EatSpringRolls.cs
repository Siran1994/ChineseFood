using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EatSpringRolls : MonoBehaviour {

	 
	public Image[] springRolls; 
	public Image[] springRollsSauce; 

	public Image imgSauce; 
	 

	public Animator PopupTapToEat;
	public Animator animTimerAndReplayHolder;
	public Animator animTimer;
	public Animator animEndMenu;

	public Animator animButtonNext;
	public  GameObject ButtonReplay;


	bool bEnableEat = false;
	public GameObject ButtonTapToEat;


	public GameTimer gameTimer;

	public Sprite[] endMessagesSprtes;
	public Image imageEndMessage;

	int springRollsLeft = 0;

	public Transform EatPos;
	DragItem activeItem;
	public Transform activeItemHolder;

	IEnumerator Start ()
	{
		PopupTapToEat.Play("defHidden",-1,0);
		yield return new WaitForEndOfFrame();
		PopupTapToEat.gameObject.SetActive(false);

		DragItem.OneItemEnabledNo = 0;
		springRollsLeft = springRolls.Length;
		for(int i = 0; i<springRolls.Length; i++)
		{
			springRollsSauce[i].color = imgSauce.color;
		}
 
		animButtonNext.gameObject.SetActive(false);
		GlobalVariables.OnPauseGame +=FLPauseGame;

		animEndMenu.gameObject.SetActive(false);
		ButtonReplay.SetActive(false);
		//BlockClicks.Instance.SetBlockAll(true);

		yield return new WaitForSeconds(.7f);
		//LevelTransition.Instance.ShowScene();
		EscapeButtonManager.AddEscapeButonFunction("ButtonBackClicked" );
		yield return new WaitForSeconds(.3f);

		//BlockClicks.Instance.SetBlockAll(false);
		if(SoundManager.Instance!=null) SoundManager.Instance.listStopSoundOnExit.Add( SoundManager.Instance.TimerSound);

		Tutorial.Instance.ShowTutorial(0);
		yield return new  WaitForSeconds(4f);
		Tutorial.Instance.StopTutorial();
		PopupTapToEat.gameObject.SetActive(true);
		PopupTapToEat.Play("show",-1,0);
	}




	public void NextPhase(string _phase) 
	{
		if(_phase.StartsWith("InSauce"))
		{
			activeItem = activeItemHolder.GetChild(0).GetComponent<DragItem>();
			activeItem.TargetPoint  = new Transform[] {EatPos};
		}
		else if(_phase=="DSEat")
		{
			Destroy(activeItem.gameObject);
			if(SoundManager.Instance!=null) SoundManager.Instance.StopAndPlay_Sound(SoundManager.Instance.EatSound);
			//dodaj PARTICLES
			activeItem = null;
			Tutorial.Instance.StopTutorial();
			springRollsLeft--;
			Debug.Log("DSEat " + springRollsLeft);
			DragItem.OneItemEnabledNo = 1;
			if(springRollsLeft == 0)
			{

				Debug.Log("KRAJ");
				if(SoundManager.Instance!=null) SoundManager.Instance.StopAndPlay_Sound(SoundManager.Instance.ActionCompleted);
				gameTimer.StopTimer();
				bEnableEat = false;
				StartCoroutine("LevelCompleted");
			}
		}

	}


	//---------------------------------------------------------------

	public void ButtonTapToEatClicked()
	{
		ButtonTapToEat.transform.GetChild(0).GetComponent<Button>().interactable = false;

		if(SoundManager.Instance!=null) SoundManager.Instance.Play_ButtonClick();
		//BlockClicks.Instance.SetBlockAll(true);
		//BlockClicks.Instance.SetBlockAllDelay(1f,false);
		StartCoroutine("WTapToEeat");
	}

	IEnumerator WTapToEeat()
	{
		yield return new WaitForSeconds(.1f);
		PopupTapToEat.CrossFade("hide",.1f,-1,0);

	 
		DragItem.OneItemEnabledNo = 1;
		animTimerAndReplayHolder.SetBool("bShow",true);
		yield return new WaitForSeconds(1f);

		gameTimer.StartTimer();
		if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound(SoundManager.Instance.TimerSound);	
		bEnableEat = true;
		Debug.Log("START");
		PopupTapToEat.gameObject.SetActive(false);

		//Tutorial.Instance.ShowTutorial(0);

	}


	public void ButtonReplayClicked()
	{
        //SDKManager.Instance.ShowAd(ShowAdType.VideoAD, 2, "重吃春卷");
        if (SoundManager.Instance!=null) 
		{
			SoundManager.Instance.Stop_Sound(SoundManager.Instance.TimerSound);	
			SoundManager.Instance.Play_ButtonClick();
			SoundManager.Instance.Stop_Sound(SoundManager.Instance.OutOfTimeSound);	
		}


		 SceneManager.LoadScene("EatSpringRolls");
		//BlockClicks.Instance.SetBlockAll(true);
		//BlockClicks.Instance.SetBlockAllDelay(1f,false);

	}

    #region 成功和失败结算

    public GameObject SuccessPan;
    public GameObject FailPan;


    #endregion


    public void Time10SecLeft()
	{
		animTimer.Play ("10sec");
	}

	public void OutOfTime()
	{
      
		StartCoroutine("WOutOfTime");
	}


	IEnumerator WOutOfTime()//失败
	{
       
        Debug.Log("OUT OF TIME");
		bEnableEat = false;

		if( activeItemHolder.childCount >0 )   activeItem = activeItemHolder.GetChild(0).GetComponent<DragItem>();
		if( activeItem !=null) 
		{
			activeItem.bDrag = false;
			activeItem.bIskoriscen = true;
			DragItem.OneItemEnabledNo = 3;
		}


		if(SoundManager.Instance!=null)
		{
			SoundManager.Instance.Stop_Sound(SoundManager.Instance.TimerSound);	
			SoundManager.Instance.Play_Sound(SoundManager.Instance.OutOfTimeSound);	
		}

		imageEndMessage.sprite = endMessagesSprtes[0];
		animEndMenu.gameObject.SetActive(true); 

		yield return new WaitForSeconds(1f);
		animTimerAndReplayHolder.SetBool("bShow",false);
		yield return new WaitForSeconds(1f);
		animTimerAndReplayHolder.SetBool("bShow",true);

        //animButtonNext.gameObject.SetActive(true);
        //animTimer.gameObject.SetActive(false);
        //ButtonReplay.SetActive(true);
        //SDKManager.Instance.ShowAd(ShowAdType.Native,1,"展示原生广告");
        SuccessPan.SetActive(false);
        FailPan.SetActive(true);

    }

	IEnumerator LevelCompleted()//成功
	{
        
        imageEndMessage.sprite = endMessagesSprtes[Random.Range(1, 3)];
		animEndMenu.gameObject.SetActive(true); 

		yield return new WaitForSeconds(1f);
		animTimerAndReplayHolder.SetBool("bShow",false);
		yield return new WaitForSeconds(1f);
		animTimerAndReplayHolder.SetBool("bShow",true);

        //animButtonNext.gameObject.SetActive(true);

        //animTimer.gameObject.SetActive(false);
        //ButtonReplay.SetActive(true);
        SuccessPan.SetActive(true);
        FailPan.SetActive(false);
    }


	bool bEnableEatTmp;
	public Transform  PopupAreYouSure;

	public void ButtonHomeClicked()
	{
        
        bEnableEatTmp = bEnableEat;
		bEnableEat = false;
		//BlockClicks.Instance.SetBlockAll(true);
		//BlockClicks.Instance.SetBlockAllDelay(.5f,false);
		if(SoundManager.Instance!=null) SoundManager.Instance.Play_ButtonClick();

		PopupAreYouSure.parent.parent .GetComponent<MenuManager>().ShowPopUpMenu( PopupAreYouSure.gameObject);
		StartCoroutine(SetTimeScale(0, 0f));

	}

	public void ButtonHomeYesClicked()
	{
        SDKManager.Instance.ShowAd(ShowAdType.ChaPing, 1, "返回首页");
        StartCoroutine(SetTimeScale(1, 1f));

		if(SoundManager.Instance!=null) 
		{
			SoundManager.Instance.Stop_Sound(SoundManager.Instance.TimerSound);	
			SoundManager.Instance.Play_ButtonClick();
			SoundManager.Instance.Stop_Sound(SoundManager.Instance.OutOfTimeSound);	
		}

		//TODO:ADS -  INTERSTITIAL_HOME
		//AdsManager.Instance.ShowInterstitial(AdsManager.INTERSTITIAL_HOME);
        GlobalVariables.ShowHomeNextInterstitial("home");

		 SceneManager.LoadScene("HomeScene");
		//BlockClicks.Instance.SetBlockAll(true);
	}

	public void ButtonHomeNoClicked()
	{
       
        StartCoroutine(SetTimeScale(1, 1f));
		//BlockClicks.Instance.SetBlockAll(true);
		//BlockClicks.Instance.SetBlockAllDelay(.5f,false);
		if(SoundManager.Instance!=null) SoundManager.Instance.Play_ButtonClick();
		PopupAreYouSure.parent.parent .GetComponent<MenuManager>().ClosePopUpMenu( PopupAreYouSure.gameObject);
		if(EscapeButtonManager.EscapeButonFunctionStack.Count == 0)  EscapeButtonManager.AddEscapeButonFunction("ButtonBackClicked" );
		StartCoroutine(CEnableEat(1f,true));
	}

	IEnumerator SetTimeScale(float timeScale, float waitTime)
	{
		yield return new WaitForSecondsRealtime(waitTime);
		Time.timeScale = timeScale;
	}

	IEnumerator CEnableEat (  float waitTime, bool val)
	{
		yield return new WaitForSecondsRealtime(waitTime);
		bEnableEat = val;
	}

	void FLPauseGame()
	{
		if( bEnableEat  )
		{
			PopUpPause.transform.parent.parent.GetComponent<MenuManager>().ShowPopUpMenu(PopUpPause);
			GlobalVariables.bPauseUI = true;
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


	public void ButtonNextClicked()
	{
        SDKManager.Instance.ShowAd(ShowAdType.ChaPing, 1, "吃春卷界面完成后点下一步");
        StartCoroutine("CNextPhase");
	}

	IEnumerator CNextPhase()
	{
		if(SoundManager.Instance!=null) SoundManager.Instance.Play_ButtonClick();
		//BlockClicks.Instance.SetBlockAll(true);

		yield return new WaitForSeconds(.1f);
		//load next
		 SceneManager.LoadScene("SelectMiniGame");


		//TODO:ADS  INTERSTITIAL_NEXT
		//AdsManager.Instance.ShowInterstitial(AdsManager.INTERSTITIAL_NEXT);
        GlobalVariables.ShowHomeNextInterstitial("next");
		yield return new WaitForEndOfFrame();

	}


}
