using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Shop : MonoBehaviour {

	public static Shop Instance = null;


	 

	public int StarsToAdd  = 0;
	public int StarsToAddStart = 0;

    //FIXME vratiti na 0
	public static int UnlockAll = 0;
	public static int RemoveAds = 0;
	public static int SpecialOffer = 0; //special offer je UnlockAll_RemoveAds

	public static bool bShowSpecialOfferInShop = false;

	public bool bShopWatchVideo = false;

	public string ShopItemID = "";

	public Text[] txtDispalyStars; //SVA POLJA NA SCENI KOJA TREBA DA SE AZURIRAJU PRILIKOM DODAVANJA ILI ODUZIMANJA ZVEZDICA

 
	/*
	public static void InitShop()
	{
		GameObject container;
		if(Instance == null) 
		{ 	
			if(GameObject.Find("DATA_MANAGER") == null)
			{
				container = new GameObject();
				container.name = "DATA_MANAGER";
			}
			else 	container = GameObject.Find("DATA_MANAGER");
			
			
			if(container.GetComponent<Shop>() == null) 	
				Instance = container.AddComponent<Shop>(); 
			else 
				Instance = container.GetComponent<Shop>();
			
			DontDestroyOnLoad(container);
		}
		 
	}
*/
	void Awake()
	{
		if(Instance !=null &&  Instance != this ) GameObject.Destroy(gameObject);
		else {  Instance = this; DontDestroyOnLoad(this.gameObject); }

		//Shop.InitShop();
		GameData.Init();
		 
	}


 

	//***************************************************************
	//ODBROJAVANJE NOVCICA
	 
	public void AnimiranjeDodavanjaZvezdica(int _StarsToAdd,  Text txtStars  = null , string message = "STARS: " )
	{
		 
		 
		SoundManager.Instance.Coins.Stop();
		SoundManager.Instance.Play_Sound(SoundManager.Instance.Coins);
		 

/*	StarsToAddStart =  GameData.TotalStars;
		GameData.TotalStars +=_StarsToAdd;*/
		StarsToAdd = _StarsToAdd;
//		 GameData.SetStarsToPP();

//		Debug.Log(Coins);

		if(txtStars !=null)
		{

			StartCoroutine(animShopCoins(txtStars, message ));
		}
		else 
			StartCoroutine(animShopStarsAllTextFilds( ));
	}

	IEnumerator animShopCoins( Text txtStars , string message  )
	{
		//AUDIO.PlaySound(  "shop_coin");
		int  StarsToAddProg=0;

		int addC = 0;
		int stepUL = Mathf.FloorToInt(StarsToAdd*0.175f);
		int stepLL = Mathf.FloorToInt(StarsToAdd*0.19f);
 
		while( (Mathf.Abs(StarsToAddProg) + Mathf.Abs(addC)) < Mathf.Abs(StarsToAdd) )
		{
			StarsToAddProg+=addC;
			txtStars.text = message+  (StarsToAddStart + StarsToAddProg).ToString();
			//Debug.Log(CoinsToAddStart + CoinsToAddProg);
			yield return new WaitForSeconds (0.05f);
			addC = Mathf.FloorToInt(UnityEngine.Random.Range(stepLL, stepUL));
		}
		
		StarsToAddProg = StarsToAdd;
//		txtStars.text = message + GameData.TotalStars.ToString();

		//DataManager.Instance.SaveLastLevelData();
	}

	IEnumerator animShopStarsAllTextFilds(     )
	{
		//AUDIO.PlaySound(  "shop_coin");
		int  StarsToAddProg=0;
		
		int addC = 0;
		int stepUL = Mathf.FloorToInt(StarsToAdd*0.175f);
		int stepLL = Mathf.FloorToInt(StarsToAdd*0.22f);
		if(txtDispalyStars!=null)
		{
			while( (Mathf.Abs(StarsToAddProg) + Mathf.Abs(addC)) < Mathf.Abs(StarsToAdd) )
			{
				StarsToAddProg+=addC;
				for(int i = 0; i<txtDispalyStars.Length;i++)
				{
					if(txtDispalyStars[i].text == "") continue;
					if(txtDispalyStars[i].text.Contains("/"))
					{
						string[] split = txtDispalyStars[i].text.Split('/');
						txtDispalyStars[i].text =   (StarsToAddStart + StarsToAddProg).ToString() + "/" + split[1];
					}
					else
						txtDispalyStars[i].text =   (StarsToAddStart + StarsToAddProg).ToString();
				}
				//Debug.Log(CoinsToAddStart + CoinsToAddProg);
				yield return new WaitForSeconds (0.05f);
				addC = Mathf.FloorToInt(UnityEngine.Random.Range(stepLL, stepUL));
			}
			
			StarsToAddProg = StarsToAdd;

			for(int i = 0; i<txtDispalyStars.Length;i++)
			{
				if(txtDispalyStars[i].text == "") continue;
				if(txtDispalyStars[i].text.Contains("/"))
				{
					string[] split = txtDispalyStars[i].text.Split('/');
					txtDispalyStars[i].text =   (StarsToAddStart + StarsToAddProg).ToString() + "/" + split[1];
				}
//				else
//					txtDispalyStars[i].text =   GameData.TotalStars.ToString();
			}


		}
//		DataManager.Instance.SaveLastLevelData();
//		Debug.Log(" ** " + Coins);
	}


	
	//********************************************************










	//***************************************************************
	//ODBROJAVANJE DODAVANJA VREDNOSTI
	public Text[] txtFields;
	int StartVal = 0;
	int ValToAdd = 0;

	public void AnimiranjeDodavanjaVrednosti ( int _Start,  int _Add,   string message = "" )
	{
		 
		SoundManager.Instance.Coins.Stop();
		SoundManager.Instance.Play_Sound(SoundManager.Instance.Coins);
		 
		StartVal =  _Start;
 
		ValToAdd = _Add;
		//StopAllCoroutines();
		if(txtFields !=null)
			StartCoroutine(animValue(  message ));
		 
	}
	
	 
	
	IEnumerator animValue(   string message = ""  )
	{
		//AUDIO.PlaySound(  "shop_coin");
		int  ValToAddProg=0;
		
		int addC = 0;
		int stepUL = Mathf.FloorToInt(ValToAdd*0.175f);
		int stepLL = Mathf.FloorToInt(ValToAdd*0.22f);
		if(stepLL == 0 ) stepLL =1;
		if(stepUL ==0 ) stepUL =1;
		if(txtFields!=null)
		{
			while( (Mathf.Abs(ValToAddProg) + Mathf.Abs(addC)) < Mathf.Abs(ValToAdd) )
			{
				ValToAddProg+=addC;
				for(int i = 0; i<txtFields.Length;i++)
				{
					txtFields[i].text = message+  (StartVal + ValToAddProg).ToString();
				}
 
				yield return new WaitForSeconds (0.05f);
				addC = Mathf.FloorToInt(UnityEngine.Random.Range(stepLL, stepUL));
			}
			
			ValToAddProg = ValToAdd;
//			Debug.Log(StartVal + ValToAddProg);
			for(int i = 0; i<txtFields.Length;i++)
			{
				txtFields[i].text = message + (StartVal +ValToAdd).ToString();
			}
		}
	}
	
	
	


  
}
