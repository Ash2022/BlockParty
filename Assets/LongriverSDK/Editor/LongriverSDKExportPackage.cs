#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.IO;

/*
/Applications/Unity/Hub/Editor/2022.3.20f1c1/Unity.app/Contents/MacOS/Unity \
-quit \
-batchmode \
-projectPath /Users/bepic/Documents/Bepic/Project/LongriverUnitySDK \
-executeMethod LongriverSDKExportPackage.Export \
-exportPath "target/LongriverSDK.unitypackage" \
-assetPaths "Assets/LongriverSDK"
*/
public class LongriverSDKExportPackage
{
    [MenuItem("LongriverTools/Export Package")]
    static public void Export()
    {
        string[] assetPaths = new string[] 
        {
            "Assets/LongriverSDK",
        };
        string exportPath = "target/LongriverSDK.unitypackage";

        string[] args = Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "-exportPath" && i + 1 < args.Length)
            {
                exportPath = args[i + 1];
            }
            else if (args[i] == "-assetPaths" && i + 1 < args.Length)
            {
                assetPaths = args[i + 1].Split(',');
            }
        }

        if (string.IsNullOrEmpty(exportPath) || assetPaths == null)
        {
            Debug.LogError("Invalid arguments. Usage: -exportPath <path> -assetPaths <paths>");
            return;
        }

        AssetDatabase.ExportPackage(assetPaths, exportPath, ExportPackageOptions.Recurse);
    }
}
#endif
