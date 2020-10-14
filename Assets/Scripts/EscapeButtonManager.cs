using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class EscapeButtonManager : MonoBehaviour {

	bool bDisableEsc = false;
	public static  Stack<string> EscapeButonFunctionStack = new Stack<string>();

	void Start () {
		DontDestroyOnLoad (this.gameObject);
	
	}

	void OnEnable() {
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	void OnDisable() {
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode) 
	{
		EscapeButonFunctionStack.Clear();
		bDisableEsc = false;
		//Debug.Log(scene.name);
		if(  scene.name == "HomeScene") AddEscapeButonFunction("ExitGame");
	}
	
/*	
	void OnLevelWasLoaded(int level) {

		EscapeButonFunctionStack.Clear();
		bDisableEsc = false;
		if(  Application.loadedLevelName == "HomeScene") AddEscapeButonFunction("ExitGame");
		if(  Application.loadedLevelName.StartsWith("Room"))  AddEscapeButonFunction("btnHomeClicked");
		if(  Application.loadedLevelName.StartsWith( "MiniGame") ) AddEscapeButonFunction("ButtonFinishClicked");

		//if(Application.loadedLevelName == "Map") AddEscapeButonFunction("btnBackClick");
		//if(Application.loadedLevelName == "Room") AddEscapeButonFunction("btnPauseClick");
	}
*/	

	
	public static void  AddEscapeButonFunction( string functionName, string functionParam = "")
	{
		if(functionParam != "") functionName +="*"+functionParam;
		EscapeButonFunctionStack.Push(functionName);
	}




	void Update()
	{
		if( Input.GetKeyDown(KeyCode.P) )
		{
			//Debug.Log("esc stack count "+  EscapeButonFunctionStack.Count  );
			//if(  EscapeButonFunctionStack.Count > 0 ) Debug.Log(  EscapeButonFunctionStack.Peek() );
		}
		//if(  EscapeButonFunctionStack.Count > 0 ) Debug.Log( EscapeButonFunctionStack.Count + "    " + EscapeButonFunctionStack.Peek() );
		
		if( !bDisableEsc  && Input.GetKeyDown(KeyCode.Escape) )
		{
            //if(  EscapeButonFunctionStack.Count > 0 )  Debug.Log( EscapeButonFunctionStack.Count + "    " + EscapeButonFunctionStack.Peek() );
            Debug.Log("Back pressed!");
			if(EscapeButonFunctionStack.Count>0)
			{
				bDisableEsc = true;

				if( EscapeButonFunctionStack.Peek().Contains("*") )
				{
					string[] funcAndParam = EscapeButonFunctionStack.Peek().Split('*');
					if(funcAndParam[0] == "ClosePopUpMenuEsc") 
					{
						if(GameObject.Find("Canvas/Menus")!=null)
						{
                            Debug.Log("Back pressed!");
                            GameObject.Find("Canvas/Menus").transform.parent.SendMessage(funcAndParam[0],funcAndParam[1], SendMessageOptions.DontRequireReceiver);
						}
					}
					else
					{
                        Debug.Log("Back pressed!");
                        Camera.main.SendMessage(funcAndParam[0],funcAndParam[1], SendMessageOptions.DontRequireReceiver);
						EscapeButonFunctionStack.Pop();
					}
				}
				else
				{
					if( EscapeButonFunctionStack.Count == 1 && EscapeButonFunctionStack.Peek() == "btnPauseClick" )
                    {
                        Debug.Log("Back pressed!");
                        Camera.main.SendMessage("btnPauseClick", SendMessageOptions.DontRequireReceiver); //pauza se ne uklanja iz staka ako je na prvom mestu
                    }
					else if(EscapeButonFunctionStack.Count >= 1 && EscapeButonFunctionStack.Peek() == "CloseDailyReward") 
					{
                        Debug.Log("Back pressed!");
                        GameObject.Find("PopUps/DailyReward").GetComponent <DailyRewards>().Collect();
					}
					else
                    {
                        Debug.Log("Back pressed! " + EscapeButonFunctionStack.Peek());
                        Camera.main.SendMessage(EscapeButonFunctionStack.Pop(), SendMessageOptions.DontRequireReceiver);
                    }
				}
			} 
			StartCoroutine("DisableEsc");
		}
	}
	
	IEnumerator DisableEsc()
	{
		yield return new WaitForSeconds(2);
		bDisableEsc = false;
	}


}



