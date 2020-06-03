using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SectorChart))]
public class SectorChartEditor : ChartBaseEditor
{

    protected override void OnEnable()
    {
        chartBase = (ChartBase)target;
        if (chartBase.pointsInfos.Count == 0)
        {
            AddBigInfoAction();
        }
    }
    protected override void SetBtnNames()
    {
        bigAddBtnName = "增加一个圆柱体";
        bigReduceBtnName = "删除一个圆柱体";
        bigBtnInfo = "第{0}个圆柱体：";
        smallBtnInfo = "第{0}个扇形：";
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

    }

    protected override void AddOrDeletePointInfos()
    {
        for (int i = 0; i < chartBase.pointsInfos.Count; i++)
        {
            ItemInfo(i);
        }
    }

    protected override void ItemInfo(int index)
    {
        EditorGUILayout.BeginVertical(GUI.skin.customStyles[1]);
        SettingInfos(index);
        ShowInfos(index);
        EditorGUILayout.EndVertical();
    }

    protected override void AddBigInfoAction()
    {
        chartBase.pointsInfos.Add(new PointsInfo(new Vector3[1], Vector3.one, 0.5f,0.1f,6,new float[6]));
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
        EditorGUILayout.BeginVertical(GUI.skin.box);
        chartBase.pointsInfos[index].size = EditorGUILayout.Vector3Field("图表尺寸", chartBase.pointsInfos[index].size);
        chartBase.pointsInfos[index].sertorCount = EditorGUILayout.IntField("圆形有多少分段（圆滑程度）：", chartBase.pointsInfos[index].sertorCount);
        chartBase.pointsInfos[index].inCirclePercent = EditorGUILayout.FloatField("圆形内环所占半分比：", chartBase.pointsInfos[index].inCirclePercent);
        chartBase.pointsInfos[index].insertValue = EditorGUILayout.FloatField("扇形之间的间隔半分比：", chartBase.pointsInfos[index].insertValue);
        EditorGUILayout.EndVertical();


        EditorGUILayout.BeginVertical(GUI.skin.customStyles[1]);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("增加一个扇形"))
        {
            float[] tempF = new float[chartBase.pointsInfos[index].itemSectorPercent.Length+1];
            for (int i = 0; i < chartBase.pointsInfos[index].itemSectorPercent.Length; i++)
            {
                tempF[i] = chartBase.pointsInfos[index].itemSectorPercent[i];
            }
            chartBase.pointsInfos[index].itemSectorPercent = tempF;
        }
        if (GUILayout.Button("删除一个扇形"))
        {
            if (chartBase.pointsInfos[index].itemSectorPercent.Length > 0)
            {
                float[] tempF = new float[chartBase.pointsInfos[index].itemSectorPercent.Length - 1];
                for (int i = 0; i < tempF.Length; i++)
                {
                    tempF[i] = chartBase.pointsInfos[index].itemSectorPercent[i];
                }
                chartBase.pointsInfos[index].itemSectorPercent = tempF;
            }
        }
        EditorGUILayout.EndHorizontal();

        for (int i = 0; i < chartBase.pointsInfos[index].itemSectorPercent.Length; i++)
        {
            chartBase.pointsInfos[index].itemSectorPercent[i] = EditorGUILayout.FloatField("扇形所占总体的半分比：", chartBase.pointsInfos[index].itemSectorPercent[i]);
        }
        EditorGUILayout.EndVertical();

    }

}