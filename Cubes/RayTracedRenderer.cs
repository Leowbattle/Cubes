using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubes
{
	public class RayTracedRenderer
	{
		// Geometry to hold a simple cube
		int vao;
		int vbo;

		Shader shader;

		public RayTracedRenderer(Vox model)
		{
			CreateCubeGeometry();

			shader = Shader.Load("Voxel Ray Tracer", "Content/Shaders/VoxelRayTracer.vs", "Content/Shaders/VoxelRayTracer.fs");
		}

		private readonly float[] CubeVertices =
		{
			-0.5f, -0.5f, -0.5f,
			 0.5f, -0.5f, -0.5f,
			 0.5f,  0.5f, -0.5f,
			 0.5f,  0.5f, -0.5f,
			-0.5f,  0.5f, -0.5f,
			-0.5f, -0.5f, -0.5f,               
            // Front face
            -0.5f, -0.5f,  0.5f,
			 0.5f,  0.5f,  0.5f,
			 0.5f, -0.5f,  0.5f,
			 0.5f,  0.5f,  0.5f,
			-0.5f, -0.5f,  0.5f,
			-0.5f,  0.5f,  0.5f,    
            // Left face
            -0.5f,  0.5f,  0.5f,
			-0.5f, -0.5f, -0.5f,
			-0.5f,  0.5f, -0.5f,
			-0.5f, -0.5f, -0.5f,
			-0.5f,  0.5f,  0.5f,
			-0.5f, -0.5f,  0.5f,
            // Right face
             0.5f,  0.5f,  0.5f,
			 0.5f,  0.5f, -0.5f,
			 0.5f, -0.5f, -0.5f,
			 0.5f, -0.5f, -0.5f,
			 0.5f, -0.5f,  0.5f,
			 0.5f,  0.5f,  0.5f,
            // Bottom face      
            -0.5f, -0.5f, -0.5f,
			 0.5f, -0.5f,  0.5f,
			 0.5f, -0.5f, -0.5f,
			 0.5f, -0.5f,  0.5f,
			-0.5f, -0.5f, -0.5f,
			-0.5f, -0.5f,  0.5f,
            // Top face
            -0.5f,  0.5f, -0.5f,
			 0.5f,  0.5f, -0.5f,
			 0.5f,  0.5f,  0.5f,
			 0.5f,  0.5f,  0.5f,
			-0.5f,  0.5f,  0.5f,
			-0.5f,  0.5f, -0.5f,
		};

		void CreateCubeGeometry()
		{
			vao = GL.GenVertexArray();
			GL.BindVertexArray(vao);

			vbo = GL.GenBuffer();
			GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);

			GL.BufferData(BufferTarget.ArrayBuffer, CubeVertices.Length * sizeof(float), CubeVertices, BufferUsageHint.StaticDraw);

			GL.EnableVertexAttribArray(0);
			GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Vector3.SizeInBytes, 0);
		}

		public void Draw(ref Matrix4 transform, ref Matrix4 modelMatrix, ref Vector3 camPos, ref Vector3 camDir)
		{
			GL.UseProgram(shader.Program);
			GL.UniformMatrix4(0, false, ref transform);
			GL.UniformMatrix4(1, false, ref modelMatrix);
			GL.Uniform3(2, ref camPos);
			GL.Uniform3(3, ref camDir);

			GL.BindVertexArray(vao);

			GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
		}
	}
}
