using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;

 
[AddComponentMenu("Custom Tools/CustomButton")]
public class CustomButton : Button  
{
	// Event delegate triggered on mouse or touch down.
	[SerializeField]
	ButtonDownEvent _onDown = new ButtonDownEvent();
 
    protected CustomButton() { }

	public override void OnPointerDown(PointerEventData eventData)
	{
		if(eventData.pointerDrag == null || (eventData.pointerDrag != null && !eventData.dragging) ) //ovo je zbog scroll rect
		{
			base.OnPointerDown(eventData);

			if (eventData.button != PointerEventData.InputButton.Left)
				return;

			_onDown.Invoke();
		}
	}

	public ButtonDownEvent onDown
	{
		get { return _onDown; }
		set { _onDown = value; }
	}

	[Serializable]
	public class ButtonDownEvent : UnityEvent { }

	//--------------------------------------------------------------

	public override void OnPointerUp(PointerEventData eventData)
	{
		//Debug.Log(eventData.pointerDrag);
		if(eventData.pointerDrag == null || (eventData.pointerDrag != null && !eventData.dragging) ) //ovo je zbog scroll rect
		{
			//Debug.Log("OnUP");
			base.OnPointerUp(eventData);

			if (eventData.button != PointerEventData.InputButton.Left)
				return;

			_onUp.Invoke();
		}
	}



	[SerializeField]
	ButtonUpEvent _onUp = new ButtonUpEvent();

	public ButtonUpEvent onUp
	{
		get { return _onUp; }
		set { _onUp = value; }
	}

	[Serializable]
	public class ButtonUpEvent : UnityEvent { }

	 
 
}    