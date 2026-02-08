using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

public class LongriverSDKApplovinPostBuild
{

#if UNITY_ANDROID

#elif (UNITY_IOS || UNITY_IPHONE)
    static private string AppLovinSDK = "AppLovinSDK";
    static private string WaboMaxIOSSDK = "WaboMaxIOSSDK";
    static private string V1 = "12.6.0";
    static private string V2 = "13.0.1";
    static private string V3 = "13.3.1";
    static private string DEFAULT_VERSION = V1;
    static private string LATEST_VERSION = V3;
    static private string DEFAULT_XCODE_VERSION = "Xcode 15.4";
    static private Dictionary<string, Dictionary<string, string>> SubFrameworks = new Dictionary<string, Dictionary<string, string>>() {
        {V1, new Dictionary<string, string>() {
            // {"AppLovinMediationFyberAdapter"            , "8.3.1.1"     },
            {"AppLovinMediationByteDanceAdapter"        , "6.2.0.5.0"   },
            {"AppLovinMediationFacebookAdapter"         , "6.15.0.0"    },
            {"AppLovinMediationIronSourceAdapter"       , "8.1.0.0.1"   },
            {"AppLovinMediationMintegralAdapter"        , "7.7.1.0.0"   },
            {"AppLovinMediationUnityAdsAdapter"         , "4.10.0.0"    },
            {"AppLovinMediationVungleAdapter"           , "7.4.0.0"     },
            {"AppLovinMediationGoogleAdManagerAdapter"  , "11.5.0.0"    },
            {"AppLovinMediationGoogleAdapter"           , "11.5.0.0"    },
            {"bigo-ads-max-adapter"                     , "4.4.0.1"     },
        }},
        {V2, new Dictionary<string, string>() {
            // {"AppLovinMediationFyberAdapter"            , "8.3.4.0"     },
            {"AppLovinMediationByteDanceAdapter"        , "6.2.0.7.2"   },
            {"AppLovinMediationFacebookAdapter"         , "6.15.2.1"    },
            {"AppLovinMediationIronSourceAdapter"       , "8.5.0.0.0"   },
            {"AppLovinMediationMintegralAdapter"        , "7.7.3.0.0"   },
            {"AppLovinMediationUnityAdsAdapter"         , "4.12.5.0"    },
            {"AppLovinMediationVungleAdapter"           , "7.4.3.0"     },
            {"AppLovinMediationGoogleAdManagerAdapter"  , "11.13.0.0"   },
            {"AppLovinMediationGoogleAdapter"           , "11.13.0.0"   },
            {"bigo-ads-max-adapter"                     , "4.6.0.0"     },
        }},
        {V3, new Dictionary<string, string>() {
            // {"AppLovinMediationFyberAdapter"            , "8.3.8.0"     },
            {"AppLovinMediationByteDanceAdapter"        , "7.6.0.4.0"   },
            {"AppLovinMediationFacebookAdapter"         , "6.20.1.0"    },
            {"AppLovinMediationIronSourceAdapter"       , "8.11.0.0.0"   },
            {"AppLovinMediationMintegralAdapter"        , "7.7.9.0.0"   },
            {"AppLovinMediationUnityAdsAdapter"         , "4.16.1.0"    },
            {"AppLovinMediationVungleAdapter"           , "7.5.3.0"     },
            {"AppLovinMediationGoogleAdManagerAdapter"  , "12.11.0.0"    },
            {"AppLovinMediationGoogleAdapter"           , "12.11.0.0"    },
            {"bigo-ads-max-adapter"                     , "4.9.3.0"     },
        }},
    };

    static private Dictionary<string, Dictionary<string, string>> ExtraFrameworks = new Dictionary<string, Dictionary<string, string>>() {
        {V1, new Dictionary<string, string>() {
            {AppLovinSDK, V1},
            {"Google-Mobile-Ads-SDK", "11.5.0"},
        }},
        {V2, new Dictionary<string, string>() {
            {AppLovinSDK, V2},
            {"Google-Mobile-Ads-SDK", "11.13.0"},
        }},
        {V3, new Dictionary<string, string>() {
            {AppLovinSDK, V3},
            {"Google-Mobile-Ads-SDK", "12.11.0.0"},
        }},
    };

    static private Dictionary<string, string> XcodeToApplovin = new Dictionary<string, string>() {
        {$"Xcode 15.0 - {AppLovinSDK}", V1},
        {$"Xcode 15.1 - {AppLovinSDK}", V1},
        {$"Xcode 15.2 - {AppLovinSDK}", V1},
        {$"Xcode 15.3 - {AppLovinSDK}", V2},
        {$"Xcode 15.4 - {AppLovinSDK}", V2},
        {$"Xcode 16.0 - {AppLovinSDK}", V3},
        {$"Xcode 16.1 - {AppLovinSDK}", V3},
        {$"Xcode 16.2 - {AppLovinSDK}", V3},
        {$"Xcode 16.3 - {AppLovinSDK}", V3},
        {$"Xcode 16.4 - {AppLovinSDK}", V3},
        // -----
        {$"Xcode 15.0 - {WaboMaxIOSSDK}", V1},
        {$"Xcode 15.1 - {WaboMaxIOSSDK}", V1},
        {$"Xcode 15.2 - {WaboMaxIOSSDK}", V1},
        {$"Xcode 15.3 - {WaboMaxIOSSDK}", V1},
        {$"Xcode 15.4 - {WaboMaxIOSSDK}", V1},
        {$"Xcode 16.0 - {WaboMaxIOSSDK}", V1},
        {$"Xcode 16.1 - {WaboMaxIOSSDK}", V1},
        {$"Xcode 16.2 - {WaboMaxIOSSDK}", V1},
        {$"Xcode 16.3 - {WaboMaxIOSSDK}", V1},
        {$"Xcode 16.4 - {WaboMaxIOSSDK}", V1},
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

        if (1 != LongriverSDKCommonPostBuild.CheckLibraryForDependencies("WaboMaxIOSSDK") 
            && 1 != LongriverSDKCommonPostBuild.CheckLibraryForDependencies("AppLovinSDK"))
        {
            return;
        }

        string profilePath = $"{exportProjectPath}/Podfile";
        Debug.Log($"LongriverSDKApplovinPostBuild - profilePath: {profilePath} - {System.IO.File.Exists(profilePath)}");
        if (!LongriverSDKCommonPostBuild.CheckFrameworksForProfile(profilePath, new HashSet<string>() {AppLovinSDK, WaboMaxIOSSDK}))
        {
            return;
        }

        // string xcodeVersion = LongriverSDKCommonPostBuild.GetXcodeVersion();
        // if (string.IsNullOrWhiteSpace(xcodeVersion))
        // {
        //     xcodeVersion = DEFAULT_XCODE_VERSION;
        // }
        // string applovinVersion = DEFAULT_VERSION;
        // bool hasHit = false;
        // if (LongriverSDKCommonPostBuild.CheckFrameworkForProfile(profilePath, WaboMaxIOSSDK))
        // {
        //     string key = $"{xcodeVersion} - {WaboMaxIOSSDK}";
        //     if (XcodeToApplovin.ContainsKey(key))
        //     {
        //         hasHit = true;
        //         applovinVersion = XcodeToApplovin[key];
        //     }
        // }
        // else if (LongriverSDKCommonPostBuild.CheckFrameworkForProfile(profilePath, AppLovinSDK))
        // {
        //     string key = $"{xcodeVersion} - {AppLovinSDK}";
        //     if (XcodeToApplovin.ContainsKey(key))
        //     {
        //         hasHit = true;
        //         applovinVersion = XcodeToApplovin[key];
        //     }
        // }
        // if (!hasHit)
        // {
        //     if (LongriverSDKCommonPostBuild.CompareVersions(xcodeVersion, DEFAULT_XCODE_VERSION) > 0)
        //     {
        //         applovinVersion = LATEST_VERSION;
        //     }
        // }
        // Debug.Log($"LongriverSDKApplovinPostBuild - Xcode: {xcodeVersion}; Applovin: {applovinVersion}; hasHit: {hasHit}");

        // // Add or Modify max sub network to profile
        // if (SubFrameworks.ContainsKey(applovinVersion))
        // {
        //     Dictionary<string, string> subFrameworkDict = SubFrameworks[applovinVersion];
        //     LongriverSDKCommonPostBuild.AddOrModifyFrameworksToProfile(profilePath, "UnityFramework", subFrameworkDict);
        // }
        // else
        // {
        //     Debug.LogError($"Unsupported versions of max {applovinVersion}");
        // }

        // // Modify specialty framework to profile
        // if (ExtraFrameworks.ContainsKey(applovinVersion))
        // {
        //     Dictionary<string, string> extraFrameworkDict = ExtraFrameworks[applovinVersion];
        //     HashSet<string> extraFrameworkSet = new HashSet<string>(extraFrameworkDict.Keys);
        //     Dictionary<string, bool> result =  LongriverSDKCommonPostBuild.CheckFrameworkForProfile(profilePath, extraFrameworkSet);
        //     if (null != result && result.Count == extraFrameworkSet.Count)
        //     {
        //         Dictionary<string, string> modifyFrameworkDict = new Dictionary<string, string>();
        //         foreach (var item in result)
        //         {
        //             if (item.Value)
        //             {
        //                 modifyFrameworkDict.Add(item.Key, extraFrameworkDict[item.Key]);
        //             }
        //         }
        //         LongriverSDKCommonPostBuild.ModifyFrameworksToProfile(profilePath, "UnityFramework", modifyFrameworkDict);
        //     }
        //     else
        //     {
        //         Debug.LogError($"Modify specialty framework to profile error {applovinVersion}");
        //     }
        // }
        // else
        // {
        //     Debug.LogError($"Unsupported versions of max {applovinVersion}");
        // }

        // Add .framework as Embed & Sign
        string version = LongriverSDKCommonPostBuild.FindVersionForProfile(profilePath, AppLovinSDK);
        string frameworkPath = $"Pods/{AppLovinSDK}/applovin-ios-sdk-{version}/{AppLovinSDK}.xcframework";
        LongriverSDKCommonPostBuild.AddFrameworksAsEmbedAndSign(exportProjectPath, "Unity-iPhone", new List<string>() {frameworkPath});
    }
#endif

}

