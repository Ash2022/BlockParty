using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class OnlineConfigResult
{
    public bool isSuccess;
    public List<OnlineParamPair> data;
}

[Serializable]
public class OnlineParamPair
{
    public string key;
    public string value;
}
