using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wwise_pd3
{
	public class WAVE
	{

		public static byte[] JUNK =
		{
		0x06, 0x00, 0x00, 0x00, 0x02, 0x31, 0x00, 0x00, 0x4A, 0x55, 0x4E, 0x4B, 0x04, 0x00, 0x00, 0x00,
		0x00, 0x00, 0x00, 0x00
		};
		public struct Header
		{
			// always 16
			public UInt32 lengthofformatdata;
			// should be 1 for putting audio into pd3
			public UInt16 type;
			// usually 2
			public short channels;
			// 48,000hz for menu background
			public UInt32 samplerate;
			// cant remember
			public int averagebytespersecond;
			public short blockalign;
			// usually 16
			public short bitspersample;
			// data continues
		}

		/// <summary>
		/// Reads the WAV header from a wav file, moves binary reader position to "data"
		/// </summary>
		/// <param name="br"></param>
		/// <returns></returns>
		public static Header ReadHeaderFromWAV(BinaryReader br)
		{
			Header header = new Header();

			// "RIFF"
			br.ReadBytes(4);
			// file size
			br.ReadInt32();
			// "WAVE"
			br.ReadBytes(4);
			// "fmt " format marker
			br.ReadBytes(4);

			header.lengthofformatdata = br.ReadUInt32();

			header.type = br.ReadUInt16();

			header.channels = br.ReadInt16();

			header.samplerate = br.ReadUInt32();

			header.averagebytespersecond = br.ReadInt32();

			header.blockalign = br.ReadInt16();

			header.bitspersample = br.ReadInt16();

			return header;
		}

		public static Header ReadWEMHeaderToWAVHeader(BinaryReader br)
		{
			Header header = new Header();

			br.ReadBytes(4);

			br.ReadInt32();

			br.ReadBytes(4);

			br.ReadBytes(4);

			header.lengthofformatdata = 0x10;

			br.ReadInt32();

			header.type = br.ReadUInt16();

			header.channels = br.ReadInt16();

			header.samplerate = br.ReadUInt32();

			header.averagebytespersecond = br.ReadInt32();

			header.blockalign = br.ReadInt16();

			header.bitspersample = br.ReadInt16();

			br.ReadBytes(JUNK.Length);

			return header;
		}

		public static void WriteWAVHeader(BinaryWriter bw, Header header, bool wem = true)
		{
			bw.Write(Encoding.Default.GetBytes("RIFF"));
			bw.Write((UInt32)0);
			bw.Write(Encoding.Default.GetBytes("WAVE"));

			bw.Write(Encoding.Default.GetBytes("fmt"));

			// double check if all files have 0x20
			bw.Write((char)0x20);

			if (wem)
			{
				bw.Write(header.lengthofformatdata + 8);
			} else
			{
				bw.Write(16);
			}

			// wwise writes 0xFE_FF
			//bw.Write(header.type);

			if (wem)
			{
				bw.Write((byte)0xFE);
				bw.Write((byte)0xFF);
			}else
			{
				bw.Write((byte)0x01);
				bw.Write((byte)0x00);
			}

			bw.Write(header.channels);

			bw.Write(header.samplerate);

			bw.Write(header.averagebytespersecond);

			bw.Write(header.blockalign);

			bw.Write(header.bitspersample);

			if (wem) bw.Write(JUNK);
		}
	}
}
