using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class BarChart : ChartBase
{

    protected override void SetXyzValueRange()
    {
        List<Vector3> tempSortV3 = pointsInfos[0].points.ToList();
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
    /// 根据顶点和位移差获取最终位置
    /// </summary>
    /// <param name="vertices"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public override Vector3[] SetVerticesOffset(Vector3[] vertices, PointsInfo _points)
    {

        Vector3[] tempV3 = new Vector3[vertices.Length];

        for (int i = 0; i < vertices.Length; i++)
        {
            tempV3[i] = new Vector3(vertices[i].x, 0, vertices[i].z);

            int indexX = (int)(vertices[i].x / _points.size.x);


            if (_points.size.y == vertices[i].y)
            {
                tempV3[i] += new Vector3(0, _points.points[indexX].y, 0);
            }
            tempV3[i] += new Vector3(_points.points[indexX].x, 0, _points.points[indexX].z);

        }

        return tempV3;
    }

}
