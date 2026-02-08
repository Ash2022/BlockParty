using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;
#if UNITY_IOS
using UnityEngine.Networking;
using System;
using System.Linq;
using UnityEditor.iOS.Xcode;


public class LongriverSDKSDKNetworkIDEditorPostBuild
{
    [Serializable]
    public class SkAdNetworkData
    {
        [SerializeField] public string[] SkAdNetworkIds;
    }

    static string[] localSkAdNetworkIds = new string[]
    {
        "7ug5zh24hu.skadnetwork",
        "84993kbrcf.skadnetwork",
        "89z7zv988g.skadnetwork",
        "8c4e2ghe7u.skadnetwork",
        "8m87ys6875.skadnetwork",
        "8r8llnkz5a.skadnetwork",
        "8s468mfl3y.skadnetwork",
        "97r2b46745.skadnetwork",
        "9b89h5y424.skadnetwork",
        "9g2aggbj52.skadnetwork",
        "9nlqeag3gk.skadnetwork",
        "9rd848q2bz.skadnetwork",
        "9t245vhmpl.skadnetwork",
        "9vvzujtq5s.skadnetwork",
        "9yg77x724h.skadnetwork",
        "a2p9lx4jpn.skadnetwork",
        "a7xqa6mtl2.skadnetwork",
        "a8cz6cu7e5.skadnetwork",
        "av6w8kgt66.skadnetwork",
        "b9bk5wbcq9.skadnetwork",
        "bvpn9ufa9b.skadnetwork",
        "bxvub5ada5.skadnetwork",
        "c3frkrj4fj.skadnetwork",
        "c6k4g5qg8m.skadnetwork",
        "cg4yq2srnc.skadnetwork",
        "cj5566h2ga.skadnetwork",
        "cp8zw746q7.skadnetwork",
        "cs644xg564.skadnetwork",
        "cstr6suwn9.skadnetwork",
        "dbu4b84rxf.skadnetwork",
        "dkc879ngq3.skadnetwork",
        "dzg6xy7pwj.skadnetwork",
        "e5fvkxwrpn.skadnetwork",
        "ecpz2srf59.skadnetwork",
        "eh6m2bh4zr.skadnetwork",
        "ejvt5qm6ak.skadnetwork",
        "f38h382jlk.skadnetwork",
        "f73kdq92p3.skadnetwork",
        "f7s53z58qe.skadnetwork",
        "feyaarzu9v.skadnetwork",
        "g28c52eehv.skadnetwork",
        "g2y4y55b64.skadnetwork",
        "g69uk9uh2b.skadnetwork",
        "g6gcrrvk4p.skadnetwork",
        "ggvn48r87g.skadnetwork",
        "glqzh8vgby.skadnetwork",
        "gta8lk7p23.skadnetwork",
        "gta9lk7p23.skadnetwork",
        "gvmwg8q7h5.skadnetwork",
        "h65wbv5k3f.skadnetwork",
        "hb56zgv37p.skadnetwork",
        "hdw39hrw9y.skadnetwork",
        "hs6bdukanm.skadnetwork",
        "k674qkevps.skadnetwork",
        "k6y4y55b64.skadnetwork",
        "kbd757ywx3.skadnetwork",
        "kbmxgpxpgc.skadnetwork",
        "klf5c3l5u5.skadnetwork",
        "krvm3zuq6h.skadnetwork",
        "ln5gz23vtd.skadnetwork",
        "lr83yxwka7.skadnetwork",
        "ludvb6z3bs.skadnetwork",
        "m297p6643m.skadnetwork",
        "m5mvw97r93.skadnetwork",
        "m8dbw4sv7c.skadnetwork",
        "mj797d8u6f.skadnetwork",
        "mlmmfzh3r3.skadnetwork",
        "mls7yz5dvl.skadnetwork",
        "mp6xlyr22a.skadnetwork",
        "mqn7fxpca7.skadnetwork",
        "mtkv5xtk9e.skadnetwork",
        "n38lu8286q.skadnetwork",
        "n66cz3y3bx.skadnetwork",
        "n6fk4nfna4.skadnetwork",
        "n9x2a789qt.skadnetwork",
        "ns5j362hk7.skadnetwork",
        "nzq8sh4pbs.skadnetwork",
        "p78axxw29g.skadnetwork",
        "ppxm28t8ap.skadnetwork",
        "prcb7njmu6.skadnetwork",
        "pu4na253f3.skadnetwork",
        "pwa73g5rt2.skadnetwork",
        "pwdxu55a5a.skadnetwork",
        "qqp299437r.skadnetwork",
        "qu637u8glc.skadnetwork",
        "qwpu75vrh2.skadnetwork",
        "r45fhb6rf7.skadnetwork",
        "rvh3l7un93.skadnetwork",
        "rx5hdcabgc.skadnetwork",
        "s39g8k73mm.skadnetwork",
        "s69wq72ugq.skadnetwork",
        "su67r6k2v3.skadnetwork",
        "t38b2kh725.skadnetwork",
        "t6d3zquu66.skadnetwork",
        "tl55sbb4fm.skadnetwork",
        "tvvz7th9br.skadnetwork",
        "u679fj5vs4.skadnetwork",
        "uw77j35x4d.skadnetwork",
        "v4nxqhlyqp.skadnetwork",
        "v72qych5uu.skadnetwork",
        "v79kvwwj4g.skadnetwork",
        "v9wttpbfk9.skadnetwork",
        "vcra2ehyfk.skadnetwork",
        "vhf287vqwu.skadnetwork",
        "vutu7akeur.skadnetwork",
        "w9q455wk68.skadnetwork",
        "wg4vff78zm.skadnetwork",
        "wzmmz9fp6w.skadnetwork",
        "x44k69ngh6.skadnetwork",
        "x5l83yy675.skadnetwork",
        "x8jxxk4ff5.skadnetwork",
        "x8uqf25wch.skadnetwork",
        "xga6mpmplv.skadnetwork",
        "xy9t38ct57.skadnetwork",
        "y45688jllp.skadnetwork",
        "y5ghdn5j9k.skadnetwork",
        "yclnxrl5pm.skadnetwork",
        "ydx93a7ass.skadnetwork",
        "z4gj7hsk7h.skadnetwork",
        "z959bm4gru.skadnetwork",
        "zmvfpc5aq8.skadnetwork",
        "zq492l623r.skadnetwork"
    };

    private static readonly List<string> Networks = new List<string>
        {
            "AdColony",
            //"Amazon",
            "ByteDance",
            //"Chartboost",
            "Facebook",
            "Fyber",
            "Google",
            //"InMobi",
            "IronSource",
            //"Maio",
            "Mintegral",
            //"MyTarget",
            //"MoPub",
            //"Nend",
            //"Ogury",
            //"Smaato",
            "Tapjoy",
            //"TencentGDT",
            "UnityAds",
            //"VerizonAds",
            "Vungle",
            //"Yandex"
        };

    private static SkAdNetworkData GetSkAdNetworkData()
    {
        var uriBuilder = new UriBuilder("https://dash.applovin.com/docs/v1/unity_integration_manager/sk_ad_networks_info");

        // Get the list of installed ad networks to be passed up
        
        var adNetworks = string.Join(",", Networks);
        if (!string.IsNullOrEmpty(adNetworks))
        {
            uriBuilder.Query += string.Format("adnetworks={0}", adNetworks);
        }
        

        var unityWebRequest = UnityWebRequest.Get(uriBuilder.ToString());

#if UNITY_2017_2_OR_NEWER
        var operation = unityWebRequest.SendWebRequest();
#else
            var operation = unityWebRequest.Send();
#endif
        // Wait for the download to complete or the request to timeout.
        while (!operation.isDone) { }

#if UNITY_2017_2_OR_NEWER
        if (unityWebRequest.isNetworkError || unityWebRequest.isHttpError)
#else
            if (unityWebRequest.isError)
#endif
        {
            Debug.Log("Failed to retrieve SKAdNetwork IDs with error: " + unityWebRequest.error);
            return new SkAdNetworkData();
        }

        try
        {
            return JsonUtility.FromJson<SkAdNetworkData>(unityWebRequest.downloadHandler.text);
        }
        catch (Exception exception)
        {
            Debug.Log("Failed to parse data '" + unityWebRequest.downloadHandler.text + "' with exception: " + exception);
            return new SkAdNetworkData();
        }
    }


    [PostProcessBuild(999)]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string path)
    {
        if (buildTarget == BuildTarget.iOS)
        {
            string projectPath = path + "/Unity-iPhone.xcodeproj/project.pbxproj";

            PBXProject pbxProject = new PBXProject();
            pbxProject.ReadFromFile(projectPath);

            var plistPath = Path.Combine(path, "Info.plist");
            PlistDocument plist = new PlistDocument();
            plist.ReadFromFile(plistPath);
            if(plist.root.values.ContainsKey("SKAdNetworkItems"))
            {
                Debug.Log("ready to delete the old SKAdNetworkItems");
                plist.root.values.Remove("SKAdNetworkItems");
            }
            PlistElementArray skAdNetworkItems = plist.root.CreateArray("SKAdNetworkItems");
            var skAdNetworkIds = GetSkAdNetworkData();
            if (null != skAdNetworkIds && null != skAdNetworkIds.SkAdNetworkIds)
            {
                foreach (string skAdNetworkId in skAdNetworkIds.SkAdNetworkIds)
                {
                    PlistElementDict dict = skAdNetworkItems.AddDict();
                    dict.SetString("SKAdNetworkIdentifier", skAdNetworkId);
                }
            } 
            else
            {
                foreach (var skAdNetworkId in localSkAdNetworkIds)
                {
                    PlistElementDict dict = skAdNetworkItems.AddDict();
                    dict.SetString("SKAdNetworkIdentifier", skAdNetworkId);
                }
            }
            plist.WriteToFile(plistPath);
        }
    }

}

#endif

