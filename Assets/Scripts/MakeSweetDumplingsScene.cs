using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MakeSweetDumplingsScene : MonoBehaviour {
	public Animator animButtonNext;
	public ItemsColors sweetDumplingsDoughColors;

	int phase = 0;
	public ScrollMenu scrollMenu; 
	public Transform scrollMenuContent;

	public Transform[] Dough;

	public RollingPin rollingPin;
	public Image imgRollingPin;
	Vector3 rollingPinStartPos;
	float normalisedTime = 0;
	Animator animKneadDough;

	int sweetDumplingsCount = 0;
	public ParticleSystem psLevelCompleted;

	IEnumerator Start ()
	{
		scrollMenu.gameObject.SetActive(false);
		animButtonNext.gameObject.SetActive(false);
		RollingPin.OnStretchDough +=OnStretchDough;
		rollingPin.transform.parent.gameObject.SetActive(false);
		rollingPinStartPos = rollingPin.transform.position;

		try{ 
		for (int i = 0; i < 4; i++) 
		{
			 
			Image[] imgs = Dough[i].GetComponentsInChildren<Image>(true);
			foreach(Image img in imgs) img.color = sweetDumplingsDoughColors.colors [GameData.selectedFlavor];

			//Dough[i].color= sweetDumplingsDoughColors.colors [GameData.selectedFlavor];
			//Dough[i].transform.parent.GetChild(1).GetComponent<Image>().color= sweetDumplingsDoughColors.colors [GameData.selectedFlavor];
		}
		} catch{}

		//BlockClicks.Instance.SetBlockAll(true);
		yield return new WaitForSeconds(.1f);
		//LevelTransition.Instance.ShowScene();

		yield return new WaitForSeconds(.1f);
		//BlockClicks.Instance.SetBlockAll(false);

		 
 
	

		Tutorial.Instance.ShowTutorial(0);
	}


	public void NextPhase(string _phase) 
	{
		if( _phase.StartsWith("D") )
		{
			DragItem.OneItemEnabledNo = 0;
			int i= int.Parse(_phase.Substring(1,1)) -1;
			animKneadDough = Dough[i].GetComponent<Animator>();
			phase++;
			StartCoroutine("CNextPhase");
			 
		}
		else if( _phase.StartsWith("F") )
		{
			scrollMenu.HideMenu();
			if(phase == 1 ) Tutorial.Instance.ShowTutorial(3);
		}
		else if( _phase == "End" )
		{
			sweetDumplingsCount++;
			phase++;
			Tutorial.Instance.StopTutorial();
			if(phase == 8 ) StartCoroutine("CNextPhase");
			else 	DragItem.OneItemEnabledNo = 1;
		}
	}
	 
	IEnumerator CNextPhase()
	{
		Tutorial.Instance.StopTutorial();

		if(phase == 1 || phase == 3 || phase == 5 || phase == 7)//razvlacenje testa
		{
			DragItem.OneItemEnabledNo = 0;
			Tutorial.Instance.StopTutorial();
 			//PRIKAZ OKLAGIJE
			if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound( SoundManager.Instance.ShowItemSound);
			rollingPin.transform.parent.gameObject.SetActive(true);
			rollingPin.transform.transform.position = rollingPinStartPos;
			float pom = 0;
			while(pom<1)
			{
				pom+=Time.deltaTime*2;
				imgRollingPin.color = new Color(1,1,1,pom);
				yield return new WaitForEndOfFrame();
			}
			imgRollingPin.color = Color.white;

			yield return new WaitForEndOfFrame();
			rollingPin.bEnabled = true;
			normalisedTime = 0;
			if(phase == 1 ) Tutorial.Instance.ShowTutorial(1);
		}

		if(phase == 8) 
		{
			Debug.Log("Kraj IGRE");
			//yield return new WaitForSe
			//Tutorial.Instance.StopTutorial();

			psLevelCompleted.gameObject.SetActive(true);
			psLevelCompleted.Play();
			animButtonNext.gameObject.SetActive(true);
			if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound(SoundManager.Instance.ActionCompleted);	
		}
		yield return null;
	}
 
	 
	void OnStretchDough()
	{
		if(rollingPin.bStretchDough)
		{
			rollingPin.bStretchDough= false;
			normalisedTime +=Time.deltaTime/4f;
			animKneadDough.Play("stretchDough",-1,normalisedTime);
			if(normalisedTime >= 1)
			{
				StartCoroutine("CEndStretching");
				Debug.Log("CEndStretching");
			} 
		}
		 
	}

	IEnumerator CEndStretching()
	{
		DragItem.OneItemEnabledNo = 0;
		yield return new WaitForEndOfFrame();
		rollingPin.EndStretching();

		float pom = 1;
		while(pom>0)
		{
			pom-=Time.deltaTime*2;
			imgRollingPin.color = new Color(1,1,1,pom);
			yield return new WaitForEndOfFrame();
		}
		scrollMenu.gameObject.SetActive(true);
		scrollMenu.ShowMenu(0);
		if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound(SoundManager.Instance.ShowItemSound);
		ScrollMenuDragItem.bEnableDrag = true;
		yield return new WaitForSeconds(1);
		rollingPin.transform.parent.gameObject.SetActive(false);
		 
		if(phase == 1 ) Tutorial.Instance.ShowTutorial(2);
		//DragItem.OneItemEnabledNo = 1;
	}
 

 
	//-------------------------------------------------------------------------------------------------------------------

	public void ButtonNextClicked()
	{
        SDKManager.Instance.ShowAd(ShowAdType.ChaPing, 1, "做元宵包馅界面点下一步");
        //BlockClicks.Instance.SetBlockAll(true);
		if(SoundManager.Instance!=null) SoundManager.Instance.Play_ButtonClick();
		StartCoroutine ("LoadNextScene");

		//TODO:ADS  INTERSTITIAL_NEXT
		//AdsManager.Instance.ShowInterstitial(AdsManager.INTERSTITIAL_NEXT);
        GlobalVariables.ShowHomeNextInterstitial("next");
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
            SceneManager.LoadScene("SugarGlaze");
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
        RollingPin.OnStretchDough -=OnStretchDough;
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