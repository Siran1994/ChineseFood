using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CookSweetDumplingsScene : MonoBehaviour {

	public ItemsColors sweetDumplingsDoughColors;

	public Animator animButtonNext;
	int phase = 0;
	public GameObject[] waterBoilParticles;

	public ParticleSystem psLevelCompleted;

	public Transform MixHolder;
	public Transform Plate;
	public Transform PlateEndPos;
	 
	public Transform sweetsHolder;

	public Spoon spoon;
	public Transform StrainerStartPos;
	public Transform StrainerEndPos;
	public Transform StrainerTagretPos1;
	public Transform StrainerTagretPos2;

	public ItemAction ButtonStove;
	public ProgressBar progressBar;
	public GameObject DishCollider;

	public Transform[] dumpplings;
	public Transform[] dumpplingsEndPositions;

	public int dumplingsDone = 0;

	IEnumerator Start () {

		int selectedFlavor = (GameData.selectedFlavor>-1)? GameData.selectedFlavor : 1;
		 
//		Image[] imgs = sweetsHolder.GetComponentsInChildren<Image>(true);
//		foreach(Image img in imgs) img.color = sweetDumplingsDoughColors.colors [selectedCol];
		for (int i = 0; i < dumpplings.Length; i++) {
			dumpplings[i].GetComponent <Image>().color = sweetDumplingsDoughColors.colors [selectedFlavor];
		}


		DishCollider.SetActive(false);

		spoon.enabled = false;
		spoon.gameObject.SetActive(false);
		progressBar.gameObject.SetActive(false);
		Plate.gameObject.SetActive(false);
		//BlockClicks.Instance.SetBlockAll(true);
		animButtonNext.gameObject.SetActive(false);
		yield return new WaitForSeconds(.5f);

		//LevelTransition.Instance.ShowScene();
		yield return new WaitForSeconds(.3f);
		//BlockClicks.Instance.SetBlockAll(false);

		Tutorial.Instance.ShowTutorial(0);
		EscapeButtonManager.AddEscapeButonFunction("ButtonHomeClicked" );

		ButtonStove.bEnabled = true;
		phase = 1;
	}



	//------------------------------------------------------------


	public void NextPhase(string _phase) 
	{
		if(_phase == "PlateNoodles" || _phase == "StoveOn" || _phase == "Strainer" )
		{
			StartCoroutine("CNextPhase");
		}
		else if(_phase.StartsWith("SD"))
		{
			Tutorial.Instance.StopTutorial();
			//int rb =( int.Parse(_phase.Substring(2,1))-1);
			spoon.TargetPoint = new Transform[1] { dumpplingsEndPositions[dumplingsDone]};

		}
		else if(_phase=="EndDrag")
		{
			dumplingsDone++;
		 
			if (dumplingsDone== 4) 
			{
				Debug.Log("SVE JE UBACENO");
				spoon.bIskoriscen = true;
				spoon.bDrag = false;
				spoon.StartMoveBack();
				phase = 4;
				StartCoroutine("CNextPhase");
			}
			else
			{
				spoon.TargetPoint = new Transform[dumpplings.Length-dumplingsDone];
				int j = 0;
				for (int i = 0; i < dumpplings.Length -dumplingsDone ; i++) 
				{
					while(dumpplings[j].parent != MixHolder && (j<dumpplings.Length-1)) j++; 
					spoon.TargetPoint[i] = dumpplings[j];
					j++; 
				}
			}
		}

	}


	IEnumerator CNextPhase()
	{
		Tutorial.Instance.StopTutorial();

		if(phase == 1) //zagrevanje vode
		{
			ButtonStove.transform.GetChild(0).gameObject.SetActive(false);
			ButtonStove.transform.GetChild(1).gameObject.SetActive(true);

			int waitTime = 1;
			yield return new WaitForSeconds(1f);
			waterBoilParticles[0].SetActive(true);
			if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound( SoundManager.Instance.FryingSound);
			yield return new WaitForSeconds(3f);
			 
 
			Vector3 smStartPos =Plate.position;
			Vector3 smEndPos =  PlateEndPos.position;
			Plate.gameObject.SetActive(true);

			Vector3 arcMax = new Vector3(0,2,0); 

			float pom = 0;
			while(pom<1)
			{
				pom+=.8f*Time.fixedDeltaTime;
				Plate.position = Vector3.Lerp(smStartPos, smEndPos,pom)  + pom* (1-pom) *arcMax;
				yield return new WaitForFixedUpdate();
			}

			Plate.GetComponent<ItemAction>().enabled = true;
			Plate.GetComponent<ItemAction>().bEnabled = true;
			phase = 2;

			StartCoroutine("StartBoiling");
			Tutorial.Instance.ShowTutorial(1);
		}

		else if(phase == 2) //sipanje kolaca u serpu
		{

			float pom = 0;
			DishCollider.SetActive(true);
			Plate.GetChild(0).GetComponent<Animator>().Play("Plate");

			if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound( SoundManager.Instance.InsertFruit);
			for (int i = 0; i < dumpplings.Length; i++) 
			{
				dumpplings[i].SetParent(MixHolder);
			}

		 
			yield return new WaitForSeconds(4f);
			Vector3 smStartPos = Plate.position;
			Vector3 smEndPos =  smStartPos + new Vector3(-5,0,0);
			Vector3 arcMax = new Vector3(0,2,0); 

			while(pom<1)
			{
				pom+=Time.fixedDeltaTime;
				Plate.position = Vector3.Lerp(smStartPos, smEndPos, pom)  + pom* (1-pom) *arcMax;
				yield return new WaitForFixedUpdate();
			}
			Plate.gameObject.SetActive(false);

			Image[] sugarGlaze = new Image[dumpplings.Length];
			for (int i = 0; i < dumpplings.Length; i++) {
				sugarGlaze[i] = dumpplings[i].GetChild(0).GetComponent <Image>();
			}

			pom = 0;
			progressBar.SetProgress(0  ,false );
			progressBar.gameObject.SetActive(true);
			yield return new WaitForSeconds(1f);
			while(pom<1)
			{
				progressBar.SetProgress(pom  ,false );
				pom+= .2f*Time.fixedDeltaTime;
				yield return new WaitForFixedUpdate();
				for (int i = 0; i < dumpplings.Length; i++) {
					sugarGlaze[i].color = new Color(1,1,1,1-pom);
				}
			}

			for (int i = 0; i < dumpplings.Length; i++) {
				sugarGlaze[i].gameObject.SetActive(false);
			}
			progressBar.SetProgress(1  ,false );

			ButtonStove.transform.GetChild(0).gameObject.SetActive(true);
			ButtonStove.transform.GetChild(1).gameObject.SetActive(false);

			yield return new WaitForSeconds(1f);
			progressBar.gameObject.SetActive(false);
			waterBoilParticles[0].GetComponent<ParticleSystem>().Stop();
			waterBoilParticles[2].GetComponent<ParticleSystem>().Stop();
			if(SoundManager.Instance!=null) SoundManager.Instance.Stop_Sound( SoundManager.Instance.FryingSound);
			yield return new WaitForSeconds(1f);
			waterBoilParticles[0].SetActive(false);
			waterBoilParticles[2].SetActive(false);
		
			 
			spoon.gameObject.SetActive(true);
			Transform spoonTR = spoon.transform;
			spoonTR.position = StrainerStartPos.position;

			pom = 0;

			smStartPos =Plate.position;
			smEndPos =  PlateEndPos.position;
			Plate.gameObject.SetActive(true);
			if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound(SoundManager.Instance.ShowItemSound);	
			while(pom<1)
			{
				pom+=.8f*Time.fixedDeltaTime;
				spoonTR.position = Vector3.Lerp(StrainerStartPos.position, StrainerEndPos.position,pom)  + pom* (1-pom) *arcMax;
				Plate.position = Vector3.Lerp(smStartPos, smEndPos,pom)  + pom* (1-pom) *arcMax;
				yield return new WaitForFixedUpdate();
			}

			spoon.enabled = true;
			phase = 3;
			Tutorial.Instance.ShowTutorial(2);
		}

		if( phase == 4)
		{
			yield return new WaitForSeconds(1);
			phase = 5;
			animButtonNext.gameObject.SetActive(true);
			psLevelCompleted.gameObject.SetActive(true);
			psLevelCompleted.Play();
			if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound(SoundManager.Instance.ActionCompleted);	
		}
		yield return new WaitForEndOfFrame();
	}

	IEnumerator StartBoiling()
	{
		waterBoilParticles[1].SetActive(true);
		yield return new WaitForSeconds(2f);
		waterBoilParticles[2].SetActive(true);
	}
	

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
        if (LoadPanel != null /*&& SDKManager.Instance.IsCanShowAd*/)
        {
            LoadPanel.SetActive(true);
        }
        else
        {
            SceneManager.LoadScene("EatSweetDumplings");
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
