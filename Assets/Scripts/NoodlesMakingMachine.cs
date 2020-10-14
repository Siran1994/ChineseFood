using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NoodlesMakingMachine : MonoBehaviour {

	public ScrollMenuGroup smgDough;
	public ScrollMenu scrollMenu;//testo
	public Transform scrollMenuContent;
	public Transform DoughParent;
	public Transform Cable; 
	public Animator animNoodleMachine;
	public Animator animButtonNext;
	public ItemsColors noodleColors;
	public Image noodlesPlate;
	public Image noodlesMachine;
	public ItemAction ButtonNoodleMachine;
	Transform Dough; 
	int phase = 0;
	public ParticleSystem psLevelCompleted;

	IEnumerator Start () {
		//BlockClicks.Instance.SetBlockAll(true);
		animButtonNext.gameObject.SetActive(false);
		yield return new WaitForSeconds(.1f);
		scrollMenu.ShowMenu(0);
		yield return new WaitForSeconds(.9f);
		//LevelTransition.Instance.ShowScene();

		yield return new WaitForSeconds(1f);
		//BlockClicks.Instance.SetBlockAll(false);

		 Tutorial.Instance.ShowTutorial(0);
		EscapeButtonManager.AddEscapeButonFunction("ButtonHomeClicked" );
	}


	public void	ScrollMenuButtonClicked(int itemIndex)
	{
		if(phase == 0)
		{
			Tutorial.Instance.StopTutorial();
			phase = 1;
			//BlockClicks.Instance.SetBlockAll(true);
			Dough = scrollMenuContent.Find("Dough"+(itemIndex+1).ToString()+"/imgPlate/imgDough"); 
			//Debug.Log(Dough.name);
			Dough.SetParent(DoughParent);
			scrollMenu.scrollRect.enabled = false;

            GameObject.Destroy(Dough.GetComponent<CustomButton>());
			GameObject.Destroy(Dough.GetComponent<Animator>());

			StartCoroutine("CNextPhase");

			noodlesPlate.color = noodleColors.colors[itemIndex];
			noodlesMachine.color = noodleColors.colors[itemIndex];
			GameData.selectedColor = itemIndex;
		}
	}

 



	public void NextPhase(string _phase) 
	{
		if(_phase == "Plug" || _phase == "NoodleMachineOn" )
		{
			StartCoroutine("CNextPhase");
		}
	}


	public void NooldesMachineAnimEnd()
	{
		StartCoroutine("CNextPhase");
	}

	IEnumerator CNextPhase()
	{
		Tutorial.Instance.StopTutorial();
	 
		if(phase == 1) //odabiranje testa i sakrivanje skrol menija
		{
			//BlockClicks.Instance.SetBlockAll(true);
			phase =  2; 

			float pom = 0;
			Vector3 smStartPos = scrollMenu.transform.position;
			Vector3 smEndPos = scrollMenu.transform.position + new Vector3(-25,0,0);

			Vector3 dougStartPos = Dough.position;
			Vector3 doughEndPos = DoughParent.position;
			Vector3 arcMax = new Vector3(0,5,0); 
			yield return new WaitForEndOfFrame();
			while(pom<1)
			{
				pom+=.8f*Time.deltaTime;
				Dough.position =  Vector3.Lerp(dougStartPos, doughEndPos,pom) + pom* (1-pom) *arcMax; 
				Dough.localScale = Vector3.Lerp(Vector3.one, 1.2f * Vector3.one, pom);
				scrollMenu.transform.position = Vector3.Lerp(smStartPos, smEndPos, pom);
				yield return new WaitForEndOfFrame();

			}
			Dough.localScale =  1.2f * Vector3.one;
			GameObject.Destroy(scrollMenu.gameObject);

			//BlockClicks.Instance.SetBlockAll(false);

			//tutorijal povezivanje kabla
			Tutorial.Instance.ShowTutorial(1);
			GameObject.Destroy(Dough.GetComponent<LayoutElement>());
			Cable.GetComponent<ItemAction>().bEnabled = true;

			//BlockClicks.Instance.SetBlockAll(false);
			Tutorial.Instance.ShowTutorial(1);
		}
		else if(phase == 2) //povezivanje kabla
		{
			//BlockClicks.Instance.SetBlockAll(true);
			phase =  3; 

			float pom = 0;
			while(pom<1)
			{
				pom+=.5f*Time.deltaTime;
				Cable.localPosition = Vector3.Lerp(Cable.localPosition,Vector3.zero,pom);
				yield return new WaitForEndOfFrame();
			}
			Cable.localPosition = Vector3.zero;

			 
			//BlockClicks.Instance.SetBlockAll(false);
			ButtonNoodleMachine.bEnabled = true;
			Tutorial.Instance.ShowTutorial(2);
		}
		else if(phase == 3)
		{
			if(SoundManager.Instance!=null) SoundManager.Instance.StopAndPlay_Sound( SoundManager.Instance.MachineOnSound);
			phase = 4;
			animNoodleMachine.Play("noodleMachineOn");
		}
		else if(phase == 4)
		{
			
			//kraj
			phase = 5;
			animButtonNext.gameObject.SetActive(true);
			psLevelCompleted.gameObject.SetActive(true);
			psLevelCompleted.Play();
			if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound(SoundManager.Instance.ActionCompleted);	
			yield return new WaitForSeconds(1);

		}
		 
		yield return new WaitForEndOfFrame();
	}

	public void ButtonNextClicked()//完成面条制作点下一步
	{
        SDKManager.Instance.ShowAd(ShowAdType.ChaPing,1,"完成面条制作点下一步");
		if(SoundManager.Instance!=null) SoundManager.Instance.Stop_Sound(SoundManager.Instance.MachineOnSound);	
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
        if (LoadPanel!=null/*&&SDKManager.Instance.IsCanShowAd*/)
        {
            LoadPanel.SetActive(true);
        }
        else
        {
            SceneManager.LoadScene("BoilNoodles");
        }
	}


	public void ButtonHomeClicked()
	{
      
        //BlockClicks.Instance.SetBlockAll(true);
		//BlockClicks.Instance.SetBlockAllDelay(.5f,false);
		if(SoundManager.Instance!=null) SoundManager.Instance.Play_ButtonClick();

		PopupAreYouSure.parent.parent .GetComponent<MenuManager>().ShowPopUpMenu( PopupAreYouSure.gameObject);
		animNoodleMachine.speed = 0;
	}

	public Transform PopupAreYouSure;
	public void ButtonHomeYesClicked()
	{
        SDKManager.Instance.ShowAd(ShowAdType.ChaPing, 1, "返回首页");
        if (SoundManager.Instance!=null) SoundManager.Instance.Stop_Sound(SoundManager.Instance.MachineOnSound);	
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
		animNoodleMachine.speed = 1;
	}








	 
}
