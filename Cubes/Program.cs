using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.IO;

namespace Cubes
{
	class Program : GameWindow
	{
		public Program() : base(new GameWindowSettings(), new NativeWindowSettings
		{
			Size = new Vector2i(1920, 1080),
			NumberOfSamples = 4
		})
		{ }

		static void Main(string[] args)
		{
			using var program = new Program();
			program.Run();
		}

		protected override void OnLoad()
		{
			base.OnLoad();

			model = Vox.Load("Content/Models/chr_knight.vox");
			PointCloudRenderer.Point[] points = new PointCloudRenderer.Point[model.Voxels.Length];
			for (int i = 0; i < model.Voxels.Length; i++)
			{
				var v = model.Voxels[i];
				var c = model.Palette[v.C];

				points[i] = new PointCloudRenderer.Point
				{
					// In MagicaVoxel z is up.
					Position = new Vector3(-v.X, v.Z, v.Y),
					Colour = new Color4(c.R, c.G, c.B, c.A)
				};
			}

			pcr = new PointCloudRenderer(points);

			rtr = new RayTracedRenderer(model);

			CursorGrabbed = true;
			VSync = VSyncMode.On;
			//WindowState = WindowState.Maximized;

			UpdateCamera();
			UpdateProjectionMatrix();

			GL.Enable(EnableCap.DepthTest);
			GL.Enable(EnableCap.Multisample);
			GL.Enable(EnableCap.CullFace);

			GL.Enable(EnableCap.Blend);
			GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
		}

		Vox model;

		PointCloudRenderer pcr;
		RayTracedRenderer rtr;

		// Camera data
		Vector3 position = new Vector3(2, 0, 0);
		float yaw = 0;
		float pitch = 0;
		Vector3 camDir;
		Matrix4 viewMatrix;

		Matrix4 projectionMatrix;
		Matrix4 viewProjectionMatrix;

		protected override void OnRenderFrame(FrameEventArgs args)
		{
			base.OnRenderFrame(args);

			UpdateCamera();

			GL.ClearColor(Color4.CornflowerBlue);
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			//Matrix4 modelMatrix = Matrix4.CreateScale(model.Size);
			Matrix4 modelMatrix = Matrix4.CreateScale(1);
			Matrix4 mvp = modelMatrix * viewProjectionMatrix;
			
			rtr.Draw(ref mvp, ref modelMatrix, ref position, ref camDir);

			Matrix4 mvp2 = Matrix4.CreateTranslation(0, 0, 10) * viewProjectionMatrix;
			pcr.Draw(ref mvp2);

			SwapBuffers();
		}

		const float MouseSensitivity = 0.01f;

		private void UpdateCamera()
		{
			yaw += MouseState.Delta.X * MouseSensitivity;
			pitch -= MouseState.Delta.Y * MouseSensitivity;

			pitch = MathHelper.Clamp(pitch, MathHelper.DegreesToRadians(-89.9f), MathHelper.DegreesToRadians(89.9f));

			camDir = new Vector3(
				MathF.Cos(yaw) * MathF.Cos(pitch),
				MathF.Sin(pitch),
				MathF.Sin(yaw) * MathF.Cos(pitch)
 			);

			viewMatrix = Matrix4.LookAt(position, position + camDir, Vector3.UnitY);

			viewProjectionMatrix = viewMatrix * projectionMatrix;
		}

		private void UpdateProjectionMatrix()
		{
			projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(70), (float)Size.X / Size.Y, 0.01f, 100);

			//projectionMatrix = Matrix4.CreateScale(10) * Matrix4.CreateOrthographic(Size.X, Size.Y, 0.01f, 100);
		}

		const float MoveSpeed = 0.05f;

		protected override void OnUpdateFrame(FrameEventArgs args)
		{
			base.OnUpdateFrame(args);

			if (KeyboardState.IsKeyDown(Keys.W))
			{
				position += MoveSpeed * camDir;
			}
			if (KeyboardState.IsKeyDown(Keys.S))
			{
				position -= MoveSpeed * camDir;
			}
			if (KeyboardState.IsKeyDown(Keys.A))
			{
				position -= MoveSpeed * Vector3.Cross(camDir, Vector3.UnitY).Normalized();
			}
			if (KeyboardState.IsKeyDown(Keys.D))
			{
				position += MoveSpeed * Vector3.Cross(camDir, Vector3.UnitY).Normalized();
			}
			if (KeyboardState.IsKeyDown(Keys.Space))
			{
				position.Y += MoveSpeed;
			}
			if (KeyboardState.IsKeyDown(Keys.LeftShift))
			{
				position.Y -= MoveSpeed;
			}
		}

		protected override void OnResize(ResizeEventArgs e)
		{
			base.OnResize(e);

			UpdateProjectionMatrix();
			GL.Viewport(0, 0, Size.X, Size.Y);
		}
	}
}
