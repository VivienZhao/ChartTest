using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

            tempV3[i] = new Vector3(tempV3[i].x - indexX, tempV3[i].y, tempV3[i].z) + _points.points[indexX];
            // tempV3[i] += _points.points[indexX];

            if (_points.points[indexX].y == tempV3[i].y)//底部点
            {
                if (tempV3[i].z == _points.points[0].z)//区分前后面的点
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

