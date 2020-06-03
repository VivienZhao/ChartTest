using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LineChart))]
public class LineChartEditor : ChartBaseEditor
{

    protected override void OnEnable()
    {
        chartBase = (ChartBase)target;
    }
    protected override void SetBtnNames()
    {
        bigAddBtnName = "增加一条折线";
        bigReduceBtnName = "删除一条折线";
        smallAddBtnName = "增加一个分段";
        smallReduceBtnName = "删除一个分段";
        bigBtnInfo = "第{0}个折线分段：";
        smallBtnInfo = "第{0}个折线分段：";
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

    }

    protected override void AddSmallInfoAction(int index)
    {
        Vector3[] tempV3s = new Vector3[chartBase.pointsInfos[index].points.Length + 1];
        for (int j = 0; j < tempV3s.Length; j++)
        {
            if (j <= chartBase.pointsInfos[index].points.Length - 1)
            {
                tempV3s[j] = chartBase.pointsInfos[index].points[j];
            }
        }
        chartBase.pointsInfos[index].points = tempV3s;
    }

    protected override void RemoveBigInfoAction(int index)
    {
        if (chartBase.pointsInfos[index].points.Length - 1 > 0)
        {
            Vector3[] tempV3s = new Vector3[chartBase.pointsInfos[index].points.Length - 1];
            for (int j = 0; j < tempV3s.Length; j++)
            {
                tempV3s[j] = chartBase.pointsInfos[index].points[j];
            }
            chartBase.pointsInfos[index].points = tempV3s;
        }
    }

    protected override void ShowInfos(int index)
    {
        for (int j = 0; j < chartBase.pointsInfos[index].points.Length; j++)
        {
            chartBase.pointsInfos[index].points[j] = EditorGUILayout.Vector3Field(string.Format(smallBtnInfo, j), chartBase.pointsInfos[index].points[j]);
        }
        EditorGUILayout.BeginVertical(GUI.skin.box);
        chartBase.pointsInfos[index].size = EditorGUILayout.Vector3Field("图表尺寸", chartBase.pointsInfos[index].size);
        EditorGUILayout.EndVertical();
    }

}