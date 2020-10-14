using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MakeSpringRollsScene : MonoBehaviour {
	
	public Animator animButtonNext;

	int phase = 0;
	public ScrollMenuGroup smgMeat;
	public ScrollMenuGroup smgVegetables;

	public ScrollMenu scrollMenu; //ovo je meni za izbor mesa i povrca
	public Transform scrollMenuContent;

	public ScrollMenu scrollMenu2; //ovo je meni za izbor preliva
	public Transform scrollMenuContent2;
	public ItemsColors SpringRollsFlavorColors;

	 

	public Image imgMeat;
	public Transform Meat1StartPos;
	public Transform Meat1EndPos;

	public Image imgMeat2;
	public Transform Meat2StartPos;
	public Transform Meat2EndPos;

	public Image imgVegetables1;
	public Transform Vegetables1StartPos;
	public Transform Vegetables1EndPos;

	public Image imgVegetables2;
	public Transform Vegetables2StartPos;
	public Transform Vegetables2EndPos;

	public Image imgSauce;
	public ItemAction actionSpringRolls;
	public Animator animSpringRoll;
	public ParticleSystem psLevelCompleted;

	IEnumerator Start ()
	{
		imgMeat.gameObject.SetActive(false);
		imgMeat2.gameObject.SetActive(false);

		imgVegetables1.gameObject.SetActive(false);
		imgVegetables2.gameObject.SetActive(false);

		imgSauce.gameObject.SetActive(false);

		animButtonNext.gameObject.SetActive(false);
		//imgFlavorDough.gameObject.SetActive(false);
		//BlockClicks.Instance.SetBlockAll(true);
		yield return new WaitForSeconds(.1f);
		//LevelTransition.Instance.ShowScene();

		yield return new WaitForSeconds(.1f);
		//BlockClicks.Instance.SetBlockAll(false);

		scrollMenu.gameObject.SetActive(true);
		scrollMenu.ShowMenu(0);
		if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound( SoundManager.Instance.ShowItemSound);
		phase =  1;
		Tutorial.Instance.ShowTutorial(0);
	}


	public void	ScrollMenuButtonClicked(int itemIndex)
	{
		Tutorial.Instance.StopTutorial();
		if(phase ==  1 || phase ==  3 || phase == 5) //biranje sastojaka koje se dodaju
		{
			if(phase ==  1) 
			{
				imgMeat.sprite = smgMeat.MenuGroupSpritesActive[itemIndex];
				imgMeat2.sprite = imgMeat.sprite;

			}
			else if(phase ==  3) 
			{
				imgVegetables1.sprite = smgVegetables.MenuGroupSpritesActive[itemIndex];
			 
			}
			else if(phase ==  5)
			{
				imgVegetables2.sprite = smgVegetables.MenuGroupSpritesActive[itemIndex];
				 
			}
			phase ++;
			StartCoroutine( "CNextPhase" );
		}

	}


	 
	public void NextPhase(string gameStatePhase)
	{
		if(phase ==  7)
		{
			Tutorial.Instance.StopTutorial();
			int itemIndex = int.Parse(  gameStatePhase.Replace("F","")) -1;
			imgSauce.color = SpringRollsFlavorColors.colors[itemIndex];
			phase ++;
			StartCoroutine( "CNextPhase" );
		}

	}

	IEnumerator CNextPhase()
	{
		if(phase == 2) //dodavanje mesa  
		{
			//BlockClicks.Instance.SetBlockAll(true);


			imgMeat.gameObject.SetActive(true);
			imgMeat.color = Color.clear;
			imgMeat2.gameObject.SetActive(true);
			imgMeat2.color = Color.clear;
			float pom = 0;

			while(pom<1)
			{
				pom+=Time.fixedDeltaTime;
				imgMeat.transform.position = Vector3.Lerp(Meat1StartPos.position, Meat1EndPos.position,pom) ;
				imgMeat.color = new Color(1,1,1,pom);
				imgMeat2.transform.position = Vector3.Lerp(Meat2StartPos.position, Meat2EndPos.position,pom) ;
				imgMeat2.color = imgMeat.color;

				yield return new WaitForFixedUpdate();
			}
			imgMeat.color = Color.white;
			imgMeat2.color = Color.white;

			scrollMenu.ChangeMenu(1);
			yield return new WaitForSeconds(1);
			if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound( SoundManager.Instance.ShowItemSound);
			//BlockClicks.Instance.SetBlockAll(false);
			phase = 3;
			Tutorial.Instance.ShowTutorial(1);
		}

		else if(phase == 4) //dodavanje povrca  
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
			phase = 5;
			Tutorial.Instance.ShowTutorial(2);
		}

		else if(phase == 6) //dodavanje povrca  
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
			phase = 7;
 			
			//yield return new WaitForSeconds(1);
			scrollMenu.gameObject.SetActive(false);
			scrollMenu2.gameObject.SetActive(true);
			if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound( SoundManager.Instance.ShowItemSound);
			scrollMenu2.ShowMenu(0);
			ScrollMenuDragItem.bEnableDrag = true;
			Tutorial.Instance.ShowTutorial(3);
		}

		else if(phase == 8) //dodavanje sosa
		{
			yield return new WaitForSeconds(1);
			//BlockClicks.Instance.SetBlockAll(true);

			Color c = imgSauce.color;
			imgSauce.gameObject.SetActive(true);
			imgSauce.color = Color.clear;
			float pom = 0;

			while(pom<1)
			{
				pom+=Time.fixedDeltaTime;
				imgSauce.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, pom) ;
				imgSauce.color = new Color(c.r,c.g,c.b,pom);
				yield return new WaitForFixedUpdate();
			}
			imgSauce.color = c;

			yield return new WaitForSeconds(2);

			scrollMenu2.HideMenu();

			yield return new WaitForSeconds(1);
			//BlockClicks.Instance.SetBlockAll(false);
			phase = 9;
			scrollMenu2.gameObject.SetActive(false);

			animSpringRoll.enabled = true;
			animSpringRoll.Play("P1");
			actionSpringRolls.enabled =true;

			//animButtonNext.gameObject.SetActive(true);
			Tutorial.Instance.ShowTutorial(4);
		}
		else if(phase == 10) {	animSpringRoll.Play("P2"); 	yield return new WaitForSeconds(1);  actionSpringRolls.bEnabled = true; 	}
		else if(phase == 11) {	animSpringRoll.Play("P3"); 	yield return new WaitForSeconds(1.5f);  actionSpringRolls.bEnabled = true; 	}
		else if(phase == 12) 
		{	
			
			Tutorial.Instance.StopTutorial();
			animSpringRoll.Play("P4"); 
			yield return new WaitForSeconds(2f);  
			if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound( SoundManager.Instance.ShowItemSound);
			yield return new WaitForSeconds(.5f);  
			actionSpringRolls.bEnabled = true; 	
			actionSpringRolls.bDragForAction = true;
			Tutorial.Instance.ShowTutorial(5);
		}
		else if(phase == 13) 
		{	
			Tutorial.Instance.StopTutorial();
			animSpringRoll.Play("P5"); 	

			yield return new WaitForSeconds(1);  
			animButtonNext.gameObject.SetActive(true); 	
			psLevelCompleted.gameObject.SetActive(true);
			psLevelCompleted.Play();
			if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound(SoundManager.Instance.ActionCompleted);	
		
		
		}

	}


	public void ShapeSprinRolls(int _phase)
	{
		phase ++;
		StartCoroutine( "CNextPhase" );

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
            SceneManager.LoadScene("FrySpringRolls");
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
        SDKManager.Instance.ShowAd(ShowAdType.ChaPing, 1, "春卷加馅界面返回首页点下一步");
        //BlockClicks.Instance.SetBlockAll(true);
		StartCoroutine("LoadNextScene");
		if(SoundManager.Instance!=null) SoundManager.Instance.Play_ButtonClick();
		//TODO:ADS  INTERSTITIAL_NEXT
		//AdsManager.Instance.ShowInterstitial(AdsManager.INTERSTITIAL_NEXT);
        GlobalVariables.ShowHomeNextInterstitial("next");
	}
}
