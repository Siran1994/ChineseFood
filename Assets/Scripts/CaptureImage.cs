using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using System;

public class CaptureImage : MonoBehaviour {


	 //TODO: BRISI
	//public static CaptureImage Instance;

	public GameObject[] HideOnScreenCapture;
	 
	 
	public Camera camera2;
	public Canvas canvas1;
	public Canvas canvas2;
 
	public RectTransform SC_ImageRect;

 
	Texture2D _Texture;
	string _DirectoryName = "CandyFactory";
	string _PictureName = "img" ;


	public RawImage savedImg;

	void Start () {
 
	}



	public void ScreenshotMeal () 
	{
		StartCoroutine("CScreenshotMeal");
	}

	IEnumerator CScreenshotMeal()
	{
		canvas2.gameObject.SetActive(true);
		camera2.gameObject.SetActive(true);
		yield return new WaitForSeconds(.1f);

		int scWidth = camera2.pixelWidth;
		int scHeight = camera2.pixelHeight;

		float cs =    canvas2.scaleFactor ;


		int texWidth = Mathf.CeilToInt (SC_ImageRect.rect.width*cs*SC_ImageRect.transform.localScale.x  );
		int texHeight = Mathf.CeilToInt (SC_ImageRect.rect.height*cs *SC_ImageRect.transform.localScale.x );

		RenderTexture rt = new RenderTexture(scWidth, scHeight, 32);
		camera2.targetTexture = rt;
		Texture2D screenShot = new Texture2D(texWidth, texHeight, TextureFormat.ARGB32, false);

		camera2.Render();
		RenderTexture.active = rt;
		screenShot.ReadPixels(new Rect((scWidth-texWidth)/2f,  (scHeight- texHeight )/2f  + SC_ImageRect.anchoredPosition.y*cs , texWidth, texHeight), 0, 0); 

		screenShot.Apply ();


		camera2.targetTexture = null;
		RenderTexture.active = null;  
		rt.Release();
		Destroy(rt);

		GameData.FinishedMealSprite  = Sprite.Create(    screenShot, new Rect(0,0,texWidth,texHeight), Vector2.zero ) ;
		if(savedImg!=null) savedImg.texture = screenShot;

	}



	public void ScreenshotFortuneCookiesMessage () 
	{
		StartCoroutine("CScreenshotFortuneCookiesMessage");
	}

	IEnumerator CScreenshotFortuneCookiesMessage()
	{
		canvas2.gameObject.SetActive(true);
		camera2.gameObject.SetActive(true);
		yield return new WaitForSeconds(.1f);

		int scWidth = camera2.pixelWidth;
		int scHeight = camera2.pixelHeight;

		float cs =    canvas2.scaleFactor ;


		int texWidth = Mathf.CeilToInt (SC_ImageRect.rect.width*cs*SC_ImageRect.transform.localScale.x  );
		int texHeight = Mathf.CeilToInt (SC_ImageRect.rect.height*cs *SC_ImageRect.transform.localScale.x );

		RenderTexture rt = new RenderTexture(scWidth, scHeight, 32);
		camera2.targetTexture = rt;
		Texture2D screenShot = new Texture2D(texWidth, texHeight, TextureFormat.ARGB32, false);

		camera2.Render();
		RenderTexture.active = rt;
		screenShot.ReadPixels(new Rect((scWidth-texWidth)/2f,  (scHeight- texHeight )/2f  + SC_ImageRect.anchoredPosition.y*cs , texWidth, texHeight), 0, 0); 

		screenShot.Apply ();


		camera2.targetTexture = null;
		RenderTexture.active = null;  
		rt.Release();
		Destroy(rt);

		GameData.FortuneMessageSprite  = Sprite.Create(    screenShot, new Rect(0,0,texWidth,texHeight), Vector2.zero ) ;
		if(savedImg!=null) savedImg.texture = screenShot;

	}
 
 

 
	public void CaptureScreenForGallery()
	{
		int scWidth = camera2.pixelWidth;
		int scHeight = camera2.pixelHeight;

		float cs =    canvas2.scaleFactor ;//camera2.pixelHeight/ 1400f;
 
		RenderTexture rt = new RenderTexture(scWidth, scHeight, 32);
		camera2.targetTexture = rt;
		Texture2D screenShot = new Texture2D(scWidth, scHeight, TextureFormat.ARGB32, false);

		camera2.Render();
		RenderTexture.active = rt;
		screenShot.ReadPixels(new Rect(0, 0 , scWidth, scHeight), 0, 0);


		screenShot.Apply ();


		camera2.targetTexture = null;
		RenderTexture.active = null;  
		rt.Release();
		Destroy(rt);


		_Texture = screenShot;
		//savedImg.texture = _Texture;
	}

	//--------------------------------------------------------------------------------------------------------------------------------------



	public void btnSaveCick()
	{
		if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound(SoundManager.Instance.CameraSound);
		CaptureScreenForGallery();
	 
		#if UNITY_ANDROID
		try
		{
			using (AndroidJavaClass cls_UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) 
			{
				using (AndroidJavaObject obj_Activity = cls_UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity")) 
				{
					obj_Activity.Call("checkPermissionAndRun");
				}
			} 
			return;
		}
		catch
		{
			Debug.Log("Error Contacting Android save image!");
			return;
		}
		#endif
		 
	//	SaveImage();
	}
 

	public void SaveImage()
	{
 
//		Texture2D texture = _Texture;
//		string directoryName = _DirectoryName ;
//		string pictureName  = _PictureName + DateTime.Now.ToString("yyyyMMddhhmmss");
//		 
// 
//
//		if (!Directory.Exists (Application.persistentDataPath + "/"+directoryName))
//			Directory.CreateDirectory (Application.persistentDataPath + "/"+directoryName);
//		//Debug.Log (Application.persistentDataPath + "/test");
//		byte[] bytes = texture.EncodeToPNG ();
//
//		#if !UNITY_WINRT
//		File.WriteAllBytes (Application.persistentDataPath + "/"+directoryName+"/"+pictureName +".png", bytes);
//		#else
//		UnityEngine.Windows.File.WriteAllBytes(Application.persistentDataPath + "/"+directoryName+"/"+pictureName +".png", bytes);
//		#endif
//
// 
//		#if UNITY_ANDROID &&  !UNITY_EDITOR
//		string path2 = Share.ReturnGalleryFolder() ;
//		//Debug.Log("M1 path2 "+path2);
//
//		if(path2 !="")
//		{
//			try 
//			{
//				if(!Directory.Exists(path2+directoryName))
//				Directory.CreateDirectory(path2+directoryName);
//
//				path2+= directoryName+"/"+pictureName+".png";
//				string path = Application.persistentDataPath + "/"+directoryName+"/"+pictureName +".png";
//				File.Copy(path,path2,false);
//				MenuManager.Instance.ShowPopUpDialogTitleText("PICTURE SAVED");
//				MenuManager.Instance.ShowPopUpDialogCustomMessageText("Picture saved in album");
//				//Debug.Log("M1 "+ "picture saved " + path);
//				//ShowPopUp("PICTURE SAVED"," ");
//				
//			} 
//			catch(Exception e)  
//			{ 	
//				Debug.Log("NAIL PATH 2 "+ e.Message);
//				if(savedImg!=null) 
//				{	
//					savedImg.enabled = false;
//					savedImg.transform.parent.GetComponent<Image>().color = Color.white;
//				}
//				MenuManager.Instance.ShowPopUpDialogTitleText("PICTURE NOT SAVED");
//				MenuManager.Instance.ShowPopUpDialogCustomMessageText(" ");
//			}
//		}
//		 
//		//MenuShare.anchoredPosition = hidePos;
//		//bMenuShare = false;
//
//		Share.RefreshGalleryFolder(path2);
//
//		#endif
//
//
//
//		#if UNITY_EDITOR
//		MenuManager.Instance.ShowPopUpDialogTitleText("PICTURE SAVED");
//		MenuManager.Instance.ShowPopUpDialogCustomMessageText(" ");
//
//		Debug.Log( "picture saved");
//		#endif
//
//		#if UNITY_IOS && !UNITY_EDITOR
//		OtherMessagesBinding.sendMessage("SaveToGallery###" + Application.persistentDataPath + "/"+directoryName+"/"+pictureName +".png");
//		MenuManager.Instance.ShowPopUpDialogTitleText("PICTURE SAVED");
//		MenuManager.Instance.ShowPopUpDialogCustomMessageText(" ");
//		#endif
//
//		#if UNITY_WINRT &&!UNITY_EDITOR 
//		byte[] _bytes =   UnityEngine.Windows.File.ReadAllBytes(Application.persistentDataPath + "/"+directoryName+"/"+pictureName +".png");
//
//		WPUtils.Utils u = new WPUtils.Utils ();
//		string tmp = u.ShareIMG (pictureName +".png", _bytes);
//		#endif


	}
 
	 
}