using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class Blink : MonoBehaviour
{
	private readonly float Delay = 0.05f;

	public VolumeButtonsEventType InterestingEvent;

	public void DoBlink(VolumeButtonsEventType e)
	{
		if (InterestingEvent == e)
		{
    		Debug.LogFormat("DoBlink for {0} event", InterestingEvent.ToString());
			StartCoroutine(Flash(2));
		}
	}

    IEnumerator Flash(int flashCount)
	{
		Image image = gameObject.GetComponent<Image>();

	    for (int i = 0; i < flashCount; i++)
	    {
	        image.color = Color.white;
	        yield return new WaitForSeconds(Delay);
	        image.color = Color.clear;
	        yield return new WaitForSeconds(Delay);
	    }
	}
}
