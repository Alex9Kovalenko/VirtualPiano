using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;
using System.IO;

namespace SynthFromScratch
{
    public partial class Synth : Form
    {
        private const int SAMPLE_RATE = 44100;
        private const short BITS_PER_SAMPLE = 16;

        public Synth()
        {
            InitializeComponent();
        }

        private void Synth_KeyDown(object sender, KeyEventArgs e)
        {
            short[] wave = new short[SAMPLE_RATE];
            byte[] binaryWave = new byte[SAMPLE_RATE * sizeof(short)];

            float frequency = 440f;

            for (int i = 0; i < SAMPLE_RATE; ++i)
            {
                wave[i] = Convert.ToInt16(short.MaxValue * Math.Sin(Math.PI * 2 * frequency * i / SAMPLE_RATE));
            }

            Buffer.BlockCopy(wave, 0, binaryWave, 0, wave.Length * sizeof(short));

            MemoryStream memoryStream = new MemoryStream();
            BinaryWriter binaryWriter = new BinaryWriter(memoryStream);

            short blockAlign = BITS_PER_SAMPLE / 8; // bytes per sample
            int subChunk2Size = SAMPLE_RATE * blockAlign;   // number of bytes of the data

            binaryWriter.Write(new[] { 'R', 'I', 'F', 'F' });
            binaryWriter.Write(36 + subChunk2Size); // 4 + (8 + SubChunk1Size == 16) + (8 + SubChunk2Size)
            binaryWriter.Write(new[] { 'W', 'A', 'V', 'E' });

            binaryWriter.Write(new[] { 'f', 'm', 't', ' ' });
            binaryWriter.Write(16); // SubChunk1Size; 16 for PCM
            binaryWriter.Write((short)1);   // AudioFormat; 1 for PCM
            binaryWriter.Write((short)1);   // NumChannels; 1 for mono
            binaryWriter.Write(SAMPLE_RATE);
            binaryWriter.Write(SAMPLE_RATE * blockAlign);   // ByteRate
            binaryWriter.Write(blockAlign);
            binaryWriter.Write(BITS_PER_SAMPLE);

            binaryWriter.Write(new[] { 'd', 'a', 't', 'a' });
            binaryWriter.Write(subChunk2Size);
            binaryWriter.Write(binaryWave); // writing the data

            memoryStream.Position = 0;

            new SoundPlayer(memoryStream).Play();
        }
    }
}
