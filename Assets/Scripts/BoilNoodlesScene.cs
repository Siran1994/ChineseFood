using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BoilNoodlesScene : MonoBehaviour {

	public ItemsColors noodleColors;

	public Animator animButtonNext;
	int phase = 0;
	public GameObject[] waterBoilParticles;

	public ParticleSystem psLevelCompleted;
	 
	public Transform MixHolder;
	public Transform Plate;
	public Transform PlateEndPos;
	public Transform NoodlesEndPos;
	public Transform noodlesPlate;

	public Transform Strainer;
	public Transform StrainerStartPos;
	public Transform StrainerEndPos;
	public Transform StrainerTagretPos1;
	public Transform StrainerTagretPos2;


 

	public ItemAction ButtonStove;

	public ProgressBar progressBar;

	IEnumerator Start () {

		int selectedCol = (GameData.selectedColor>-1)? GameData.selectedColor : 1;
		noodlesPlate.GetComponent<Image>().color = noodleColors.colors[selectedCol];

		Strainer.gameObject.SetActive(false);
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
	}

 

	public void FillWater()
	{
		StartCoroutine("CNextPhase");
	}
 

	public void NextPhase(string _phase) 
	{
		if(_phase == "PlateNoodles" || _phase == "StoveOn" || _phase == "Strainer" )
		{
			StartCoroutine("CNextPhase");
		}
	}


	IEnumerator CNextPhase()
	{
		Tutorial.Instance.StopTutorial();

		if(phase == 0)//sipanje vode
		{
			Tutorial.Instance.StopTutorial();
			float pom = 0;

			Transform water = MixHolder.GetChild(0);
			water.gameObject.SetActive(true);
			Vector3 smStartPos = water.position;
			Vector3 smEndPos =  MixHolder.position;


			yield return new WaitForSeconds (1f);
			while(pom<1)
			{
				pom+=Time.deltaTime*.45f;
				water.position =  Vector3.Lerp(smStartPos, smEndPos,pom); 
				//while(GlobalVariables.bPauseGame)  yield return new WaitForEndOfFrame();
				yield return new WaitForEndOfFrame();
			}
			//if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound(SoundManager.Instance.ShowItemSound);	
			water.position =  MixHolder.position;
			ButtonStove.bEnabled = true;
			Tutorial.Instance.ShowTutorial(1);
			phase = 1;
		}
		else if(phase == 1) //zagrevanje vode
		{
			ButtonStove.transform.GetChild(0).gameObject.SetActive(false);
			ButtonStove.transform.GetChild(1).gameObject.SetActive(true);

			int waitTime = 1;
			yield return new WaitForSeconds(1f);
			waterBoilParticles[0].SetActive(true);
			if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound(SoundManager.Instance.FryingSound);	
			while(waitTime<waterBoilParticles.Length)
			{
				yield return new WaitForSeconds(3f);
				waterBoilParticles[waitTime].SetActive(true);
				waitTime++;

			}

			Vector3 smStartPos =Plate.position;
			Vector3 smEndPos =  PlateEndPos.position;
			Plate.gameObject.SetActive(true);
			if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound(SoundManager.Instance.ShowItemSound);	
			Vector3 arcMax = new Vector3(0,2,0); 

			float pom = 0;
			while(pom<1)
			{
				pom+=.8f*Time.fixedDeltaTime;
				Plate.position = Vector3.Lerp(smStartPos, smEndPos,pom)  + pom* (1-pom) *arcMax;
				yield return new WaitForFixedUpdate();
			}
			Plate.GetComponent<ItemAction>().bEnabled = true;
			phase = 2;
			Tutorial.Instance.ShowTutorial(2);
		}
			  
		else if(phase == 2) //sipanje testa u serpu
		{
			
			float pom = 0;
  
			Vector3 smStartPos = noodlesPlate.position;
			Vector3 smEndPos =  NoodlesEndPos.position;
			Vector3 arcMax = new Vector3(0,.7f,0); 
			MixHolder.parent.GetComponent<Mask>().enabled = false;

	 
			while(pom<1)
			{
				pom+=Time.fixedDeltaTime;
				Plate.rotation = Quaternion.Lerp(Quaternion.identity ,Quaternion.Euler(0,0,-40),pom);
				yield return new WaitForFixedUpdate();
			}
			while(pom<1)
			{
				pom+=.5f*Time.fixedDeltaTime;
				Quaternion startRotation = noodlesPlate.rotation;
				noodlesPlate.rotation = Quaternion.Lerp(startRotation ,Quaternion.identity, pom);
				noodlesPlate.position = Vector3.Lerp(smStartPos, smEndPos,pom)  + pom* (1-pom) *arcMax;
			}
			if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound( SoundManager.Instance.InsertFruit);
			pom = 0;
			while(pom<1)
			{
				noodlesPlate.SetParent(MixHolder);
				pom+=Time.fixedDeltaTime;
				noodlesPlate.rotation = Quaternion.Lerp(noodlesPlate.rotation ,Quaternion.identity, pom);
				noodlesPlate.position = Vector3.Lerp(smStartPos, smEndPos,pom)  + pom* (1-pom) *arcMax;
				yield return new WaitForFixedUpdate();
			}
			pom = 0;
		 
			while(pom<1)
			{
				pom+=Time.fixedDeltaTime;
				Plate.rotation = Quaternion.Lerp(Plate.rotation ,Quaternion.identity, pom);
				yield return new WaitForFixedUpdate();
			}

			pom = 0;
			smStartPos = Plate.position;
			smEndPos =  smStartPos + new Vector3(-5,0,0);
			arcMax = new Vector3(0,2,0); 
			//if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound(SoundManager.Instance.ShowItemSound);	
			while(pom<1)
			{
				pom+=Time.fixedDeltaTime;
				Plate.position = Vector3.Lerp(smStartPos, smEndPos,pom)  + pom* (1-pom) *arcMax;
				yield return new WaitForFixedUpdate();
			}
			Plate.gameObject.SetActive(false);

			pom = 0;
			progressBar.SetProgress(0  ,false );
			progressBar.gameObject.SetActive(true);
			yield return new WaitForSeconds(1f);
			while(pom<1)
			{
				progressBar.SetProgress(pom  ,false );
				pom+=.15f*Time.fixedDeltaTime;
				yield return new WaitForFixedUpdate();
			}
			progressBar.SetProgress(1  ,false );

			ButtonStove.transform.GetChild(0).gameObject.SetActive(true);
			ButtonStove.transform.GetChild(1).gameObject.SetActive(false);
			if(SoundManager.Instance!=null) SoundManager.Instance.Stop_Sound(SoundManager.Instance.FryingSound);	
			yield return new WaitForSeconds(1f);
			progressBar.gameObject.SetActive(false);
			waterBoilParticles[0].GetComponent<ParticleSystem>().Stop();
			waterBoilParticles[2].GetComponent<ParticleSystem>().Stop();
			yield return new WaitForSeconds(1f);
			waterBoilParticles[0].SetActive(false);
			waterBoilParticles[2].SetActive(false);
			 


			Strainer.gameObject.SetActive(true);
			Strainer.position = StrainerStartPos.position;

			pom = 0;
			 
			smStartPos =Plate.position;
			smEndPos =  PlateEndPos.position;
			Plate.gameObject.SetActive(true);
			if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound(SoundManager.Instance.ShowItemSound);	
			while(pom<1)
			{
				pom+=.8f*Time.fixedDeltaTime;
				Strainer.position = Vector3.Lerp(StrainerStartPos.position, StrainerEndPos.position,pom)  + pom* (1-pom) *arcMax;
				Plate.position = Vector3.Lerp(smStartPos, smEndPos,pom)  + pom* (1-pom) *arcMax;
				yield return new WaitForFixedUpdate();
			}

			DragItem di = Strainer.GetComponent<DragItem>();
			//di.enabled = true;
		 
			phase = 3;

			Tutorial.Instance.ShowTutorial(3);
		}

		else if(phase == 3) //cediljkom su zahvacene testenine
		{

			float pom = 0;
			noodlesPlate.SetParent(Strainer.GetChild(0).GetChild(0));

			Vector3 smStartPos = noodlesPlate.localPosition;
			Vector3 smEndPos =  Vector3.zero;
 
			while(pom<1)
			{
				pom+=3*Time.fixedDeltaTime;
				noodlesPlate.localPosition = Vector3.Lerp(smStartPos, smEndPos, pom);
				yield return new WaitForFixedUpdate();
			}

			DragItem di = Strainer.GetComponent<DragItem>();
			di.TargetPoint = new Transform[] {StrainerTagretPos2};
			di.bIskoriscen = false;
			di.bDrag = true;
			di.TestPoint.position = noodlesPlate.position;
			//di.testDistance = .5f;
			phase = 4;

		

		}
		else if(phase == 4) // testenine u tanjiru;
		{

			float pom = 0;

			Strainer.GetComponent<DragItem>().enabled = false;
			Vector3 smStartPos = noodlesPlate.position;
			Vector3 smEndPos =  Plate.GetChild(0).position;

			while(pom<1)
			{
				pom+=1.03f*Time.fixedDeltaTime;
				noodlesPlate.localPosition = Vector3.Lerp(smStartPos, Vector3.zero, pom);
				yield return new WaitForFixedUpdate();
			}
			//yield return new WaitForSeconds(1);

			 
			Vector3 arcMax = new Vector3(0,2,0); 
			noodlesPlate.SetParent(Plate.GetChild(0));

			StrainerEndPos.position = Strainer.position;
			pom = 0;
			while(pom<1)
			{
				pom+=.5f*Time.fixedDeltaTime;
				Strainer.position = Vector3.Lerp(StrainerEndPos.position, StrainerStartPos.position,pom)  + pom* (1-pom) *arcMax;
				//Plate.position = Vector3.Lerp(smStartPos, smEndPos,pom)  + pom* (1-pom) *arcMax;
				yield return new WaitForFixedUpdate();
			}

			phase = 5;
			animButtonNext.gameObject.SetActive(true);
			psLevelCompleted.gameObject.SetActive(true);
			psLevelCompleted.Play();
			if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound(SoundManager.Instance.ActionCompleted);	
			//Tutorial.Instance.ShowTutorial(4);
		}
   
		yield return new WaitForEndOfFrame();
	}

	public void ButtonNextClicked()//面条煮熟了点下一步
	{
        if (SoundManager.Instance!=null) SoundManager.Instance.Stop_Sound(SoundManager.Instance.FryingSound);	
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
            SceneManager.LoadScene("CookNoodles");
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
        SDKManager.Instance.ShowAd(ShowAdType.ChaPing,1,"返回首页");
        if (SoundManager.Instance!=null) SoundManager.Instance.Stop_Sound(SoundManager.Instance.FryingSound);	
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
