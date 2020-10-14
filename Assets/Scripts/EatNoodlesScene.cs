using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EatNoodlesScene : MonoBehaviour {

	public Animator PopupTapToEat;
	public Animator animTimerAndReplayHolder;
	public Animator animTimer;
	public Animator animEndMenu;

	public Animator animButtonNext;
	public  GameObject ButtonReplay;
	public Canvas canvas;
	public RectTransform eatNoodlesImageRT;

	public RawImage eatNoodlesImage;


	Texture2D texCopy ;
	public Texture2D  BiteTex;
	float screenScale = 1;

	bool bEnableEat = false;
	public GameObject ButtonTapToEat;

	byte[] pixelBuffer;
	int activePixelsCount = 0;
	int alphaLimit = 50; //vrednost ispod koje se racuna da je piksel transparentan



	int pL = 0;
	int pR = 0;
	int pT= 0;
	int pB = 0;


	int biteTexSize = 64;//128; //OVA PODESAVANJA MORAJU DA BUDU NA OSNOVU VELICINE TEKSTURE UGRIZA
	int biteHalfTexSize =32;//64;

	int eatTexSize = 300; //256;//512  ,400,600,800
	public GameTimer gameTimer;

	public Sprite[] endMessagesSprtes;
	public Image imageEndMessage;

	public Sprite testSprite;


	IEnumerator Start ()
	{
		animButtonNext.gameObject.SetActive(false);
		GlobalVariables.OnPauseGame +=FLPauseGame;

		animEndMenu.gameObject.SetActive(false);
		ButtonReplay.SetActive(false);
		//BlockClicks.Instance.SetBlockAll(true);


		eatNoodlesImage.texture = (GameData.FinishedMealSprite !=null) ?  GameData.FinishedMealSprite.texture : testSprite.texture;// GameData.FinishedMealSprite.texture; 

		StartCoroutine("CreateTexture");

		yield return new WaitForSeconds(.7f);
		//LevelTransition.Instance.ShowScene();
		EscapeButtonManager.AddEscapeButonFunction("ButtonBackClicked" );
		yield return new WaitForSeconds(.3f);

		//BlockClicks.Instance.SetBlockAll(false);
		if(SoundManager.Instance!=null) SoundManager.Instance.listStopSoundOnExit.Add( SoundManager.Instance.TimerSound);

	}



	void CalculatePixelsRect()
	{
		int scWidth = Camera.main.pixelWidth;
		int scHeight = Camera.main.pixelHeight;

		float scale = canvas.scaleFactor; 

		screenScale =    eatTexSize / (float) (eatNoodlesImageRT.rect.width * scale);

		pL = Mathf.CeilToInt ( (eatNoodlesImageRT.rect.x * scale + eatNoodlesImageRT.anchoredPosition.x   * scale  +  Camera.main.pixelWidth/2f) * screenScale) ;
		pR = Mathf.FloorToInt ( ((eatNoodlesImageRT.rect.x + eatNoodlesImageRT.rect.width)* scale + eatNoodlesImageRT.anchoredPosition.x  * scale  + Camera.main.pixelWidth/2f) * screenScale);
		pB = Mathf.CeilToInt ( (eatNoodlesImageRT.rect.y * scale + eatNoodlesImageRT.anchoredPosition.y   * scale    +   Camera.main.pixelHeight/2f) * screenScale);
		pT = Mathf.FloorToInt (( (eatNoodlesImageRT.rect.y + eatNoodlesImageRT.rect.height)* scale + eatNoodlesImageRT.anchoredPosition.y  * scale  + Camera.main.pixelHeight/2f) * screenScale);
	}


	IEnumerator CreateTexture()
	{
		CalculatePixelsRect();
		yield return new WaitForSeconds(0.1f);
		int texSizeX = eatTexSize;
		int texSizeY = Mathf.FloorToInt( eatTexSize* eatNoodlesImage.texture.height/eatNoodlesImage.texture.width); 


		texCopy = new Texture2D( texSizeX, texSizeY  ,TextureFormat.Alpha8,false);
		pixelBuffer = new byte[texCopy.width * texCopy.height]; 

		yield return new WaitForSeconds(0.1f);

		AlphaScale( (Texture2D)eatNoodlesImage.texture, texCopy.width,texCopy.height );

		yield return new WaitForSeconds(0.1f);
		texCopy.LoadRawTextureData(pixelBuffer);
		texCopy.Apply();

		//------------------------------ -NOVO----------------------------
		yield return new WaitForSeconds(0.1f);
		Color[] pixTex  = texCopy .GetPixels();// (0,0, texSizeX, texSizeY);
		for( int i = 0;i<pixTex.Length;i++)
		{
			if(pixTex[i].a>.1f) pixTex[i].a = 1; 
		}
		texCopy.SetPixels(pixTex);
		texCopy.Apply();

		//-----------------------------------------------------------------

		yield return new WaitForSeconds(0.2f);
		eatNoodlesImage.material.SetTexture("_AlphaTex", texCopy);
	}


	void Update()
	{
		//Debug.Log(bEnableEat + "    "  +  !GlobalVariables.bPauseGame );
		if(   bEnableEat && !GlobalVariables.bPauseGame &&    Input.GetMouseButtonDown(0))
		{

			Tutorial.Instance.StopTutorial();

			int x = Mathf.CeilToInt (  ( ( Input.mousePosition.x) * screenScale) ); 
			int y = Mathf.CeilToInt ( (( Input.mousePosition.y) * screenScale) ); 

			//kliknuto unutar teksture
			if(x>pL && x<pR &&  y>pB && y<pT)  
			{
				if(SoundManager.Instance!=null) SoundManager.Instance.StopAndPlay_Sound(SoundManager.Instance.EatSound);	

				bEnableEat = false;
				// Debug.Log("In" );
				// Debug.Log(" posX:" +(x - pL)  + ",  posY:" + (y - pB)   );

				x= ( x - pL -biteHalfTexSize);
				y= ( y - pB -biteHalfTexSize);

				int x1 = 0;
				int y1 = 0; 

				int width =  biteTexSize;
				int height = biteTexSize;

				if(x<0) 
				{
					width =biteTexSize + x; 
					x1 = -x;  
					x = 0;
				}
				else if( x> texCopy.width - biteTexSize ) width = texCopy.width - x ;  

				if(y<0) 
				{ 
					height = biteTexSize + y; 
					y1 = -y;
					y = 0;
				}
				else if( y> texCopy.height -biteTexSize ) height = texCopy.height - y;



				Color[] pixTex = texCopy.GetPixels(x, y, width, height);
				Color[] pixBite = BiteTex.GetPixels(x1, y1, width, height);
				// 	Debug.Log(pixTex.Length+ "  ==  " + pixBite.Length);

				for( int i = 0;i<pixTex.Length;i++)
				{
					if(pixTex[i].a>.05f)
					{
						if(pixBite[i].b<.3f) pixTex[i].a = 0;
						else if(pixBite[i].b<.8f ) pixTex[i].a = .4f;
					}
				}
				texCopy.SetPixels(x, y, width, height, pixTex);
				texCopy.Apply();

				StartCoroutine("TestEnd");
			}
		}
	}





	IEnumerator TestEnd()
	{
		bEnableEat = false;
		if(SoundManager.Instance!=null) SoundManager.Instance.StopAndPlay_Sound(SoundManager.Instance.EatSound);

		yield return new WaitForSeconds(0.3f);
		pixelBuffer = texCopy.GetRawTextureData();
		int count= 0;
		for(int i=0;i<pixelBuffer.Length;i++)
		{
			if(pixelBuffer[i]>alphaLimit)
				count++;
		}
			
		if(count< (activePixelsCount*0.02f) )
		{
 
			Debug.Log("KRAJ");
			if(SoundManager.Instance!=null) SoundManager.Instance.StopAndPlay_Sound(SoundManager.Instance.ActionCompleted);

			gameTimer.StopTimer();
			bEnableEat = false;
			StartCoroutine("LevelCompleted");
		}
		else
		{
			bEnableEat = true;
		}
	}


	//********************************

	//skaliranje alpha kanala na zeljenu velicinu
	//broje se pikseli koji nisu providni
	public void AlphaScale ( Texture2D tex,  int w2, int h2 )
	{
		int w;
		float ratioX;
		float ratioY;

		Color32[] texColors =tex.GetPixels32();
		w = tex.width;

		ratioX =  ((float)tex.width) / (float) w2  ;
		ratioY =    ((float)tex.height) / (float) h2 ;

		//	PointScale 
		for (var y =0; y <  h2; y++)
		{
			var thisY = (int)(ratioY * y) * w;
			var yw = y * w2;
			for (var x = 0; x < w2; x++) {
				pixelBuffer[yw + x] =  texColors[Mathf.FloorToInt(thisY + ratioX*x)].a;
				if(pixelBuffer[yw + x] >alphaLimit) activePixelsCount++;
			}
		}
	}

	//---------------------------------------------------------------

	public void ButtonTapToEatClicked()
	{
		ButtonTapToEat.transform.GetChild(0).GetComponent<Button>().interactable = false;

		if(SoundManager.Instance!=null) SoundManager.Instance.Play_ButtonClick();
		//BlockClicks.Instance.SetBlockAll(true);
		//BlockClicks.Instance.SetBlockAllDelay(1f,false);
		StartCoroutine("WTapToEeat");
	}

	IEnumerator WTapToEeat()
	{
		yield return new WaitForSeconds(.1f);
		PopupTapToEat.CrossFade("hide",.1f,-1,0);
		animTimerAndReplayHolder.SetBool("bShow",true);
		yield return new WaitForSeconds(1f);

		gameTimer.StartTimer();
		if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound(SoundManager.Instance.TimerSound);	
		bEnableEat = true;
		Debug.Log("START");
		PopupTapToEat.gameObject.SetActive(false);

		Tutorial.Instance.ShowTutorial(0);

	}


	public void ButtonReplayClicked()
	{
       // SDKManager.Instance.ShowAd(ShowAdType.VideoAD, 2, "重吃吃面条");
        if (SoundManager.Instance!=null) 
		{
			SoundManager.Instance.Stop_Sound(SoundManager.Instance.TimerSound);	
			SoundManager.Instance.Play_ButtonClick();
			SoundManager.Instance.Stop_Sound(SoundManager.Instance.OutOfTimeSound);	
		}


		 SceneManager.LoadScene("EatNoodles");
		//BlockClicks.Instance.SetBlockAll(true);
		//BlockClicks.Instance.SetBlockAllDelay(1f,false);

	}
    #region 成功和失败结算

    public GameObject SuccessPan;
    public GameObject FailPan;


    #endregion


    public void Time10SecLeft()
	{
		animTimer.Play ("10sec");
	}

	public void OutOfTime()
	{
		StartCoroutine("WOutOfTime");
	}

	IEnumerator WOutOfTime()
	{
       
        Debug.Log("OUT OF TIME");
		bEnableEat = false;

		if(SoundManager.Instance!=null)
		{
			SoundManager.Instance.Stop_Sound(SoundManager.Instance.TimerSound);	
			SoundManager.Instance.Play_Sound(SoundManager.Instance.OutOfTimeSound);	
		}

		imageEndMessage.sprite = endMessagesSprtes[0];
		animEndMenu.gameObject.SetActive(true); 

		yield return new WaitForSeconds(1f);
		animTimerAndReplayHolder.SetBool("bShow",false);
		yield return new WaitForSeconds(1f);
		animTimerAndReplayHolder.SetBool("bShow",true);
        //animButtonNext.gameObject.SetActive(true);

        //animTimer.gameObject.SetActive(false);
        //ButtonReplay.SetActive(true);
        SuccessPan.SetActive(false);
        FailPan.SetActive(true);
    }

	IEnumerator LevelCompleted()
	{
      
        eatNoodlesImage.enabled = false;
		imageEndMessage.sprite = endMessagesSprtes[Random.Range(1, 3)];
		animEndMenu.gameObject.SetActive(true); 

		yield return new WaitForSeconds(1f);
		animTimerAndReplayHolder.SetBool("bShow",false);
		yield return new WaitForSeconds(1f);
		animTimerAndReplayHolder.SetBool("bShow",true);
        //animButtonNext.gameObject.SetActive(true);

        //animTimer.gameObject.SetActive(false);
        //ButtonReplay.SetActive(true);
        SuccessPan.SetActive(true);
        FailPan.SetActive(false);
    }


	bool bEnableEatTmp;
	public Transform  PopupAreYouSure;

	public void ButtonHomeClicked()
	{
       
        bEnableEatTmp = bEnableEat;
		bEnableEat = false;
		//BlockClicks.Instance.SetBlockAll(true);
		//BlockClicks.Instance.SetBlockAllDelay(.5f,false);
		if(SoundManager.Instance!=null) SoundManager.Instance.Play_ButtonClick();

		PopupAreYouSure.parent.parent .GetComponent<MenuManager>().ShowPopUpMenu( PopupAreYouSure.gameObject);
		StartCoroutine(SetTimeScale(0, 0f));

	}

	public void ButtonHomeYesClicked()
	{
        SDKManager.Instance.ShowAd(ShowAdType.ChaPing, 1, "返回首页");
        StartCoroutine(SetTimeScale(1, 1f));

		if(SoundManager.Instance!=null) 
		{
			SoundManager.Instance.Stop_Sound(SoundManager.Instance.TimerSound);	
			SoundManager.Instance.Play_ButtonClick();
			SoundManager.Instance.Stop_Sound(SoundManager.Instance.OutOfTimeSound);	
		}

		//TODO:ADS -  INTERSTITIAL_HOME
		//AdsManager.Instance.ShowInterstitial(AdsManager.INTERSTITIAL_HOME);
        GlobalVariables.ShowHomeNextInterstitial("home");
	
		 SceneManager.LoadScene("HomeScene");
		//BlockClicks.Instance.SetBlockAll(true);
	}

	public void ButtonHomeNoClicked()
	{
      
        StartCoroutine(SetTimeScale(1, 1f));
		//BlockClicks.Instance.SetBlockAll(true);
		//BlockClicks.Instance.SetBlockAllDelay(.5f,false);
		if(SoundManager.Instance!=null) SoundManager.Instance.Play_ButtonClick();
		PopupAreYouSure.parent.parent .GetComponent<MenuManager>().ClosePopUpMenu( PopupAreYouSure.gameObject);
		if(EscapeButtonManager.EscapeButonFunctionStack.Count == 0)  EscapeButtonManager.AddEscapeButonFunction("ButtonBackClicked" );
		StartCoroutine(CEnableEat(1f,true));
	}

	IEnumerator SetTimeScale(float timeScale, float waitTime)
	{
		yield return new WaitForSecondsRealtime(waitTime);
		Time.timeScale = timeScale;
	}

	IEnumerator CEnableEat (  float waitTime, bool val)
	{
		yield return new WaitForSecondsRealtime(waitTime);
		bEnableEat = val;
	}

	void FLPauseGame()
	{
		if( bEnableEat  )
		{
			PopUpPause.transform.parent.parent.GetComponent<MenuManager>().ShowPopUpMenu(PopUpPause);
			GlobalVariables.bPauseUI = true;
		}
	}


	public GameObject PopUpPause;

	public void ButtonUnpauseGameClicked( )
	{
		if(SoundManager.Instance!=null) SoundManager.Instance.Play_ButtonClick();

		PopUpPause.transform.parent.parent.GetComponent<MenuManager>().ClosePopUpMenu(PopUpPause);
		StopCoroutine("CUnpause");
		StartCoroutine("CUnpause");
	}

	IEnumerator CUnpause()
	{
		yield return new WaitForSecondsRealtime(1f);
		GlobalVariables.UnpauseGame(GlobalVariables.PauseSource.UI);
	}


	public void ButtonNextClicked()
	{
        SDKManager.Instance.ShowAd(ShowAdType.ChaPing, 1, "吃面条界面完成后点下一步");
        StartCoroutine("CNextPhase");
	}

	IEnumerator CNextPhase()
	{
		if(SoundManager.Instance!=null) SoundManager.Instance.Play_ButtonClick();
		//BlockClicks.Instance.SetBlockAll(true);

		yield return new WaitForSeconds(.1f);
		//load next
		 SceneManager.LoadScene("SelectMiniGame");


		//TODO:ADS  INTERSTITIAL_NEXT
		//AdsManager.Instance.ShowInterstitial(AdsManager.INTERSTITIAL_NEXT);
        GlobalVariables.ShowHomeNextInterstitial("next");
		yield return new WaitForEndOfFrame();

	}


}
