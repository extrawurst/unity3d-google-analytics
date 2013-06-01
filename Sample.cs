using UnityEngine;
using System.Collections;

public class Sample : MonoBehaviour
{
	// setup this component correctly befor. you will need a google analytics profileId for this
	public CGoogleAnalytics ga;
	
	// Use this for initialization
	void Start ()
	{
		// session starts
		ga.analytics.TrackSession(true);
		
		// tracking screens
		ga.analytics.TrackAppview("SampleScreen");
		
		// track some event
		ga.analytics.TrackEvent("eventCategory","eventLabel","eventAction",1234);
		
		// session ends
		ga.analytics.TrackSession(false);
	}
	
	// Update is called once per frame
	void Update ()
	{
	}
}

