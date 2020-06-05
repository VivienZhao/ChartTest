using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BarChart))]
public class BarChartEditor : ChartBaseEditor
{

    protected override void OnEnable()
    {
        chartBase = (ChartBase)target;
    }

    protected override void SetBtnNames()
    {
        bigAddBtnName = "增加一个柱形集合";
        bigReduceBtnName = "删除一个柱形集合";
        //smallAddBtnName = "";
        //smallReduceBtnName = "";
        bigBtnInfo = "第{0}个柱形集合：";
        smallBtnInfo = "第{0}个柱形：";
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

    }

    protected override void AddBigInfoAction()
    {
        chartBase.pointsInfos.Add(new PointsInfo(new Vector3[1], Vector3.one));
    }

    protected override void ReduceBigInfoAction()
    {
        if (chartBase.pointsInfos.Count > 0)
        {
            chartBase.pointsInfos.RemoveAt(chartBase.pointsInfos.Count - 1);
        }
    }

    protected override void SettingInfos(int index)
    {

    }

    protected override void ShowInfos(int index)
    {
        for (int j = 0; j < chartBase.pointsInfos[index].points.Length; j++)
        {
            chartBase.pointsInfos[index].points[j] = EditorGUILayout.Vector3Field(string.Format(smallBtnInfo, j), chartBase.pointsInfos[index].points[j]);
        }
        EditorGUILayout.BeginHorizontal(GUI.skin.box);
        chartBase.pointsInfos[index].size = EditorGUILayout.Vector3Field("图表尺寸", chartBase.pointsInfos[index].size);
        EditorGUILayout.EndHorizontal();
    }
}
