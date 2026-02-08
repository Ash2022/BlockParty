#if UNITY_ANDROID
using UnityEditor;
using UnityEditor.Callbacks;

public class LongriverSDKSDKAdPluginAndroidPostBuild 
{

    [PostProcessBuild(1)]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string buildPath)
    {
        // if (buildTarget == BuildTarget.Android)
        // {
        //     AppLovinSettings.Instance.SdkKey = LongriverSDKAdPluginHelper.GetSdkKey("android");
        // }
    }
}
#endif
