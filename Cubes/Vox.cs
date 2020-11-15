using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Cubes
{
	/// <summary>
	/// Loads MagicaVoxel VOX files. <see href="https://github.com/ephtracy/voxel-model/blob/master/MagicaVoxel-file-format-vox.txt"/> <see href="https://www.giawa.com/www.giawa.com/magicavoxel-c-importer/index.html"/>
	/// </summary>
	public class Vox
	{
		public static Vox Load(string path)
		{
			return Load(File.OpenRead(path));
		}

		/// <summary>
		/// If this sequence is not present at the start of the file, it is not a valid VOX file.
		/// </summary>
		private static readonly int MagicNumber = BitConverter.ToInt32(Encoding.UTF8.GetBytes("VOX "));

		public static Vox Load(Stream stream)
		{
			using var br = new BinaryReader(stream);

			int magic = br.ReadInt32();
			if (magic != MagicNumber)
			{
				throw new InvalidDataException("Cannot load VOX file: Not a VOX file");
			}

			int version = br.ReadInt32();

			int sizeX = 0;
			int sizeY = 0;
			int sizeZ = 0;
			Voxel[]? voxels = null;
			Colour[] palette = DefaultPalette;

			while (br.BaseStream.Position < br.BaseStream.Length)
			{
				string chunkID = Encoding.UTF8.GetString(br.ReadBytes(4));
				int chunkSize = br.ReadInt32();
				int childrenSize = br.ReadInt32();

				long offset = br.BaseStream.Position;

				// The only important chunks are SIZE, XYZI (actual voxel data), and RGBA (palette) 

				if (chunkID == "SIZE")
				{
					sizeX = br.ReadInt32();
					sizeY = br.ReadInt32();
					sizeZ = br.ReadInt32();
				}
				else if (chunkID == "XYZI")
				{
					int numVoxels = br.ReadInt32();

					voxels = new Voxel[numVoxels];

					for (int i = 0; i < numVoxels; i++)
					{
						byte x = br.ReadByte();
						byte y = br.ReadByte();
						byte z = br.ReadByte();
						byte c = br.ReadByte();

						voxels[i] = new Voxel
						{
							X = x,
							Y = y,
							Z = z,
							C = c
						};
					}
				}
				else if (chunkID == "RGBA")
				{
					// The way the palette is stored is a bit strange.
					// It is made up of 255 RGBA32 colours (the first one is implicitly 0x00000000)
					// however there is still an extra 0x00000000 at the end to bring the total chunk
					// size up to 1024 bytes.

					palette = new Colour[256];
					for (int i = 0; i < 255; i++)
					{
						palette[i + 1] = new Colour
						{
							R = br.ReadByte(),
							G = br.ReadByte(),
							B = br.ReadByte(),
							A = br.ReadByte()
						};
					}
				}

				br.BaseStream.Position = offset + chunkSize;
			}

			return new Vox
			{
				Size = new Vector3i(sizeX, sizeY, sizeZ),
				Voxels = voxels ?? throw new InvalidDataException("Cannot load VOX file: No voxel data"),
				Palette = palette
			};
		}

		/// <summary>
		/// The dimensions of the model.
		/// </summary>
		public Vector3i Size { get; init; }

		public struct Voxel
		{
			public byte X;
			public byte Y;
			public byte Z;

			/// <summary>
			/// Colour as an index into the palette.
			/// </summary>
			public byte C;
		}

		/// <summary>
		/// The list of voxels in the model.
		/// </summary>
		public Voxel[] Voxels { get; init; } = Array.Empty<Voxel>();

		[StructLayout(LayoutKind.Explicit, Size = 4)]
		public struct Colour
		{
			[FieldOffset(0)]
			public byte R;

			[FieldOffset(1)]
			public byte G;

			[FieldOffset(2)]
			public byte B;

			[FieldOffset(3)]
			public byte A;

			[FieldOffset(0)]
			public uint Packed;

			public static implicit operator Colour(uint c)
			{
				return new Colour { Packed = c };
			}
		}

		/// <summary>
		/// The colour palette of RGBA32 colours.
		/// </summary>
		public Colour[] Palette { get; init; } = DefaultPalette;

		/// <summary>
		/// The colour palette used if there is no RGBA chunk.
		/// </summary>
		private static readonly Colour[] DefaultPalette =
		{
			0x00000000, 0xffffffff, 0xffccffff, 0xff99ffff, 0xff66ffff, 0xff33ffff, 0xff00ffff, 0xffffccff, 0xffccccff, 0xff99ccff, 0xff66ccff, 0xff33ccff, 0xff00ccff, 0xffff99ff, 0xffcc99ff, 0xff9999ff,
			0xff6699ff, 0xff3399ff, 0xff0099ff, 0xffff66ff, 0xffcc66ff, 0xff9966ff, 0xff6666ff, 0xff3366ff, 0xff0066ff, 0xffff33ff, 0xffcc33ff, 0xff9933ff, 0xff6633ff, 0xff3333ff, 0xff0033ff, 0xffff00ff,
			0xffcc00ff, 0xff9900ff, 0xff6600ff, 0xff3300ff, 0xff0000ff, 0xffffffcc, 0xffccffcc, 0xff99ffcc, 0xff66ffcc, 0xff33ffcc, 0xff00ffcc, 0xffffcccc, 0xffcccccc, 0xff99cccc, 0xff66cccc, 0xff33cccc,
			0xff00cccc, 0xffff99cc, 0xffcc99cc, 0xff9999cc, 0xff6699cc, 0xff3399cc, 0xff0099cc, 0xffff66cc, 0xffcc66cc, 0xff9966cc, 0xff6666cc, 0xff3366cc, 0xff0066cc, 0xffff33cc, 0xffcc33cc, 0xff9933cc,
			0xff6633cc, 0xff3333cc, 0xff0033cc, 0xffff00cc, 0xffcc00cc, 0xff9900cc, 0xff6600cc, 0xff3300cc, 0xff0000cc, 0xffffff99, 0xffccff99, 0xff99ff99, 0xff66ff99, 0xff33ff99, 0xff00ff99, 0xffffcc99,
			0xffcccc99, 0xff99cc99, 0xff66cc99, 0xff33cc99, 0xff00cc99, 0xffff9999, 0xffcc9999, 0xff999999, 0xff669999, 0xff339999, 0xff009999, 0xffff6699, 0xffcc6699, 0xff996699, 0xff666699, 0xff336699,
			0xff006699, 0xffff3399, 0xffcc3399, 0xff993399, 0xff663399, 0xff333399, 0xff003399, 0xffff0099, 0xffcc0099, 0xff990099, 0xff660099, 0xff330099, 0xff000099, 0xffffff66, 0xffccff66, 0xff99ff66,
			0xff66ff66, 0xff33ff66, 0xff00ff66, 0xffffcc66, 0xffcccc66, 0xff99cc66, 0xff66cc66, 0xff33cc66, 0xff00cc66, 0xffff9966, 0xffcc9966, 0xff999966, 0xff669966, 0xff339966, 0xff009966, 0xffff6666,
			0xffcc6666, 0xff996666, 0xff666666, 0xff336666, 0xff006666, 0xffff3366, 0xffcc3366, 0xff993366, 0xff663366, 0xff333366, 0xff003366, 0xffff0066, 0xffcc0066, 0xff990066, 0xff660066, 0xff330066,
			0xff000066, 0xffffff33, 0xffccff33, 0xff99ff33, 0xff66ff33, 0xff33ff33, 0xff00ff33, 0xffffcc33, 0xffcccc33, 0xff99cc33, 0xff66cc33, 0xff33cc33, 0xff00cc33, 0xffff9933, 0xffcc9933, 0xff999933,
			0xff669933, 0xff339933, 0xff009933, 0xffff6633, 0xffcc6633, 0xff996633, 0xff666633, 0xff336633, 0xff006633, 0xffff3333, 0xffcc3333, 0xff993333, 0xff663333, 0xff333333, 0xff003333, 0xffff0033,
			0xffcc0033, 0xff990033, 0xff660033, 0xff330033, 0xff000033, 0xffffff00, 0xffccff00, 0xff99ff00, 0xff66ff00, 0xff33ff00, 0xff00ff00, 0xffffcc00, 0xffcccc00, 0xff99cc00, 0xff66cc00, 0xff33cc00,
			0xff00cc00, 0xffff9900, 0xffcc9900, 0xff999900, 0xff669900, 0xff339900, 0xff009900, 0xffff6600, 0xffcc6600, 0xff996600, 0xff666600, 0xff336600, 0xff006600, 0xffff3300, 0xffcc3300, 0xff993300,
			0xff663300, 0xff333300, 0xff003300, 0xffff0000, 0xffcc0000, 0xff990000, 0xff660000, 0xff330000, 0xff0000ee, 0xff0000dd, 0xff0000bb, 0xff0000aa, 0xff000088, 0xff000077, 0xff000055, 0xff000044,
			0xff000022, 0xff000011, 0xff00ee00, 0xff00dd00, 0xff00bb00, 0xff00aa00, 0xff008800, 0xff007700, 0xff005500, 0xff004400, 0xff002200, 0xff001100, 0xffee0000, 0xffdd0000, 0xffbb0000, 0xffaa0000,
			0xff880000, 0xff770000, 0xff550000, 0xff440000, 0xff220000, 0xff110000, 0xffeeeeee, 0xffdddddd, 0xffbbbbbb, 0xffaaaaaa, 0xff888888, 0xff777777, 0xff555555, 0xff444444, 0xff222222, 0xff111111
		};
	}
}
