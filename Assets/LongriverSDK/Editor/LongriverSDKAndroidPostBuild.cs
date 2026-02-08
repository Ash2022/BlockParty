using System.Collections;
using System.Collections.Generic;
using System;

#if UNITY_ANDROID
using UnityEditor.Android;
using UnityEngine;
using System.IO;

public class LongriverSDKAndroidPostBuild : IPostGenerateGradleAndroidProject
{
    public int callbackOrder
    {
        get { return 1001; }
    }

    public void OnPostGenerateGradleAndroidProject(string path)
    {
        try
        {
#if UNITY_2019_3_OR_NEWER
            // On Unity 2019.3+, the path returned is the path to the unityLibrary's module.
            // The AppLovin Quality Service buildscript closure related lines need to be added to the root build.gradle file.
            var rootPropertiesFilePath = Path.Combine(path, "../gradle.properties");
            var rootBuildFilePath = Path.Combine(path, "../build.gradle");
#else

            var rootPropertiesFilePath = Path.Combine(path, "gradle.properties");
            var rootBuildFilePath = Path.Combine(path, "build.gradle");
#endif
            Debug.Log("LongriverSDKAndroidPostBuild BUILD path" + rootPropertiesFilePath);
            
            var writer = new StreamWriter(rootPropertiesFilePath, true);
            writer.WriteLine("");

            string gradleVersion = LongriverSDKCommonPostBuild.FindGradleVersion(path);
            if (!string.IsNullOrWhiteSpace(gradleVersion))
            {
                int result = LongriverSDKCommonPostBuild.CompareVersions(gradleVersion, "8.0.0");
                Debug.Log($"Find gradle version: {gradleVersion} - {result}");
                if (result <= 0) 
                {
                    writer.WriteLine("android.enableDexingArtifactTransform=false");
                }
                else 
                {
                    writer.WriteLine("android.useFullClasspathForDexingTransform=true");
                }
            }
            else 
            {
                Debug.LogWarning("Can not find gradle version");
                writer.WriteLine("android.enableDexingArtifactTransform=false");
            }
            writer.WriteLine("android.useAndroidX=true");
            writer.Close();
        }
        catch(Exception exception)
        {
            Debug.Log("LongriverSDKAndroidPostBuild catch unknow exception" + exception.Message);
        }
    }
}
#endif

