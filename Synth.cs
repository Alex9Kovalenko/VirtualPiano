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
using System.Windows.Input;

namespace SynthFromScratch
{
    public partial class Synth : Form
    {
        private const int SAMPLE_RATE = 44100;
        private const short BITS_PER_SAMPLE = 16;

        private Dictionary<Keys, SoundPlayer> keysDown = new Dictionary<Keys, SoundPlayer>();

        public Synth()
        {
            InitializeComponent();
        }

        private void Synth_KeyDown(object sender, KeyEventArgs e)
        {
            Keys key = e.KeyCode;

            if (keysDown.ContainsKey(key)) return;

            float frequency = 78f;
            int numSamples = Convert.ToInt32(Math.Ceiling((double)SAMPLE_RATE / frequency));

            short[] wave = new short[numSamples];
            byte[] binaryWave = new byte[numSamples * sizeof(short)];

            for (int i = 0; i < numSamples; ++i)
            {
                wave[i] = Convert.ToInt16(short.MaxValue * Math.Sin(Math.PI * 2 * frequency * i / SAMPLE_RATE));
            }

            Buffer.BlockCopy(wave, 0, binaryWave, 0, wave.Length * sizeof(short));

            MemoryStream memoryStream = new MemoryStream();
            BinaryWriter binaryWriter = new BinaryWriter(memoryStream);

            short blockAlign = BITS_PER_SAMPLE / 8; // bytes per sample
            int subChunk2Size = numSamples * blockAlign;   // number of bytes of the data

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

            SoundPlayer sp = new SoundPlayer(memoryStream);

            sp.PlayLooping();

            keysDown.Add(key, sp);
        }

        private void Synth_KeyUp(object sender, KeyEventArgs e)
        {
            Keys key = e.KeyCode;

            if (keysDown.TryGetValue(key, out SoundPlayer sp))
            {
                sp.Stop();
                keysDown.Remove(key);
            }
        }
    }
}
