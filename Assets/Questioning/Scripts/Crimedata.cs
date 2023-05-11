using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��^�҂̃��X�g���X�N���v�^�u���I�u�W�F�N�g�ŊǗ��B
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
     * ���݂̖��_
     * public���ǂ��ɂ��������B
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
