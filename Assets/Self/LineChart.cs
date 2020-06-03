using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LineChart : ChartBase
{

    List<Vector3> fowardLine = new List<Vector3>();
    List<Vector3> backLine = new List<Vector3>();

    List<Vector3> shouldMovePoss = new List<Vector3>();
    List<Vector3> shouldMoveBackPoss = new List<Vector3>();

    /// <summary>
    /// 底部Y轴坐标延展到0
    /// </summary>
    [Header("底部Y轴坐标是否延展到0")]
    public bool bottomExtension = false;


    public override Vector3[] SetVerticesOffset(Vector3[] vertices, PointsInfo _points)
    {
        shouldMovePoss.Clear();
        shouldMoveBackPoss.Clear();

        fowardLine.Clear();
        backLine.Clear();
        Vector3[] tempV3 = new Vector3[vertices.Length];

        for (int i = 0; i < vertices.Length; i++)
        {
            tempV3[i] = vertices[i];

            int indexX = int.Parse((vertices[i].x / _points.size.x).ToString());

            tempV3[i] += _points.points[indexX];

            if (_points.points[indexX].y == tempV3[i].y)//底部点
            {
                if (tempV3[i].z == 0)//区分前后面的点
                {
                    if (!fowardLine.Contains(tempV3[i]))
                        fowardLine.Add(tempV3[i]);
                }
                else
                {
                    if (!backLine.Contains(tempV3[i]))
                        backLine.Add(tempV3[i]);
                }
            }
        }

        if (!bottomExtension)
        {
            GetShouldPos(fowardLine, _points.size.y, false);

            GetShouldPos(backLine, _points.size.y, true);

            for (int i = 0; i < tempV3.Length; i++)
            {
                for (int j = 1; j < fowardLine.Count - 1; j++)
                {
                    if (tempV3[i] == fowardLine[j])
                    {
                        tempV3[i] = shouldMovePoss[j - 1];
                    }
                }
                for (int j = 1; j < backLine.Count - 1; j++)
                {
                    if (tempV3[i] == backLine[j])
                    {
                        tempV3[i] = shouldMoveBackPoss[j - 1];
                    }
                }
            }
        }
        else
        {
            SetPosYToZero(fowardLine, false);
            SetPosYToZero(backLine, true);

            for (int i = 0; i < tempV3.Length; i++)
            {
                for (int j = 0; j < fowardLine.Count; j++)
                {
                    if (tempV3[i] == fowardLine[j])
                    {
                        tempV3[i] = shouldMovePoss[j];
                    }
                }
                for (int j = 0; j < backLine.Count; j++)
                {
                    if (tempV3[i] == backLine[j])
                    {
                        tempV3[i] = shouldMoveBackPoss[j];
                    }
                }
            }
        }



        return tempV3;
    }

    private void GetShouldPos(List<Vector3> vector3s, float yOffset, bool isBack)
    {
        for (int i = 1; i < vector3s.Count - 1; i++)
        {
            ///上方的对应点
            Vector3 upPoints = vector3s[i] + new Vector3(0, yOffset, 0);

            Vector3 midDir = ((vector3s[i - 1] - vector3s[i]).normalized + (vector3s[i + 1] - vector3s[i]).normalized);

            float angle = Vector3.Angle((vector3s[i - 1] - vector3s[i]).normalized, (vector3s[i + 1] - vector3s[i]).normalized);


            Debug.DrawLine(upPoints, upPoints + midDir);

            Vector3 shouldMovePos = vector3s[i];
            if (midDir != Vector3.zero)
            {
                shouldMovePos = LinePointProjection(vector3s[i], upPoints, upPoints + midDir);
            }


            if (isBack)
            {
                shouldMoveBackPoss.Add(shouldMovePos);
            }
            else
            {
                shouldMovePoss.Add(shouldMovePos);
            }

        }
    }

    private void SetPosYToZero(List<Vector3> vector3s, bool isBack)
    {
        for (int i = 0; i < vector3s.Count; i++)
        {
            if (isBack)
            {
                shouldMoveBackPoss.Add(new Vector3(vector3s[i].x, 0, vector3s[i].z));
            }
            else
            {
                shouldMovePoss.Add(new Vector3(vector3s[i].x, 0, vector3s[i].z));
            }

        }
    }

}

