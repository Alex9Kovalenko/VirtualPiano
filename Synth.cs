using System;
using System.Windows.Forms;
using System.IO;
using NAudio.Wave;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace SynthFromScratch
{
    public partial class Synth : Form
    {
        private const int SAMPLE_RATE = 44100;
        private const short BITS_PER_SAMPLE = 16;

        private readonly Dictionary<Keys, WaveOut> keysDown = new Dictionary<Keys, WaveOut>();

        private Graphics gObject;

        public Synth()
        {
            InitializeComponent();

            gObject = CreateGraphics();
        }

        private void Synth_KeyDown(object sender, KeyEventArgs e)
        {
            Keys key = e.KeyCode;

            if (keysDown.ContainsKey(key)) return;  // if the key is already down

            Tuple<float, int> noteData;

            if (!notes.TryGetValue(key, out noteData))
            {
                return;
            }

            float frequency = noteData.Item1;

            // mininal sufficient number of samples: number of samples in one wave
            int numSamples = Convert.ToInt32(Math.Ceiling((double)SAMPLE_RATE / frequency));

            short[] wave = new short[numSamples];
            byte[] binaryWave = new byte[numSamples * sizeof(short)];

            for (int i = 0; i < numSamples; ++i)
            {
                // wave[i] = Convert.ToInt16(short.MaxValue * Math.Sin(Math.PI * 2 * frequency * i / SAMPLE_RATE)); // sin
                wave[i] = Convert.ToInt16(short.MaxValue * Math.Sign(Math.Sin(Math.PI * 2 * frequency * i / SAMPLE_RATE))); // square
                // wave[i] = Convert.ToInt16(short.MaxValue * ((double)i / SAMPLE_RATE - Math.Floor((double)i / SAMPLE_RATE)));
                
                // stopped at coding the sawtooth
                // TODO: create a checkbox for choosing the waveform
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

            DrawKeyboard();
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

            DrawKeyboard();
        }

        private void DrawKeyboard()
        {
            gObject.Clear(Color.White);

            const int indent = 10;

            Pen pen = new Pen(new SolidBrush(Color.Black), 3);

            gObject.DrawRectangle(new Pen(new SolidBrush(Color.Aqua)),
                indent, indent, ClientRectangle.Width - 2 * indent, ClientRectangle.Height - 2 * indent);

            const int numWhiteKeys = 10;    // hardcoded because depends on the hardware (only 10 keys at the bottom row)
            int whiteKeyWidth = (ClientRectangle.Width - 2 * indent) / numWhiteKeys;
            int whiteKeyHeight = Convert.ToInt32(0.8 * ClientRectangle.Height);

            int blackKeyWidth = whiteKeyWidth / 2;
            int blackKeyHeight = Convert.ToInt32(0.6 * whiteKeyHeight);

            int topKeyboardPos = ClientRectangle.Height - indent - whiteKeyHeight;

            Keys[] pressedKeys = keysDown.Keys.ToArray();

            if (pressedKeys != null)
            {
                Tuple<float, int> noteData;
                foreach (Keys key in pressedKeys)
                {
                    if (notes.TryGetValue(key, out noteData) && noteData.Item2 > 0)
                    {  
                        // drawing pressed white keys
                        gObject.FillRectangle(new SolidBrush(Color.Gray),
                            indent + (noteData.Item2 - 1) * whiteKeyWidth, topKeyboardPos, whiteKeyWidth, whiteKeyHeight);
                    }
                }
            }

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
                    // drawing black keys
                    gObject.FillRectangle(pen.Brush,
                        leftKeyPos + Convert.ToInt32(0.75 * whiteKeyWidth), topKeyboardPos, blackKeyWidth, blackKeyHeight);
                }
            }

            if (pressedKeys != null)
            {
                Tuple<float, int> noteData;
                foreach (Keys key in pressedKeys)
                {
                    if (notes.TryGetValue(key, out noteData) && noteData.Item2 < 0)
                    {
                        // drawing pressed black keys
                        gObject.FillRectangle(new SolidBrush(Color.Gray),
                            indent + (-noteData.Item2 - 1) * whiteKeyWidth + Convert.ToInt32(0.75 * whiteKeyWidth), topKeyboardPos, blackKeyWidth, blackKeyHeight);

                        // drawing black borders of pressed black keys
                        gObject.DrawRectangle(pen,
                            indent + (-noteData.Item2 - 1) * whiteKeyWidth + Convert.ToInt32(0.75 * whiteKeyWidth), topKeyboardPos, blackKeyWidth, blackKeyHeight);
                    }
                }
            }
        }

        private void Synth_Shown(object sender, EventArgs e)
        {
            DrawKeyboard();
        }
    }
}
