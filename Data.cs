using System.Collections.Generic;
using System.Windows.Forms;

namespace SynthFromScratch
{
    public partial class Synth
    {
        Dictionary<Keys, float> frequencies = new Dictionary<Keys, float>()
        {
            { Keys.Z, 130f },   // C3
            { Keys.S, 138f },   // C#3
            { Keys.X, 146f },   // D3
            { Keys.D, 155f },   // D#3
            { Keys.C, 164f },   // E3
            { Keys.V, 174f },   // F3
            { Keys.G, 185f },   // F#3
            { Keys.B, 196f },   // G3
            { Keys.H, 208f },   // G#3
            { Keys.N, 220f },   // A3
            { Keys.J, 233f },   // A#3
            { Keys.M, 246f },   // B3
        };
    }
}