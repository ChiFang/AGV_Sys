using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpGL;
using LidarTool;
//using LidarTool;

namespace OpenGLTool
{
    class Draw3D : GLDrawObject
    {
        public static void Draw3DInformation(SharpGL.OpenGL gl_object, float[] X, float[] Y, float[] Z, DetectedRectRegion DetectField)
        {
            double MaxDistance = 0;
            double r = 0, g = 0, b = 0;
            gl_object.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl_object.LoadIdentity();
            gl_object.Translate(_LX, _LY, _LZ);
            gl_object.Rotate(_RoX, 0.0, 1.0, 0.0);
            gl_object.Rotate(_RoY, 1.0, 0.0, 0.0);
            gl_object.Rotate(_RoZ, 0.0, 0.0, 1.0);

            //畫光達自己的位置
            gl_object.Begin(OpenGL.GL_TRIANGLES);
            gl_object.Color(1.0, 1.0, 1.0);
            gl_object.Vertex(-0.2, -0.15, 0);
            gl_object.Vertex(0, 0.2, 0);
            gl_object.Vertex(0.2, -0.15, 0);
            gl_object.End();

            drawAxis(gl_object);

            //畫D場景
            gl_object.Begin(OpenGL.GL_POINTS);
            for (int i = 0; i < 23040; i++)
            {
                if (DetectField.DetectRegionX1 < X[i] && X[i] < DetectField.DetectRegionX2 && DetectField.DetectRegionY1 < Y[i] && Y[i] < DetectField.DetectRegionY2 && DetectField.DetectRegionZ1 < Z[i] && Z[i] < DetectField.DetectRegionZ2)
                    continue;
                if (X[i] != 0 && Y[i] != 0 && Z[i] != 0)
                {
                    //用XY距離計算顯示顏色
                    double XYDistance = Math.Sqrt(X[i] * X[i] + Y[i] * Y[i]);
                    if (XYDistance > MaxDistance) MaxDistance = XYDistance;
                    if (XYDistance > 10) XYDistance = 10;
                    XYDistance = XYDistance / 10 * (360 - 30) + 30;
                    HsvToRgb(XYDistance, 1, 1, out r, out g, out b);
                    gl_object.Color(r, g, b);
                    //畫上點座標
                    gl_object.Vertex(X[i], Y[i], Z[i]);
                }
            }
            gl_object.End();
            gl_object.Flush();
        }

        public static void drawAxis(SharpGL.OpenGL gl_object)
        {
            gl_object.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            gl_object.LoadIdentity();
            gl_object.Translate(_LX, _LY, _LZ);
            gl_object.Rotate(_RoX, 0.0, 1.0, 0.0);
            gl_object.Rotate(_RoY, 1.0, 0.0, 0.0);
            gl_object.Rotate(_RoZ, 0.0, 0.0, 1.0);

            //畫Z軸
            gl_object.Color(1.0, 1.0, 1.0);
            gl_object.Begin(OpenGL.GL_TRIANGLE_STRIP);
            gl_object.Vertex(-0.02, -0.02, -0.02);
            gl_object.Vertex(-0.02, -0.02, 10);
            gl_object.Vertex(0.02, -0.02, -0.02);
            gl_object.Vertex(0.02, -0.02, 10);
            gl_object.Vertex(0.02, 0.02, -0.02);
            gl_object.Vertex(0.02, 0.02, 10);
            gl_object.Vertex(-0.02, 0.02, -0.02);
            gl_object.Vertex(-0.02, 0.02, 10);
            gl_object.Vertex(-0.02, -0.02, -0.02);
            gl_object.Vertex(-0.02, -0.02, 10);
            gl_object.End();
            //up
            gl_object.Begin(OpenGL.GL_TRIANGLE_STRIP);
            gl_object.Vertex(-0.02, -0.02, 10);
            gl_object.Vertex(0.02, -0.02, 10);
            gl_object.Vertex(-0.02, 0.02, 10);
            gl_object.Vertex(0.02, 0.02, 10);
            gl_object.End();

            //down
            gl_object.Begin(OpenGL.GL_TRIANGLE_STRIP);
            gl_object.Vertex(-0.02, -0.02, -0.02);
            gl_object.Vertex(0.02, -0.02, -0.02);
            gl_object.Vertex(-0.02, 0.02, -0.02);
            gl_object.Vertex(0.02, 0.02, -0.02);
            gl_object.End();

            //畫X軸
            gl_object.Color(0, 1.0, 0);
            gl_object.Begin(OpenGL.GL_TRIANGLE_STRIP);
            gl_object.Vertex(0.02, -0.02, -0.02);
            gl_object.Vertex(10, -0.02, -0.02);
            gl_object.Vertex(0.02, 0.02, -0.02);
            gl_object.Vertex(10, 0.02, -0.02);
            gl_object.Vertex(0.02, 0.02, 0.02);
            gl_object.Vertex(10, 0.02, 0.02);
            gl_object.Vertex(0.02, -0.02, 0.02);
            gl_object.Vertex(10, -0.02, 0.02);
            gl_object.Vertex(0.02, -0.02, -0.02);
            gl_object.Vertex(10, -0.02, -0.02);
            gl_object.End();
            //up
            gl_object.Begin(OpenGL.GL_TRIANGLE_STRIP);
            gl_object.Vertex(10, -0.02, -0.02);
            gl_object.Vertex(10, 0.02, -0.02);
            gl_object.Vertex(10, -0.02, 0.02);
            gl_object.Vertex(10, 0.02, 0.02);
            gl_object.End();

            //down
            gl_object.Begin(OpenGL.GL_TRIANGLE_STRIP);
            gl_object.Vertex(0.02, -0.02, -0.02);
            gl_object.Vertex(0.02, 0.02, -0.02);
            gl_object.Vertex(0.02, -0.02, 0.02);
            gl_object.Vertex(0.02, 0.02, 0.02);
            gl_object.End();


            //畫Y軸
            gl_object.Color(0, 0, 1.0);
            gl_object.Begin(OpenGL.GL_TRIANGLE_STRIP);
            gl_object.Vertex(-0.02, 0.02, -0.02);
            gl_object.Vertex(-0.02, 10, -0.02);
            gl_object.Vertex(0.02, 0.02, -0.02);
            gl_object.Vertex(0.02, 10, -0.02);
            gl_object.Vertex(0.02, 0.02, 0.02);
            gl_object.Vertex(0.02, 10, 0.02);
            gl_object.Vertex(-0.02, 0.02, 0.02);
            gl_object.Vertex(-0.02, 10, 0.02);
            gl_object.Vertex(-0.02, 0.02, -0.02);
            gl_object.Vertex(-0.02, 10, -0.02);
            gl_object.End();
            //up
            gl_object.Begin(OpenGL.GL_TRIANGLE_STRIP);
            gl_object.Vertex(-0.02, 10, -0.02);
            gl_object.Vertex(0.02, 10, -0.02);
            gl_object.Vertex(-0.02, 10, 0.02);
            gl_object.Vertex(0.02, 10, 0.02);
            gl_object.End();

            //down
            gl_object.Begin(OpenGL.GL_TRIANGLE_STRIP);
            gl_object.Vertex(-0.02, 0.02, -0.02);
            gl_object.Vertex(0.02, 0.02, -0.02);
            gl_object.Vertex(-0.02, 0.02, 0.02);
            gl_object.Vertex(0.02, 0.02, 0.02);
            gl_object.End();

            //gl_object.Flush();
        }

        public static void DrawDetectRectRegion(SharpGL.OpenGL gl_object, DetectedRectRegion DetectField, double ColorR, double ColorG, double ColorB)
        {
            gl_object.LoadIdentity();
            gl_object.Translate(_LX, _LY, _LZ);
            gl_object.Rotate(_RoX, 0.0, 1.0, 0.0);
            gl_object.Rotate(_RoY, 1.0, 0.0, 0.0);
            gl_object.Rotate(_RoZ, 0.0, 0.0, 1.0);

            //畫範圍
            gl_object.Color(ColorR, ColorG, ColorB);
            gl_object.Begin(OpenGL.GL_LINE_STRIP);
            gl_object.Vertex(DetectField.DetectRegionX1, DetectField.DetectRegionY1, DetectField.DetectRegionZ1);
            gl_object.Vertex(DetectField.DetectRegionX2, DetectField.DetectRegionY1, DetectField.DetectRegionZ1);
            gl_object.Vertex(DetectField.DetectRegionX2, DetectField.DetectRegionY2, DetectField.DetectRegionZ1);
            gl_object.Vertex(DetectField.DetectRegionX1, DetectField.DetectRegionY2, DetectField.DetectRegionZ1);
            gl_object.Vertex(DetectField.DetectRegionX1, DetectField.DetectRegionY1, DetectField.DetectRegionZ1);
            gl_object.Vertex(DetectField.DetectRegionX1, DetectField.DetectRegionY1, DetectField.DetectRegionZ2);
            gl_object.Vertex(DetectField.DetectRegionX2, DetectField.DetectRegionY1, DetectField.DetectRegionZ2);
            gl_object.Vertex(DetectField.DetectRegionX2, DetectField.DetectRegionY1, DetectField.DetectRegionZ1);
            gl_object.Vertex(DetectField.DetectRegionX2, DetectField.DetectRegionY1, DetectField.DetectRegionZ2);
            gl_object.Vertex(DetectField.DetectRegionX2, DetectField.DetectRegionY2, DetectField.DetectRegionZ2);
            gl_object.Vertex(DetectField.DetectRegionX2, DetectField.DetectRegionY2, DetectField.DetectRegionZ1);
            gl_object.Vertex(DetectField.DetectRegionX2, DetectField.DetectRegionY2, DetectField.DetectRegionZ2);
            gl_object.Vertex(DetectField.DetectRegionX1, DetectField.DetectRegionY2, DetectField.DetectRegionZ2);
            gl_object.Vertex(DetectField.DetectRegionX1, DetectField.DetectRegionY2, DetectField.DetectRegionZ1);
            gl_object.Vertex(DetectField.DetectRegionX1, DetectField.DetectRegionY2, DetectField.DetectRegionZ2);
            gl_object.Vertex(DetectField.DetectRegionX1, DetectField.DetectRegionY1, DetectField.DetectRegionZ2);
            gl_object.End();
        }

        public static void DrawDetectCircleRegion(SharpGL.OpenGL gl_object, DetectCircleRegion DetectField, double ColorR, double ColorG, double ColorB)
        {
            float x = 0;
            float y = 0;

            gl_object.LoadIdentity();
            gl_object.Translate(_LX, _LY, _LZ);
            gl_object.Rotate(_RoX, 0.0, 1.0, 0.0);
            gl_object.Rotate(_RoY, 1.0, 0.0, 0.0);
            gl_object.Rotate(_RoZ, 0.0, 0.0, 1.0);

            //畫範圍
            gl_object.Color(ColorR, ColorG, ColorB);
            gl_object.Begin(OpenGL.GL_LINE_LOOP);
            for (double i = 0; i < 6.28; i += 0.1744)
            {
                x = DetectField.CenterX + DetectField.Radius * (float)Math.Sin(i);
                y = DetectField.CenterY + DetectField.Radius * (float)Math.Cos(i);
                gl_object.Vertex(x, y, DetectField.ZUp);
            }
            gl_object.End();

            gl_object.Color(ColorR, ColorG, ColorB);
            gl_object.Begin(OpenGL.GL_LINE_LOOP);
            for (double i = 0; i < 6.28; i += 0.1744)
            {
                x = DetectField.CenterX + DetectField.Radius * (float)Math.Sin(i);
                y = DetectField.CenterY + DetectField.Radius * (float)Math.Cos(i);
                gl_object.Vertex(x, y, DetectField.ZDown);
            }

            gl_object.End();

            float ang = 0;
            for (double i = 0; i < 4; i++)
            {
                gl_object.Color(ColorR, ColorG, ColorB);
                gl_object.Begin(OpenGL.GL_LINE_LOOP);
                x = DetectField.CenterX + DetectField.Radius * (float)Math.Sin(ang);
                y = DetectField.CenterY + DetectField.Radius * (float)Math.Cos(ang);
                gl_object.Vertex(x, y, DetectField.ZUp);
                gl_object.Vertex(x, y, DetectField.ZDown);
                ang += 1.57f;
                gl_object.End();
            }


        }

        public static void DrawObstacleInformation(SharpGL.OpenGL gl_object, ObstacleData ObData, double ColorR, double ColorG, double ColorB)
        {
            //gl_object.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl_object.LoadIdentity();
            gl_object.Translate(_LX, _LY, _LZ);
            gl_object.Rotate(_RoX, 0.0, 1.0, 0.0);
            gl_object.Rotate(_RoY, 1.0, 0.0, 0.0);
            gl_object.Rotate(_RoZ, 0.0, 0.0, 1.0);

            //畫D場景
            gl_object.Begin(OpenGL.GL_POINTS);
            gl_object.Color(ColorR, ColorG, ColorB);
            for (int i = 0; i < ObData.ObstaclePosX.Count; i++)
            {
                if (ObData.ObstaclePosX[i] != 0 && ObData.ObstaclePosY[i] != 0 && ObData.ObstaclePosZ[i] != 0)
                {
                    //畫上點座標
                    gl_object.Vertex(ObData.ObstaclePosX[i], ObData.ObstaclePosY[i], ObData.ObstaclePosZ[i]);
                }
            }
            gl_object.End();
            gl_object.Flush();
        }
    }
}
