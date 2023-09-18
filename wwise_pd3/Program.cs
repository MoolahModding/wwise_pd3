using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace wwise_pd3
{
	public class Program
	{
		public static void PrintHelp()
		{
			Console.WriteLine("Usage: wwise_pd3 -decode/-encode <input> <output>");
		}

		static void EncodeToWem(string input, string output)
		{
			BinaryReader br = new BinaryReader(File.OpenRead(input));

			var header = WAVE.ReadHeaderFromWAV(br);

			if (header.type != 1)
			{
				Console.WriteLine(String.Format("PAYDAY 3 only supports PCM, not type {0}", header.type));
			}

			Console.WriteLine(String.Format("Format Length: {0}", header.lengthofformatdata));

			Console.WriteLine(String.Format("Type: {0}", header.type == 1 ? "PCM" : "OTHER"));

			Console.WriteLine(String.Format("Channels: {0}", header.channels));

			Console.WriteLine(String.Format("Sample Rate: {0}", header.samplerate));

			Console.WriteLine(String.Format("Avg. Bytes per second: {0}", header.averagebytespersecond));

			Console.WriteLine(String.Format("Block Align: {0}", header.blockalign));

			Console.WriteLine(String.Format("Bits per Sample: {0}", header.bitspersample));

			//br.Read(data, (int)br.BaseStream.Position, (int)(br.BaseStream.Length - br.BaseStream.Position));
			byte[] data = br.ReadBytes((int)(File.ReadAllBytes(input).LongLength - br.BaseStream.Position));
			//Console.WriteLine(File.ReadAllBytes(input).LongLength);
			//Console.WriteLine(br.BaseStream.Length);

			Console.WriteLine(data.Length);

			br.Close();

			BinaryWriter bw = new BinaryWriter(File.OpenWrite(output));

			WAVE.WriteWAVHeader(bw, header, wem: true);

			bw.Write(data);

			// write file size

			var size = bw.BaseStream.Length;

			Console.WriteLine($"{size} bytes written");

			bw.BaseStream.Position = 0;

			bw.Write((byte)0x52);
			bw.Write((byte)0x49);
			bw.Write((byte)0x46);
			bw.Write((byte)0x46);

			bw.Write(size);

			bw.BaseStream.Position -= 4;

			bw.Write((byte)0x57);
			bw.Write((byte)0x41);
			bw.Write((byte)0x56);
			bw.Write((byte)0x45);

			bw.Close();
		}

		static void DecodeFromWEM(string input, string output)
		{
			BinaryReader br = new BinaryReader(File.OpenRead(input));

			var header = WAVE.ReadWEMHeaderToWAVHeader(br);

			Console.WriteLine(String.Format("Format Length: {0}", header.lengthofformatdata));

			Console.WriteLine(String.Format("Type: {0}", header.type == 1 ? "PCM" : "OTHER"));

			Console.WriteLine(String.Format("Channels: {0}", header.channels));

			Console.WriteLine(String.Format("Sample Rate: {0}", header.samplerate));

			Console.WriteLine(String.Format("Avg. Bytes per second: {0}", header.averagebytespersecond));

			Console.WriteLine(String.Format("Block Align: {0}", header.blockalign));

			Console.WriteLine(String.Format("Bits per Sample: {0}", header.bitspersample));

			byte[] data = br.ReadBytes((int)(File.ReadAllBytes(input).LongLength - br.BaseStream.Position));
			//Console.WriteLine(File.ReadAllBytes(input).LongLength);
			//Console.WriteLine(br.BaseStream.Length);

			//Console.WriteLine(data.Length);

			br.Close();

			BinaryWriter bw = new BinaryWriter(File.OpenWrite(output));

			WAVE.WriteWAVHeader(bw, header, wem: false);

			bw.Write(data);

			// write file size

			var size = bw.BaseStream.Length;

			Console.WriteLine($"{size} bytes written");

			bw.BaseStream.Position = 0;

			bw.Write((byte)0x52);
			bw.Write((byte)0x49);
			bw.Write((byte)0x46);
			bw.Write((byte)0x46);

			bw.Write(size);

			bw.BaseStream.Position -= 4;

			bw.Write((byte)0x57);
			bw.Write((byte)0x41);
			bw.Write((byte)0x56);
			bw.Write((byte)0x45);

			bw.Close();
		}

		public static void Main(string[] args)
		{
			if (args.Length == 0)
			{
				Console.WriteLine("Incorrect arguments!");
				PrintHelp();
				return;
			}

			bool encoding = false;

			switch(args[0])
			{
				case "-encode":
					encoding = true;
					break;
				case "-decode":
					encoding = false;
					break;
				default:
					Console.WriteLine("Please provide a method (encoding or decoding)"); ;
					return;
			}

			if (encoding)
			{
				EncodeToWem(args[1], args[2]);
			} else
			{
				DecodeFromWEM(args[1], args[2]);
				//Console.WriteLine("Decoding is not supported");
			}
		}
	}
}
