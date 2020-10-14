using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameData  {

	public static int selectedColor = -1;
	public static int chocolateFillColor = -1;
	public static Sprite FinishedMealSprite;
	public static Sprite FortuneMessageSprite;
	public static int[] dimSumFlavors = new int[4];
  
	 
	public static int miniGame = 0;
	public static int selectedFlavor = -1;

	public static string sTestiranje = "";

	public static int[] unlockedItems  = new int[2] {0,0};  // 0 - mixer u sceni FortuneCookieMicIngredients, 1 - mikser u sceni SpringRollsMixIngredients

	public static void Init()
	{
		 
		//-----------------------------------------------------------------
 
//#if UNITY_EDITOR
//		if( true  ) 
//		{
//			sTestiranje = "Test;"
//			 //  + "SpecialOffer;"
//			 // 	+ "OverrideShopCall;"  
//			//	+ "TestPopUpTransaction;"	
//			  	+ "WatchVideo;"
//			//	+ "FreeStar;";
//				 + "InternetOff;"
//			 ;
//
//			Debug.Log("TESTIRANJE UKLJUCENO: " + sTestiranje);
//		}
//		//-----------------------------------------------------------------------
//#endif
 

		 GetPurchasedItems();
	 
		GetUnlocekedItems();
		GetAllData();
	}

	 
	public static void GetAllData()
	{
 
	}

  
 

	public static void ResetAllImages()
	{ 
		selectedColor = -1;
		chocolateFillColor = -1;
		FinishedMealSprite  = null;
		FortuneMessageSprite  = null;
		dimSumFlavors = new int[4];
	}

 

	public static void GetPurchasedItems()
	{
		string tmp = PlayerPrefs.GetString("Data0","33114");
		tmp= tmp.Replace("<","9");
		tmp= tmp.Replace("7>q","8");
		tmp= tmp.Replace("nmFs","7");
		tmp= tmp.Replace("Vy;","6");
		tmp= tmp.Replace("*2","5");
		tmp= tmp.Replace("H","4");
		tmp= tmp.Replace("JE","3");
		tmp= tmp.Replace("B#","2");
		tmp= tmp.Replace("+0","1");
		tmp= tmp.Replace("Kce","0");

		int tmpPurchased = int.Parse(tmp);
		int purchased = tmpPurchased - 33114;


		Shop.SpecialOffer = Mathf.FloorToInt(purchased/100);
		purchased = purchased -  Shop.SpecialOffer*100;

		Shop.UnlockAll = Mathf.FloorToInt(purchased/10);
		Shop.RemoveAds = purchased - Shop.UnlockAll*10;


		if( Shop.SpecialOffer == 2)  { Shop.UnlockAll =2; Shop.RemoveAds = 2; Shop.bShowSpecialOfferInShop = false;}
		else if(Shop.UnlockAll ==2 && Shop.RemoveAds == 2)  {   Shop.SpecialOffer = 2;}
		if(Shop.RemoveAds == 2) GlobalVariables.removeAdsOwned = true;
 
	}

	public static void SetPurchasedItems()
	{

		if(Shop.SpecialOffer == 2) { Shop.UnlockAll =2; Shop.RemoveAds = 2; Shop.bShowSpecialOfferInShop = false;}
		else if(Shop.UnlockAll ==2 && Shop.RemoveAds == 2) {  Shop.SpecialOffer = 2;}
		if(Shop.RemoveAds == 2) GlobalVariables.removeAdsOwned = true;

		int purchased =   Shop.SpecialOffer*100 +   Shop.UnlockAll*10+Shop.RemoveAds ;

		string tmp = (purchased+33114).ToString();

		tmp= tmp.Replace("0","Kce");
		tmp= tmp.Replace("1","+0");
		tmp= tmp.Replace("2","B#");
		tmp= tmp.Replace("3","JE");
		tmp= tmp.Replace("4","H");
		tmp= tmp.Replace("5","*2");
		tmp= tmp.Replace("6","Vy;");
		tmp= tmp.Replace("7","nmFs");
		tmp= tmp.Replace("8","7>q");
		tmp= tmp.Replace("9","<");


		PlayerPrefs.SetString("Data0", tmp);
		PlayerPrefs.Save();

		GetAllData();
	}






	 
	public static void SaveUnlocekedItemsToPP( )
	{
		string UnlockedItems = "";
		for (int i = 0; i < unlockedItems.Length; i++) 
		{
			UnlockedItems += (unlockedItems[i] + ";" );
		}
		PlayerPrefs.SetString("Data1",UnlockedItems);
	}

	public static void GetUnlocekedItems( )
	{
		 
		if(Shop.UnlockAll !=2)
		{
			string UnlockedItems = PlayerPrefs.GetString("Data1", "");
			//Debug.Log("UNLOCKED:  "+ UnlockedItems);
			string[] unl_items = UnlockedItems.Split(new char[] {';'},System.StringSplitOptions.RemoveEmptyEntries);
			if( unl_items.Length==unlockedItems.Length)
			{
				for(int i = 0; i<unl_items.Length;i++)
				{
					unlockedItems[i]  = int.Parse(unl_items[i]);
				}
			}
		}
		else
		{
			for(int i = 0; i<unlockedItems.Length;i++)
			{
				unlockedItems[i]  = 1;
			}
		}
	}

 

	static void SetUnlockedFromString( ref bool[] unlockedItems, string data)
	{
		if(data != "")
		{
			string[] pom = data.Split(';');
			for(int i = 0; i< pom.Length;i++)
			{
				int item = 0;
				int.TryParse(pom[i],out item);
				if(item < unlockedItems.Length) unlockedItems[item] = true;
			}
		}
	}

 
	 
}
 


 
 
