using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 被疑者のリストをスクリプタブルオブジェクトで管理。
/// </summary>
[CreateAssetMenu(fileName ="Data",menuName = "ScriptableObjects/CreateEnemyParamAsset")]
public class Crimedata : ScriptableObject
{
    public List<SuspectInfo> _suspectInfo = new List<SuspectInfo>();
}

[System.Serializable]
public class SuspectInfo
{
    /*
     * 現在の問題点
     * publicをどうにかしたい。
     * 
     */
    public string _name;
    public string _age;
    public string _gender;
    public string _job;
    public string _crime;
    public string _truth;
    [TextArea]
    public string _motive;
    [TextArea]
    public string _background;
}
