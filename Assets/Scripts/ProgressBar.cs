using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ProgressBar : MonoBehaviour {


	public Image ProgressBarFill;
	RectTransform rtBar;

	public float speed = .4f;
	 
	float LastValue = 0; //0-1
	public float  Value = 0;
 
	void Awake () {
		ProgressBarFill.fillAmount = Value;
		 
		SetProgress (0,false);

		//SetProgress(.5f,true);
	}

	/// <summary>
	/// Sets the progress.
	/// </summary>
	/// <param name="value">Value (0-1).</param>
	///  <param name="bSmoothChange">postepeno povecavanje ako je true</param>
	public void SetProgress(float value, bool bSmoothChange)
	{
		LastValue = ProgressBarFill.fillAmount;
		if(value>1) value = 1;
		if(value<0) value = 0; 

		 Value = value;
 
		if( bSmoothChange  )
		{
			StopCoroutine("SmoothUpdateProgres");
			StartCoroutine("SmoothUpdateProgres");
		}
		else
			ProgressBarFill.fillAmount = Value;
 
	}
 
	IEnumerator SmoothUpdateProgres () 
	{
		if(ProgressBarFill.fillAmount < Value)
		{
			while(ProgressBarFill.fillAmount < Value)
			{
				yield return new WaitForFixedUpdate();
				ProgressBarFill.fillAmount +=Time.fixedDeltaTime * speed;
			}
		}
		else
		{
			while(ProgressBarFill.fillAmount > Value)
			{
				yield return new WaitForFixedUpdate();
				ProgressBarFill.fillAmount -=Time.fixedDeltaTime * speed;
			}
		}

		ProgressBarFill.fillAmount = Value;
 
	}

 
}
