using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CookDimSumScene : MonoBehaviour {

	public ScrollMenuGroup smgSauces;
	public ScrollMenu scrollMenu;
	public Transform scrollMenuContent;



	public Animator animDeepFryer;
	public Animator animButtonNext;
	public ItemsColors dimSumColors; //

 
	public Image[] dimSumPieces; 
	public ItemAction ButtonDeepFryer; //dugme koje pokrece pravljenje testa

	public Transform BambooTopCover; 
	public Transform BambooCoverEndPos;

	public ParticleSystem psSmoke;

	int phase = 0;
	public ParticleSystem psLevelCompleted;

	int selectedSauce = -1;

 

	IEnumerator Start () {

		//GameData.dimSumFlavors  = new int[] {-1,1,0,3};
		if(GameData.dimSumFlavors[0]>-1) dimSumPieces[0].color = dimSumColors.colors[GameData.dimSumFlavors[0]];
		if(GameData.dimSumFlavors[1]>-1) dimSumPieces[1].color = dimSumColors.colors[GameData.dimSumFlavors[1]];
		if(GameData.dimSumFlavors[2]>-1) dimSumPieces[2].color = dimSumColors.colors[GameData.dimSumFlavors[2]];
		if(GameData.dimSumFlavors[3]>-1) dimSumPieces[3].color = dimSumColors.colors[GameData.dimSumFlavors[3]];

		DragItem.OneItemEnabledNo = 0;
		scrollMenu.HideMenu();
		psSmoke.gameObject.SetActive(false);
		//BlockClicks.Instance.SetBlockAll(true);
		animButtonNext.gameObject.SetActive(false);
		yield return new WaitForSeconds(.1f);
	 

		yield return new WaitForSeconds(.5f);
		//LevelTransition.Instance.ShowScene();

		yield return new WaitForSeconds(.5f);
		//BlockClicks.Instance.SetBlockAll(false);

		DragItem.OneItemEnabledNo = 1;

		ScrollMenuDragItem.bEnableDrag = false;
		Tutorial.Instance.ShowTutorial(0);
		EscapeButtonManager.AddEscapeButonFunction("ButtonHomeClicked" );
	}


	 
	public void NextPhase(string _phase) 
	{
		if(_phase == "BSTop")
		{
			ButtonDeepFryer.bEnabled = true;
			Tutorial.Instance.ShowTutorial(1);
		}
		if(_phase == "MachineOn")
		{
			Tutorial.Instance.StopTutorial();
			animDeepFryer.enabled = true;
			animDeepFryer.Play("startCooking");
			phase = 1;
			if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound( SoundManager.Instance.FryingSound);
		}
		else	if(_phase.StartsWith("cookingEnd"))
		{
			if(SoundManager.Instance!=null) SoundManager.Instance.Stop_Sound( SoundManager.Instance.FryingSound);
			StartCoroutine("CNextPhase");
		}
		else	if(_phase.StartsWith("ds"))
		{
			Tutorial.Instance.StopTutorial();
			selectedSauce = int.Parse(_phase.Remove(0,2)) -1;
			Debug.Log(selectedSauce);
			psLevelCompleted.gameObject.SetActive(true);
			psLevelCompleted.Play();
			if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound(SoundManager.Instance.ActionCompleted);	
			animButtonNext.gameObject.SetActive(true);
			GameData.selectedColor = selectedSauce;
		}
	}



 

	IEnumerator CNextPhase()
	{
		Tutorial.Instance.StopTutorial();
		yield return new WaitForEndOfFrame();
		if(phase == 1) //kuvanje dim-suma na pari zavrseno
		{
			 animDeepFryer.enabled = false;
			 
			Tutorial.Instance.ShowTutorial(2);
			scrollMenu.ShowMenu(0);
			if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound( SoundManager.Instance.ShowItemSound);
			ScrollMenuDragItem.bEnableDrag = true;
		}
		 
	}

	//==================================================















	public void ButtonNextClicked()
	{
        if (SoundManager.Instance!=null) SoundManager.Instance.Stop_Sound( SoundManager.Instance.FryingSound);
		//BlockClicks.Instance.SetBlockAll(true);
		if(SoundManager.Instance!=null) SoundManager.Instance.Play_ButtonClick();
		StartCoroutine ("LoadNextScene");

		//TODO:ADS  INTERSTITIAL_NEXT
		//AdsManager.Instance.ShowInterstitial(AdsManager.INTERSTITIAL_NEXT);
        GlobalVariables.ShowHomeNextInterstitial("next");
	}


    //-------------------------------------------------------------------------------------------------------------------
    public GameObject LoadPanel;
    IEnumerator LoadNextScene()
    {
        Debug.Log("Load Next");
        yield return new WaitForEndOfFrame();
        if (LoadPanel != null /*/*&& SDKManager.Instance.IsCanShowAd*/)
        {
            LoadPanel.SetActive(true);
        }
        else
        {
            SceneManager.LoadScene("EatDimSum");
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
        if (SoundManager.Instance!=null) SoundManager.Instance.Stop_Sound( SoundManager.Instance.FryingSound);
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

	}



}