using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;
using UnityEngine.Networking;
using System;
using System.Linq;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
using LongriverSDKNS;
#endif

public class LongriverSDKUserPaymentPostBuild
{
    //force project to add swift
    public static bool UsingSwift = true;

#if UNITY_IOS
    //please change to your real facebook id and token
    static String facebookAppid = "1346044659727833";
    static String facebookClientToken = "8cb12336bdb0ee5e73a9259019b067ef";
    static String facebookDisplayName = "BlockTris";
    static String facebookProID = "";
    static String facebookProClientToken = "";

    //below please not change;
    private static readonly List<string> schemes = new List<string>
        {
          "fbapi",
          "fbapi20130214",
          "fbapi20130410",
          "fbapi20130702",
          "fbapi20131010",
          "fbapi20131219",
          "fbapi20140410",
          "fbapi20140116",
          "fbapi20150313",
          "fbapi20150629",
          "fbapi20160328",
          "fbauth",
          "fb-messenger-share-api",
          "fbauth2",
          "fbshareextension",
        };
#if !UNITY_2019_3_OR_NEWER
    private const string UnityMainTargetName = "Unity-iPhone";
#endif

    // [PostProcessBuild(1001)]
    // public static void OnPostProcessBuildPods(BuildTarget buildTarget, string buildPath)
    // {
    //     if (buildTarget == BuildTarget.iOS)
    //     {
    //         string profilePath = $"{buildPath}/Podfile";
    //         if (File.Exists(profilePath)) 
    //         {
    //             Debug.Log($"{profilePath} is Exists");
    //             try 
    //             {
    //                 using (StreamWriter streamWriter = File.AppendText(profilePath)) {
    //                     streamWriter.Write("use_frameworks! :linkage => :static");
    //                     streamWriter.Flush();
    //                     streamWriter.Close();
    //                 }
    //                 Debug.Log("write to podfile success");
    //             } catch (System.Exception e) 
    //             {
    //                 Debug.LogError("write to podfile error");
    //                 Debug.LogException(e);
    //             }
    //         }
    //         else
    //         {
    //             Debug.LogWarning($"{profilePath} is not Exists");
    //         }
    //     }
    // }

    [PostProcessBuild(1000)]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string buildPath)
    {
        if (buildTarget == BuildTarget.iOS)
        {
            if (UsingSwift)
            {
                Debug.Log("add swift");
                string projectPath = buildPath + "/Unity-iPhone.xcodeproj/project.pbxproj";
                PBXProject project = new PBXProject();
                project.ReadFromFile(projectPath);
                //add swift
#if UNITY_2019_3_OR_NEWER
                var unityMainTargetGuid = project.GetUnityMainTargetGuid();
                var unityFrameworkTargetGuid = project.GetUnityFrameworkTargetGuid();
#else
                var unityMainTargetGuid = project.TargetGuidByName(UnityMainTargetName);
                var unityFrameworkTargetGuid = project.TargetGuidByName(UnityMainTargetName);
#endif

                // add swift
                AddSwiftSupport(buildPath, project, unityMainTargetGuid);
                EmbedSwiftStandardLibraries(buildPath, project, unityMainTargetGuid);

                //project.AddCapability(unityMainTargetGuid, PBXCapabilityType.GameCenter);

                ModifySettings(project);

                project.WriteToFile(projectPath);
            }


            if (LongriverSDKDependencyHelper.iosFacebook())
            {
                Debug.Log("add facebook plist");
                var plistPath = Path.Combine(buildPath, "Info.plist");
                PlistDocument plist = new PlistDocument();
                plist.ReadFromFile(plistPath);

                PlistElementArray lsApplicationQueriesSchemes = plist.root.CreateArray("LSApplicationQueriesSchemes");
                foreach (string one in schemes)
                {
                    lsApplicationQueriesSchemes.AddString(one);
                }

                PlistElementArray CFBundleURLTypes = plist.root.CreateArray("CFBundleURLTypes");
                PlistElementDict dict = CFBundleURLTypes.AddDict();
                PlistElementArray CFBundleURLSchemes = dict.CreateArray("CFBundleURLSchemes");
                CFBundleURLSchemes.AddString("fb" + facebookAppid);

                plist.root.SetString("FacebookAppID", facebookAppid);
                plist.root.SetString("FacebookClientToken", facebookClientToken);
                plist.root.SetString("FacebookDisplayName", facebookDisplayName);
                if(facebookProID != null && facebookProID.Length > 1)
                {
                    plist.root.SetString("FacebookProID", facebookProID);
                    plist.root.SetString("FacebookProClientToken", facebookProClientToken);
                }

                plist.WriteToFile(plistPath);
            }

            
        }
    }

    private static void ModifySettings(PBXProject project)
    {
        List<string> guidList = new List<string>();
#if UNITY_2019_3_OR_NEWER
        string unityFrameworkTargetGuid = project.GetUnityFrameworkTargetGuid();
        string unityMainTargetGuid = project.GetUnityMainTargetGuid();
        guidList.Add(unityFrameworkTargetGuid);
        guidList.Add(unityMainTargetGuid);
#else
        string unityMainTargetGuid = project.TargetGuidByName("Unity-iPhone");
        guidList.Add(unityMainTargetGuid);
#endif
        for (int i = 0, len = guidList.Count; i < len; i++)
        {
            // note that, due to some Apple shoddyness, you usually need to turn this off
            // to allow the project to ARCHIVE correctly (ie, when sending to testflight):
            project.AddBuildProperty(guidList[i], "ENABLE_BITCODE", "NO");
            //project.AddBuildProperty(guidList[i], "Always_Embed_Swift_Standard_Libraries", "YES");
        }
    }
    private static void AddSwiftSupport(string buildPath, PBXProject project, string targetGuid)
    {
        var swiftFileRelativePath = "Classes/LongriverSDKSwiftSupport.swift";
        var swiftFilePath = Path.Combine(buildPath, swiftFileRelativePath);

        // Add Swift file
        CreateSwiftFile(swiftFilePath);
        var swiftFileGuid = project.AddFile(swiftFilePath, swiftFileRelativePath, PBXSourceTree.Source);
        project.AddFileToBuild(targetGuid, swiftFileGuid);

        // Add Swift build properties
        project.AddBuildProperty(targetGuid, "SWIFT_VERSION", "5");
        project.AddBuildProperty(targetGuid, "CLANG_ENABLE_MODULES", "YES");
    }

    /// <summary>
    /// For Swift 5+ code that uses the standard libraries, the Swift Standard Libraries MUST be embedded for iOS < 12.2
    /// Swift 5 introduced ABI stability, which allowed iOS to start bundling the standard libraries in the OS starting with iOS 12.2
    /// Issue Reference: https://github.com/facebook/facebook-sdk-for-unity/issues/506
    /// </summary>
    private static void EmbedSwiftStandardLibraries(string buildPath, PBXProject project, string mainTargetGuid)
    {
        // This needs to be added the main target. App Store may reject builds if added to UnityFramework (i.e. MoPub in FT).
        project.AddBuildProperty(mainTargetGuid, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "YES");
    }

    private static void CreateSwiftFile(string swiftFilePath)
    {
        if (File.Exists(swiftFilePath)) return;

        Debug.Log("adding swiftFilePath:" + swiftFilePath);
        // Create a file to write to.
        using (var writer = File.CreateText(swiftFilePath))
        {
            writer.WriteLine("//\n//  LongriverSDKSwiftSupport.swift\n//");
            writer.WriteLine("\nimport Foundation\n");
            writer.WriteLine("// This file ensures the project includes Swift support.");
            writer.WriteLine("// It is automatically generated by the LongriverSDK Unity Plugin.");
            writer.Close();
        }
    }
#endif
}



