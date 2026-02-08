#if UNITY_IOS || UNITY_IPHONE
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Diagnostics;
using System.IO;
using LongriverSDKNS;
using UnityEditor.iOS.Xcode;
using UnityEngine.Assertions.Must;
using System.Collections.Generic;
public class LongriverSDKiOSInfoPlistPostBuild
{
    /*
        接收指定URL Scheme请求时需要配置(例如: longriversdk)
        Configuration is required to receive specific URL Scheme requests (e.g. longriversdk).
    */
    static private Dictionary<string, List<Dictionary<string, List<string>>>> URLSchemes = new Dictionary<string, List<Dictionary<string, List<string>>>>() {{
        "CFBundleURLTypes", new List<Dictionary<string, List<string>>>() {
            new Dictionary<string, List<string>>() {{
                "CFBundleURLSchemes", new List<string>() {
                    // "longriversdk",
                    Application.identifier,
                }
            }}
        }
    }};
    /*
        发起指定URL Scheme请求时需要配置(例如: longriversdk)
        Configuration is required to initiate a specific URL Scheme request (e.g., longriversdk).
    */
    static private Dictionary<string, List<string>> QueriedURLSchemes = new Dictionary<string, List<string>>() {{
        "LSApplicationQueriesSchemes", new List<string>() {
            // "longriversdk"
        }
    }};
    
    [PostProcessBuild]
    static public void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject)
    {
        if (target != BuildTarget.iOS)
        {
            return;
        }
        string plistFullPath = Path.Combine(pathToBuiltProject, "Info.plist");
        ModifyPlist(plistFullPath);
    } 

    static private void ModifyPlist(string plistFullPath)
    {
        PlistDocument infoPlist = new PlistDocument();
        infoPlist.ReadFromString(File.ReadAllText(plistFullPath));
        // ----- URLSchemes
        if (null != URLSchemes && URLSchemes.Count > 0)
        {
            foreach (var item in URLSchemes)
            {
                PlistElementArray plistElementArray = infoPlist.root.CreateArray(item.Key);
                PlistElementDict plistElementDict = plistElementArray.AddDict();
                foreach (var dict in item.Value)
                {
                    foreach (var entry in dict)
                    {
                        PlistElementArray arr = plistElementDict.CreateArray(entry.Key);
                        foreach (var element in entry.Value)
                        {
                            arr.AddString(element);
                        }
                    }
                }
            }
        }

        // ----- QueriedURLSchemes
        if (null != QueriedURLSchemes && QueriedURLSchemes.Count > 0) 
        {
            foreach (var item in QueriedURLSchemes)
            {
                PlistElementArray plistElementArray = infoPlist.root.CreateArray(item.Key);
                foreach (var url in item.Value)
                {
                    plistElementArray.AddString(url);
                }
            }
        }

        File.WriteAllText(plistFullPath, infoPlist.WriteToString());
    }
}
#endif
