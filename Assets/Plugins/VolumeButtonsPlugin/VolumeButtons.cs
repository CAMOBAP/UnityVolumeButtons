using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class VolumeButtons : MonoBehaviour
{
    #if !UNITY_ANDROID && !UNITY_IOS
	private readonly RuntimePlatform[] SupportedPlatforms = new RuntimePlatform[] { RuntimePlatform.Android, RuntimePlatform.IPhonePlayer};
    private readonly string UnsupportedError = "Unsupported platform '{0}'! At the moment we support only {1}";
    #endif

    public VolumeButtonsEvent OnVolumeButtonEvent;

    void Start()
    {
		#if UNITY_ANDROID
	        _CallActivity("addGameObjectListener", gameObject.name);
    	#elif UNITY_IOS
	        // FIXME
    	#else
			Debug.LogFormat(UnsupportedError, Application.platform, string.Join("", SupportedPlatforms));
    	#endif
    }
 
    void OnDisable()
    {
		#if UNITY_ANDROID
	        _CallActivity("removeGameObjectListener", gameObject.name);
    	#elif UNITY_IOS
	        // FIXME
    	#else
			Debug.LogFormat(UnsupportedError, Application.platform, string.Join("", SupportedPlatforms));
    	#endif
    }

    private void _OnVolumeButtonEvent(string value)
    {
    	if (OnVolumeButtonEvent != null)
    	{
    		this.OnVolumeButtonEvent.Invoke((VolumeButtonsEventType)Int32.Parse(value));
    	}
    	else
    	{
    		Debug.Log("No event subscribed!");
    	}
    }

    private void _CallActivity(string methodName, string arg0)
    {
    	AndroidJNIHelper.debug = true;
        using (AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) { 
        	using (AndroidJavaObject activity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity")) {
        		Debug.LogFormat("Call {0}({1})", methodName, arg0);
	            activity.Call(methodName, arg0); 
	        }
        } 
    }
}
