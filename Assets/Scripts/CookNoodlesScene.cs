using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CookNoodlesScene : MonoBehaviour {

	public ScrollMenuGroup smgMeat;
	public ScrollMenuGroup smgVegetables;
	public ScrollMenu scrollMenu; 
	public Transform scrollMenuContent;
	 
	 
	public Animator animButtonNext;
	public ItemsColors noodleColors;
	public Image oilFryingPan;
 
	public Transform MixHolder;
	public Transform Plate;
	public Transform PlateEndPos;
	public Transform NoodlesEndPos;
	public Transform noodlesPlate;

	public Image imgMeat;
	public Transform MeatStartPos;
	public Transform MeatEndPos;

	public Image imgVegetables1;
	public Transform Vegetables1StartPos;
	public Transform Vegetables1EndPos;

	public Image imgVegetables2;
	public Transform Vegetables2StartPos;
	public Transform Vegetables2EndPos;

	public GameObject smokeParticles;
	public ItemAction ButtonStove;
	public ProgressBar progressBar;

	int phase = 0;
	public ParticleSystem psLevelCompleted;
	int mixingPhase = -1;
	float mixingTime;

	public Animator animIngredients;

	public Image imgSoySauce;

	public Transform Spatula;
	public Transform SpatulaStartPos;
	public Transform SpatulaEndPos;
	public Transform SpatulaEndAnimPos;

	public Animator EndGameAnim;

	public Image imgNoodlesSC;
	public Image imgVegetablesSC;
	public Image imgVegetables2SC;
	public Image imgMeatSC;

	bool bStopMixingTut = false;
	IEnumerator Start () {
		scrollMenu.gameObject.SetActive(false);
		int selectedCol = (GameData.selectedColor>-1)? GameData.selectedColor : 1;

		noodlesPlate.GetComponent<Image>().color = noodleColors.colors[selectedCol];
		imgNoodlesSC.color = noodleColors.colors[selectedCol];


		EndGameAnim.enabled = false;
		Spatula.gameObject.SetActive(false);

		animIngredients.gameObject.SetActive(false);
		imgSoySauce.gameObject.SetActive(false);
		imgMeat.gameObject.SetActive(false);
		imgVegetables1.gameObject.SetActive(false);
		imgVegetables2.gameObject.SetActive(false);
		progressBar.gameObject.SetActive(false);
		Plate.gameObject.SetActive(false);

		oilFryingPan.gameObject.SetActive(false);
		//BlockClicks.Instance.SetBlockAll(true);
		animButtonNext.gameObject.SetActive(false);
		yield return new WaitForSeconds(.1f);
		//scrollMenu.ShowMenu(0);
		yield return new WaitForSeconds(.1f);
		//LevelTransition.Instance.ShowScene();

		yield return new WaitForSeconds(.1f);
		//BlockClicks.Instance.SetBlockAll(false);


		Tutorial.Instance.ShowTutorial(0);
		EscapeButtonManager.AddEscapeButonFunction("ButtonHomeClicked" );
		DragItem.OneItemEnabledNo = -1;
		if(SoundManager.Instance!=null) SoundManager.Instance.listStopSoundOnExit.Add(SoundManager.Instance.MixerSound);

	}


	void Update () 
	{
		if(Mixer.bMixBowl )
		{
			float dt = Time.deltaTime*0.5f;
			mixingTime+=dt;
			MixHolder.Rotate(new Vector3(0,0,dt*180) );
			//imgDough2.transform.localRotation = imgDough.transform.localRotation;
			 
			if(SoundManager.Instance!=null) 
			{
 
					SoundManager.Instance.Play_Sound(SoundManager.Instance.MixerSound); 
			}   
			if(mixingTime>5 && phase ==13)
			{
				phase = 14;
				Debug.Log("EndMixing");
				if(SoundManager.Instance!=null) SoundManager.Instance.Stop_Sound(SoundManager.Instance.MixerSound);	
				StartCoroutine("CNextPhase");
			}
			progressBar.SetProgress(mixingTime/5f ,false );

			if(!Mixer.bHandMixer) Mixer.bMixBowl = false;

			 
			if(mixingTime<1)
			{
				imgSoySauce.color = new Color(1,1,1,1-mixingTime);
				oilFryingPan.color = new Color(1,1,1,1-mixingTime);
			}
			else if(mixingTime<1.2f) 
			{
				imgSoySauce.gameObject.SetActive(false);
				oilFryingPan.gameObject.SetActive(false);
			}
			if(!bStopMixingTut)
			{
				Tutorial.Instance.StopTutorial();
			}

		}
		else if( (mixingPhase >= 1 || 	mixingPhase == -1) && !Mixer.bMixBowl && Mixer.bEnabled)
		{
			if(SoundManager.Instance!=null) 
			{
 
					SoundManager.Instance.Stop_Sound(SoundManager.Instance.MixerSound);	
			}	
		}
	}


	public void	ScrollMenuButtonClicked(int itemIndex)
	{
		if(phase ==  3 || phase ==  5 || phase == 7) //biranje sastojaka koje se dodaju
		{
	 
			if(phase ==  3) imgMeat.sprite = smgMeat.MenuGroupSpritesActive[itemIndex];
			else if(phase ==  5) imgVegetables1.sprite = smgVegetables.MenuGroupSpritesActive[itemIndex];
			else if(phase ==  7)imgVegetables2.sprite = smgVegetables.MenuGroupSpritesActive[itemIndex];
			phase ++;
			StartCoroutine("CNextPhase");

 
		}
	}

 
	public void NextPhase(string _phase) 
	{
		if( _phase == "Oil"   || _phase == "StoveOn" ||  _phase == "PlateNoodles" || _phase == "Salt" || _phase == "SoySauce")
		{
			StartCoroutine("CNextPhase");
		}
	}

 
	IEnumerator CNextPhase()
	{
		Tutorial.Instance.StopTutorial();

		if(phase == 0)//sipanje ulja
		{
			Tutorial.Instance.StopTutorial();
			float pom = 0;
 
			oilFryingPan.color = new Color(1,1,1,0);
			oilFryingPan.gameObject.SetActive(true);
 
			while(pom<1)
			{
				pom+=Time.deltaTime*.45f;
				oilFryingPan.color = new Color(1,1,1,pom);
				yield return new WaitForEndOfFrame();
			}
			oilFryingPan.color = Color.white;
			ButtonStove.bEnabled = true;
			phase = 1;
			Tutorial.Instance.ShowTutorial(1);
		}

		else if(phase == 1) //zagrevanje ulja
		{
			ButtonStove.transform.GetChild(0).gameObject.SetActive(false);
			ButtonStove.transform.GetChild(1).gameObject.SetActive(true);
			if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound( SoundManager.Instance.FryingSound);
			int waitTime = 1;
			yield return new WaitForSeconds(1f);

			 

			Vector3 smStartPos =Plate.position;
			Vector3 smEndPos =  PlateEndPos.position;
			Plate.gameObject.SetActive(true);

			Vector3 arcMax = new Vector3(0,2,0); 
			if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound(SoundManager.Instance.ShowItemSound);
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

		else if(phase == 2) //sipanje testa u tiganj
		{

			float pom = 0;

			Vector3 smStartPos = noodlesPlate.position;
			Vector3 smEndPos =  NoodlesEndPos.position;
			Vector3 arcMax = new Vector3(0,.7f,0); 
 


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

			pom = 0;
			if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound( SoundManager.Instance.InsertFruit);
			while(pom<1)
			{
				
				pom+=Time.fixedDeltaTime;
				noodlesPlate.rotation = Quaternion.Lerp(noodlesPlate.rotation ,Quaternion.identity, pom);
				noodlesPlate.position = Vector3.Lerp(smStartPos, smEndPos,pom*.9f);

				noodlesPlate.localScale = Vector3.Lerp( new Vector3(1,.5f,1), new Vector3(1,.35f,1), pom);
				yield return new WaitForFixedUpdate();
			}
			if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound( SoundManager.Instance.FryingSound2);
			noodlesPlate.SetParent(MixHolder);
			noodlesPlate.SetAsFirstSibling();
			pom = .9f;
			while(pom<1)
			{

				pom+=Time.fixedDeltaTime;
				noodlesPlate.rotation = Quaternion.Lerp(noodlesPlate.rotation ,Quaternion.identity, pom);
				noodlesPlate.position = Vector3.Lerp(smStartPos, smEndPos,pom);
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
		 
			while(pom<1)
			{
				pom+=Time.fixedDeltaTime;
				Plate.position = Vector3.Lerp(smStartPos, smEndPos,pom)  + pom* (1-pom) *arcMax;
				yield return new WaitForFixedUpdate();
			}
			Plate.gameObject.SetActive(false);

			pom = 0;
		
			smokeParticles.SetActive(true);

			yield return new WaitForSeconds(1);
			scrollMenu.gameObject.SetActive(true);
			scrollMenu.ShowMenu(0);
			if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound(SoundManager.Instance.ShowItemSound);
 
			phase = 3;

			Tutorial.Instance.ShowTutorial(3);
		}

		else if(phase == 4) //dodavanje mesa u tiganj
		{
			
			//BlockClicks.Instance.SetBlockAll(true);


			imgMeat.gameObject.SetActive(true);
			imgMeat.color = Color.clear;
			float pom = 0;

			while(pom<1)
			{
				pom+=Time.fixedDeltaTime;
				imgMeat.transform.position = Vector3.Lerp(MeatStartPos.position, MeatEndPos.position,pom) ;
				imgMeat.color = new Color(1,1,1,pom);
				yield return new WaitForFixedUpdate();
			}
			imgMeat.color = Color.white;
			 
			scrollMenu.ChangeMenu(1);
			if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound(SoundManager.Instance.ShowItemSound);
			yield return new WaitForSeconds(1);
			//BlockClicks.Instance.SetBlockAll(false);
			phase = 5;
			Tutorial.Instance.ShowTutorial(3);
		}

		else if(phase == 6) //dodavanje povrca u tiganj 1.
		{
			//BlockClicks.Instance.SetBlockAll(true);


			imgVegetables1.gameObject.SetActive(true);
			imgVegetables1.color = Color.clear;
			float pom = 0;

			while(pom<1)
			{
				pom+=Time.fixedDeltaTime;
				imgVegetables1.transform.position = Vector3.Lerp(Vegetables1StartPos.position, Vegetables1EndPos.position,pom) ;
				imgVegetables1.color = new Color(1,1,1,pom);
				yield return new WaitForFixedUpdate();
			}
			imgVegetables1.color = Color.white;

			yield return new WaitForSeconds(.5f);
			//BlockClicks.Instance.SetBlockAll(false);
			phase = 7;
			Tutorial.Instance.ShowTutorial(3);
		}

		else if(phase == 8) //dodavanje povrca u tiganj 2.
		{
			//BlockClicks.Instance.SetBlockAll(true);


			imgVegetables2.gameObject.SetActive(true);
			imgVegetables2.color = Color.clear;
			float pom = 0;

			while(pom<1)
			{
				pom+=Time.fixedDeltaTime;
				imgVegetables2.transform.position = Vector3.Lerp(Vegetables2StartPos.position, Vegetables2EndPos.position,pom) ;
				imgVegetables2.color = new Color(1,1,1,pom);
				yield return new WaitForFixedUpdate();
			}
			imgVegetables2.color = Color.white;

			scrollMenu.HideMenu();
			 
			yield return new WaitForSeconds(1);
			//BlockClicks.Instance.SetBlockAll(false);
			phase = 9;
			if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound(SoundManager.Instance.ShowItemSound);
			animIngredients.gameObject.SetActive(true);
			animIngredients.Play("ShowMenu");

			 DragItem.OneItemEnabledNo = 2;
			Tutorial.Instance.ShowTutorial(4);
		}
		else if(phase == 9) //salt
		{
			Tutorial.Instance.ShowTutorial(5);
			DragItem.OneItemEnabledNo = 3;
			phase = 10;
		}
		else if(phase == 10) //pepper
		{
			Tutorial.Instance.ShowTutorial(6);
			DragItem.OneItemEnabledNo = 4;
			phase = 11;
		}
		else if(phase == 11) //soja sos
		{
			DragItem.OneItemEnabledNo = 0;
			phase = 12;

			//prikazivanje soja sosa
			imgSoySauce.gameObject.SetActive(true);
			imgSoySauce.color = Color.clear;
			float pom = 0;
			while(pom<1)
			{
				pom+=Time.fixedDeltaTime;
				imgSoySauce.color = new Color(1,1,1,pom);
				yield return new WaitForFixedUpdate();
			}

			imgSoySauce.color = Color.white;
			yield return new WaitForSeconds(3);


			//Prikazivanje kasike za mesanje
			Spatula.gameObject.SetActive(true);
			pom = 0;
			Vector3 arcMax = new Vector3(0,2,0); 
			if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound(SoundManager.Instance.ShowItemSound);
			while(pom<1)
			{
				pom+=Time.fixedDeltaTime;
				Spatula.transform.position = Vector3.Lerp(SpatulaStartPos.position, SpatulaEndPos.position,pom)  + pom* (1-pom) *arcMax;
				yield return new WaitForFixedUpdate();
			}


			mixingPhase = 1;
			mixingTime = 0;
			Spatula.GetComponent<Mixer>().Init();
			Mixer.bEnabled = true;

			progressBar.SetProgress(0  ,false );
			progressBar.gameObject.SetActive(true);


			//sakrivanje zacina
			yield return new WaitForSeconds(1);
			animIngredients.gameObject.SetActive(false);
 	 
			phase = 13;
			Tutorial.Instance.ShowTutorial(7);
		}

		else if(phase == 14) //EndMixing
		{
			if(SoundManager.Instance!=null) SoundManager.Instance.Stop_Sound( SoundManager.Instance.FryingSound);
			if(SoundManager.Instance!=null) SoundManager.Instance.Stop_Sound( SoundManager.Instance.FryingSound2);
			//kraj
			phase =15;
			Mixer.bMixBowl = false;
			Mixer.bEnabled = false;

 
			Spatula.GetComponent<Mixer>().enabled = false;

			float pom = 0;

			SpatulaEndPos.position = Spatula.transform.position;
			 
			while(pom<1)
			{
				pom+=3*Time.fixedDeltaTime;
				Spatula.transform.position = Vector3.Lerp(SpatulaEndPos.position,  SpatulaEndAnimPos.position,pom);
				yield return new WaitForFixedUpdate();
			}
 
			Spatula.parent.SetSiblingIndex(2);
			Plate.gameObject.SetActive(true);
			Plate.SetAsFirstSibling();
			MixHolder.parent.parent.parent.SetAsLastSibling();

			EndGameAnim.enabled = true;
			EndGameAnim.Play("endAnim");

			yield return new WaitForSeconds(4.2f);

			MixHolder.SetParent(Plate);
			smokeParticles.transform.SetParent(Plate);
			Plate.SetParent(GameObject.Find("ActiveItemHolder").transform);
			//povecavanje tanjira
			 pom = 0;

			EndGameAnim.enabled = false;
			SpatulaEndPos.position = Spatula.transform.position;
			Vector3 platePos = Plate.position;
			Vector3 plateSc1= Plate.localScale;
			Vector3 plateSc2 = Plate.localScale*1.5f;
			while(pom<1)
			{
				pom+= Time.fixedDeltaTime;
				Plate.position = Vector3.Lerp(Plate.position,  Vector3.zero,pom);
				Plate.localScale = Vector3.Lerp(plateSc1,  plateSc2,pom);
				yield return new WaitForFixedUpdate();
			}
 

			psLevelCompleted.gameObject.SetActive(true);
			psLevelCompleted.Play();
			if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound(SoundManager.Instance.ActionCompleted);	
			animButtonNext.gameObject.SetActive(true);

			imgVegetablesSC.sprite = imgVegetables1.sprite;
			imgVegetables2SC.sprite = imgVegetables2.sprite;
			imgMeatSC.sprite = imgMeat.sprite;
			transform.GetComponent<CaptureImage>().ScreenshotMeal();
		}

		 
		yield return new WaitForEndOfFrame();
	}
 
	//-------------------------------------------------------------------------------------------------------------------

	public void ButtonNextClicked()
	{
       
        if (SoundManager.Instance!=null) SoundManager.Instance.Stop_Sound( SoundManager.Instance.FryingSound);
		if(SoundManager.Instance!=null) SoundManager.Instance.Stop_Sound( SoundManager.Instance.FryingSound2);
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
            SceneManager.LoadScene("EatNoodles");
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
		if(SoundManager.Instance!=null) SoundManager.Instance.Stop_Sound( SoundManager.Instance.FryingSound2);
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
