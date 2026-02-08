using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LongriverSDKNS
{
	public class LongriverSDKBridgeFactory {
		static public LongriverSDKBridge instance = newBridge();
		static private LongriverSDKBridge newBridge()
        {

#if UNITY_EDITOR
            return new LongriverSDKBridgeEditor();
#elif UNITY_ANDROID
            return new LongriverSDKBridgeAndroid();
#elif UNITY_IOS
			return new LongriverSDKBridgeIOS();
#else
			return new LongriverSDKBridgeEditor();
#endif

		}
	}
}

