using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LongriverSDKNS
{
    [DoNotRename]
    public enum LOGIN_TYPE
    {
        [DoNotRename]
        DEVICE,
        [DoNotRename]
        FACEBOOK,
        [DoNotRename]
        GOOGLE_PLAY,
        [DoNotRename]
        GAME_CENTER,
        [DoNotRename]
        APPLE_SIGN
    }

    public class LOGIN_TYPE_HELPER
    {
        static public LOGIN_TYPE GetLoginTypeWithSyste()
        {
#if UNITY_IOS
            return LOGIN_TYPE.GAME_CENTER;
#else
            return LOGIN_TYPE.GOOGLE_PLAY;
#endif
        }
    }

}

