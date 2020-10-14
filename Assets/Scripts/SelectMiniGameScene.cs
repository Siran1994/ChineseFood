using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectMiniGameScene : MonoBehaviour {

 
	public Animator animScroll;
 
	void Awake()
	{ 
		Input.multiTouchEnabled = false;
	}

	IEnumerator Start () {
		//BlockClicks.Instance.SetBlockAll(true);
		////LevelTransition.Instance.ShowScene();
		//animScroll.Play("openScroll");
		yield return new WaitForSeconds(.3f);
		if(SoundManager.Instance!=null) SoundManager.Instance.StopAndPlay_Sound( SoundManager.Instance.ShowFreezer);
		yield return new WaitForSeconds(1.8f);
		if(SoundManager.Instance!=null) SoundManager.Instance.StopAndPlay_Sound( SoundManager.Instance.PopUpShow);
		//BlockClicks.Instance.SetBlockAll(false);
	}

	public void ScrollMenuButtonClicked(int btnNo)
	{
		ScrollMenuDragItem.bEnableDrag = true;
		GameData.ResetAllImages();
		if(SoundManager.Instance!=null) SoundManager.Instance.Play_ButtonClick();
		GameData.miniGame = btnNo+1;
		//animScroll.Play("closeScroll");
		Debug.Log(btnNo);
		StartCoroutine ("LoadNextScene",btnNo);

	}

	IEnumerator LoadNextScene(int btnNo)
	{
		Debug.Log("Load Next");
		yield return new WaitForEndOfFrame();
       

        switch (btnNo)
		{
			case 0:  SceneManager.LoadScene("NoodlesMakingMachine");break;
			case 1:  SceneManager.LoadScene("SweetDumplingsDough");break;
			case 2:  SceneManager.LoadScene("FortuneCookieMixIngredients");break;
			case 3:  SceneManager.LoadScene("DimSumMakingMachine");break;
			case 4:  SceneManager.LoadScene("SpringRollsMixIngredients");break;
		}
	}
		
   

	public void ButtonHomeClicked()
	{
		if(SoundManager.Instance!=null) SoundManager.Instance.Play_ButtonClick();
		 SceneManager.LoadScene("HomeScene");		
	}
}
