using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MakeFortuneCookies : MonoBehaviour {

	public ItemsColors itemsColors;
	public FortuneCookieDrag[] fortuneCookies;
	 
	int fortuneCookiesDone = 0;

	public Animator animButtonNext;
	public ParticleSystem psLevelCompleted;


	IEnumerator Start () {
		FortuneCookieDrag.bShowTut = true;
		DragItem.OneItemEnabledNo = 0;
		FortuneCookieDrag.bEnabled = false;
		 
		//GameData.selectedFlavor = 1;

		//PODESAVANJE BOJE ZA FILL


		for(int i = 0; i<fortuneCookies.Length; i++)
		{
			fortuneCookies[i].SetColor( itemsColors.colors[(GameData.selectedFlavor>-1)? GameData.selectedFlavor : 0]);
		}


		animButtonNext.gameObject.SetActive(false);
		 

		yield return new WaitForSeconds(.1f);
		//LevelTransition.Instance.ShowScene();

		yield return new WaitForSeconds(.5f);
		//BlockClicks.Instance.SetBlockAll(false);

		Tutorial.Instance.ShowTutorial(0);
		EscapeButtonManager.AddEscapeButonFunction("ButtonHomeClicked" );


		//if(SoundManager.Instance!=null) SoundManager.Instance.listStopSoundOnExit.Add( SoundManager.Instance.FreezerOnSound);
		FortuneCookieDrag.bEnabled =true;
	 
	}




	public void FinishMakingFortuneCookie()
	{
		fortuneCookiesDone++;
		Tutorial.Instance.StopTutorial();
		if(fortuneCookiesDone == 4) 
		{
			psLevelCompleted.gameObject.SetActive(true);
			psLevelCompleted.Play();
			if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound(SoundManager.Instance.ActionCompleted);	
			animButtonNext.gameObject.SetActive(true);
		}
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
            SceneManager.LoadScene("MeltChocolate");
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
		//TODO:ADS -  INTERSTITIAL_HOME
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


	public void ButtonNextClicked()
	{
         SDKManager.Instance.ShowAd(ShowAdType.ChaPing, 1, "做幸运饼干加幸运条界面返回首页点下一步");
        //BlockClicks.Instance.SetBlockAll(true);
		StartCoroutine("LoadNextScene");
		if(SoundManager.Instance!=null) SoundManager.Instance.Play_ButtonClick();
		//TODO:ADS - INTERSTITIAL_NEXT
		//AdsManager.Instance.ShowInterstitial(AdsManager.INTERSTITIAL_NEXT);
        GlobalVariables.ShowHomeNextInterstitial("next");
	}
}
