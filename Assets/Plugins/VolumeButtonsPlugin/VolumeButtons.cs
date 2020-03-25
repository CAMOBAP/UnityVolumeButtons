using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using UnityEngine;

public class VolumeButtons : MonoBehaviour
{
    #if !UNITY_ANDROID && !UNITY_IOS
	private readonly RuntimePlatform[] SupportedPlatforms = new RuntimePlatform[] { RuntimePlatform.Android, RuntimePlatform.IPhonePlayer};
    private readonly string UnsupportedError = "Unsupported platform '{0}'! At the moment we support only {1}";
    #endif

    public VolumeButtonsEvent OnVolumeButtonEvent;

    //[Range(0.0f, 1.0f)] // -1.0f in case of error or not supported
    public float SystemVolumeLevel
    {
        get
        {
            #if UNITY_ANDROID
                return _CallActivityFloat("getSystemVolumeLevel");
            #elif UNITY_IOS
                return _GetSystemVolumeLevel();
            #else
                Debug.LogFormat(UnsupportedError, Application.platform, string.Join("", SupportedPlatforms));
                return -1.0f;
            #endif
        }
    }

    void Start()
    {
		#if UNITY_ANDROID
	        _CallActivityVoid("addGameObjectListener", gameObject.name);
    	#elif UNITY_IOS
	        _AddGameObjectListener(gameObject.name, gameObject.name.Length);
    	#else
			Debug.LogFormat(UnsupportedError, Application.platform, string.Join("", SupportedPlatforms));
    	#endif
    }
 
    void OnDisable()
    {
		#if UNITY_ANDROID
	        _CallActivityVoid("removeGameObjectListener", gameObject.name);
    	#elif UNITY_IOS
	        _RemoveGameObjectListener(gameObject.name, gameObject.name.Length);
    	#else
			Debug.LogFormat(UnsupportedError, Application.platform, string.Join("", SupportedPlatforms));
    	#endif
    }

    private void _OnVolumeButtonEvent(string value)
    {
    	if (OnVolumeButtonEvent != null)
    	{
            #if DEBUG
            Debug.LogFormat("Volume event {0} delivered!", value);
            #endif
    		this.OnVolumeButtonEvent.Invoke((VolumeButtonsEventType)Int32.Parse(value), this);
    	}
    	else
    	{
    		Debug.Log("No event subscribed!");
    	}
    }

#if UNITY_ANDROID

    private float _CallActivityFloat(string methodName)
    {
        using (AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        { 
        	using (AndroidJavaObject activity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                #if DEBUG
        		Debug.LogFormat("Call {0}()", methodName);
                #endif
	            return activity.Call<float>(methodName); 
	        }
        } 
    }

    private void _CallActivityVoid(string methodName, string args0)
    {
        using (AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        { 
            using (AndroidJavaObject activity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                #if DEBUG
                Debug.LogFormat("Call {0}({1})", methodName, args0);
                #endif
                activity.Call(methodName, args0); 
            }
        } 
    }

#elif UNITY_IOS

    [DllImport("__Internal", EntryPoint="VBP_addGameObjectListener")]
    private static extern void _AddGameObjectListener([MarshalAs(UnmanagedType.LPWStr)]string gameObjectName, int gameObjectNameLen);
    [DllImport("__Internal", EntryPoint="VBP_removeGameObjectListener")]
    private static extern void _RemoveGameObjectListener([MarshalAs(UnmanagedType.LPWStr)]string gameObjectName, int gameObjectNameLen);
    [DllImport("__Internal", EntryPoint="VBP_getSystemVolumeLevel")]
    private static extern float _GetSystemVolumeLevel();
    
#endif
}