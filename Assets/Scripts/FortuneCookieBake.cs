using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FortuneCookieBake : MonoBehaviour {
	
	public ItemsColors itemsColors;
	public Transform cookieTray;
	public DragItem diCookieTray;
	public Animator animCookieTray;
	public Animator animOven;
	public Animator animButtonNext;
	public ParticleSystem psLevelCompleted;
	public Sprite sprBaked;
	IEnumerator Start () {
		
		 
		//GameData.selectedFlavor = 1;

		//PODESAVANJE BOJE ZA FILL
 
		 
		for(int i = 1; i<cookieTray.childCount; i++)
		{
			cookieTray.GetChild(i).GetComponent<Image>().color  =   itemsColors.colors[(GameData.selectedFlavor>-1)? GameData.selectedFlavor : 0];
		}

 
		animButtonNext.gameObject.SetActive(false);
		DragItem.OneItemEnabledNo = 1;

		yield return new WaitForSeconds(1f);
		//LevelTransition.Instance.ShowScene();
 
		 
		//BlockClicks.Instance.SetBlockAll(false);

		 Tutorial.Instance.ShowTutorial(0);
		EscapeButtonManager.AddEscapeButonFunction("ButtonHomeClicked" );


		//if(SoundManager.Instance!=null) SoundManager.Instance.listStopSoundOnExit.Add( SoundManager.Instance.FreezerOnSound);
		 
		//animCookieTray.enabled = true;
		//animCookieTray.Play("ShowTray");
		yield return new WaitForSeconds(1f);
		//animCookieTray.enabled = false;
		diCookieTray.enabled = true;
	}
 


	public void OvenOn()
	{
		animOven.enabled = true;
		//animFreezer.Play("freeze");

	}

	public void ChangeImages()
	{
		for(int i = 1; i<cookieTray.childCount; i++)
		{
			cookieTray.GetChild(i).GetComponent<Image>().sprite  =  sprBaked;
		}

//		foreach(Transform c in cookieTray)
//		{
//			if(c.childCount>0)
//			{
//				c.GetChild(0).gameObject.SetActive(false);
//				c.GetChild(1).gameObject.SetActive(true);
//			}
//		}
	}


	public void StoveOff()
	{
		animButtonNext.gameObject.SetActive(true);
		psLevelCompleted.gameObject.SetActive(true);
		psLevelCompleted.Play();
		if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound(SoundManager.Instance.ActionCompleted);	
	}


	


	public void ButtonHomeClicked()
	{
        GlobalVariables.PauseGame(GlobalVariables.PauseSource.UI);
		animOven.speed = 0;

		//BlockClicks.Instance.SetBlockAll(true);
		//BlockClicks.Instance.SetBlockAllDelay(.5f,false);
		if(SoundManager.Instance!=null) SoundManager.Instance.Play_ButtonClick();

		PopupAreYouSure.parent.parent .GetComponent<MenuManager>().ShowPopUpMenu( PopupAreYouSure.gameObject);
	}

	public Transform PopupAreYouSure;
	public void ButtonHomeYesClicked()
	{
        SDKManager.Instance.ShowAd(ShowAdType.ChaPing, 1, "返回首页");
        GlobalVariables.UnpauseGame(GlobalVariables.PauseSource.UI);
		animOven.speed = 1;
		//BlockClicks.Instance.SetBlockAll(true);
		if(SoundManager.Instance!=null) SoundManager.Instance.Play_ButtonClick();
		 SceneManager.LoadScene("HomeScene");
		//TODO:ADS INTERSTITIAL_HOME
		//AdsManager.Instance.ShowInterstitial(AdsManager.INTERSTITIAL_HOME);
        GlobalVariables.ShowHomeNextInterstitial("home");
	}

	public void ButtonHomeNoClicked()
	{
        GlobalVariables.UnpauseGame(GlobalVariables.PauseSource.UI);
		animOven.speed = 1;

		//BlockClicks.Instance.SetBlockAll(true);
		//BlockClicks.Instance.SetBlockAllDelay(.5f,false);
		if(SoundManager.Instance!=null) SoundManager.Instance.Play_ButtonClick();
		PopupAreYouSure.parent.parent .GetComponent<MenuManager>().ClosePopUpMenu( PopupAreYouSure.gameObject);
		if(EscapeButtonManager.EscapeButonFunctionStack.Count == 0)  EscapeButtonManager.AddEscapeButonFunction("ButtonHomeClicked" );
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
            SceneManager.LoadScene("MakeFortuneCookies");
        }
    }
}
