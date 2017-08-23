using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;
using System;

public class FBManager : PluginSingleton<FBManager> {

	[Serializable]
	public class FBInfo
	{
		public string link = "";
		public string name = "";
		public string description = "";
	}

	public FBInfo FB_Android = new FBInfo();
	public FBInfo FB_IOS = new FBInfo();
	private string fbname = "";

	protected void Awake()
	{
		DontDestroyOnLoad(gameObject);
		FB.Init(InitCompleteCallback, UnityCallbackDelegate);
	}

	public void Login()
	{
		if (!FB.IsLoggedIn)
		{
			FB.LogInWithReadPermissions(new List<string> { "public_profile", "email", "user_friends" }, LogInCallback);
		}
	}
	public void ShareLink()
	{
		if (!FB.IsLoggedIn)
		{
			Login();
		}
		else
		{
#if UNITY_ANDROID
			FB.ShareLink(new Uri(FB_Android.link), FB_Android.name, FB_Android.description, null, ShareLinkCallback);
#elif UNITY_IOS
		FB.ShareLink(new Uri(FB_IOS.link), FB_IOS.name, FB_IOS.description, null, ShareLinkCallback);
#endif
		}
	}
	public void SetupUserName()
	{
		if (!fbname.Equals(""))
		{
			return;
		}

		if (FB.IsLoggedIn)
		{
			FB.API("/me?fields=name", HttpMethod.GET, GetUserCallback);
		}
	}
	public string GetUserName()
	{
		return this.fbname;
	}
	
	private void LogInCallback(IResult result)
	{
		if (result.Cancelled)
		{
			Debug.Log("User cancelled");
		}
		else
		{
			Debug.Log("Login successfully");
			SetupUserName();
		}
	}
	private void ShareLinkCallback(IShareResult result)
	{
		if (result.Cancelled)
		{
			Debug.Log("Share cancelled");
		}
		else
		{
			Debug.Log("Share successfully");
		}
	}
	private void InitCompleteCallback()
	{
		if (FB.IsInitialized)
		{
			Debug.Log("Succesfully initialized");
			SetupUserName();
		}
		else
		{
			Debug.Log("Init failed");
		}
	}
	private void UnityCallbackDelegate(bool isUnity)
	{
		if (isUnity)
		{
			Time.timeScale = 1f;
		}
		else
		{
			Time.timeScale = 0f;
		}
	}
	private void GetUserCallback(IGraphResult result)
	{
		if (FB.IsLoggedIn)
		{
			IDictionary<string, object> dict = result.ResultDictionary;
			fbname = dict["name"].ToString();
		}
	}
}
