using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WriteFortuneCookieMessageScene : MonoBehaviour {

 
	public ParticleSystem psLevelCompleted;

	public Color[] paintingColors;
	public Color[] bgColors;
	public Animator animColorsHolder;
	public Animator animStickersHolder;
	public Animator animBGColorsHolder;

	Animator animActiveMenu;
	int LastMenu = 0;
	 

	public Image imgFortuneMessage;
	public Image CupMask;
	//public RawImage paintedTex;
	public AdvancedMobilePaint.AdvancedMobilePaint paintEngine;
	public Texture2D inkBrush;
	public Texture2D eraserBrush;
	Texture2D paintSurface;
	public Texture2D maskTexture;
	Texture2D tmpTex ;


	bool bAMPInitialised = false;
	 



	public Image SC_imgFortuneMessage;
	public RawImage SC_paintedTex;
	public Transform SC_StickersHolder;


	bool bFirstInitAmp = false;
	int activeColorNo = 1;
 
	public Stickers stickers;
	 


	public void Awake()
	{
		paintSurface = new Texture2D(1024,1024,TextureFormat.ARGB32,false);
		ClearStartImage(paintSurface);
		//paintedTex.texture = paintSurface;
		//SC_paintedTex.texture = paintSurface;

		StartCoroutine("WShowAMP");
		animActiveMenu = animColorsHolder;
    }

	  



	IEnumerator Start ()
	{
		Decoration.bEnableDrag = false;
		stickers.decorationTransform.bMoveDecoratins = false;
		//BlockClicks.Instance.SetBlockAll(true);
		 
		DragItem.OneItemEnabledNo = 1;

		yield return new WaitForSeconds(1f);
        //LevelTransition.Instance.ShowScene();


        //BlockClicks.Instance.SetBlockAll(false);

        StickyerPanel.SetActive(true);
        Tutorial.Instance.ShowTutorial(0);
		EscapeButtonManager.AddEscapeButonFunction("ButtonHomeClicked" );

       

    }
	
 

	//---------aktiviranje AMP--------------------------------------
	IEnumerator WShowAMP()
	{
		//animColorsHolder.Play("ShowMenu");
		yield return new WaitForSeconds(.1f);

		if(!bAMPInitialised)
		{
			paintEngine.transform.gameObject.SetActive(true);
			paintEngine.SetDrawingTexture( paintSurface); //ako ovo ne ubacim izbacuje gresku

			yield return new WaitForSeconds(.1f);
			paintEngine.undoEnabled = false;
			paintEngine.multitouchEnabled = false;
			paintEngine.InitializeEverything();

			paintEngine.SetDrawingTexture( paintSurface);

			paintEngine.SetBitmapBrush(inkBrush,AdvancedMobilePaint.BrushProperties.Default,true,false,paintingColors[0],true,true,null);
			//ColorButtonBG.transform.position = PopUpFingerPainting.transform.FindChild("AnimationHolder/Body/ContentHolder/ButtonColor1").position;
			//animColorsHolder.transform.Find("ButtonColor" + activeColorNo.ToString()).GetChild(0).gameObject.SetActive(true);

			yield return new WaitForSeconds(.1f);

			paintEngine.useLockArea=false;
			paintEngine.useAdditiveColors = false;

			paintEngine.maskTex = maskTexture;
			paintEngine.SetDrawingMask( maskTexture );

			paintEngine.SetMaskTextureMode();
			bAMPInitialised = true;
			paintEngine.CreateAreaLockMask( paintSurface.width/2, paintSurface.height/2); 
		}

		yield return new WaitForSeconds(.1f);
		if( bFirstInitAmp)  paintEngine.drawEnabled=true;
		else 	bFirstInitAmp = true;


		paintEngine.drawEnabled=true;
	}




	void ClearImage( Texture2D texOriginal )
	{
		if(paintEngine.pixels.Length == 0 ) return;
		int pix = 0;
		Color[] tmp = texOriginal.GetPixels(); 
		//		 Debug.Log(tmp.Length);
		for (int i = 0; i < tmp.Length; i++) 
		{
			paintEngine.pixels[pix] = 1;
			paintEngine.pixels[pix+1] = 1;
			paintEngine.pixels[pix+2] =  1;
			paintEngine.pixels[pix+3] =  0;
			pix+=4;
		}
		paintEngine.textureNeedsUpdate = true;
		paintEngine.UpdateTexture();

	}

	void ClearStartImage( Texture2D texOriginal )
	{

		Color[] tmp = texOriginal.GetPixels(); 
		Color c = new Color(1,1,1,0);
		for (int i = 0; i < tmp.Length; i++) 
		{
			tmp[i] = c;
		}
		texOriginal.SetPixels(tmp);
		texOriginal.Apply();

	}



    //-------------------------------------------------------------------------------------------------
    public GameObject BgColorPanel;
    public GameObject StickyerPanel;

    public void ButtonShowChangeBGColorMenuClicked()//背景颜色
    {
        if (SoundManager.Instance != null) SoundManager.Instance.Play_ButtonClick();
        BgColorPanel.SetActive(true);
        StickyerPanel.SetActive(false);
    }

    public void ButtonShowStickersMenuClicked()//贴纸
    {
        if (SoundManager.Instance != null) SoundManager.Instance.Play_ButtonClick();
        BgColorPanel.SetActive(false);
        StickyerPanel.SetActive(true);
    }




    IEnumerator CShowStickers()
    {
        animStickersHolder.gameObject.SetActive(true);
        paintEngine.drawEnabled = false;
        animColorsHolder.Play("HideMenu");
        yield return new WaitForSeconds(.2f);
        animStickersHolder.Play("ShowMenu");
        yield return new WaitForSeconds(.5f);
        //animColorsHolder.gameObject.SetActive(false);
        //BlockClicks.Instance.SetBlockAll(false);
    }
    IEnumerator CShowMenu(int menuNo)
    {
        Tutorial.Instance.StopTutorial();
        if (LastMenu != menuNo)
        {
            if (animActiveMenu != null) animActiveMenu.Play("HideMenu");
            if (menuNo == 0)//amp
            {
                animColorsHolder.gameObject.SetActive(true);
                paintEngine.drawEnabled = true;
                yield return new WaitForSeconds(.2f);
                animColorsHolder.Play("ShowMenu");
                Decoration.bEnableDrag = false;
                stickers.decorationTransform.bMoveDecoratins = false;
                stickers.decorationTransform.HideDecorationTransformTool();

                paintEngine.SetBitmapBrush(inkBrush, AdvancedMobilePaint.BrushProperties.Default, false, false, paintingColors[activeColorNo - 1], true, true, null);
            }
            else if (menuNo == 1)//stikeri
            {
                animStickersHolder.gameObject.SetActive(true);
                paintEngine.drawEnabled = false;
                yield return new WaitForSeconds(.2f);
                animStickersHolder.Play("ShowMenu");
                Decoration.bEnableDrag = true;
                stickers.decorationTransform.bMoveDecoratins = true;
            }
            else if (menuNo == 2)//pozadina
            {
                animBGColorsHolder.gameObject.SetActive(true);
                paintEngine.drawEnabled = false;
                yield return new WaitForSeconds(.2f);
                animBGColorsHolder.Play("ShowMenu");
                Decoration.bEnableDrag = false;
                stickers.decorationTransform.bMoveDecoratins = false;
                stickers.decorationTransform.HideDecorationTransformTool();
            }
            else
            {
                Decoration.bEnableDrag = false;
                stickers.decorationTransform.bMoveDecoratins = false;
                stickers.decorationTransform.HideDecorationTransformTool();
                yield return new WaitForSeconds(.2f);
                paintEngine.drawEnabled = true;

                paintEngine.clearColor = new Color32(0, 0, 0, 0);
                paintEngine.SetBitmapBrush(eraserBrush, AdvancedMobilePaint.BrushProperties.Clear, false, true, paintEngine.clearColor, false, true, null);
            }

            yield return new WaitForSeconds(.5f);
            if (animActiveMenu != null) animActiveMenu.gameObject.SetActive(false);

        }

        if (menuNo == 0) animActiveMenu = animColorsHolder;
        else if (menuNo == 1) animActiveMenu = animStickersHolder;
        else if (menuNo == 2) animActiveMenu = animBGColorsHolder;
        else animActiveMenu = null;
        LastMenu = menuNo;
        //BlockClicks.Instance.SetBlockAll(false);

    }




    public void ButtonEraserClicked()
	{
		//BlockClicks.Instance.SetBlockAll(true);
		if(SoundManager.Instance!=null) SoundManager.Instance.Play_ButtonClick();
		StartCoroutine("CShowMenu", 3);
	}


	

	


	public void ButtonBGColorClicked(int colorNo)
	{
		//BlockClicks.Instance.SetBlockAll(true);
		//BlockClicks.Instance.SetBlockAllDelay(.3f,false);
		if(SoundManager.Instance!=null) SoundManager.Instance.Play_ButtonClick();
		 
		imgFortuneMessage.color = bgColors[colorNo-1];
		StartCoroutine("CChangeColor", (colorNo-1) );
	}

	IEnumerator CChangeColor(int colorInd)
	{
		Color oldCol = imgFortuneMessage.color;
		Color newCol = bgColors[colorInd];
		float pom = 0;
		while(pom<1)
		{
			pom +=Time.deltaTime*4;
			yield return new WaitForEndOfFrame();
			imgFortuneMessage.color = Color.Lerp(oldCol, newCol,pom);
		}
	}




	public void ButtonColorClicked(int colorNo)
	{
		Tutorial.Instance.StopTutorial();
		//BlockClicks.Instance.SetBlockAll(true);
		//BlockClicks.Instance.SetBlockAllDelay(.3f,false);
		if(SoundManager.Instance!=null) SoundManager.Instance.Play_ButtonClick();
		paintEngine.SetBitmapBrush(inkBrush,AdvancedMobilePaint.BrushProperties.Default,false,false,paintingColors[colorNo-1],true,true,null);

		if(activeColorNo > 0) animColorsHolder.transform.Find("ButtonColor" + activeColorNo.ToString()).GetChild(0).gameObject.SetActive(false);
		animColorsHolder.transform.Find("ButtonColor" + colorNo.ToString()).GetChild(0).gameObject.SetActive(true);
		activeColorNo = colorNo;
	}


	 
	//---------------------------------------------------------------------------------------------------
 
	public void ButtonShowChangeColorMenuClicked()
	{
		//BlockClicks.Instance.SetBlockAll(true);
		if(SoundManager.Instance!=null) SoundManager.Instance.Play_ButtonClick();
		//StartCoroutine("CShowColors");
		StartCoroutine("CShowMenu", 0);
	}

	IEnumerator CShowColors()
	{
		animColorsHolder.gameObject.SetActive(true);
		paintEngine.drawEnabled=true;
		animStickersHolder.Play("HideMenu");
		yield return new WaitForSeconds(.2f);
		animColorsHolder.Play("ShowMenu");
		yield return new WaitForSeconds(.5f);
		animStickersHolder.gameObject.SetActive(false);
		//BlockClicks.Instance.SetBlockAll(false);
	}

	
	


	 
	//--------------------------------------------------------------------------------------------------
	bool bSetStickerButtons =false;

	void SetStickerButtons()
	{
 
	}

 

	public void ButtonStickerClicked(int stickerInd)
	{
		//BlockClicks.Instance.SetBlockAll(true);
		//BlockClicks.Instance.SetBlockAllDelay(.3f,false);
		//if(  GameData.unlockedStickers[stickerInd] == 1) 
		{
 
			stickers.CreateSticker(stickerInd-1);
			if(SoundManager.Instance!=null) SoundManager.Instance.Play_ButtonClick();
		}
		/*else
		{
			//ZAKLJUCANO
			//Watch video
			videoStickerInd = stickerInd;
			WatchVideoPopUp.instance.ShowPopUpWatchVideo();	
			//if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound(SoundManager.Instance.EmptyFlavor);
		}*/
	}


	int videoStickerInd = -1;
	int videoPatternInd = -1;

	/*
	public void	UnlockItem()
	{
		if(videoStickerInd >-1)
		{
			GameData.unlockedStickers[videoStickerInd]  = 1;
			StickerButtonsHolder.GetChild(videoStickerInd).GetChild(1).gameObject.SetActive(false);

			GameData.SetCupStickersList();
			videoStickerInd = -1;

			if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound(SoundManager.Instance.Coins);
		}
		else if(videoPatternInd >-1)
		{
			GameData.unlockedPatterns[videoPatternInd]  = 1;
			PatternButtonsHolder.GetChild(videoPatternInd).GetChild(1).gameObject.SetActive(false);
			GameData.SetCupPatternList();
			videoPatternInd = -1;
			if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound(SoundManager.Instance.Coins);
		}
	}
*/


	//-------------------------------------------------------------------------------------------------------------------
	 

	public void ButtonNextClicked()
	{
        SDKManager.Instance.ShowAd(ShowAdType.ChaPing, 1, "写幸运字条界面返回首页点下一步");
        Tutorial.Instance.StopTutorial();
		//BlockClicks.Instance.SetBlockAll(true);
		if(SoundManager.Instance!=null) SoundManager.Instance.Play_ButtonClick();
		StartCoroutine("CNextPhase");
	}

	IEnumerator CNextPhase()
	{
		if(SoundManager.Instance!=null) SoundManager.Instance.Play_ButtonClick();
		//BlockClicks.Instance.SetBlockAll(true);

		SC_paintedTex.texture = paintEngine.tex;
		SC_imgFortuneMessage.color = imgFortuneMessage.color;

		foreach(Transform t in stickers.StickersHolder)
		{
			Debug.Log(t.name);
			GameObject go = GameObject.Instantiate(t.gameObject,SC_StickersHolder);
			go.transform.localScale = t.localScale;
			go.transform.localPosition = t.localPosition;
			go.transform.localRotation = t.localRotation;
		}


		transform.GetComponent<CaptureImage>().ScreenshotFortuneCookiesMessage(); 

		yield return new WaitForSeconds(.1f);
        if (LoadPanel != null /*/*&& SDKManager.Instance.IsCanShowAd*/)
        {
            LoadPanel.SetActive(true);
        }
        else
        {
            SceneManager.LoadScene("BakeFortuneCookies");
        }
	}
    public GameObject LoadPanel;
    //-------------------------------------------------------------------------------------------------------------------


    public void ButtonHomeClicked()
	{
        
        Tutorial.Instance.StopTutorial();
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
