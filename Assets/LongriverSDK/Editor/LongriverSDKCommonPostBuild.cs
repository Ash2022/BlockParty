using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongriverSDKCommonPostBuild
{

    /*
        result = 0 => not exist
        result = 1 => exist
        result = -1 => unknown
    */
    static internal int CheckLibraryForDependencies(string libraryName)
    {
        string dataPath = Application.dataPath;
        string dependenciesPath = $"{dataPath}/LongriverSDK/Editor/Dependencies.xml";
        if (!System.IO.File.Exists(dependenciesPath))
        {
            Debug.LogError("Dependencies.xml file is missing, please check the configuration");
            return -1;
        }

        int result = 0;
        using (System.IO.StreamReader reader = new System.IO.StreamReader(dependenciesPath))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (!line.Trim().StartsWith("<!--") && !line.Trim().EndsWith("-->") && line.Trim().Contains(libraryName))
                {
                    result = 1;
                    break;
                }
            }
            reader.Close();
        }
        return result;
    }

    static internal string FindLibraryVersionForDependencies(string libraryName)
    {
        string dataPath = Application.dataPath;
        string dependenciesPath = $"{dataPath}/LongriverSDK/Editor/Dependencies.xml";
        if (!System.IO.File.Exists(dependenciesPath))
        {
            Debug.LogError("Dependencies.xml file is missing, please check the configuration");
            return null;
        }
        string hitLine = null;
        using (System.IO.StreamReader reader = new System.IO.StreamReader(dependenciesPath))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(hitLine) && !line.Trim().StartsWith("<!--") && !line.Trim().EndsWith("-->") && line.Trim().Contains(libraryName))
                {
                    hitLine = line;
                }
                if (!string.IsNullOrWhiteSpace(hitLine) && line.Trim().StartsWith("-->")) 
                {
                    hitLine = null;
                    break;
                }
                else if (!string.IsNullOrWhiteSpace(hitLine) && line.Trim().StartsWith("<!--"))
                {
                    break;
                }
            }
            reader.Close();
        }

        string version = null;
        if (!string.IsNullOrWhiteSpace(hitLine))
        {
#if UNITY_ANDROID
        string str = hitLine.Trim();
        version = str.Split(":")[2].Trim().Replace("/>", "").Replace(" ", "").Replace("\"", "");
#elif (UNITY_IOS || UNITY_IPHONE)
        string str = hitLine.Trim();
        version = str.Split("version")[1].Trim().Replace("/>", "").Replace("=", "").Replace(" ", "").Replace("\"", "");
#endif
        }
        return version;
    }

#if UNITY_ANDROID
    static internal string FindGradleVersion(string exportProjectPath)
    {
        string version = null;
#if UNITY_2019_3_OR_NEWER
        var gradleWrapperPropertiesFilePath = System.IO.Path.Combine(exportProjectPath, "../gradle/wrapper/gradle-wrapper.properties");
#else
        var gradleWrapperPropertiesFilePath = System.IO.Path.Combine(exportProjectPath, "gradle/wrapper/gradle-wrapper.properties");
#endif

        if (!System.IO.File.Exists(gradleWrapperPropertiesFilePath))
        {
            Debug.LogError($"can not find file {gradleWrapperPropertiesFilePath}");
            return version;
        }

        using (System.IO.StreamReader reader = new System.IO.StreamReader(gradleWrapperPropertiesFilePath))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (line.Trim().StartsWith("distributionUrl"))
                {
                    version = line.Trim().Split("-")[1].Trim();
                    break;
                }
            }
            reader.Close();
        }

        return version;
    }

#elif (UNITY_IOS || UNITY_IPHONE)
    static internal int AddFrameworksAsEmbedAndSign(string exportProjectPath, string targetName, List<string> frameworkPathList)
    {
        if (!System.IO.Directory.Exists(exportProjectPath))
        {
            Debug.LogError($"Directory is missing, please check the configuration {exportProjectPath}");
            return -1;
        }

        string pbxProjectPath = UnityEditor.iOS.Xcode.PBXProject.GetPBXProjectPath(exportProjectPath);
        UnityEditor.iOS.Xcode.PBXProject pbxProject = new UnityEditor.iOS.Xcode.PBXProject();
        pbxProject.ReadFromString(System.IO.File.ReadAllText(pbxProjectPath));

        // Get the target GUID
        string targetGuid = null;
        if ("UnityMain".Equals(targetName) || "Unity-iPhone".Equals(targetName))
        {
#if UNITY_2019_3_OR_NEWER
            targetGuid = pbxProject.GetUnityMainTargetGuid();
#else
            targetGuid = pbxProject.TargetGuidByName("Unity-iPhone");
#endif  
        }
        else
        {
            targetGuid = pbxProject.TargetGuidByName(targetName);
        }

        if (string.IsNullOrWhiteSpace(targetGuid))
        {
            Debug.LogError("Target Guid is missing, please check the configuration");
            return 0;
        }

        // .framework Embed & Sign
        foreach (var frameworkPath in frameworkPathList)
        {
            string frameworkName = System.IO.Path.GetFileName(frameworkPath);
            string fileGuid = pbxProject.AddFile(frameworkPath, "Frameworks/" + frameworkName, UnityEditor.iOS.Xcode.PBXSourceTree.Source);
            pbxProject.AddFileToBuild(targetGuid, fileGuid);
            UnityEditor.iOS.Xcode.Extensions.PBXProjectExtensions.AddFileToEmbedFrameworks(pbxProject, targetGuid, fileGuid);
        }

        // Save the modified project file
        System.IO.File.WriteAllText(pbxProjectPath, pbxProject.WriteToString());

        return 1;
    }

    static internal int AddOrModifyFrameworksToProfile(string profilePath, string targetName, Dictionary<string, string> frameworkDict)
    {
        int isOk = 0;
        HashSet<string> extraFrameworkSet = new HashSet<string>(frameworkDict.Keys);
        Dictionary<string, bool> result =  LongriverSDKCommonPostBuild.CheckFrameworkForProfile(profilePath, extraFrameworkSet);
        if (null != result && result.Count == extraFrameworkSet.Count)
        {
            Dictionary<string, string> addFrameworkDict = new Dictionary<string, string>();
            Dictionary<string, string> modifyFrameworkDict = new Dictionary<string, string>();
            foreach (var item in result)
            {
                if (item.Value)
                {
                    modifyFrameworkDict.Add(item.Key, frameworkDict[item.Key]);
                }
                else
                {
                    addFrameworkDict.Add(item.Key, frameworkDict[item.Key]);
                }
            }
            if (modifyFrameworkDict.Count > 0)
            {
                LongriverSDKCommonPostBuild.ModifyFrameworksToProfile(profilePath, targetName, modifyFrameworkDict);
            }
            if (addFrameworkDict.Count > 0)
            {
                LongriverSDKCommonPostBuild.AddFrameworksToProfile(profilePath, targetName, addFrameworkDict);
            }
            isOk = 1;
        }
        else
        {
            Debug.LogError($"Add or Modify .framework to profile error");
        }
        return isOk;
    }

    static internal int AddFrameworksToProfile(string profilePath, string targetName, Dictionary<string, string> frameworkDict)
    {
        if (!System.IO.File.Exists(profilePath))
        {
            Debug.LogError($"Podfile file is missing, please check the configuration {profilePath}");
            return -1;
        }

        bool hasAdd = false;
        List<string> newTextList = new List<string>();
        using (System.IO.StreamReader reader = new System.IO.StreamReader(profilePath))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                newTextList.Add(line);
                if (line.Trim().StartsWith("target") && line.Trim().Contains(targetName))
                {
                    hasAdd = true;
                    foreach (var item in frameworkDict)
                    {
                        newTextList.Add($"  pod '{item.Key}', '{item.Value}'");
                    }
                }
            }

            reader.Close();
        }
        System.IO.File.WriteAllLines(profilePath, newTextList);

        return hasAdd ? 1 : 0;
    }

    static internal int ModifyFrameworksToProfile(string profilePath, string targetName, Dictionary<string, string> frameworkDict)
    {
        if (!System.IO.File.Exists(profilePath))
        {
            Debug.LogError($"Podfile file is missing, please check the configuration {profilePath}");
            return -1;
        }

        bool hasModify = false;
        bool isTarget = false;
        List<string> newTextList = new List<string>();
        using (System.IO.StreamReader reader = new System.IO.StreamReader(profilePath))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (isTarget)
                {
                    bool hasHit = false;
                    if (line.Trim().StartsWith("end"))
                    {
                        isTarget = false;
                    } 
                    else 
                    {
                        foreach (var item in frameworkDict)
                        {
                            if (line.Trim().Contains(item.Key))
                            {
                                hasModify = true;
                                hasHit = true;
                                newTextList.Add($"  pod '{item.Key}', '{item.Value}'");
                                break;
                            }
                        }
                    }

                    if (!hasHit)
                    {
                        newTextList.Add(line);
                    }
                }
                else
                {
                    newTextList.Add(line);
                    if (line.Trim().StartsWith("target") && line.Trim().Contains(targetName))
                    {
                        isTarget = true;
                    }
                }
            }

            reader.Close();
        }
        System.IO.File.WriteAllLines(profilePath, newTextList);

        return hasModify ? 1 : 0;
    }

    static internal bool CheckFrameworkForProfile(string profilePath, string frameworkName)
    {
        Dictionary<string, bool> result = CheckFrameworkForProfile(profilePath, new HashSet<string>() {frameworkName});
        if (null != result && result.Count == 1)
        {
            return result[frameworkName];
        }
        return false;
    }

    static internal bool CheckFrameworksForProfile(string profilePath, HashSet<string> frameworks)
    {
        Dictionary<string, bool> result = CheckFrameworkForProfile(profilePath, frameworks);
        if (null != result && result.Count == frameworks.Count)
        {
            return result.ContainsValue(true);
        }
        return false;
    }

    static internal Dictionary<string, bool> CheckFrameworkForProfile(string profilePath, HashSet<string> frameworks)
    {
        if (!System.IO.File.Exists(profilePath))
        {
            Debug.LogError("Podfile file is missing, please check the configuration");
            return null;
        }

        Dictionary<string, bool> result = new Dictionary<string, bool>();
        using (System.IO.StreamReader reader = new System.IO.StreamReader(profilePath))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                foreach (var item in frameworks)
                {
                    if (!result.ContainsKey(item) && line.Trim().Contains(item))
                    {
                        result.Add(item, true);
                        break;
                    }
                }
            }
            reader.Close();
        }

        foreach (var item in frameworks)
        {
            if (!result.ContainsKey(item))
            {
                result.Add(item, false);
            }
        }

        return result;
    }

    static internal string FindVersionForProfile(string profilePath, string frameworkName)
    {
        if (!System.IO.File.Exists(profilePath))
        {
            Debug.LogError("Podfile file is missing, please check the configuration");
            return null;
        }

        string result = null;
        using (System.IO.StreamReader reader = new System.IO.StreamReader(profilePath))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (line.Trim().StartsWith("pod") && line.Trim().Contains(frameworkName))
                {
                    result = line.Replace("pod", "").Replace(frameworkName, "").Replace("'", "").Replace(",", "").Trim();
                    break;
                }
            }
            reader.Close();
        }

        return result;
    }

    static internal string GetXcodeVersion()
    {
        // 使用终端命令获取Xcode版本
        System.Diagnostics.Process process = new System.Diagnostics.Process();
        System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
        startInfo.FileName = "/usr/bin/xcodebuild"; // 确保这是正确的路径，如果xcodebuild不在/usr/bin/下，请更改此路径
        startInfo.Arguments = "-version";
        startInfo.RedirectStandardOutput = true;
        startInfo.UseShellExecute = false;
        startInfo.CreateNoWindow = true;
        process.StartInfo = startInfo;
        process.Start();

        string output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();

        // 解析输出以获取版本号
        string[] lines = output.Split('\n');
        foreach (string line in lines)
        {
            if (line.StartsWith("Xcode"))
            {
                return line; // 或者进一步解析只返回版本号
            }
        }
        return null;
    }
#endif
    /*
        result < 0 => order
        result > 0 => newer
        result = 0 => same
    */
    static internal int CompareVersions(string versionA, string versionB)
    {
        // 去除前缀如"Xcode "，只保留版本号部分
        string[] partsA = versionA.Replace("Xcode ", "").Split('.');
        string[] partsB = versionB.Replace("Xcode ", "").Split('.');

        int minLength = System.Math.Min(partsA.Length, partsB.Length);

        for (int i = 0; i < minLength; i++)
        {
            // 尝试将版本号部分转换为整数
            if (int.TryParse(partsA[i], out int numA) && int.TryParse(partsB[i], out int numB))
            {
                if (numA != numB)
                {
                    return numA.CompareTo(numB); // 返回第一个不相同的数字部分的比较结果
                }
            }
            else
            {
                // 如果无法转换为整数，则直接按字符串比较
                int strCompare = string.Compare(partsA[i], partsB[i], System.StringComparison.Ordinal);
                if (strCompare != 0)
                {
                    return strCompare;
                }
            }
        }

        // 若前面部分都相同，则长度较长的版本视为较大
        return partsA.Length.CompareTo(partsB.Length);
    }
}

