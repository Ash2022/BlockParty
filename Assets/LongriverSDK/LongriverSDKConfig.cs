using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.Networking;

namespace LongriverSDKNS
{
    public class LongriverSDKConfig
    {
        static readonly string[] fileNames = new string[] {
            "LongriverSDKConfig.txt",
            "LongriverSDKConfig.json",
            "LongriverSDKConfig",
        };
        static private Action<string> configCallback;

        static public void GetConfig(MonoBehaviour mono, Action<string> configCallback)
        {
            LongriverSDKConfig.configCallback = configCallback;
            mono.StartCoroutine(ReadStreamAssetsFile(fileNames, RunAfterReadFile));
        }

        static public void RunAfterReadFile(string fileContent)
        {
            if(configCallback != null)
            {
                Debug.Log("config load finish and callback");
                configCallback.Invoke(fileContent);
            }
        }
        static public IEnumerator ReadStreamAssetsFile(string[] fileNames, Action<string> fileContent)
        {
            yield return null;
            string findConfigString = null;
            for (int i = 0, len = fileNames.Length; i < len; i++) 
            {
                string fileName = fileNames[i];
                string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, fileName);
                Debug.Log($"filePath: {filePath}");
#if UNITY_EDITOR
                if (System.IO.File.Exists(filePath)) 
                {
                    string tempString = System.IO.File.ReadAllText(filePath);
                    findConfigString = tempString.Trim().Replace("\n", "");
                    break;
                }

#elif UNITY_ANDROID && !UNITY_EDITOR
                UnityWebRequest www = UnityWebRequest.Get(filePath);
                yield return www.SendWebRequest();
                if (!www.isNetworkError && !www.isHttpError) 
                {
                    string tempString = www.downloadHandler.text;
                    tempString = tempString.Trim().Replace("\n", "");
                    if (!tempString.StartsWith("{") && !tempString.EndsWith("}"))
                    {
                        findConfigString = tempString;
                        break;
                    }
                }
#elif UNITY_IOS && !UNITY_EDITOR
                if (System.IO.File.Exists(filePath)) 
                {
                    string tempString = System.IO.File.ReadAllText(filePath);
                    findConfigString = tempString.Trim().Replace("\n", "");
                    break;
                }
#endif
            }
            if (!string.IsNullOrWhiteSpace(findConfigString))
            {
                Debug.Log($"ConfigString: {findConfigString}");
                fileContent(findConfigString);
            }
            else 
            {
                UnityEngine.Debug.LogError($"missing config file - LongriverSDKConfig");
            }



            /*
            string result = null;
            for (int i = 0, len = fileNames.Length; i < len; i++) 
            {
                string fileName = fileNames[i];
                string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, fileName);
                if (filePath.Contains("://") || filePath.Contains(":///"))
                {
                    WWW www = new WWW(filePath);
                    yield return www;
                    if (string.IsNullOrWhiteSpace(www.error)) 
                    {
                        result = www.text;
#if UNITY_ANDROID && !UNITY_EDITOR
                        if (!result.StartsWith("{"))
                        {
                            break;
                        }
#endif
                    }
                } 
                else 
                {
                  if (System.IO.File.Exists(filePath)) 
                    {
                        result = System.IO.File.ReadAllText(filePath);
#if UNITY_EDITOR
                        if (!result.StartsWith("{")) 
                        {
                            break;
                        } 
#elif UNITY_IOS && !UNITY_EDITOR
                        if (result.StartsWith("{"))
                        {
                            break;
                        }
#endif
                    }
                }
            }
            if (!string.IsNullOrWhiteSpace(result))
            {
                fileContent(result);
            }
            else 
            {
                UnityEngine.Debug.LogError($"missing config file - LongriverSDKConfig");
            }*/
        }
    }
}

