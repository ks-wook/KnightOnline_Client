using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_EventHandler : MonoBehaviour, IPointerClickHandler, IDragHandler
{
	public Action<PointerEventData> OnClickHandler = null;
	public Action<PointerEventData> OnDragHandler = null;

	public Action<PointerEventData, string> OnClickHandlerWithName = null;
	public Action<PointerEventData, string> OnDragHandlerWithName = null;

	public void OnPointerClick(PointerEventData eventData)
	{

		Managers.Sound.Play("UI/UIClick");

		if (OnClickHandler != null)
			OnClickHandler.Invoke(eventData);

		if (OnClickHandlerWithName != null)
			OnClickHandlerWithName.Invoke(eventData, gameObject.name);
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (OnDragHandler != null)
			OnDragHandler.Invoke(eventData);
	}
}
