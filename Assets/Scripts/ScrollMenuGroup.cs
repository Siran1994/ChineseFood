using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMenuGroup", menuName = "MenuGroup") ]
public class ScrollMenuGroup : ScriptableObject  {

	public bool bChangeImage = true;
	public bool bChangeColor = false;

	public string  PlayerPrefsName;
	public Sprite[] MenuGroupSprites;
	public bool[] UnlockedItems;
	public ItemsColors itemColors;


	public Sprite[] MenuGroupSpritesActive;

	public void GetUnlockedItems()
	{
		Debug.Log( Shop.UnlockAll);
		if(Shop.UnlockAll == 2)
		{
			for(int i = 0; i<UnlockedItems.Length; i++)
			{
					UnlockedItems[i] = true;
			}
		}
		else
		{
			string unlocked = PlayerPrefs.GetString(PlayerPrefsName,"");
			if(unlocked!="")
			{
				string[] unlockedS = unlocked.Split( new char [] {';'}, System.StringSplitOptions.RemoveEmptyEntries );
				for(int i = 0; i<UnlockedItems.Length; i++)
				{
					if(i<unlocked.Length)
					{
						UnlockedItems[i] = (unlocked.Substring(i,1) == "1") ;
					}
					else break;
				}
			}
		}
	}

	public void SetUnlockedItems()
	{
		string s = "";
		for(int i = 0; i<UnlockedItems.Length; i++)
		{
			s += UnlockedItems[i] ? "1":"0";
		}
		PlayerPrefs.SetString(PlayerPrefsName,s);
	}

}
