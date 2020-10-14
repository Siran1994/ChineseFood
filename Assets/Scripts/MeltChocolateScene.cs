using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MeltChocolateScene : MonoBehaviour {

	public ScrollMenuGroup smgChocolate;
	public ItemsColors itemsColors;
	public ScrollMenu scrollMenu; 
	public Transform scrollMenuContent;

 
	public ItemAction ButtonStove;
	public ProgressBar progressBar;

	int phase = 0;
	public ParticleSystem psLevelCompleted;
	int mixingPhase = -1;
	float mixingTime;

	public Transform bowl;
	public Transform BowlStartPos;
	public Transform BowlEndPos;
	public GameObject BowlCollider;
	 
	public ParticleSystem chocolateBoilParticles;
	public Image imgMeltedChocolate;
	public Transform MeltedChocolateEndPos;

	public Transform Dish;

	public Animator animButtonNext;
	 
	int chocolateFillColor = 0;


	public Transform Tray;
	public Transform TrayStartPos;
	public Transform TrayEndPos;

	public DragItem[] FortuneCookies;

	int cookiesDone = 0;


	IEnumerator Start () {
		DragItem.OneItemEnabledNo = 0;
		ButtonStove.enabled = false;
		bowl.GetComponent<ItemAction>().enabled = false;
		BowlCollider.SetActive(false);
		Tray.gameObject.SetActive(false);
		int selectedCol = (GameData.selectedColor>-1)? GameData.selectedColor : 1;

		 
		progressBar.gameObject.SetActive(false);
	 
		//BlockClicks.Instance.SetBlockAll(true);
		animButtonNext.gameObject.SetActive(false);
		yield return new WaitForSeconds(.1f);
		scrollMenu.ShowMenu(0);
		yield return new WaitForSeconds(.1f);
		//LevelTransition.Instance.ShowScene();

		yield return new WaitForSeconds(.3f);
		//BlockClicks.Instance.SetBlockAll(false);
		bowl.gameObject.SetActive(false);
		 
		Tutorial.Instance.ShowTutorial(0);
		EscapeButtonManager.AddEscapeButonFunction("ButtonHomeClicked" );
 
	}

	public void	ScrollMenuButtonClicked(int itemIndex)
	{
		if(phase ==  0 ) //biranje sastojaka koje se dodaju
		{
			Debug.Log("itemIndex:: " + itemIndex);
			phase =1;
			Debug.Log(itemIndex);
			GameData.chocolateFillColor = itemIndex;
			StartCoroutine("CNextPhase");
		}
	}


	public void NextPhase(string _phase) 
	{
		if( _phase == "ChocolateBowl"  )
		{
			phase = 3;
			StartCoroutine("CNextPhase");

		}
		else if( _phase == "StoveOn"  )
		{
			phase = 4;
			StartCoroutine("CNextPhase");
		}
		else if( _phase == "FCChocolateDone"  )
		{
			cookiesDone++;
			Tutorial.Instance.StopTutorial();

			if(cookiesDone == 4) 
			{
				Debug.Log("KRAJ");
				psLevelCompleted.gameObject.SetActive(true);
				psLevelCompleted.Play();
				animButtonNext.gameObject.SetActive(true);
				if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound(SoundManager.Instance.ActionCompleted);	
			}
		}

	}


	IEnumerator CNextPhase()
	{
		Tutorial.Instance.StopTutorial();

		if(phase == 1) 
		{
			phase = 2;
			Tutorial.Instance.StopTutorial();
			 
			scrollMenu.HideMenu();
			yield return new WaitForSeconds(.2f);

			bowl.gameObject.SetActive(true);
			foreach( Transform t in bowl.GetChild(0))
			{
				if(t.CompareTag("ScrollMenuColor") )t.GetComponent<Image>().color = smgChocolate.itemColors.colors[GameData.chocolateFillColor];
			}

			float pom = 0;
			Vector3 arcMax = new Vector3(0,.7f,0); 
			while(pom<1)
			{
				pom+=Time.fixedDeltaTime;
				bowl.transform.position = Vector3.Lerp(BowlStartPos.position, BowlEndPos.position, pom)  + pom* (1-pom) *arcMax;
				yield return new WaitForFixedUpdate();
			}
			yield return new WaitForFixedUpdate();
			bowl.GetComponent<ItemAction>().enabled = true;
			BowlCollider.SetActive(true);
			Tutorial.Instance.ShowTutorial(1);
		}
		else if(phase == 3) 
		{
			if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound( SoundManager.Instance.InsertFruit);
			yield return new WaitForSeconds(0.5f);
			for( int i = bowl.GetChild(0).childCount-1; i>=0 ; i--)
			{
				Transform t = bowl.GetChild(0).GetChild(i);
				if(t.CompareTag("ScrollMenuColor") ) t.SetParent(Dish.GetChild(4));
			}
			yield return new WaitForSeconds(4.0f);



			float pom = 0;
			Vector3 arcMax = new Vector3(0,.7f,0); 
			while(pom<1)
			{
				pom+=Time.fixedDeltaTime;
				bowl.transform.position = Vector3.Lerp(BowlEndPos.position, BowlStartPos.position, pom)  + pom* (1-pom) *arcMax;
				yield return new WaitForFixedUpdate();
			}

			ButtonStove.enabled = true;
			ButtonStove.bEnabled = true;
			bowl.gameObject.SetActive(false);
			Tutorial.Instance.ShowTutorial(2);
		}
		else if(phase == 4) 
		{
			ButtonStove.transform.GetChild(0).gameObject.SetActive(false);
			ButtonStove.transform.GetChild(1).gameObject.SetActive(true);
 
			float pom = 0;
			progressBar.SetProgress(0  ,false );
			progressBar.gameObject.SetActive(true);
			yield return new WaitForSeconds(1f);
			if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound( SoundManager.Instance.FryingSound);
			CanvasGroup cg = Dish.GetChild(4).GetComponent<CanvasGroup>();
			Color c = smgChocolate.itemColors.colors[GameData.chocolateFillColor];

			Vector3 MeltedChocolateStartPos = imgMeltedChocolate.transform.position;
			while(pom<.5f)
			{
				progressBar.SetProgress(pom  ,false );
				pom+=.1f*Time.fixedDeltaTime;
				yield return new WaitForFixedUpdate();
				cg.alpha = 1- 2*pom;
				imgMeltedChocolate.color = new Color(c.r,c.g,c.b, pom*2);
				imgMeltedChocolate.transform.position= Vector3.Lerp(MeltedChocolateStartPos, MeltedChocolateEndPos.position, pom*2);
			}
			chocolateBoilParticles.gameObject.SetActive(true);
			chocolateBoilParticles .Play();

			cg.gameObject.SetActive(false);
			imgMeltedChocolate.color = c;
			while(pom<1f)
			{
				progressBar.SetProgress(pom  ,false );
				pom+=0.1f*Time.fixedDeltaTime;
				yield return new WaitForFixedUpdate();
			}
 
			progressBar.SetProgress(1  ,false );

			ButtonStove.transform.GetChild(0).gameObject.SetActive(true);
			ButtonStove.transform.GetChild(1).gameObject.SetActive(false);
			if(SoundManager.Instance!=null) SoundManager.Instance.Stop_Sound( SoundManager.Instance.FryingSound);
			yield return new WaitForSeconds(.1f);
			progressBar.gameObject.SetActive(false);
			chocolateBoilParticles .Stop();
		 
			yield return new WaitForSeconds(1f);
			chocolateBoilParticles.gameObject.SetActive(false);
			Tray.gameObject.SetActive(true);
		 
			for (int i = 0; i < FortuneCookies.Length; i++) 
			{
				FortuneCookies[i].transform.GetChild(0).GetComponent<Image>().color = itemsColors.colors[(GameData.selectedFlavor>-1)? GameData.selectedFlavor : 0];
				FortuneCookies[i].transform.GetChild(0).GetChild(0).GetComponent<Image>().color =  smgChocolate.itemColors.colors[GameData.chocolateFillColor];
			}


			pom = 0;
			Vector3 arcMax = new Vector3(0,.7f,0); 
			while(pom<1)
			{
				pom+=Time.fixedDeltaTime;
				Tray.transform.position = Vector3.Lerp(TrayStartPos.position, TrayEndPos.position, pom)  + pom* (1-pom) *arcMax;
				yield return new WaitForFixedUpdate();
			}

			 
			phase = 5;

			DragItem.OneItemEnabledNo = 1;
			Tutorial.Instance.ShowTutorial(3);
		}
	}

	//-------------------------------------------------------------------------------------------------------------------

	public void ButtonNextClicked()
	{
        SDKManager.Instance.ShowAd(ShowAdType.ChaPing, 1, "煮巧克力界面返回首页点下一步");
        if (SoundManager.Instance!=null) SoundManager.Instance.Stop_Sound( SoundManager.Instance.FryingSound);
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
            SceneManager.LoadScene("DecorateFortuneCookie");
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
        if (SoundManager.Instance!=null) SoundManager.Instance.Stop_Sound( SoundManager.Instance.FryingSound);
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
