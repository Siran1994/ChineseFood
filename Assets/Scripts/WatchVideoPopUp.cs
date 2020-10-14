using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WatchVideoPopUp : MonoBehaviour {
	public static WatchVideoPopUp instance;
	MenuManager menuManager;
	public string MenuItemName = "";
 
	bool bTestiranje = false;
	void Awake()
	{
		instance = this;
//		# if UNITY_EDITOR
//			bTestiranje = true;
//		#endif
	}
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void ShowPopUpWatchVideo()
	{
		MenuManager.Instance.ShowPopUpMenu(gameObject);
		MenuItemName = "";
	}


	public void ShowPopUpWatchVideo( GameObject senderGO )
	{
		MenuManager.Instance.ShowPopUpMenu(gameObject);
		MenuItemName =  GetFullPath(senderGO);
	}

	public void ButtonYesClicked()
	{
		//BlockClicks.Instance.SetBlockAll(true);
		//BlockClicks.Instance.SetBlockAllDelay(.5f,false);
		if(SoundManager.Instance!=null) SoundManager.Instance.Play_ButtonClick();

		WatchVideo();
		if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound( SoundManager.Instance.Locked);
		MenuManager.Instance.ClosePopUpMenu(gameObject);
	 
	}

	public void ButtonNoClicked()
	{
		//BlockClicks.Instance.SetBlockAll(true);
		//BlockClicks.Instance.SetBlockAllDelay(.5f,false);
		if(SoundManager.Instance!=null) SoundManager.Instance.Play_ButtonClick();

		MenuManager.Instance.ClosePopUpMenu(gameObject);
		MenuItemName = "";
	}

	void OnDestroy()
	{
		instance = null;
	}

	private string GetFullPath(GameObject go)
	{
		return go.transform.parent == null
			? go.name
				: GetFullPath(go.transform.parent.gameObject) + "/" + go.name;
	}



	//***************************WATCH VIDEO******************
 
	public void WatchVideo( )
	{
		//zahtev da se prikaze video
		//Debug.Log(  "WATCH VIDEO");
	//if( GameData.sTestiranje.Contains("WatchVideo;") || 
		if( bTestiranje  ) //TEST
		{
			// FinishWatchingVideoError();
			 FinishWatchingVideo();
		}
		else
		{
			//AdsManager.bPlayVideoReward = true;
//			AdsManager.Instance.IsVideoRewardAvailable(AdsManager.WATCH_VIDEO_ID);
           // AdsManager.Instance.IsVideoRewardAvailable();
		}
	}


	public void FinishWatchingVideoError()
	{

		MenuManager.Instance.ShowPopUpDialogTitleText("Video not available");
		MenuManager.Instance.ShowPopUpDialogCustomMessageText("Video is not available at this moment. Thank you for understanding."); 
		MenuItemName = "";
	}




	public void FinishWatchingVideo( )
	{
		// potvrda da je odgledan video...
		//poziva se iz native...

		if(MenuItemName !="")
		{
			Debug.Log("Odlgedan video za  item: " + MenuItemName );
			GameObject locked_item =  GameObject.Find(MenuItemName);
			if(locked_item !=null) locked_item.SendMessage("UnlockItem");
		}
		else
		{
			Camera.main.SendMessage("UnlockItem");
		}
			
		if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound( SoundManager.Instance.Coins);
		MenuItemName = "";
	}





}
