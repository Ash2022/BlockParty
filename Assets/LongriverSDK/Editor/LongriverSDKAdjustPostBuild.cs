using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using NUnit.Framework;

public class LongriverSDKAdjustPostBuild
{
#if UNITY_ANDROID

#elif (UNITY_IOS || UNITY_IPHONE)
    static private string Adjust = "Adjust";
    static private string WaboCoreIOSSDK = "WaboCoreIOSSDK";

    static private Dictionary<string, string> EmbedAndSignFrameworks = new Dictionary<string, string>() {
        {"AdjustSigSdk", "Pods/AdjustSignature/AdjustSigSdk.xcframework"},
    };

    /*
        Must be between 40 and 50 to ensure that it's not overriden by Podfile generation (40) and
        that it's added before "pod install" (50).
    */
    [PostProcessBuildAttribute(45)]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string exportProjectPath)
    {
        if (buildTarget != BuildTarget.iOS) 
        {
            return;
        }

        string profilePath = $"{exportProjectPath}/Podfile";
        Debug.Log($"LongriverSDKAdjustPostBuild - profilePath: {profilePath} - {System.IO.File.Exists(profilePath)}");
        HashSet<string> set = new HashSet<string>() {WaboCoreIOSSDK, Adjust};
        Dictionary<string, bool> result = LongriverSDKCommonPostBuild.CheckFrameworkForProfile(profilePath, set);

        // Add .framework as Embed & Sign
        if (null != result && result.Count == set.Count)
        {
            List<string> frameworkPathList = new List<string>();
            foreach (var item in EmbedAndSignFrameworks)
            {
                frameworkPathList.Add(item.Value);
            }

            if (result[WaboCoreIOSSDK])
            {
                string version = LongriverSDKCommonPostBuild.FindVersionForProfile(profilePath, WaboCoreIOSSDK);
                Debug.Log($"LongriverSDKAdjustPostBuild - version: {WaboCoreIOSSDK} - {version}");
                if (LongriverSDKCommonPostBuild.CompareVersions(version, "0.1.56") >= 0) 
                {
                    LongriverSDKCommonPostBuild.AddFrameworksAsEmbedAndSign(exportProjectPath, "Unity-iPhone", frameworkPathList);
                }
            }
            else if (result[Adjust])
            {
                string version = LongriverSDKCommonPostBuild.FindVersionForProfile(profilePath, Adjust);
                Debug.Log($"LongriverSDKAdjustPostBuild - version: {Adjust} - {version}");
                if (LongriverSDKCommonPostBuild.CompareVersions(version, "5.0.0") >= 0) 
                {
                    LongriverSDKCommonPostBuild.AddFrameworksAsEmbedAndSign(exportProjectPath, "Unity-iPhone", frameworkPathList);
                }
            }
        }
        else
        {
            Debug.LogError($"LongriverSDKAdjustPostBuild - no find {set}");
        }
    }
#endif
}

