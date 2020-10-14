using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AnimationEvents : MonoBehaviour {
	Animator anim;

	public void AnimEventHideSceneAnimStarted()
	{
		//LevelTransition.Instance.AnimEventHideSceneAnimStarted();
	}
	
	public void AnimEventShowSceneAnimFinished()
	{
		//LevelTransition.Instance.AnimEventShowSceneAnimFinished();
	}


	 
 

	public void CleaningAnimationFinished()
	{
		transform.parent.SendMessage("CleaningAnimationFinished",SendMessageOptions.DontRequireReceiver);
	}
 

	public void StartParticles()
	{
		transform.GetComponentInChildren<ParticleSystem>().Play();
	}
		
 
 
	 

	 
 
	public void  PitcherStarFillingWaterToMold()
	{
//		DragItem PitcherDragItem = transform.parent.GetComponent<DragItem>();
//		PitcherDragItem.PitcherFillWater();
	}
 
	public void  PitcherFinishFillingWaterToMold()
	{
 
	}

	public void  PitcherAnimEnd()
	{
		//DragItem PitcherDragItem = transform.parent.GetComponent<DragItem>();
		//PitcherDragItem.PitcherAnimationFinished();
	}


	public void  DoughCutterAnimEnd()
	{
		transform.parent.GetComponent<DragItem>().CutDough();
	}


 


	public void TimerAnimEnd()
	{
		//Camera.main.SendMessage("OutOfTime");
	}

	public void NooldesMachineStopSound()
	{
		if(SoundManager.Instance!=null) SoundManager.Instance.Stop_Sound( SoundManager.Instance.MachineOnSound);
	}

	public void NooldesMachineAnimEnd()
	{
		Camera.main.SendMessage("NooldesMachineAnimEnd");
	}


	public void TurnJellyGumsBox1()
	{
		Camera.main.SendMessage("TurnJellyGumsBox1");
	}
	public void TurnJellyGumsBox2()
	{
		Camera.main.SendMessage("TurnJellyGumsBox2");
	}

	public void TurnJellyGumsBox3()
	{
		Camera.main.SendMessage("TurnJellyGumsBox3");
	}

	public void AnimDimSumCookingEnd()
	{
		Camera.main.SendMessage("NextPhase","cookingEnd");
	}
	 
}
