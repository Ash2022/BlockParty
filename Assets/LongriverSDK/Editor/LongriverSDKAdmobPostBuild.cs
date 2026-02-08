using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

public class LongriverSDKAdmobPostBuild
{
#if UNITY_ANDROID

#elif (UNITY_IOS || UNITY_IPHONE)
    static private string AppLovinSDK = "AppLovinSDK";
    static private string GoogleMobileAdsMediationAppLovin = "GoogleMobileAdsMediationAppLovin";

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

        string appLovinAdapterVersion = LongriverSDKCommonPostBuild.FindLibraryVersionForDependencies(GoogleMobileAdsMediationAppLovin);
        Debug.Log($"Find AppLovin Adapter Version for Dependencies.xml: {appLovinAdapterVersion}");
        if (!string.IsNullOrWhiteSpace(appLovinAdapterVersion))
        {
            string[] versionSplits = appLovinAdapterVersion.Split(".");
            if (versionSplits.Length >= 3) 
            {
                string appLovinVersion = $"{versionSplits[0]}.{versionSplits[1]}.{versionSplits[2]}";
                Debug.Log($"Find AppLovin Version for Dependencies.xml: {appLovinVersion}");
                // Add .framework as Embed & Sign
                string frameworkPath = $"Pods/{AppLovinSDK}/applovin-ios-sdk-{appLovinVersion}/{AppLovinSDK}.xcframework";
                LongriverSDKCommonPostBuild.AddFrameworksAsEmbedAndSign(exportProjectPath, "Unity-iPhone", new List<string>() {frameworkPath});
            }
        }
    }
#endif
}

