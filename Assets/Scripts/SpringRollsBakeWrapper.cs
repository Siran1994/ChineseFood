using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SpringRollsBakeWrapper : MonoBehaviour {

	public Animator animButtonNext;
	public Animator animBG;
	public Animator animTutorialRotation;
	 
	public Image imgDough;
	public Image imgWrapperRaw;
	public Image imgWrapperBaked;
	public Transform StoveEndPoint;
	public Transform Ladle;
	public Transform LadleEndPosition;
	 
	public ParticleSystem psLevelCompleted;
	int phase =0;

	public Transform spatula;

	 
	IEnumerator Start () 
	{
		animTutorialRotation.gameObject.SetActive(false);
		spatula.GetComponent<Spatula>().Init();
		spatula.gameObject.SetActive(false);
		animButtonNext.gameObject.SetActive(false);
		imgDough.gameObject.SetActive(false);

		//BlockClicks.Instance.SetBlockAll(true);
		//BlockClicks.Instance.SetBlockAllDelay(.5f,false);
 
		DragItem.OneItemEnabledNo = 0;
		yield return new WaitForSeconds(1);
		DragItem.OneItemEnabledNo = 1; 
 		
 
		//LevelTransition.Instance.ShowScene();
		EscapeButtonManager.AddEscapeButonFunction("ButtonBackClicked" );
		yield return new WaitForSeconds(1);
		 
		//scrollMenu.ShowMenu(0);

		Vector3 ladleStartPos = Ladle.position;
		Vector3 arcMax = new Vector3(0,2,0); 
		float pom = 0;
		while(pom<1)
		{
			pom+=Time.deltaTime*.8f;
			Ladle.position =  Vector3.Lerp(ladleStartPos,  LadleEndPosition.position, pom) + pom* (1-pom) *arcMax; 
			yield return new WaitForEndOfFrame();
		}
		Ladle.transform.position = LadleEndPosition.position;
		Tutorial.Instance.ShowTutorial(0);
	}


	public void NextPhase(string gameStatePhase)
	{
		if(gameStatePhase == "Ladle")
		{
			Tutorial.Instance.StopTutorial();
			DragItem di =	Ladle.GetComponent<DragItem>();
			di.TargetPoint = new Transform[1] { StoveEndPoint };
			//di.TestPoint = Ladle;

		}
		else if(gameStatePhase == "Ladle2")
		{
			phase= 1;
			StartCoroutine( "CNextPhase" );
		}
		else if(gameStatePhase == "Spatula")
		{
			phase= 2;
			StartCoroutine( "CNextPhase" );
		}

	}

	IEnumerator CNextPhase()
	{
		if(phase ==1) 
		{
			if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound( SoundManager.Instance.InsertFruit);
			yield return new WaitForSeconds(.8f);
			float pom = 0;

			imgDough.gameObject.SetActive(true);
			Color c = new Color(1,1,1,0);
			imgDough.color = c;
			while(pom<1)
			{
				yield return new WaitForEndOfFrame();
				pom +=Time.deltaTime*3;
				imgDough.color = Color.Lerp(c, Color.white,pom);
			}
			imgDough.color = Color.white;

			yield return new WaitForSeconds(1.8f);

			Vector3 ladleStartPos = Ladle.position;
			Vector3 ladleEndPos = Ladle.position + new Vector3(6,0,0);
			Vector3 arcMax = new Vector3(0,2,0); 
			pom = 0;

			while(pom<1)
			{
				pom+=Time.deltaTime;
				Ladle.position =  Vector3.Lerp(ladleStartPos, ladleEndPos, pom) + pom* (1-pom) *arcMax; 
				yield return new WaitForEndOfFrame();
			}
			Ladle.transform.position = ladleEndPos;
			Ladle.gameObject.SetActive(false);
			//ANIMACIJA POZADINE

			animBG.Play( "ChangeAngle");
			if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound( SoundManager.Instance.ShowItemSound);
			yield return new WaitForSeconds(2f);
			spatula.gameObject.SetActive(true);

			Vector3 spatulaStartPos = spatula.position + new Vector3(6,0,0);
			Vector3 spatulaEndPos = spatula.position;
		 
			pom = 0;
			spatula.position = spatulaStartPos;
			while(pom<1)
			{
				pom+=Time.deltaTime;
				spatula.position =  Vector3.Lerp(spatulaStartPos, spatulaEndPos, pom) + pom* (1-pom) *arcMax; 
				yield return new WaitForEndOfFrame();
			}
			spatula.position = spatulaEndPos;
			spatula.GetComponent<Spatula>().Init();

			phase = 2;
			animTutorialRotation.gameObject.SetActive(true);
			animTutorialRotation.Play("ShowCircle");
		}
		else if(phase == 2) 
		{
			animTutorialRotation.Play("HideCircle");

			yield return new WaitForSeconds(1f);
			Vector3 spatulaStartPos = spatula.position;
			Vector3 spatulaEndPos = spatula.position+ new Vector3(8,0,0);
			animTutorialRotation.gameObject.SetActive(false);
			float pom = 0;
			spatula.position = spatulaStartPos;
			Vector3 arcMax = new Vector3(0,2,0); 
			while(pom<1)
			{
				pom+=Time.deltaTime;
				spatula.position =  Vector3.Lerp(spatulaStartPos, spatulaEndPos, pom) + pom* (1-pom) *arcMax; 
				yield return new WaitForEndOfFrame();
			}
			spatula.position = spatulaEndPos;

			yield return new WaitForSeconds(1f);
	 
		 	pom = 0;
			imgWrapperBaked.gameObject.SetActive(true);
			Color c = new Color(1,1,1,0);
			imgWrapperBaked.color = c;
			while(pom<1)
			{
				yield return new WaitForEndOfFrame();
				pom +=Time.deltaTime*.5f;
				imgWrapperBaked.color = Color.Lerp(c, Color.white,pom);
			}
			imgWrapperBaked.color = Color.white;
			pom = 0;
			while(pom<1)
			{
				yield return new WaitForEndOfFrame();
				pom +=Time.deltaTime*4;
				imgWrapperRaw.color = Color.Lerp(c, Color.white,1-pom);
			}
		
			imgWrapperRaw.gameObject.SetActive(false);
			animButtonNext.gameObject.SetActive(true);
			psLevelCompleted.gameObject.SetActive(true);
			psLevelCompleted.Play();
			if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound(SoundManager.Instance.ActionCompleted);	

			Debug.Log("KRAJ");
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
            SceneManager.LoadScene("MakeSpringRolls");
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


	public void ButtonNextClicked()
	{
        SDKManager.Instance.ShowAd(ShowAdType.ChaPing, 1, "烫春卷界面返回首页点下一步");
        //BlockClicks.Instance.SetBlockAll(true);
		StartCoroutine("LoadNextScene");
		if(SoundManager.Instance!=null) SoundManager.Instance.Play_ButtonClick(); 
		//TODO:ADS  INTERSTITIAL_NEXT
		//AdsManager.Instance.ShowInterstitial(AdsManager.INTERSTITIAL_NEXT);
        GlobalVariables.ShowHomeNextInterstitial("next");
	}
}
