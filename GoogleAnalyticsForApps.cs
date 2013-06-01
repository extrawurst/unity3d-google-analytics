using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Google analytics for apps.
/// using the new "measurement protocol"
/// see https://developers.google.com/analytics/devguides/collection/protocol/v1/
/// 
/// Requires a Google Analytics Account and an app profile for the tracking.
/// see https://support.google.com/analytics/answer/2614741?hl=en
/// </summary>
public class GoogleAnalyticsForApps
{
	private string m_version;
	private string m_appname;
	private string m_profileId;
	
	private string m_customerId;
	private string m_screenResolution;
	private string m_viewportSize;
	private string m_userLanguage;
	
	private Dictionary<int,int> m_customMetrics = new Dictionary<int, int>();
	private Dictionary<int,string> m_customDimensions = new Dictionary<int, string>();
	
	private static readonly string m_googleURL = "http://www.google-analytics.com/collect";
	
	private MonoBehaviour m_mono;
	
	public GoogleAnalyticsForApps (string _profileId, string _appname,string _version,MonoBehaviour _mono)
	{	
		m_profileId = _profileId;
		m_appname = _appname;
		m_version = _version;
		m_mono = _mono;
		
		m_customerId = SystemInfo.deviceUniqueIdentifier;
		
		//note: this happens on BB10 for example when the "Device ID" flag was not set in the player
		if(m_customerId.Length == 0){
			string pseudoRandom = (Time.time*1000.0f).ToString() + (new System.Random()).Next();
			
			m_customerId = "rnd."+pseudoRandom.GetHashCode().ToString();
			
			//Debug.Log ("pseudo random customerID: "+m_customerId);
		}
		
		//Debug.Log ("customerID: "+m_customerId);
		
		m_screenResolution = Screen.width+"x"+Screen.height;
		m_viewportSize = Screen.currentResolution.width+"x"+Screen.currentResolution.height;
		m_userLanguage = Application.systemLanguage.ToString();
	}
	
	/// <summary>
	/// Sets the customer identifier by hand. Default is using SystemInfo.deviceUniqueIdentifier as the customerId
	/// </summary>
	/// <param name='_customerId'>
	/// _customer identifier.
	/// </param>
	public void SetCustomerId(string _customerId)
	{
		m_customerId = _customerId;
	}
	
	public void SetVersion(string _version){
		m_version = _version;
	}
	
	/// <summary>
	/// Tracks an event.
	/// 
	/// for detailed parameter desc:
	/// https://developers.google.com/analytics/devguides/collection/protocol/v1/parameters#events
	/// </param>
	public void TrackEvent(string _category,string _label,string _action, int _value){
		WWWForm postParams = new WWWForm();
		
		AddDefaultFields(ref postParams);
		
		postParams.AddField("t","event");
		postParams.AddField("ec",_category);
		postParams.AddField("el",_label);
		postParams.AddField("ea",_action);
		postParams.AddField("ev",_value);
		
		MakeRequest(postParams);
	}
	
	public void TrackSession(bool _start){
		
		WWWForm postParams = new WWWForm();
		
		AddDefaultFields(ref postParams);
		
		if(_start)
			postParams.AddField("sc","start");
		else
			postParams.AddField("sc","end");
		
		MakeRequest(postParams);
	}
	
	public void TrackAppview(string _contentDescription){
		
		WWWForm postParams = new WWWForm();
		
		AddDefaultFields(ref postParams);
		
		postParams.AddField("t","appview");
		postParams.AddField("cd",_contentDescription);
		
		MakeRequest(postParams);
	}
	
	// not working correctly: this appears as a separat user
	public void TrackTiming(string _category,string _userTimingVarName,string _timingLabel,int _timeInMS){
		
		WWWForm postParams = new WWWForm();
		
		AddDefaultFields(ref postParams);
		
		postParams.AddField("t","timing");
		postParams.AddField("utc",_category);
		postParams.AddField("utv",_userTimingVarName);
		postParams.AddField("utl",_timingLabel);
		postParams.AddField("utt",_timeInMS);
		
		MakeRequest(postParams);
	}
	
	public void SetCustomMetric(int _metricId,int _value){
		
		m_customMetrics[_metricId] = _value;
	}
	
	public void SetCustomDimension(int _dimensionId,string _value){
		
		if(_value.Length > 150)
			throw new Exception("dimension value too long, limit is 150 bytes");
		
		m_customDimensions[_dimensionId] = _value;
	}
	
	private void AddDefaultFields(ref WWWForm _params){
		_params.AddField("v",1);
		_params.AddField("tid",m_profileId);
		_params.AddField("an",m_appname);
		_params.AddField("av",m_version);
		_params.AddField("cid",m_customerId);
		_params.AddField("sr",m_screenResolution);
		_params.AddField("vp",m_viewportSize);
		_params.AddField("ul",m_userLanguage);
		
		foreach(KeyValuePair<int,int> keyVal in m_customMetrics){
			_params.AddField("cm"+keyVal.Key.ToString(),keyVal.Value);
		}
		m_customMetrics.Clear();
		
		foreach(KeyValuePair<int,string> keyVal in m_customDimensions){
			_params.AddField("cd"+keyVal.Key.ToString(),keyVal.Value);
		}
		m_customDimensions.Clear();
	}
	
	private void MakeRequest(WWWForm _params) {
		WWW www = new WWW(m_googleURL,_params);
				
        m_mono.StartCoroutine(WaitForRequest(www));
	}
	
	private IEnumerator WaitForRequest(WWW www)
    {	
        yield return www;
 
        // check for errors
        if (www.error != null)
        {
            Debug.Log("WWW Error: "+ www.error);
		}
    }	
}