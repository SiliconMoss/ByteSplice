using System;

namespace ByteSplice.Config
{
    public class PatchDefinition
    {
        public string Name { get; set; }
        public byte[] Pattern { get; set; }
        public byte[] Mask { get; set; } // 0xFF = exact match, 0x00 = wildcard
        public byte[] Replacement { get; set; }
        public int Offset { get; set; }
        public int ExpectedMatches { get; set; }

        public PatchDefinition()
        {
            ExpectedMatches = -1; // -1 means ignore count
        }
    }
}
