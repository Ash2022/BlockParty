using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

public class LongriverSDKFacebookPostBuild
{

#if UNITY_ANDROID
#elif (UNITY_IOS || UNITY_IPHONE)
    static private string FBSDKCoreKit = "FBSDKCoreKit";
    static private string WaboFacebookIOSSDK = "WaboFacebookIOSSDK";
    static private string V1 = "17.0.0";
    static private string V2 = "17.4.0";
    static private string LATEST_VERSION = V2;
    static private string DEFAULT_VERSION = V1;
    static private string DEFAULT_XCODE_VERSION = "Xcode 15.4";
    
    static private Dictionary<string, Dictionary<string, string>> ExtraFrameworks = new Dictionary<string, Dictionary<string, string>>() {
        {V1, new Dictionary<string, string>() {
            {"FBSDKCoreKit", V1},
            {"FBSDKLoginKit", V1},
            {"FBSDKShareKit", V1},
        }},
        {V2, new Dictionary<string, string>() {
            {"FBSDKCoreKit", V2},
            {"FBSDKLoginKit", V2},
            {"FBSDKShareKit", V2},
        }},
    };

    static private Dictionary<string, string> EmbedAndSignFrameworks = new Dictionary<string, string>() {
        {"FBAEMKit", "Pods/FBAEMKit/XCFrameworks/FBAEMKit.xcframework"},
        {"FBSDKCoreKit", "Pods/FBSDKCoreKit/XCFrameworks/FBSDKCoreKit.xcframework"},
        {"FBSDKCoreKit_Basics", "Pods/FBSDKCoreKit_Basics/XCFrameworks/FBSDKCoreKit_Basics.xcframework"},
        {"FBSDKLoginKit", "Pods/FBSDKLoginKit/XCFrameworks/FBSDKLoginKit.xcframework"},
        {"FBSDKShareKit", "Pods/FBSDKShareKit/XCFrameworks/FBSDKShareKit.xcframework"},
    };

    static private Dictionary<string, string> XcodeToFacebook = new Dictionary<string, string>() {
        {$"Xcode 15.0 - {FBSDKCoreKit}", V1},
        {$"Xcode 15.1 - {FBSDKCoreKit}", V1},
        {$"Xcode 15.2 - {FBSDKCoreKit}", V1},
        {$"Xcode 15.3 - {FBSDKCoreKit}", V1},
        {$"Xcode 15.4 - {FBSDKCoreKit}", V1},
        {$"Xcode 16.0 - {FBSDKCoreKit}", V2},
        {$"Xcode 16.1 - {FBSDKCoreKit}", V2},
        {$"Xcode 16.2 - {FBSDKCoreKit}", V2},
        // - 
        {$"Xcode 15.0 - {WaboFacebookIOSSDK}", V1},
        {$"Xcode 15.1 - {WaboFacebookIOSSDK}", V1},
        {$"Xcode 15.2 - {WaboFacebookIOSSDK}", V1},
        {$"Xcode 15.3 - {WaboFacebookIOSSDK}", V1},
        {$"Xcode 15.4 - {WaboFacebookIOSSDK}", V1},
        {$"Xcode 16.0 - {WaboFacebookIOSSDK}", V1},
        {$"Xcode 16.1 - {WaboFacebookIOSSDK}", V1},
        {$"Xcode 16.2 - {WaboFacebookIOSSDK}", V1},
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
        Debug.Log($"LongriverSDKApplovinPostBuild - profilePath: {profilePath} - {System.IO.File.Exists(profilePath)}");
        if (!LongriverSDKCommonPostBuild.CheckFrameworksForProfile(profilePath, new HashSet<string>() {FBSDKCoreKit, WaboFacebookIOSSDK}))
        {
            return;
        }
        
        string xcodeVersion = LongriverSDKCommonPostBuild.GetXcodeVersion();
        if (string.IsNullOrWhiteSpace(xcodeVersion))
        {
            xcodeVersion = DEFAULT_XCODE_VERSION;
        }
        string facebookVersion = DEFAULT_VERSION;
        bool hasHit = false;
        if (LongriverSDKCommonPostBuild.CheckFrameworkForProfile(profilePath, WaboFacebookIOSSDK))
        {
            string key = $"{xcodeVersion} - {WaboFacebookIOSSDK}";
            if (XcodeToFacebook.ContainsKey(key))
            {
                hasHit = true;
                facebookVersion = XcodeToFacebook[key];
            }
        }
        else if (LongriverSDKCommonPostBuild.CheckFrameworkForProfile(profilePath, FBSDKCoreKit))
        {
            string key = $"{xcodeVersion} - {FBSDKCoreKit}";
            if (XcodeToFacebook.ContainsKey(key))
            {
                hasHit = true;
                facebookVersion = XcodeToFacebook[key];
            }
        }
        if (!hasHit)
        {
            if (LongriverSDKCommonPostBuild.CompareVersions(xcodeVersion, DEFAULT_XCODE_VERSION) > 0)
            {
                facebookVersion = LATEST_VERSION;
            }
        }
        Debug.Log($"LongriverSDKFacebookPostBuild - Xcode: {xcodeVersion}; Facebook: {facebookVersion}; hasHit: {hasHit}");

        // Add or Modify facebook .framework to profile
        if (LongriverSDKCommonPostBuild.CheckFrameworkForProfile(profilePath, FBSDKCoreKit))
        {
            if (ExtraFrameworks.ContainsKey(facebookVersion))
            {
                Dictionary<string, string> extraFrameworkDict = ExtraFrameworks[facebookVersion];
                LongriverSDKCommonPostBuild.AddOrModifyFrameworksToProfile(profilePath, "UnityFramework", extraFrameworkDict);
            }
            else
            {
                Debug.LogError($"Unsupported versions of max {facebookVersion}");
            }
        }

        // Add .framework as Embed & Sign
        List<string> frameworkPathList = new List<string>();
        foreach (var item in EmbedAndSignFrameworks)
        {
            frameworkPathList.Add($"{exportProjectPath}/{item.Value}");
            
        }
        LongriverSDKCommonPostBuild.AddFrameworksAsEmbedAndSign(exportProjectPath, "Unity-iPhone", frameworkPathList);
    }
#endif

}

