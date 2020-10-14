using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class MakeDimSumScene : MonoBehaviour {

	public ScrollMenuGroup smgFlavors;
	public ScrollMenu scrollMenu;//testo
	public Transform scrollMenuContent;

 
	public Transform DoughParent;
	public Transform Cable; 
	public Animator animDimSumMachine;
	public Animator animButtonNext;
	public ItemsColors dimSumColors; //
 
	public Image dimSumDough; //testo u masini
	public Image[] dimSumPiece; 
	public Image imgDimSumDoughFlavor; //slika tecnosti koja se vidi kada se sipa aroma na testo
	public ItemAction ButtonDimSumMachine; //dugme koje pokrece pravljenje testa

	public Transform Plate; 
	public Transform PlateHolder; 
	public Transform FillDoughPos;

	int phase = 0;
	public ParticleSystem psLevelCompleted;

	int selectedFlavor = -1;

	public Transform fillPlateHolder;
	public Transform fillPlateEndPos;

	public Scoop scoop;
	public Transform scoopEndPos;

	public Transform bambooSteamer;
	public Transform bambooSteamerEndPos; //pozicija na koju treba da se postavi korpa od bambusa pri pojavljivanju na scenu
	public Transform[] dimSumBSEndPos; //zavrsne pozicije u korpi od bambusa

	int dimSumLeftToDo = 3;

	IEnumerator Start () {
		scrollMenu.gameObject.SetActive(false);
		bambooSteamer.gameObject.SetActive(false);
		scoop.gameObject.SetActive(false);
		fillPlateHolder.gameObject.SetActive(false);
		//BlockClicks.Instance.SetBlockAll(true);
		animButtonNext.gameObject.SetActive(false);
		yield return new WaitForSeconds(.1f);

		ScrollMenuDragItem.bEnableDrag = false;
		yield return new WaitForSeconds(.5f);
		//LevelTransition.Instance.ShowScene();

		yield return new WaitForSeconds(.5f);
		//BlockClicks.Instance.SetBlockAll(false);

		Tutorial.Instance.ShowTutorial(0);
		EscapeButtonManager.AddEscapeButonFunction("ButtonHomeClicked" );
	}


	 

	bool bShowTut2 = true;
	public void NextPhase(string _phase) 
	{
		if(_phase == "Plug" || _phase == "MachineOn" )
		{
			StartCoroutine("CNextPhase");
		}
		else if(_phase.StartsWith("ds"))
		{
			Tutorial.Instance.StopTutorial();
			selectedFlavor = int.Parse(_phase.Remove(0,2)) -1;
			Debug.Log(selectedFlavor);
			StartCoroutine("CChangeDoughColor");
			if(bShowTut2 ) 
			{
				Tutorial.Instance.ShowTutorial(2);
				bShowTut2= false;
			}
		}
	}


	IEnumerator CChangeDoughColor()
	{
		ButtonDimSumMachine.bEnabled =false;
		yield return new WaitForSeconds(1);
		Color c1 = dimSumDough.color;
		Color c = dimSumColors.colors[selectedFlavor];

		imgDimSumDoughFlavor.color = c;

		Color c2 = new Color(c.r,c.g,c.b,0);;
		imgDimSumDoughFlavor.gameObject.SetActive(true);
		imgDimSumDoughFlavor.transform.localScale =  Vector3.zero;
		 
		float p = 0;
		while(p<1)
		{
			p+=Time.deltaTime;
			imgDimSumDoughFlavor.transform.localScale = Vector3.Lerp (Vector3.zero, Vector3.one,p);
			yield return new WaitForEndOfFrame();
		}
		p = 0;
		while(p<1)
		{
			p+=Time.deltaTime*2f;
			dimSumDough.color = Color.Lerp( c1, c,p);
			imgDimSumDoughFlavor.color = Color.Lerp( c, c2, p);
			yield return new WaitForEndOfFrame();
		}
		//sledeci komadic 
		dimSumDough.color = c;
		imgDimSumDoughFlavor.gameObject.SetActive(false);
		yield return new WaitForSeconds(2f);
		ButtonDimSumMachine.bEnabled =true;
	}

	IEnumerator CNextPhase()
	{
		
		if(phase ==0)//ukljucena masina
		{
			Tutorial.Instance.StopTutorial();
			//BlockClicks.Instance.SetBlockAll(true);
			 
			float pom = 0;
			Vector3 sp = Cable.localPosition;
			while(pom<1)
			{
				pom+=1.5f* Time.deltaTime;
				Cable.localPosition = Vector3.Lerp(sp ,Vector3.zero,pom);
				yield return new WaitForEndOfFrame();
			}
			Cable.localPosition = Vector3.zero;


			//BlockClicks.Instance.SetBlockAll(false);
			ButtonDimSumMachine.bEnabled = true;
 

			phase = 1;
			yield return new WaitForEndOfFrame();
			ScrollMenuDragItem.bEnableDrag = true;
			scrollMenu.gameObject.SetActive(true);
			scrollMenu.ShowMenu(0);
			Tutorial.Instance.ShowTutorial(1);

		}
		else if(phase > 0 && phase < 5) //ukljucena je masina za pravljenje testa
		{
			Tutorial.Instance.StopTutorial();
			ScrollMenuDragItem.bEnableDrag = false;
			if(selectedFlavor >=0) 
			{
				dimSumPiece[phase-1].color = dimSumColors.colors[selectedFlavor];
				foreach( Transform t in dimSumPiece[phase-1].transform)
				{
					if(t.CompareTag("ScrollMenuColor")) t.GetComponent<Image>().color = dimSumColors.colors[selectedFlavor];
				}
			}
		 
			if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound( SoundManager.Instance.MachineOnSound);
			ButtonDimSumMachine.transform.GetChild(0).gameObject.SetActive(false);
			ButtonDimSumMachine.transform.GetChild(1).gameObject.SetActive(true);
			ButtonDimSumMachine.transform.GetChild(2).gameObject.SetActive(true);

			animDimSumMachine.Play("p"+phase);
			yield return new WaitForSeconds(3);
			ButtonDimSumMachine.bEnabled = true;
			if(SoundManager.Instance!=null) SoundManager.Instance.Stop_Sound( SoundManager.Instance.MachineOnSound);
			ButtonDimSumMachine.transform.GetChild(0).gameObject.SetActive(true);
			ButtonDimSumMachine.transform.GetChild(1).gameObject.SetActive(false);
			ButtonDimSumMachine.transform.GetChild(2).gameObject.SetActive(false);

			GameData.dimSumFlavors[phase-1] = selectedFlavor;

			phase++;
			if(phase == 5) //ZAVRSENO PRESOVANJE KOMADICA TESTA I POTREBNO JE POJAVITI TANJIR SA FILOM, KASIKU...
			{ 
				bShowTut2= false;
				yield return new WaitForSeconds(1);
				Cable.gameObject.SetActive(false);
				scrollMenu.HideMenu();
				animDimSumMachine.Play("Hide");
				Plate.SetParent(PlateHolder);
				DoughParent.SetParent(Plate);
				DoughParent.SetAsFirstSibling();
				yield return new WaitForSeconds(2);
				scrollMenu.gameObject.SetActive(false);
				animDimSumMachine.gameObject.SetActive(false);


				fillPlateHolder.gameObject.SetActive(true);

				Vector3 smStartPos =fillPlateHolder.position;
				Vector3 smEndPos =  fillPlateEndPos.position;
				Vector3 bsStartPos = bambooSteamer.position;
				Vector3 bsEndPos =  bambooSteamerEndPos.position;
				Vector3 scStartPos = scoop.transform.position;
				Vector3 scEndPos =  scoopEndPos.position;
				Plate.gameObject.SetActive(true);
				scoop.gameObject.SetActive(true);
				bambooSteamer.gameObject.SetActive(true);

				Vector3 arcMax = new Vector3(0,2,0); 

				float pom = 0;
				while(pom<1)
				{
					pom+=.8f*Time.fixedDeltaTime;
					fillPlateHolder.position = Vector3.Lerp(smStartPos, smEndPos,pom)  + pom* (1-pom) *arcMax;
					bambooSteamer.position = Vector3.Lerp(bsStartPos, bsEndPos,pom)  + pom* (1-pom) *arcMax;

					yield return new WaitForFixedUpdate();
				}
 
				phase = 6;

				smStartPos = dimSumPiece[3].transform.position;
				smEndPos =  FillDoughPos.position;

				pom = 0;

				//podesava se image punjenja
				scoop.imgFillOnDough = dimSumPiece[3].transform.GetChild(0).GetComponent<Image>();

				if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound( SoundManager.Instance.ShowItemSound);
				while(pom<1)
				{
					
					pom += Time.fixedDeltaTime;
					dimSumPiece[3].transform.position = Vector3.Lerp(smStartPos, smEndPos,pom);
					dimSumPiece[3].transform.localScale = Vector3.Lerp(Vector3.one, 1.8f * Vector3.one,pom);
					scoop.transform.position = Vector3.Lerp(scStartPos, scEndPos,pom)  + pom* (1-pom) *arcMax;
					yield return new WaitForFixedUpdate();
				}
				dimSumPiece[3].transform.SetParent(FillDoughPos);
				dimSumLeftToDo = 3;
				Tutorial.Instance.ShowTutorial(3);
			}
			else ScrollMenuDragItem.bEnableDrag = true;
		}
		 
 
	}

	public void ShapeDimSum(int _shapePhase)
	{
		// oblikovanje testa  


		StartCoroutine( CShapeDimSum( _shapePhase) );
	}

	IEnumerator CShapeDimSum(int _shapePhase)
	{
		

		if(_shapePhase<4) 
		{
			Animator animDS = FillDoughPos.GetComponent<Animator>();
			animDS.enabled = true;
			animDS.Play("poskakivanje",-1,0);
			yield return new WaitForSeconds(.75f);
			if(_shapePhase==1) 
			{
				dimSumPiece[dimSumLeftToDo].enabled = false;
			}

			dimSumPiece[dimSumLeftToDo].transform.GetChild(_shapePhase-1).gameObject.SetActive(false);
			dimSumPiece[dimSumLeftToDo].transform.GetChild(_shapePhase).gameObject.SetActive(true);
			yield return new WaitForSeconds(.85f);
			dimSumPiece[dimSumLeftToDo].GetComponent<ItemAction>().bEnabled = true;
			animDS.enabled = false;
			Debug.Log("_shapePhase " + _shapePhase);
		}
		else
		{
			Tutorial.Instance.StopTutorial();
			Debug.Log("DONE: CNextPhase " + phase);
			dimSumPiece[dimSumLeftToDo].GetComponent<ItemAction>().enabled = false;

			Transform tr = dimSumPiece[dimSumLeftToDo].transform;
			Vector3 startPos = tr.position;
			Vector3 endPos = dimSumBSEndPos[3-dimSumLeftToDo].position;

			//pomeranje na poziciju u korpi
			float pom = 0;
			while(pom<1)
			{
				pom += 1.2f*Time.fixedDeltaTime;
				tr.transform.position = Vector3.Lerp(startPos, endPos ,pom);
				tr.transform.localScale = Vector3.Lerp(tr.transform.localScale, Vector3.one ,pom);
				yield return new WaitForFixedUpdate();
			}


			dimSumPiece[dimSumLeftToDo].transform.SetParent(dimSumBSEndPos[3-dimSumLeftToDo]);
			yield return new WaitForSeconds(1);
			//ZAVRSENO OBLIKOVANJE JEDNOG KOMADA I PRELAZAK NA SLEDECI  ILI ZAVRSETAK AKO SU SVI GOTOVI
			if(dimSumLeftToDo>0)
			{
				dimSumLeftToDo--;

				//podesava se image punjenja
				startPos = dimSumPiece[dimSumLeftToDo].transform.position;
				endPos =  FillDoughPos.position;
				pom = 0;
				if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound( SoundManager.Instance.ShowItemSound);
				while(pom<1)
				{

					pom += 1.5f* Time.fixedDeltaTime;
					dimSumPiece[dimSumLeftToDo].transform.position = Vector3.Lerp(startPos, endPos,pom);
					dimSumPiece[dimSumLeftToDo].transform.localScale = Vector3.Lerp(Vector3.one, 1.8f * Vector3.one,pom);
					yield return new WaitForFixedUpdate();
				}
				scoop.imgFillOnDough = dimSumPiece[dimSumLeftToDo].transform.GetChild(0).GetComponent<Image>();
				scoop.bIskoriscen = false;

				dimSumPiece[dimSumLeftToDo].transform.SetParent(FillDoughPos);
			}
			else 
			{
				//ZAVRSENO SVE
				psLevelCompleted.gameObject.SetActive(true);
				psLevelCompleted.Play();
				if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound(SoundManager.Instance.ActionCompleted);	
				animButtonNext.gameObject.SetActive(true);
				Debug.Log( GameData.dimSumFlavors[0]+ ", " +  GameData.dimSumFlavors[1]+ ", " +  GameData.dimSumFlavors[2]+ ", " +  GameData.dimSumFlavors[3]);
			}
		}
		 

	}






	public void ButtonNextClicked()
	{
        SDKManager.Instance.ShowAd(ShowAdType.ChaPing, 1, "切饺子皮界面返回首页点下一步");
        if (SoundManager.Instance!=null) SoundManager.Instance.Stop_Sound( SoundManager.Instance.MachineOnSound);
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
            SceneManager.LoadScene("CookDimSum");
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
        if (SoundManager.Instance!=null) SoundManager.Instance.Stop_Sound( SoundManager.Instance.MachineOnSound);
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
