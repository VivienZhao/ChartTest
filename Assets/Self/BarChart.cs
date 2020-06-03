using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BarChart : ChartBase
{

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
