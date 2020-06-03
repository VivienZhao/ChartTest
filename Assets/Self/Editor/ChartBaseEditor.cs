using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ChartBase))]
public class ChartBaseEditor : Editor
{
    protected ChartBase chartBase;

    protected string bigAddBtnName;
    protected string bigReduceBtnName;

    protected string smallAddBtnName;
    protected string smallReduceBtnName;

    protected string bigBtnInfo;
    protected string smallBtnInfo;



    protected virtual void OnEnable()
    {
        chartBase = (ChartBase)target;

    }

    protected virtual void SetBtnNames()
    {
        bigAddBtnName = "";
        bigReduceBtnName = "";
        smallAddBtnName = "";
        smallReduceBtnName = "";
        bigBtnInfo = "第{0}个柱形集合：";
        smallBtnInfo = "第{0}个柱形：";
    }

    public override void OnInspectorGUI()
    {
        SetBtnNames();
        base.OnInspectorGUI();

        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUILayout.Space();

        AddOrDeletePointInfos();

        EditorGUILayout.EndVertical();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }

    }

    protected virtual void AddOrDeletePointInfos()
    {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button(bigAddBtnName))
        {
            AddBigInfoAction();
        }
        if (GUILayout.Button(bigReduceBtnName))
        {
            ReduceBigInfoAction();
        }
        EditorGUILayout.EndHorizontal();

        for (int i = 0; i < chartBase.pointsInfos.Count; i++)
        {
            ItemInfo(i);
        }
    }

    protected virtual void AddBigInfoAction()
    {
        chartBase.pointsInfos.Add(new PointsInfo(new Vector3[1], Vector3.one));
    }

    protected virtual void ReduceBigInfoAction()
    {
        if (chartBase.pointsInfos.Count > 0)
        {
            chartBase.pointsInfos.RemoveAt(chartBase.pointsInfos.Count - 1);
        }
    }

    protected virtual void ItemInfo(int index)
    {
        EditorGUILayout.LabelField(string.Format(bigBtnInfo, index));
        EditorGUILayout.BeginVertical(GUI.skin.customStyles[1]);
        SettingInfos(index);
        ShowInfos(index);
        EditorGUILayout.EndVertical();
    }

    protected virtual void SettingInfos(int index)
    {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button(smallAddBtnName))
        {
            AddSmallInfoAction(index);
        }
        if (GUILayout.Button(smallReduceBtnName))
        {
            RemoveBigInfoAction(index);
        }
        EditorGUILayout.EndHorizontal();
    }

    protected virtual void AddSmallInfoAction(int index)
    {

    }

    protected virtual void RemoveBigInfoAction(int index)
    {

    }


    protected virtual void ShowInfos(int index)
    {

    }
}
