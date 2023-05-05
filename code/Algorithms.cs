using System;
using System.Collections.Generic;
using System.Drawing;

namespace LR5
{
    struct VectorC
    {
        public VectorC(PointF begin, PointF end)
        {
            A = begin.X - end.X;
            B = begin.Y - end.Y;
        }
        public float A;
        public float B;
    }
    struct ForSutherlandCohen
    {
        public ForSutherlandCohen(Point min, Point max)
        {
            minPoint = min;
            pMax = max;
        }
        public ForSutherlandCohen(float x_min, float y_min, float x_max, float y_max)
        {
            minPoint = new PointF(x_min, y_min);
            pMax = new PointF(x_max, y_max);
        }
        public PointF minPoint;
        public PointF pMax;
    }
    class Algorithms
    {
        public void SetRectangleClipper(float x_min, float y_min, float x_max, float y_max)
        {
            rect = new ForSutherlandCohen(x_min, y_min, x_max, y_max);
        }
        public string GetCode(PointF a)
        {
            string code = "";
            code += (a.Y > rect.pMax.Y ? '1' : '0');
            code += (a.Y < rect.minPoint.Y ? '1' : '0');
            code += (a.X > rect.pMax.X ? '1' : '0');
            code += (a.X < rect.minPoint.X ? '1' : '0');
            return code;
        }
        public PointF MoveBit(PointF a, float k)
        {
            var code = GetCode(a);
            if (code[2] == '1')
            {
                return new PointF(rect.pMax.X, a.Y + k * (rect.pMax.X - a.X));
            }
            if (code[3] == '1')
            {
                return new PointF(rect.minPoint.X, a.Y + k * (rect.minPoint.X - a.X));
            }
            if (code[0] == '1')
            {
                return new PointF(a.X + (1 / k) * (rect.pMax.Y - a.Y), rect.pMax.Y);
            }
            if (code[1] == '1')
            {
                return new PointF(a.X + (1 / k) * (rect.minPoint.Y - a.Y), rect.minPoint.Y);
            }
            return a;
        }

        public ForSutherlandCohen rect;
        public List<VectorC> vectors;
        public List<KeyValuePair<PointF, PointF>> edges;
        public void SetPolygon(List<KeyValuePair<PointF, PointF>> list)
        {
            vectors = new List<VectorC>();
            edges = new List<KeyValuePair<PointF, PointF>>(list);
            for (int i = 0; i < list.Count; i++)
            {
                vectors.Add(new VectorC(list[i].Key, list[i].Value));
            }

        }

        public bool SutherlandCohen(PointF a, PointF b, ref PointF p1, ref PointF p2)
        {
            string aCode = GetCode(a);
            string bCode = GetCode(b);
            var k = (a.Y - b.Y) / (a.X - b.X);
            while (true)
            {
                if (aCode == "0000" && bCode == "0000") // Both points are inside
                {
                    p1 = a;
                    p2 = b;
                    return true;
                }
                else if ((Convert.ToInt32(aCode, 2) & Convert.ToInt32(bCode, 2)) != 0) // Both points are outside the same region
                {
                    return false;
                }
                else if (aCode != "0000") // a is outside
                {
                    if ((Convert.ToInt32(aCode, 2) & Convert.ToInt32("1000", 2)) == 8) // Top
                    {
                        a = MoveBit(a, k);
                    }
                    else if ((Convert.ToInt32(aCode, 2) & Convert.ToInt32("0100", 2)) == 4) // Bottom
                    {
                        a = MoveBit(a, k);
                    }
                    else if ((Convert.ToInt32(aCode, 2) & Convert.ToInt32("0010", 2)) == 2) // Right
                    {
                        a = MoveBit(a, k);
                    }
                    else if ((Convert.ToInt32(aCode, 2) & Convert.ToInt32("0001", 2)) == 1) // Left
                    {
                        a = MoveBit(a, k);
                    }
                    aCode = GetCode(a);
                }
                else if (bCode != "0000") // b is outside
                {
                    if ((Convert.ToInt32(bCode, 2) & Convert.ToInt32("1000", 2)) == 8) // Top
                    {
                        b = MoveBit(b, k);
                    }
                    else if ((Convert.ToInt32(bCode, 2) & Convert.ToInt32("0100", 2)) == 4) // Bottom
                    {
                        b = MoveBit(b, k);
                    }
                    else if ((Convert.ToInt32(bCode, 2) & Convert.ToInt32("0010", 2)) == 2) // Right
                    {
                        b = MoveBit(b, k);
                    }
                    else if ((Convert.ToInt32(bCode, 2) & Convert.ToInt32("0001", 2)) == 1) // Left
                    {
                        b = MoveBit(b, k);
                    }
                    bCode = GetCode(b);
                }
            }
        }

        public List<PointF> CyrusBeck(PointF p1, PointF p2, List<PointF> polygon)
        {
            List<PointF> intersectionPoints = new List<PointF>();
            int n = polygon.Count;
            List<PointF> normalVectors = new List<PointF>();
            for (int i = 0; i < n; i++)
            {
                PointF p3 = polygon[i];
                PointF p4 = polygon[(i + 1) % n];
                float nx = p4.Y - p3.Y;
                float ny = p3.X - p4.X;
                normalVectors.Add(new PointF(nx, ny));
            }

            PointF d = new PointF(p2.X - p1.X, p2.Y - p1.Y);
            List<float> dotProducts = new List<float>();
            for (int i = 0; i < n; i++)
            {
                PointF p3 = polygon[i];
                float nx = normalVectors[i].X;
                float ny = normalVectors[i].Y;
                float dotProduct = nx * d.X + ny * d.Y;
                dotProducts.Add(dotProduct);
            }
            List<float> dotProductsP1 = new List<float>();
            for (int i = 0; i < n; i++)
            {
                PointF p3 = polygon[i];
                float nx = normalVectors[i].X;
                float ny = normalVectors[i].Y;
                float dx = p3.X - p1.X;
                float dy = p3.Y - p1.Y;
                float dotProductP1 = nx * dx + ny * dy;
                dotProductsP1.Add(dotProductP1);
            }
            List<float> ts = new List<float>();
            for (int i = 0; i < n; i++)
            {
                if (dotProducts[i] != 0)
                {
                    float t = dotProductsP1[i] / dotProducts[i];
                    ts.Add(t);
                }
                else
                {
                    ts.Add(float.NaN);
                }
            }

            for (int i = 0; i < n; i++)
            {
                if (!float.IsNaN(ts[i]) && ts[i] >= 0 && ts[i] <= 1)
                {
                    PointF intersectionPoint = new PointF(p1.X + ts[i] * d.X, p1.Y + ts[i] * d.Y);
                    intersectionPoints.Add(intersectionPoint);
                }
            }

            bool p1Inside = PointInPolygon(p1, polygon);
            bool p2Inside = PointInPolygon(p2, polygon);
            if (p1Inside && p2Inside)
            {
                intersectionPoints.Add(p1);
                intersectionPoints.Add(p2);
                return intersectionPoints;
            }
            else if (p1Inside)
            {
                intersectionPoints.Add(p1);
            }
            else if (p2Inside)
            {
                intersectionPoints.Add(p2);
            }
            return intersectionPoints;
        }
        bool PointInPolygon(PointF point, List<PointF> polygon)
        {
            int count = 0;
            int n = polygon.Count;
            for (int i = 0; i < n; i++)
            {
                PointF p1 = polygon[i];
                PointF p2 = polygon[(i + 1) % n];
                if ((p1.Y <= point.Y && point.Y < p2.Y || p2.Y <= point.Y && point.Y < p1.Y) &&
                    point.X < (p2.X - p1.X) * (point.Y - p1.Y) / (p2.Y - p1.Y) + p1.X)
                {
                    count++;
                }
            }
            return count % 2 != 0;
        }

    }

} 
