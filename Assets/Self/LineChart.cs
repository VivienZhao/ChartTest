using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class LineChart : ChartBase
{

    /// <summary>
    /// 底部Y轴坐标延展到0
    /// </summary>
    [Header("底部Y轴坐标是否延展到0")]
    public bool bottomExtension = false;


    /// <summary>
    /// 是否重新生成辅助点的条件
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    protected override bool IsCanInstanceGuidePointObj(out int count)
    {
        count = pointsInfos[0].points.Length;
        return guidePointObjs.Count != count;
    }

    /// <summary>
    /// 刷新辅助点位置
    /// </summary>
    protected override void ResetSetGuidePoints()
    {
        ResetSetGuidePointsTrans(pointsInfos[0].points);
    }

    protected override void SetXyzValueRange()
    {
        List<Vector3> tempSortV3 = pointsInfos[0].points.ToList();
        tempSortV3.Sort((x, y) => { if (x.y > y.y) return 1; else if (x == y) return 0; else return -1; });

        minX = 0;
        maxX = pointsInfos[0].points[pointsInfos[0].points.Length - 1].x;

        maxY = tempSortV3[tempSortV3.Count - 1].y;
        minY = 0;

        minZ = 0;
        maxZ = pointsInfos[0].points[pointsInfos[0].points.Length - 1].z;

        xCount = Mathf.Abs((int)(maxX - minX));
        yCount = Mathf.Abs((int)(maxY - minY));
        zCount = Mathf.Abs((int)(maxZ - minZ));

    }

    protected override Vector3[] GetPointsVector3s(PointsInfo _points)
    {

        List<Vector3> tempPoints = new List<Vector3>();

        for (int i = 1; i < _points.points.Length; i++)
        {
            Vector3 lastPoint = _points.points[i - 1];
            Vector3 curPoint = _points.points[i];

            tempPoints.AddRange(GetCubePointFromPoint(new Vector3((lastPoint.x + curPoint.x) / 2f, 0, (lastPoint.z + curPoint.z) / 2f), new Vector3(curPoint.x - lastPoint.x, _points.size.y, _points.size.z), i == 1, i == _points.points.Length - 1));

        }

        return tempPoints.ToArray();
    }

    public override Vector3[] SetVerticesOffset(Vector3[] vertices, PointsInfo _points)
    {
        List<Vector3> tempV3s = new List<Vector3>();
        for (int i = 0; i < vertices.Length; i++)
        {
            for (int j = 0; j < _points.points.Length; j++)
            {
                if (vertices[i].x == _points.points[j].x)
                {
                    vertices[i].y += _points.points[j].y;
                }
                if (!bottomExtension)
                {
                    if (j != 0 && j != _points.points.Length - 1 && vertices[i].y == _points.points[j].y - _points.size.y / 2f)
                    {
                        Vector3 upPoint = vertices[i] + new Vector3(0, _points.size.y, 0);

                        Vector3 midDir = (_points.points[j - 1] - _points.points[j]).normalized + (_points.points[j + 1] - _points.points[j]).normalized;

                        Debug.DrawLine(upPoint, upPoint + midDir * 10, Color.red);

                        // vertices[i] = upPoint +(_points.points[j].y< _points.points[j+1].y?-1:1)* _points.size.y * midDir.normalized;
                        vertices[i] = LinePointProjection(vertices[i], upPoint, upPoint + midDir);
                    }
                }
                else
                {
                    if (vertices[i].y == _points.points[j].y - _points.size.y / 2f)
                    {
                        vertices[i].y = 0;
                    }
                }

            }

        }

        return vertices;
    }

}

