using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Cubes
{
	public class PointCloudRenderer
	{
		[StructLayout(LayoutKind.Sequential)]
		public struct Point
		{
			public Vector3 Position;
			public Color4 Colour;
		}

		int vao;
		int buffer;

		int pointCount;

		Shader shader;

		public PointCloudRenderer(ReadOnlySpan<Point> points)
		{
			pointCount = points.Length;

			shader = Shader.Load("Point cloud", "Content/Shaders/PointCloud.vs", "Content/Shaders/PointCloud.fs");

			vao = GL.GenVertexArray();
			GL.BindVertexArray(vao);
			
			buffer = GL.GenBuffer();
			GL.BindBuffer(BufferTarget.ArrayBuffer, buffer);

			GL.BufferData(BufferTarget.ArrayBuffer, points.Length * Marshal.SizeOf<Point>(), ref MemoryMarshal.GetReference(points), BufferUsageHint.StaticDraw);

			GL.EnableVertexAttribArray(0);
			GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Marshal.SizeOf<Point>(), 0);

			GL.EnableVertexAttribArray(1);
			GL.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, Marshal.SizeOf<Point>(), Vector3.SizeInBytes);
		}

		public void Draw(ref Matrix4 transform)
		{
			GL.UseProgram(shader.Program);
			GL.UniformMatrix4(0, false, ref transform);

			GL.BindVertexArray(vao);

			GL.PointSize(4);
			GL.DrawArrays(PrimitiveType.Points, 0, pointCount);
		}
	}
}
