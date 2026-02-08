using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public class LongriverSDKDependencyHelper
{
    static public string xmlFile = "./Assets/LongriverSDK/Editor/Dependencies.xml";
    static public bool androidUsePayment()
    {
        return FindDependencyInAndroid("LongriverUserPayment");
    }
    static public bool androidFacebook()
    {
        return FindDependencyInAndroid("LongriverFacebook");
    }
    static public bool androidFirebase()
    {
        return FindDependencyInAndroid("LongriverFirebase");
    }
    
    static private bool FindDependencyInAndroid(string name)
    {
        XmlDocument doc = new XmlDocument();
        doc.Load(xmlFile);
        {
            XmlNodeList nodes = doc.DocumentElement.SelectNodes("/dependencies/androidPackages/androidPackage");
            foreach (XmlNode one in nodes)
            {
                string x = one.Attributes["spec"].Value;
                if (x.Contains(name))
                {
                    Debug.Log("find "+name+" in dependency ");
                    return true;
                }
            }
        }
        return false;
    }
    static public bool iosUsePayment()
    {
        return FindDependencyInIOS("WaboLoginPurchaseIOSSDK");
    }
    static public bool iosFacebook()
    {
        bool hasFacbook = FindDependencyInIOS("WaboFacebookIOSSDK");
        if (!hasFacbook)
        {
            hasFacbook = FindDependencyInIOS("FBSDKCoreKit");
        }
        return hasFacbook;
    }
    static public bool iosFirebase()
    {
        bool hasFirebase = FindDependencyInIOS("WaboFirebaseIOSSDK");
        if (!hasFirebase) 
        {
            hasFirebase = FindDependencyInIOS("Firebase");
        }
        return hasFirebase;
    }

    static private bool FindDependencyInIOS(string name)
    {
        XmlDocument doc = new XmlDocument();
        doc.Load(xmlFile);
        {
            XmlNodeList nodes = doc.DocumentElement.SelectNodes("/dependencies/iosPods/iosPod");
            foreach (XmlNode one in nodes)
            {
                string x = one.Attributes["name"].Value;
                if (x.Contains(name))
                {
                    Debug.Log("find "+name+" in dependency ");
                    return true;
                }
            }
        }
        return false;
    }
}


