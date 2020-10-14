using UnityEngine;
using System.Collections;
using UnityEngine.UI;
 

/**
  * Scene:Splash
  * Object:Main Camera
  * Description: F-ja zaduzena za ucitavanje MainScene-e, kao i vizuelni prikaz inicijalizaije CrossPromotion-e i ucitavanja scene
  **/
public class SplashScene : MonoBehaviour {
	
	int appStartedNumber;
	//AsyncOperation progress = null;
//	Image progressBar;
	//float myProgress=0;
	string sceneToLoad;
 
	void Start ()
	{
 
		sceneToLoad ="HomeScene";//
		if(PlayerPrefs.HasKey("appStartedNumber"))
		{
			appStartedNumber = PlayerPrefs.GetInt("appStartedNumber");
		}
		else
		{
			appStartedNumber = 0;
		}
		appStartedNumber++;
		PlayerPrefs.SetInt("appStartedNumber",appStartedNumber);
	
		StartCoroutine(LoadScene());

    }
    
    /// <summary>
    /// Coroutine koja ceka dok se ne inicijalizuje CrossPromotion, menja progres ucitavanja CrossPromotion-a, kao i progres ucitavanje scene, i taj progres se prikazuje u Update-u
    /// </summary>
    IEnumerator LoadScene()
    {
        yield return new WaitForSeconds(3f);
        Application.LoadLevel(sceneToLoad);
	}
}
