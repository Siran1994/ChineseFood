using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FortuneCookieStretchAndCutDough : MonoBehaviour {

	public Animator animButtonNext;
	public Animator animDough;
	float normalisedTime = 0;
	public Animator animTray;

	public RollingPin rollingPin;
	public GameObject  DoughCutPrefab;
	public Transform[] DoughEndPos;
	int doughCount = 0;
	int doughInTrayCount = 0;

	int CompletedActionNo = 0;
	Color imgDoughColor;
	public ItemsColors itemsColors;
	public ParticleSystem psLevelCompleted;




	IEnumerator Start () {
		animButtonNext.gameObject.SetActive(false);
		animTray.gameObject.SetActive(false);
		imgDoughColor  = itemsColors.colors[(GameData.selectedFlavor>-1)? GameData.selectedFlavor : 0];

		foreach( Transform t in animDough.transform)
		{
			 t.GetComponent<Image>().color = imgDoughColor;
		}

		//BlockClicks.Instance.SetBlockAll(true);
		//BlockClicks.Instance.SetBlockAllDelay(.5f,false);

		 

		DragItem.OneItemEnabledNo = 0;
		animDough.speed = 0;
		yield return new WaitForSeconds(1);
		rollingPin.bEnabled = true;
		RawDoughCut.bEnabled = false;
	 

	 


		//LevelTransition.Instance.ShowScene();

		yield return new WaitForSeconds(1);
		Tutorial.Instance.ShowTutorial(0);
		EscapeButtonManager.AddEscapeButonFunction("ButtonHomeClicked" );
	}

 
 
	void Update () {

		if(rollingPin.bStretchDough)
		{
			rollingPin.bStretchDough= false;
			normalisedTime +=Time.deltaTime/4f;
			animDough.Play("stretchDough",-1,normalisedTime);
			if(normalisedTime >= 1)
			{
				StartCoroutine("EndStretching");
			}
			 
		}
		else  
		{
			 
		}
	}


	IEnumerator EndStretching () 
	{
		CompletedActionNo++;
		if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound(SoundManager.Instance.ActionCompleted);	
	

		rollingPin.EndStretching();
		yield return new WaitForSeconds(.5f);

		rollingPin.enabled = false;
		yield return new WaitForSeconds(1);

		Vector3 sPos = rollingPin.transform.position;
		Vector3 ePos = sPos+ new Vector3(-5.5f,0,0);
		Vector3 arcMax = new Vector3(0,2,0); 

		DragItem.OneItemEnabledNo = -1;

		float pom = 0;
		while(pom<1)
		{
			pom+=Time.fixedDeltaTime;
			rollingPin.transform.position = Vector3.Lerp(sPos, ePos,pom)  + pom* (1-pom) *arcMax;;

			yield return new WaitForFixedUpdate();
		}

		yield return new WaitForSeconds( .3f);
		rollingPin.gameObject.SetActive (false);
	
	 
		Tutorial.Instance.ShowTutorial(1);
	}


	public void CutDough( Transform doughParent)
	{
		Tutorial.Instance.StopTutorial();
		GameObject go =	GameObject.Instantiate(DoughCutPrefab, doughParent);
		go.transform.localScale = Vector3.one;
		go.transform.localPosition = Vector3.zero;
        go.transform.Find("DoughCut").GetComponent<RawDoughCut>().TopMovementLimit = GameObject.Find("Canvas/TopMovementLimit").transform;
        go.transform.Find("DoughCut").GetComponent<RawDoughCut>().BotMovementLimit = GameObject.Find("Canvas/BotMovementLimit").transform;
        go.transform.Find("DoughCut").GetComponent<RawDoughCut>().TestTopMovementLimit = go.transform.Find("DoughCut").transform;
        go.transform.Find("DoughCut").GetComponent<RawDoughCut>().TestBotMovementLimit = go.transform.Find("DoughCut").transform;


		 
		go.transform.Find("DoughCut").GetComponent<RawDoughCut>().TargetPoint =  DoughEndPos;
		go.transform.Find("DoughCut").GetComponent<Image>().color = imgDoughColor;
 
		doughCount++;

		//CompletedActionNo++;
		if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound(SoundManager.Instance.CutDoughSound);
	

		if(doughCount == 4)
		{
			if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound(SoundManager.Instance.ActionCompleted);	
			StartCoroutine("ShowTray");
			Tutorial.Instance.ShowTutorial(2);
		}
		else
			DragItem.OneItemEnabledNo = -1;
	}

	IEnumerator ShowTray()
	{
		animTray.gameObject.SetActive(true);
		animTray.Play("ShowTray",-1,0);
		yield return new WaitForSeconds(.5f);
		RawDoughCut.bEnabled = true;
	}


	//-------------locekd item----------
	int unlockItem = -1;
	public void LockedItemClick(int _itemNo)
	{
		if(CompletedActionNo>0 && CompletedActionNo<5)
		{
			unlockItem = _itemNo;
			WatchVideoPopUp.instance.ShowPopUpWatchVideo();
		}

	}

	public void UnlockItem()
	{
//		if(unlockItem == 2)
//			GameData.unlockedItems[0]  = 1;
//		else if(unlockItem == 4)
//			GameData.unlockedItems[1]  = 1;
//
//		GameObject go =GameObject.Find("Canvas/DoughCutters/Cutter"+unlockItem.ToString()+"Holder/Lock");
//		go.transform.parent.GetComponent<DragItem>().enabled = true;
//		GameObject.Destroy(go.transform.parent.GetComponent<EventTrigger>()); 
//		go.SetActive(false);
//		GameData.SetUnlockedItems();
//		 
//		unlockItem = -1;
	}


	public void NextPhase(string gameStatePhase)
	{

		if(gameStatePhase == "")
		{
			StartCoroutine("WaitNextPhase",1);
		}
		else if(gameStatePhase == "Dough")
		{
			Tutorial.Instance.StopTutorial();
			doughInTrayCount++;
			if(doughInTrayCount == 4)
			{

				//Debug.Log("Kraj");
				CompletedActionNo++;
				if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound(SoundManager.Instance.ActionCompleted);	

				StartCoroutine("WaitNextPhase",10);
			}
		}

	}

	IEnumerator WaitNextPhase(int phase)
	{
		if(phase == 1)
		{
 
		}
		if(phase == 10)
		{

			animTray.Play("HideTray");

			psLevelCompleted.gameObject.SetActive(true);
			psLevelCompleted.Play();

			yield return new WaitForSeconds(2);
			animButtonNext.gameObject.SetActive(true);
			psLevelCompleted.gameObject.SetActive(true);
			psLevelCompleted.Play();
			 
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
            SceneManager.LoadScene("WriteFortuneCookieMessage");
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

		//TODO:ADS - INTERSTITIAL_HOME
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
