using System;
using System.Windows.Forms;
using System.IO;
using NAudio.Wave;
using System.Collections.Generic;
using System.Drawing;

namespace SynthFromScratch
{
    public partial class Synth : Form
    {
        private const int SAMPLE_RATE = 44100;
        private const short BITS_PER_SAMPLE = 16;

        private readonly Dictionary<Keys, WaveOut> keysDown = new Dictionary<Keys, WaveOut>();

        public Synth()
        {
            InitializeComponent();
        }

        private void Synth_KeyDown(object sender, KeyEventArgs e)
        {
            Keys key = e.KeyCode;

            if (keysDown.ContainsKey(key)) return;  // if the key is already down

            float frequency;

            if (!frequencies.TryGetValue(key, out frequency))
            {
                return;
            }

            // mininal sufficient number of samples: number of samples in one wave
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

            WaveFileReader reader = new WaveFileReader(memoryStream);
            LoopStream loop = new LoopStream(reader);
            WaveOut waveOut = new WaveOut();
            waveOut.Init(loop);
            waveOut.Play();

            keysDown.Add(key, waveOut);
        }

        private void Synth_KeyUp(object sender, KeyEventArgs e)
        {
            Keys key = e.KeyCode;

            if (keysDown.TryGetValue(key, out WaveOut waveOut))
            {
                waveOut.Stop();
                waveOut.Dispose();

                keysDown.Remove(key);
            }
        }

        private void Synth_Paint(object sender, PaintEventArgs e)
        {
            const int indent = 10;

            Graphics gObject = CreateGraphics();
            Pen pen = new Pen(new SolidBrush(Color.Black), 3);

            gObject.DrawRectangle(new Pen(new SolidBrush(Color.Aqua)),
                indent, indent, ClientRectangle.Width - 2 * indent, ClientRectangle.Height - 2 * indent);

            const int numWhiteKeys = 10;
            int whiteKeyWidth = (ClientRectangle.Width - 2 * indent) / numWhiteKeys;
            int whiteKeyHeight = Convert.ToInt32(0.8 * ClientRectangle.Height);

            int blackKeyWidth = whiteKeyWidth / 2;
            int blackKeyHeight = Convert.ToInt32(0.6 * whiteKeyHeight);

            int topKeyboardPos = ClientRectangle.Height - indent - whiteKeyHeight;

            for (int i = 0; i < numWhiteKeys; ++i)
            {
                int leftKeyPos = indent + i * whiteKeyWidth;

                // drawing white keys
                gObject.DrawRectangle(pen,
                    leftKeyPos, topKeyboardPos, whiteKeyWidth, whiteKeyHeight);

                int r = (i + 1) % 7;
                if (r == 3 || r == 0 || i == numWhiteKeys - 1)
                {
                    continue;
                }
                else
                {
                    gObject.DrawRectangle(pen,
                        leftKeyPos + Convert.ToInt32(0.75 * whiteKeyWidth), topKeyboardPos, blackKeyWidth, blackKeyHeight);
                }
            }
        }
    }
}
