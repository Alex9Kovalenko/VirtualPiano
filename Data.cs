using System.Collections.Generic;
using System.Windows.Forms;

namespace SynthFromScratch
{
    public partial class Synth
    {
        Dictionary<Keys, float> frequencies = new Dictionary<Keys, float>()
        {
            { Keys.Z, 130.8f },   // C3
            { Keys.S, 138.6f },   // C#3
            { Keys.X, 146.8f },   // D3
            { Keys.D, 155.6f },   // D#3
            { Keys.C, 164.8f },   // E3
            { Keys.V, 174.6f },   // F3
            { Keys.G, 185f },   // F#3
            { Keys.B, 196f },   // G3
            { Keys.H, 208.7f },   // G#3
            { Keys.N, 220f },   // A3
            { Keys.J, 233.1f },   // A#3
            { Keys.M, 246.9f },   // B3
            { Keys.Oemcomma, 261.6f },  // C4
            { Keys.L, 277.2f }, // C#4
            { Keys.OemPeriod, 293.7f }, // D4
            { Keys.Oem1, 311.1f },   // D#4
            { Keys.OemQuestion, 329.6f },   // E4
        };
    }
}