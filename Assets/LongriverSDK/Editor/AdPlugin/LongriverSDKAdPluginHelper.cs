using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;
using System.Security.Cryptography;

public class LongriverSDKAdPluginHelper : MonoBehaviour
{
    static private string[] fileNames = new string[] {
        "LongriverSDKConfig.json",
        "LongriverSDKConfig.txt",
        "LongriverSDKConfig",
    };

    public static string GetSdkKey(string system)
    {
        string maxSdkKey = null;
        for (int i = 0, len = fileNames.Length; i < len; i++)
        {
            string fileName = fileNames[i];
            string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, fileName);
            if (System.IO.File.Exists(filePath))
            {
                string configContent = System.IO.File.ReadAllText(filePath);
                configContent = configContent.Trim().Replace("\n", "");
                Debug.Log($"longriver sdk start with {filePath}\n{configContent}");
                string findConfigString = null;
                if (configContent.StartsWith("{") && configContent.EndsWith("}"))
                {
                    findConfigString = configContent;
                }
                else
                {
                    findConfigString = AesDecryptorBase64(configContent, "aGVsbG93b3JsZGtl");
                }

                if (null != findConfigString && AppLovinMax.ThirdParty.MiniJson.Json.Deserialize(findConfigString) is Dictionary<string, object> dict)
                {
                    string key = "androidTopOnKey";
                    if (dict.ContainsKey(key))
                    {
                        maxSdkKey = dict[key] as string;
                    } 
                    else
                    {
                        key = "iosTopOnKey";
                        if (dict.ContainsKey(key))
                        {
                            maxSdkKey = dict[key] as string;
                        }
                    }
                    break;
                }
            }
        }
        if (null == maxSdkKey)
        {
            Debug.LogError($"longriver can not find max sdk key");
        }
        else
        {
            Debug.Log($"longriver max sdk key {maxSdkKey}");
        }
        return maxSdkKey;
    }


    private static RijndaelManaged GetRijndaelManaged(string key)
    {
        byte[] keyArray = Encoding.UTF8.GetBytes(key);
        RijndaelManaged rDel = new RijndaelManaged();
        rDel.Key = keyArray;
        rDel.Mode = CipherMode.ECB;
        rDel.Padding = PaddingMode.PKCS7;
        return rDel;
    }

    public static string AesDecryptorBase64(string DecryptStr, string Key)
    {
        try
        {
            // Debug.Log($"AesDecryptorBase64 - \n{DecryptStr}\n{Key}");
            byte[] toEncryptArray = Convert.FromBase64String(DecryptStr);
            ICryptoTransform cTransform = GetRijndaelManaged(Key).CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Encoding.UTF8.GetString(resultArray);//  UTF8Encoding.UTF8.GetString(resultArray);
        }
        catch (Exception ex)
        {
            Debug.Log("aes decryptor error");
            Debug.LogException(ex);
            return null;
        }
    }
}


