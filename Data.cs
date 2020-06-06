using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SynthFromScratch
{
    public partial class Synth
    {
        // the key is the keyboard key corresponding the note
        // the value is a tuple of frequency and a code of a synth's key
        Dictionary<Keys, Tuple<float, int>> notes = new Dictionary<Keys, Tuple<float, int>>()
        {
            { Keys.Z, new Tuple<float, int>(130.8f, 1) },   // C3
            { Keys.S, new Tuple<float, int>(138.6f, -1) },   // C#3
            { Keys.X, new Tuple<float, int>(146.8f, 2) },   // D3
            { Keys.D, new Tuple<float, int>(155.6f, -2) },   // D#3
            { Keys.C, new Tuple<float, int>(164.8f, 3) },   // E3
            { Keys.V, new Tuple<float, int>(174.6f, 4) },   // F3
            { Keys.G, new Tuple<float, int>(185f, -4) },   // F#3
            { Keys.B, new Tuple<float, int>(196f, 5) },   // G3
            { Keys.H, new Tuple<float, int>(208.7f, -5) },   // G#3
            { Keys.N, new Tuple<float, int>(220f, 6) },   // A3
            { Keys.J, new Tuple<float, int>(233.1f, -6) },   // A#3
            { Keys.M, new Tuple<float, int>(246.9f, 7) },   // B3
            { Keys.Oemcomma, new Tuple<float, int>(261.6f, 8) },  // C4
            { Keys.L, new Tuple<float, int>(277.2f, -8) }, // C#4
            { Keys.OemPeriod, new Tuple<float, int>(293.7f, 9) }, // D4
            { Keys.Oem1, new Tuple<float, int>(311.1f, -9) },   // D#4
            { Keys.OemQuestion, new Tuple<float, int>(329.6f, 10) },   // E4
        };
    }
}