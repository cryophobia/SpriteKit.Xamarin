using System;
using System.Drawing;

namespace Tut001
{
	public static class MathHelpers
	{
		public static PointF PointAdd(PointF a, PointF b){
			return new PointF(a.X + b.X, a.Y + b.Y);
		}

		public static PointF PointSubtract(PointF a, PointF b){
			return new PointF(a.X - b.X, a.Y - b.Y);
		}

		public static PointF PointMultiplyScalar(PointF a, double b){
			return new PointF(a.X * (float)b, a.Y * (float)b);
		}

		public static float PointLength(PointF a){
			return (float) Math.Sqrt(a.X * a.X + a.Y * a.Y);
		}

		public static PointF PointNormalize(PointF a){
			float length = PointLength(a);
			return new PointF(a.X / length, a.Y / length);
		}

		public static float PointToAngle(PointF a){
			return (float) Math.Atan2(a.Y, a.X);
		}

		public static float ScalarSign(float a){
			return a >= 0 ? 1 : -1;
		}

		public static float ScalarShortestAngleBetween(float a, float b){
			var diff = b - a;
			var angle = diff % (Math.PI * 2);
			if (angle >= Math.PI){
				angle -= Math.PI * 2;
			}
			return (float)angle;
		}
	}
}

