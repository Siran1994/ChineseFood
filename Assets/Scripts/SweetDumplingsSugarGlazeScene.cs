using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SweetDumplingsSugarGlazeScene : MonoBehaviour {

	public Image[] sweetDumplingsImgs;
	public ItemsColors sweetsColors;

	public Animator animButtonNext;
	public ParticleSystem psLevelCompleted;

	int sweetDumplingsDone = 0;

	IEnumerator Start () {
		SweetDumplingsSugar.bFirstTut = true;
		//BlockClicks.Instance.SetBlockAll(true);
		 
		// GameData.selectedFlavor = 0;
		try{
			//PODESAVANJE BOJE ZA FILL
			for(int i = 0; i<sweetDumplingsImgs.Length; i++)
			{
			 	 sweetDumplingsImgs[i].color  =  sweetsColors.colors[GameData.selectedFlavor];
			}
		}catch{}

		animButtonNext.gameObject.SetActive(false);
		DragItem.OneItemEnabledNo = 1;

		yield return new WaitForSeconds(1f);
		//LevelTransition.Instance.ShowScene();

		yield return new WaitForSeconds(1f);
		//BlockClicks.Instance.SetBlockAll(false);

		Tutorial.Instance.ShowTutorial(0);
		EscapeButtonManager.AddEscapeButonFunction("ButtonHomeClicked" );

		//scrollMenu.ChangeMenu(0);
		//scrollMenu.scrollRect.horizontalNormalizedPosition = 0;


	}



	public void FinishAddingSugar()
	{
		sweetDumplingsDone++;
		if(sweetDumplingsDone == 4) 
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
            SceneManager.LoadScene("CookSweetDumplings");
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
		//TODO:ADS INTERSTITIAL_HOME
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
        SDKManager.Instance.ShowAd(ShowAdType.ChaPing, 1, "元宵蘸糖界面点下一步");
        //BlockClicks.Instance.SetBlockAll(true);
		StartCoroutine("LoadNextScene");
		if(SoundManager.Instance!=null) SoundManager.Instance.Play_ButtonClick();
		//TODO:ADS  INTERSTITIAL_NEXT
        //AdsManager.Instance.ShowInterstitial(AdsManager.INTERSTITIAL_NEXT);
        GlobalVariables.ShowHomeNextInterstitial("next");
	}
}
