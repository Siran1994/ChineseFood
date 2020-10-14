using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;


public class HomeScene : MonoBehaviour 
{
	public Image SoundOff;
	public Image SoundOn;
 
	public MenuManager menuManager;
 
	void Awake()
	{	
		Input.multiTouchEnabled = false;
	}

	IEnumerator Start () 
	{
	
		if(SoundManager.soundOn == 1)
		{
			SoundOff.enabled = false;
			SoundOn.enabled = true;
		}
		else
		{
			SoundOff.enabled = true;
			SoundOn.enabled = false;
		}
		yield return new WaitForSeconds(1);
	}

 
	public void btnSoundClicked()
	{
		if(SoundManager.soundOn == 1)
		{
			SoundOff.enabled = true;
			 SoundOn.enabled = false;
			SoundManager.soundOn = 0;
			SoundManager.Instance.MuteAllSounds();
		}
		else
		{
			SoundOff.enabled = false;
			 SoundOn.enabled = true;
			SoundManager.soundOn = 1;
			SoundManager.Instance.UnmuteAllSounds();
			if(SoundManager.Instance!=null) SoundManager.Instance.Play_ButtonClick();
		}
			
		if(SoundManager.musicOn == 1)
		{
			SoundManager.Instance.Stop_Music();
			SoundManager.musicOn = 0;
		}
		else
		{
			SoundManager.musicOn = 1;
			SoundManager.Instance.Play_Music();
		}
		PlayerPrefs.SetInt("SoundOn",SoundManager.soundOn);
		PlayerPrefs.SetInt("MusicOn",SoundManager.musicOn);
		PlayerPrefs.Save();
	}

   
	public void btnPlayClick( )
	{
        if (SoundManager.Instance!=null) SoundManager.Instance.Play_ButtonClick();
        SceneManager.LoadScene("SelectMiniGame");
    }
}
