using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class Tutorial : MonoBehaviour {

	public bool bTutorialFinished;

	public Animator animTutorial;
	public Animator animTutorialHolder;
 
	 int phase = -1;
	
	public Transform[] tutStartPos;
	public Transform[] tutEndPos;
	
	bool bActive = false;
 
	float repeatTime = 3;
	float lefttTimeToRepeat = 0;
	string lastTutorial = "";
	Vector3 RepStartPosition;

	public static Tutorial Instance;

	void Awake()
	{
		Instance = this;
	}

	public void ShowPointer()
	{
		animTutorialHolder.CrossFade("ShowTutorial",.05f);
		 
	}

	public void HidePointer()
	{
		 if(animTutorialHolder.GetComponent<CanvasGroup>().alpha>0.05f)
			animTutorialHolder.CrossFade("HideTutorial",.05f); 
	}


	public void ShowPointerAndMoveToPosition(int  tutorialPhase)
	{
		phase = tutorialPhase;
		StopAllCoroutines();
		CancelInvoke("RepeatTutorial");
		lastTutorial = "";
		StartCoroutine("CShowPointerAndMoveToPosition", 0);
	}

	public void ShowPointerAndMoveToPosition(int  tutorialPhase, float delay)
	{
		phase = tutorialPhase;
		StopAllCoroutines();
		CancelInvoke("RepeatTutorial");
		lastTutorial = "";
		StartCoroutine("CShowPointerAndMoveToPosition", delay);
	}
	
	IEnumerator CShowPointerAndMoveToPosition( float delay)
	{
		if(bActive )
		{
			HidePointer();
			yield return new WaitForSeconds(0.1f);
		}

		yield return new WaitForSeconds(0.1f);
		animTutorial.Play("default");
		 

		yield return new WaitForSeconds(delay);
		animTutorial.transform.localScale =  tutStartPos[phase].localScale;
		animTutorial.transform.position =  tutStartPos[phase].position;
		bActive = true;
		ShowPointer();
		yield return new WaitForSeconds(0.2f);

		animTutorial.CrossFade("pointerDown",.05f);
		yield return new WaitForSeconds(1f);
	 
		float speed = 1.3f;//Vector3.Magnitude(tutEndPos[phase].position - tutStartPos[phase].position) *.2f;
		float timeMove = 0;
		while(timeMove<1)
		{
			timeMove+=speed * Time.fixedDeltaTime;
			animTutorial.transform.position = Vector3.Lerp(         tutStartPos[phase].position,  tutEndPos[phase].position, timeMove );
			yield return new WaitForFixedUpdate();
		}
		animTutorial.transform.position =  tutEndPos[phase].position;
		 
 	yield return new WaitForSeconds(.3f);
//		animTutorial.CrossFade("pointerUp",.05f);
//		yield return new WaitForSeconds(1f);
		HidePointer();
		yield return new WaitForSeconds(.2f);
		bActive = false;
		lastTutorial = "MoveToPosition";
		InvokeRepeating("RepeatTutorial",.5f,.5f);
	}


	public void ShowPointerAndTapOnPosition(int  tutorialPhase)
	{
		phase = tutorialPhase;
		StopAllCoroutines();
		CancelInvoke("RepeatTutorial");
		lastTutorial = "";
		StartCoroutine("CShowPointerAndTapOnPosition", 0);
	}

	public void ShowPointerAndTapOnPosition(int  tutorialPhase, float delay)
	{
		phase = tutorialPhase;
		StopAllCoroutines();
		CancelInvoke("RepeatTutorial");
		lastTutorial = "";
		StartCoroutine("CShowPointerAndTapOnPosition", delay);
	}

	IEnumerator CShowPointerAndTapOnPosition(float delay)
	{
		if(bActive )
		{
			HidePointer();
			yield return new WaitForSeconds(0.1f);
		}

		yield return new WaitForSeconds(0.1f);
		animTutorial.Play("default");

		 
		yield return new WaitForSeconds(delay);
		animTutorial.transform.localScale =  tutStartPos[phase].localScale;
		animTutorial.transform.position =  tutStartPos[phase].position;
		bActive = true;
		ShowPointer();
		yield return new WaitForSeconds(0.8f);
		
		animTutorial.CrossFade("pointerTap",.05f);
		yield return new WaitForSeconds(1f);

		HidePointer();
		yield return new WaitForSeconds(.2f);
		bActive = false;
		lastTutorial = "TapOnPosition";
		InvokeRepeating("RepeatTutorial",.5f,.5f);
	}



	public void ShowPointerAndMoveRepeating(int  tutorialPhase, float dly)
	{
		phase = tutorialPhase;
		StopAllCoroutines();
		CancelInvoke("RepeatTutorial");
		lastTutorial = "";
		StartCoroutine( CShowPointerAndMoveRepeating(tutStartPos[phase].position, dly));
	}

	public void ShowPointerAndMoveRepeating(int  tutorialPhase, Vector3 StartPosition, float dly)
	{
		phase = tutorialPhase;
		StopAllCoroutines();
		CancelInvoke("RepeatTutorial");
		lastTutorial = "";
		StartCoroutine( CShowPointerAndMoveRepeating( StartPosition, dly));
	}
	
	IEnumerator CShowPointerAndMoveRepeating(      Vector3 StartPosition , float delay)
	{
		Vector3 _StartPosition = StartPosition;
		if(bActive )
		{
			HidePointer();
			yield return new WaitForSeconds(0.1f);
		}

		yield return new WaitForSeconds(0.1f);
		animTutorial.Play("default");
	 
		yield return new WaitForSeconds(delay);
		animTutorial.transform.localScale =  tutStartPos[phase].localScale;
		animTutorial.transform.position =  StartPosition;
		bActive = true;
		ShowPointer();
		yield return new WaitForSeconds(0.2f);
		
		animTutorial.CrossFade("pointerDown",.05f);
		yield return new WaitForSeconds(1f);
		
		float speed = 1.3f;//Vector3.Magnitude(tutEndPos[phase].position - tutStartPos[phase].position) *.2f;
		int  repeatCycles = 2;

		for(int i = 0; i < repeatCycles; i++)
		{
			float timeMove = 0;
			while(timeMove<1)
			{
				timeMove+=speed * Time.fixedDeltaTime;
				animTutorial.transform.position = Vector3.Lerp(    _StartPosition ,  tutEndPos[phase].position, timeMove );
				yield return new WaitForFixedUpdate();
			}
			animTutorial.transform.position =  tutEndPos[phase].position;
			_StartPosition =    tutStartPos[phase].position;
			while(timeMove > 0)
			{
				timeMove -=speed * Time.fixedDeltaTime;
				animTutorial.transform.position = Vector3.Lerp(     tutStartPos[phase].position,  tutEndPos[phase].position, timeMove );
				yield return new WaitForFixedUpdate();
			}

		}

 		yield return new WaitForSeconds(.3f);
//		animTutorial.CrossFade("pointerUp",.05f);
//		yield return new WaitForSeconds(1f);
		HidePointer();
		yield return new WaitForSeconds(.2f);
		bActive = false;
		lastTutorial = "MoveRepeating";
		RepStartPosition = StartPosition;
		InvokeRepeating("RepeatTutorial",.5f,.5f);
	}

	 

	//===============================================

	int MidPointsCount = 0;
	//ovo je za slucajeve kada treba da se pokze na vise pozicia
	public void ShowPointerAndMoveToPosition2(int  tutorialPhase, int midPointsCount)
	{
		MidPointsCount  =midPointsCount;
		phase = tutorialPhase;
		StopAllCoroutines();
		CancelInvoke("RepeatTutorial");
		lastTutorial = "";
		StartCoroutine("CShowPointerAndMoveToPosition2", 0);
	}

	public void ShowPointerAndMoveToPosition2(int  tutorialPhase, float delay, int midPointsCount)
	{
		MidPointsCount  = midPointsCount;
		phase = tutorialPhase;
		StopAllCoroutines();
		CancelInvoke("RepeatTutorial");
		lastTutorial = "";
		StartCoroutine("CShowPointerAndMoveToPosition2", delay );
	}

	IEnumerator CShowPointerAndMoveToPosition2(float delay)
	{
		if(bActive )
		{
			HidePointer();
			yield return new WaitForSeconds(0.1f);
		}

		yield return new WaitForSeconds(0.1f);
		animTutorial.Play("default");


		yield return new WaitForSeconds(delay);
		animTutorial.transform.localScale =  tutStartPos[phase].localScale;
		animTutorial.transform.position =  tutStartPos[phase].position;
		bActive = true;
		ShowPointer();
		yield return new WaitForSeconds(0.2f);

		animTutorial.CrossFade("pointerDown",.05f);
		yield return new WaitForSeconds(1f);

		float speed = 1.3f;//Vector3.Magnitude(tutEndPos[phase].position - tutStartPos[phase].position) *.2f;
		float timeMove = 0;
		int c =0;
		while(c<MidPointsCount)
		{
			timeMove = 0;
			while(timeMove<1)
			{
				timeMove+=speed * Time.fixedDeltaTime;
				animTutorial.transform.position = Vector3.Lerp( tutStartPos[phase+ c].position,  tutStartPos[phase+ c+1].position, timeMove );
				yield return new WaitForFixedUpdate();
			}
			c++;
		}

		//POSLEDNJA TACKA
		timeMove = 0;
		while(timeMove<1)
		{
			timeMove+=speed * Time.fixedDeltaTime;
			animTutorial.transform.position = Vector3.Lerp( tutStartPos[phase + MidPointsCount].position,  tutEndPos[phase].position, timeMove );
			yield return new WaitForFixedUpdate();
		}
		animTutorial.transform.position =  tutEndPos[phase].position;

		yield return new WaitForSeconds(.3f);
		//		animTutorial.CrossFade("pointerUp",.05f);
		//		yield return new WaitForSeconds(1f);
		HidePointer();
		yield return new WaitForSeconds(.2f);
		bActive = false;
		lastTutorial = "MoveToPosition2";
		InvokeRepeating("RepeatTutorial",.5f,.5f);
	}


	//=============
 
	public void RepeatTutorial()
	{
		if(lefttTimeToRepeat< repeatTime)
		{
			lefttTimeToRepeat+=0.5f;
		}
		 else
		{
			lefttTimeToRepeat = 0;
			if(lastTutorial == "MoveToPosition") ShowPointerAndMoveToPosition(phase);
			else if(lastTutorial == "TapOnPosition") ShowPointerAndTapOnPosition(phase);
			else if(lastTutorial == "MoveRepeating") ShowPointerAndMoveRepeating(phase, RepStartPosition,0);
			else if(lastTutorial == "MoveToPosition2") ShowPointerAndMoveToPosition2(phase, MidPointsCount);

		}
	}
		







	public void SwitchState()
	{
		if(bActive) 
		{
			StopAllCoroutines();
			HidePointer();
			
			bActive = false;
		}
		else
		{
			ShowTutorial(phase);
		}
	}

	public void ShowTutorial( int _phase)
	{
		string name = SceneManager.GetActiveScene().name;
		//Debug.Log("TUT "+ _phase);


		//----------mg1---------------------------------
		if( name == "BoilNoodles")
		{
			float dly = 2;
			if(_phase == 0)  ShowPointerAndMoveToPosition(0,dly);
			else if(_phase == 1)  ShowPointerAndMoveToPosition(1,dly+2);
			else if(_phase == 2)   ShowPointerAndTapOnPosition(2,dly);
			else if(_phase == 3)   ShowPointerAndMoveToPosition2(3, dly, 1);
			else if(_phase == 4)   ShowPointerAndTapOnPosition(5,dly);
			 
		}
		else if( name == "CookNoodles")
		{
			float dly = 2;
			if(_phase == 0)  ShowPointerAndMoveToPosition(0, dly);
			else if(_phase == 1)   ShowPointerAndTapOnPosition(1, dly+2);
			else if(_phase == 2)   ShowPointerAndTapOnPosition(2, dly);
			else if(_phase == 3)   ShowPointerAndTapOnPosition(3, dly);
			else if(_phase == 4)   ShowPointerAndMoveToPosition(4, dly);
			else if(_phase == 5)   ShowPointerAndMoveToPosition(5, dly);
			else if(_phase == 6)   ShowPointerAndMoveToPosition(6, dly);
			else if(_phase == 7)  ShowPointerAndMoveRepeating(7,dly);
		}
		else if( name == "EatNoodles")
		{
			float dly = 1;
			if(_phase == 0)  ShowPointerAndTapOnPosition(0,dly);
		}
		else if( name == "NoodlesMakingMachine")
		{
			float dly = 2;
			if(_phase == 0)  ShowPointerAndTapOnPosition(0,dly);
			if(_phase == 1)  ShowPointerAndTapOnPosition(1,dly);
			if(_phase == 2)  ShowPointerAndTapOnPosition(2,dly);
		}
		//----------mg2---------------------------------
		else if( name == "CookSweetDumplings")
		{
			float dly = 2;
			if(_phase == 0)  ShowPointerAndMoveToPosition(0, dly);
			else if(_phase == 1)   ShowPointerAndTapOnPosition(1, dly);
			else if(_phase == 2)   ShowPointerAndMoveToPosition2(2, dly, 1);
		}
		else if( name == "EatSweetDumplings")
		{
			float dly = 0;
			if(_phase == 0)  ShowPointerAndMoveToPosition2(0, dly,1);
		}
		else if( name == "MakeSweetDumplings")
		{
			float dly = 2;
			if(_phase == 0)  ShowPointerAndMoveToPosition(0, dly);
			else if(_phase == 1)   ShowPointerAndMoveRepeating(1, dly);
			else if(_phase == 2)  ShowPointerAndMoveToPosition(2, dly);
			else if(_phase == 3)   ShowPointerAndTapOnPosition(3,dly);
			 
		}

		else if( name == "SugarGlaze")
		{
			float dly = 2;
			if(_phase == 0)  ShowPointerAndMoveToPosition(0, dly);
			else if(_phase == 1)   ShowPointerAndMoveRepeating(1, dly);
		}
		else if( name == "SweetDumplingsDough")
		{
			float dly = 2;
			if(_phase == 0)  ShowPointerAndMoveToPosition(0, dly);
			else if(_phase == 1)   ShowPointerAndMoveRepeating(1, dly);
			else if(_phase == 2)  ShowPointerAndMoveToPosition(2, dly);
		}



		//----------mg3---------------------------------
 
		else if( name == "BakeFortuneCookies")
		{
			float dly = 2;
			if(_phase == 0)  ShowPointerAndMoveToPosition(0, dly);
			 
		}
		else if( name == "DecorateFortuneCookie")
		{
			float dly = 2;
			if(_phase == 0)  ShowPointerAndTapOnPosition(0, dly);
			else if(_phase == 1)  ShowPointerAndTapOnPosition(1, dly);
		}

		else if( name == "EatFortuneCookie")
		{
			float dly = 2;
			if(_phase == 0)  ShowPointerAndTapOnPosition(0, dly);
		}

		else if( name == "FortuneCookieMixIngredients")
		{
			float dly = 2;
			if(_phase == 0)  ShowPointerAndMoveToPosition(0, dly);
			else if(_phase == 1)  ShowPointerAndMoveToPosition(1,dly);
			else if(_phase == 2)  ShowPointerAndMoveToPosition(2,dly);
			else if(_phase == 3)  ShowPointerAndMoveToPosition(3,dly);
			else if(_phase == 4)  ShowPointerAndMoveToPosition(4,dly);
			else if(_phase == 5)   ShowPointerAndMoveToPosition2(5, dly, 3);
		}

		else if( name == "MakeFortuneCookies")
		{
			float dly = 2;
			if(_phase == 0)  ShowPointerAndMoveToPosition(0, dly);
			else if(_phase == 1)  ShowPointerAndMoveToPosition(1,dly);
			else if(_phase == 2)  ShowPointerAndTapOnPosition(2,dly);
		}

		else if( name == "MeltChocolate")
		{
			float dly = 2;
			if(_phase == 0)  ShowPointerAndTapOnPosition(0, dly);
			else if(_phase == 1)  ShowPointerAndTapOnPosition(1,dly);
			else if(_phase == 2)  ShowPointerAndTapOnPosition(2,dly);
			else if(_phase == 3)  ShowPointerAndMoveToPosition(3,dly);
		}

		else if( name == "StretchAndCutDough")
		{
			float dly = 2;
			if(_phase == 0)  ShowPointerAndMoveRepeating(0, dly);
			else if(_phase == 1)  ShowPointerAndMoveToPosition(1,dly);
			else if(_phase == 2)  ShowPointerAndMoveToPosition(2,dly);
		}

		else if( name == "WriteFortuneCookieMessage")
		{
			float dly = 2;
			if(_phase == 0)  ShowPointerAndMoveToPosition(0, dly);
		 
		}

		//----------mg4---------------------------------

		else if( name == "CookDimSum")
		{
			float dly = 3;
			if(_phase == 0)  ShowPointerAndMoveToPosition(0, dly);
			else if(_phase == 1)  ShowPointerAndTapOnPosition(1,dly);
			else if(_phase == 2)  ShowPointerAndMoveToPosition(2,dly);

		}
		else if( name == "DimSumMakingMachine")
		{
			float dly = 2;
			if(_phase == 0)  ShowPointerAndTapOnPosition(0,dly);
			else if(_phase == 1)  ShowPointerAndMoveToPosition(1, dly);
			else if(_phase == 2)  ShowPointerAndTapOnPosition(2,dly);
			else if(_phase == 3)  ShowPointerAndMoveToPosition2(3,dly,1);
			else if(_phase == 4)  ShowPointerAndTapOnPosition(5,dly);

		}
		else if( name == "EatDimSum")
		{
			float dly = 0;
			if(_phase == 0) ShowPointerAndMoveToPosition2(0, dly, 1);
		}
		//----------mg5---------------------------------

		else if( name == "EatSpringRolls")
		{
			float dly = 0;
			if(_phase == 0) ShowPointerAndMoveToPosition2(0, dly, 1);
		}
		else if( name == "FrySpringRolls")
		{
			float dly = 2;
			if(_phase == 0)  ShowPointerAndTapOnPosition(0,dly);
			else if(_phase == 1)  ShowPointerAndTapOnPosition(1, dly);
			else if(_phase == 2)  ShowPointerAndTapOnPosition(2,0);
			else if(_phase == 3)  ShowPointerAndMoveToPosition(3,dly);
			else if(_phase == 4)  ShowPointerAndTapOnPosition(4,dly);

		}
		else if( name == "MakeSpringRolls")
		{
			float dly = 2;
			if(_phase == 0)  ShowPointerAndTapOnPosition(0,dly);
			else if(_phase == 1)  ShowPointerAndTapOnPosition(1, dly);
			else if(_phase == 2)  ShowPointerAndTapOnPosition(2,dly);
			else if(_phase == 3)  ShowPointerAndMoveToPosition(3,dly);
			else if(_phase == 4)  ShowPointerAndTapOnPosition(4,dly);
			else if(_phase == 5)  ShowPointerAndMoveToPosition(5,dly);
		}
		else if( name == "SpringRollsBakeWrapper2")
		{
			float dly = 2;
			if(_phase == 0)  ShowPointerAndTapOnPosition(0,dly);
			else if(_phase == 1)  ShowPointerAndTapOnPosition(1, dly);
			else if(_phase == 2)  ShowPointerAndTapOnPosition(2,dly);
			else if(_phase == 3)  ShowPointerAndMoveToPosition(3,dly);
			else if(_phase == 4)  ShowPointerAndTapOnPosition(4,dly);
			else if(_phase == 5)  ShowPointerAndMoveToPosition(5,dly);
		}
		else if( name == "SpringRollsBakeWrapper")
		{
			float dly = 2;
			if(_phase == 0) ShowPointerAndMoveToPosition2(0, dly, 1);
		}
		else if( name == "SpringRollsMixIngredients")
		{
			float dly = 3;
			if(_phase == 0)  ShowPointerAndMoveToPosition(0, dly);
			else if(_phase == 1)  ShowPointerAndMoveToPosition(1,dly);
			else if(_phase == 2)  ShowPointerAndMoveToPosition(2,dly);
			else if(_phase == 3)   ShowPointerAndMoveToPosition2(3, dly, 3);
		}


	}

	public void StopTutorial( )
	{
//		Debug.Log("STOP TUT");
		CancelInvoke("RepeatTutorial");
		lastTutorial = "";

		StopAllCoroutines();
		//if(bActive)
		{
			HidePointer();
			bActive = false;
		}
	}

	public void PauseTutorial( string state )  
	{
		 
	}


 
}
