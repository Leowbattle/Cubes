using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Cubes
{
	public class Shader : IDisposable
	{
		public int Program { get; private set; }

		private Shader() { }

		public void Use()
		{
			GL.UseProgram(Program);
		}

		public static Shader LoadSource(string name, string vsSource, string fsSource)
		{
			var vs = LoadShader(ShaderType.VertexShader, vsSource, "Vertex");
			var fs = LoadShader(ShaderType.FragmentShader, fsSource, "Fragment");

			var program = LoadProgram(name, vs, fs);

			GL.DeleteShader(vs);
			GL.DeleteShader(fs);

			return new Shader { Program = program };
		}

		public static Shader Load(string name, string vsPath, string fsPath)
		{
			string vsSource = File.ReadAllText(vsPath);
			string fsSource = File.ReadAllText(fsPath);

			var vs = LoadShader(ShaderType.VertexShader, vsSource, vsPath);
			var fs = LoadShader(ShaderType.FragmentShader, fsSource, fsPath);

			var program = LoadProgram(name, vs, fs);

			GL.DeleteShader(vs);
			GL.DeleteShader(fs);

			return new Shader { Program = program };
		}

		private static int LoadShader(ShaderType type, string source, string name)
		{
			var shader = GL.CreateShader(type);

			GL.ShaderSource(shader, source);
			GL.CompileShader(shader);

			GL.GetShader(shader, ShaderParameter.CompileStatus, out int compilationStatus);
			if (compilationStatus == 0)
			{
				string log = GL.GetShaderInfoLog(shader);

				throw new ShaderException($"Error loading shader {name}:\n{log}");
			}

			return shader;
		}

		private static int LoadProgram(string name, int vs, int fs)
		{
			var program = GL.CreateProgram();

			GL.AttachShader(program, vs);
			GL.AttachShader(program, fs);

			GL.LinkProgram(program);
			GL.GetProgram(program, GetProgramParameterName.LinkStatus, out int linkStatus);
			if (linkStatus == 0)
			{
				string log = GL.GetProgramInfoLog(program);

				throw new ShaderException($"Error loading shader {name}:\n{log}");
			}

			GL.DetachShader(program, vs);
			GL.DetachShader(program, fs);

			return program;
		}

		public void Dispose()
		{
			GL.DeleteProgram(Program);
		}
	}

	public class ShaderException : Exception
	{
		public override string Message { get; }

		public ShaderException(string message)
		{
			Message = message;
		}
	}
}
