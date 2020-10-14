using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SweetDumplingsDoughScene : MonoBehaviour {

	public Animator animButtonNext;
	public ItemsColors sweetDumplingsDoughColors;

	int phase = 0;
	public ScrollMenu scrollMenu; 
	public Transform scrollMenuContent;

	public Image imgFlavorDough;
	public KneadDoughScript knd;

	float transition = 0;
	public  CanvasGroup DoughKnead;
	public  CanvasGroup DoughCut;
	public Vector3 DoughKneadEndScale;
 
	public CutScript cs;
	public ParticleSystem psLevelCompleted;


	IEnumerator Start ()
	{
		animButtonNext.gameObject.SetActive(false);
		cs.knife.gameObject.SetActive(false);
		DoughCut.gameObject.SetActive(false);
		knd.enabled = false;
		knd.bEnableDrag = false;
		imgFlavorDough.gameObject.SetActive(false);
		//BlockClicks.Instance.SetBlockAll(true);
		yield return new WaitForSeconds(.1f);
		//LevelTransition.Instance.ShowScene();

		yield return new WaitForSeconds(.1f);
		//BlockClicks.Instance.SetBlockAll(false);

		scrollMenu.gameObject.SetActive(true);
		scrollMenu.ShowMenu(0);

		ScrollMenuDragItem.bEnableDrag = true;
		//scrollMenu.gameObject.SetActive(false);
		Tutorial.Instance.ShowTutorial(0);
		 
	}
	
	public void NextPhase(string _phase) 
	{
		if( _phase.StartsWith("F") )
		{
			GameData.selectedFlavor = int.Parse(_phase.Substring(1,1)) -1;

			StartCoroutine("CNextPhase");
		}
		else if( _phase == "DoughReady") 
		{
			StartCoroutine("CNextPhase");
		}
		else if(_phase == "CutEnd")
		{
			psLevelCompleted.gameObject.SetActive(true);
			psLevelCompleted.Play();
			if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound(SoundManager.Instance.ActionCompleted);	 
			animButtonNext.gameObject.SetActive(true);
		}
	}


	IEnumerator CNextPhase()
	{
		Tutorial.Instance.StopTutorial();

		if(phase == 0)//sipanje arome
		{
			 
			yield return new WaitForSeconds(1);
			Color c =  sweetDumplingsDoughColors.colors[ GameData.selectedFlavor];
			knd.color = c;
			float pom = 0;

			imgFlavorDough.color = new Color(c.r,c.g,c.b,0);
			Vector3 startScale =.3f* Vector3.one;
			imgFlavorDough.gameObject.SetActive(true);
			while(pom<1)
			{
				pom+=Time.deltaTime*.45f;
				imgFlavorDough.color = new Color(c.r,c.g,c.b,pom*2);
				imgFlavorDough.transform.localScale = Vector3.Lerp(startScale, Vector3.one,pom);
				yield return new WaitForEndOfFrame();
			}
			imgFlavorDough.color =  new Color(c.r,c.g,c.b,1);
			yield return new WaitForSeconds(1);
			scrollMenu.HideMenu();
			yield return new WaitForSeconds(1);

			GameObject.Destroy(scrollMenu.gameObject);

			phase = 1;
			knd.enabled = true;
			knd.bEnableDrag = true;

			Tutorial.Instance.ShowTutorial(1);

		}
		else if(phase == 1) //seckanje testa
		{
			for (int i = 1; i <= 4; i++) 
			{
				//zamena boje
				 DoughCut.transform.Find("P"+i.ToString()).GetComponent<Image>().color = sweetDumplingsDoughColors.colors[ GameData.selectedFlavor];

				//DoughCut.transform.FindChild("P"+i.ToString()).GetComponent<Image>().material.SetColor("_Color",sweetDumplingsDoughColors.colors[ GameData.selectedFlavor]);
			}
			//prikazivanje  gotovog testa
			float pom = 0;
			float pom2 = 0;
			 
			while(pom<1)
			{
				DoughCut.gameObject.SetActive(true);
				DoughCut.alpha = 0;
				pom+=Time.deltaTime ;
				DoughKnead.transform.localScale = Vector3.Lerp(DoughKnead.transform.localScale,DoughKneadEndScale,pom);
				if(DoughCut.alpha <1) DoughCut.alpha = pom*1.5f;
				if(pom>0.5f)
				{
					pom2 += 2*Time.deltaTime;
					DoughKnead.alpha = 1-pom2;
				}
				yield return new WaitForEndOfFrame();
			}

			DoughKnead.gameObject.SetActive(false);
			DoughCut.alpha =1;
			yield return new WaitForEndOfFrame();


			cs.knife.gameObject.SetActive(true);
			cs.InitKnife();
			if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound(SoundManager.Instance.ShowItemSound);
			Tutorial.Instance.ShowTutorial(2);
		}

  
	}

	//-------------------------------------------------------------------------------------------------------------------

	public void ButtonNextClicked()
	{
        SDKManager.Instance.ShowAd(ShowAdType.ChaPing, 1, "元宵和面界面点下一步");
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
            SceneManager.LoadScene("MakeSweetDumplings");
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
