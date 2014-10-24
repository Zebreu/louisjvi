using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Emotiv;

public class EmotivTracking : MonoBehaviour {
	public bool tracking = false;
	
	EmoEngine engine;
	
	static Profile profile;
	
	static void engine_EmoEngineConnected(object sender, EmoEngineEventArgs e)
	{
		Console.WriteLine("connected");
	}
	
	static void engine_EmoEngineDisconnected(object sender, EmoEngineEventArgs e)
	{
		Console.WriteLine("disconnected");
	}
	static void engine_UserAdded(object sender, EmoEngineEventArgs e)
	{
		Console.WriteLine("user added ({0})", e.userId);
		Profile profile = EmoEngine.Instance.GetUserProfile(0);
		profile.GetBytes();
	}
	static void engine_UserRemoved(object sender, EmoEngineEventArgs e)
	{
		Console.WriteLine("user removed");
	}
	
	static void engine_EmoStateUpdated(object sender, EmoStateUpdatedEventArgs e)
	{
		EmoState es = e.emoState;
		
		Single timeFromStart = es.GetTimeFromStart();
		// Console.WriteLine("new emostate {0}", timeFromStart);
	}
	
	static void engine_EmoEngineEmoStateUpdated(object sender, EmoStateUpdatedEventArgs e)
	{
		EmoState es = e.emoState;
		
		Single timeFromStart = es.GetTimeFromStart();
		
		Int32 headsetOn = es.GetHeadsetOn();
		Int32 numCqChan = es.GetNumContactQualityChannels();            
		EdkDll.EE_EEG_ContactQuality_t[] cq = es.GetContactQualityFromAllChannels();
		for (Int32 i = 0; i < numCqChan; ++i)
		{
			if (cq[i] != es.GetContactQuality(i))
			{
				throw new Exception();
			}
		}
		EdkDll.EE_SignalStrength_t signalStrength = es.GetWirelessSignalStatus();
		Int32 chargeLevel = 0;
		Int32 maxChargeLevel = 0;
		es.GetBatteryChargeLevel(out chargeLevel, out maxChargeLevel);
		
		/*
		engineLog.Write(
			"{0},{1},{2},{3},{4},",
			timeFromStart,
			headsetOn, signalStrength, chargeLevel, maxChargeLevel);
		
		for (int i = 0; i < cq.Length; ++i)
		{
			engineLog.Write("{0},", cq[i]);
		}
		engineLog.WriteLine("");
		engineLog.Flush();
		*/
	}      
	
	static void engine_AffectivEmoStateUpdated(object sender, EmoStateUpdatedEventArgs e)
	{
		EmoState es = e.emoState;
		
		Single timeFromStart = es.GetTimeFromStart();
		
		EdkDll.EE_AffectivAlgo_t[] affAlgoList = { 
			EdkDll.EE_AffectivAlgo_t.AFF_ENGAGEMENT_BOREDOM,
			EdkDll.EE_AffectivAlgo_t.AFF_EXCITEMENT,
			EdkDll.EE_AffectivAlgo_t.AFF_FRUSTRATION,
			EdkDll.EE_AffectivAlgo_t.AFF_MEDITATION,
		};
		
		Boolean[] isAffActiveList = new Boolean[affAlgoList.Length];
		
		Single longTermExcitementScore = es.AffectivGetExcitementLongTermScore();
		Single shortTermExcitementScore = es.AffectivGetExcitementShortTermScore();
		for (int i = 0; i < affAlgoList.Length; ++i)
		{
			isAffActiveList[i] = es.AffectivIsActive(affAlgoList[i]);
		}
		Single meditationScore = es.AffectivGetMeditationScore();
		Single frustrationScore = es.AffectivGetFrustrationScore();
		Single boredomScore = es.AffectivGetEngagementBoredomScore();
		
		double rawScoreEc = 0, rawScoreMd = 0, rawScoreFt = 0, rawScoreEg = 0;
		double minScaleEc = 0, minScaleMd = 0, minScaleFt = 0, minScaleEg = 0;
		double maxScaleEc = 0, maxScaleMd = 0, maxScaleFt = 0, maxScaleEg = 0;
		double scaledScoreEc = 0, scaledScoreMd = 0, scaledScoreFt = 0, scaledScoreEg = 0;
		
		es.AffectivGetExcitementShortTermModelParams(out rawScoreEc, out minScaleEc, out maxScaleEc);
		if (minScaleEc != maxScaleEc)
		{
			if (rawScoreEc < minScaleEc)
			{
				scaledScoreEc = 0;
			}
			else if (rawScoreEc > maxScaleEc)
			{
				scaledScoreEc = 1;
			}
			else
			{
				scaledScoreEc = (rawScoreEc - minScaleEc) / (maxScaleEc - minScaleEc);
			}
			Console.WriteLine("Affectiv Short Excitement: Raw Score {0:f5} Min Scale {1:f5} max Scale {2:f5} Scaled Score {3:f5}\n", rawScoreEc, minScaleEc, maxScaleEc, scaledScoreEc);
		}
		
		es.AffectivGetEngagementBoredomModelParams(out rawScoreEg, out minScaleEg, out maxScaleEg);
		if (minScaleEg != maxScaleEg)
		{
			if (rawScoreEg < minScaleEg)
			{
				scaledScoreEg = 0;
			}
			else if (rawScoreEg > maxScaleEg)
			{
				scaledScoreEg = 1;
			}
			else
			{
				scaledScoreEg = (rawScoreEg - minScaleEg) / (maxScaleEg - minScaleEg);
			}
			Console.WriteLine("Affectiv Engagement : Raw Score {0:f5}  Min Scale {1:f5} max Scale {2:f5} Scaled Score {3:f5}\n", rawScoreEg, minScaleEg, maxScaleEg, scaledScoreEg);
		}
		es.AffectivGetMeditationModelParams(out rawScoreMd, out minScaleMd, out maxScaleMd);
		if (minScaleMd != maxScaleMd)
		{
			if (rawScoreMd < minScaleMd)
			{
				scaledScoreMd = 0;
			}
			else if (rawScoreMd > maxScaleMd)
			{
				scaledScoreMd = 1;
			}
			else
			{
				scaledScoreMd = (rawScoreMd - minScaleMd) / (maxScaleMd - minScaleMd);
			}
			Console.WriteLine("Affectiv Meditation : Raw Score {0:f5} Min Scale {1:f5} max Scale {2:f5} Scaled Score {3:f5}\n", rawScoreMd, minScaleMd, maxScaleMd, scaledScoreMd);
		}
		es.AffectivGetFrustrationModelParams(out rawScoreFt, out minScaleFt, out maxScaleFt);
		if (maxScaleFt != minScaleFt)
		{
			if (rawScoreFt < minScaleFt)
			{
				scaledScoreFt = 0;
			}
			else if (rawScoreFt > maxScaleFt)
			{
				scaledScoreFt = 1;
			}
			else
			{
				scaledScoreFt = (rawScoreFt - minScaleFt) / (maxScaleFt - minScaleFt);
			}
			Console.WriteLine("Affectiv Frustation : Raw Score {0:f5} Min Scale {1:f5} max Scale {2:f5} Scaled Score {3:f5}\n", rawScoreFt, minScaleFt, maxScaleFt, scaledScoreFt);
		}
		
		Debug.Log ("Long term excitation: "+longTermExcitementScore.ToString());
		Debug.Log ("Frustration score: "+frustrationScore.ToString());
		/*
		affLog.Write(
			"{0},{1},{2},{3},{4},{5},",
			timeFromStart,
			longTermExcitementScore, shortTermExcitementScore, meditationScore, frustrationScore, boredomScore);
		
		for (int i = 0; i < affAlgoList.Length; ++i)
		{
			affLog.Write("{0},", isAffActiveList[i]);
		}
		affLog.WriteLine("");
		affLog.Flush();
		*/
	}
	
	// Use this for initialization
	void Start () {
		if (tracking)
		{
			engine = EmoEngine.Instance;
			
			engine.EmoEngineConnected += 
				new EmoEngine.EmoEngineConnectedEventHandler(engine_EmoEngineConnected);
			engine.EmoEngineDisconnected += 
				new EmoEngine.EmoEngineDisconnectedEventHandler(engine_EmoEngineDisconnected);
			engine.UserAdded += 
				new EmoEngine.UserAddedEventHandler(engine_UserAdded);
			engine.UserRemoved += 
				new EmoEngine.UserRemovedEventHandler(engine_UserRemoved);
			engine.EmoStateUpdated += 
				new EmoEngine.EmoStateUpdatedEventHandler(engine_EmoStateUpdated);
				
			engine.EmoEngineEmoStateUpdated += 
				new EmoEngine.EmoEngineEmoStateUpdatedEventHandler(engine_EmoEngineEmoStateUpdated);
			
			engine.AffectivEmoStateUpdated += 
				new EmoEngine.AffectivEmoStateUpdatedEventHandler(engine_AffectivEmoStateUpdated);
			
			engine.RemoteConnect("127.0.0.1",3008);	
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (tracking)
		{
			engine.ProcessEvents(1000);
		}
	}
	
	void OnApplicationQuit()
	{
		engine.Disconnect();
	}
}
