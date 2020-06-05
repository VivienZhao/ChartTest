using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class BarChart : ChartBase
{

    protected override void SetXyzValueRange()
    {
        List<Vector3> tempSortV3 = new List<Vector3>();
        for (int i = 0; i < pointsInfos.Count; i++)
        {
            tempSortV3.Add(pointsInfos[i].points[0]);
        }

        tempSortV3.Sort((x, y) => { if (x.y > y.y) return 1; else if (x == y) return 0; else return -1; });

        minX = 0;
        maxX = pointsInfos[pointsInfos.Count - 1].points[0].x;

        maxY = tempSortV3[tempSortV3.Count - 1].y;
        minY = 0;

        minZ = 0;
        maxZ = pointsInfos[0].points[0].z;

        xCount = Mathf.Abs((int)(maxX - minX));
        yCount = Mathf.Abs((int)(maxY - minY));
        zCount = Mathf.Abs((int)(maxZ - minZ));

    }

    /// <summary>
    /// 刷新辅助点位置
    /// </summary>
    protected override void ResetSetGuidePoints()
    {
        Vector3[] tempPos = new Vector3[pointsInfos.Count];
        for (int i = 0; i < tempPos.Length; i++)
        {
            tempPos[i] = pointsInfos[i].points[0];
        }
        ResetSetGuidePointsTrans(tempPos);
    }


    /// <summary>
    /// 是否重新生成辅助点的条件
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    protected override bool IsCanInstanceGuidePointObj(out int count)
    {
        count = pointsInfos.Count;
        return guidePointObjs.Count != count;
    }



    protected override Vector3[] GetPointsVector3s(PointsInfo _points)
    {

        List<Vector3> tempPoints = new List<Vector3>();

        for (int i = 0; i < _points.points.Length; i++)
        {
            tempPoints.AddRange(GetCubePointFromPoint(new Vector3(_points.points[i].x, _points.points[i].y / 2f, _points.points[i].z), new Vector3(_points.size.x, _points.points[i].y, _points.size.z)));
        }

        return tempPoints.ToArray();
    }

}
