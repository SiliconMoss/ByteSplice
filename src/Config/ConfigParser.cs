using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;

namespace ByteSplice.Config
{
    public class ConfigParser
    {
        public static List<PatchDefinition> Parse(string filePath)
        {
            var patches = new List<PatchDefinition>();
            PatchDefinition currentPatch = null;

            var lines = File.ReadAllLines(filePath);
            foreach (var rawLine in lines)
            {
                string line = rawLine.Trim();
                // Remove comments
                int commentIndex = line.IndexOfAny(new char[] { ';', '#' });
                if (commentIndex >= 0)
                    line = line.Substring(0, commentIndex).Trim();

                if (string.IsNullOrEmpty(line)) continue;

                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    currentPatch = new PatchDefinition();
                    currentPatch.Name = line.Substring(1, line.Length - 2);
                    patches.Add(currentPatch);
                    continue;
                }

                if (currentPatch == null) continue; // Skip orphan keys

                int splitIndex = line.IndexOf('=');
                if (splitIndex > 0)
                {
                    string key = line.Substring(0, splitIndex).Trim();
                    string value = line.Substring(splitIndex + 1).Trim();

                    switch (key.ToLower())
                    {
                        case "pattern":
                            byte[] pattern;
                            byte[] mask;
                            ParsePattern(value, out pattern, out mask);
                            currentPatch.Pattern = pattern;
                            currentPatch.Mask = mask;
                            break;
                        case "replacement":
                            currentPatch.Replacement = HexStringToBytes(value);
                            break;
                        case "offset":
                            int offset;
                            if (int.TryParse(value, out offset) || 
                                (value.StartsWith("0x") && int.TryParse(value.Substring(2), NumberStyles.HexNumber, null, out offset)))
                            {
                                currentPatch.Offset = offset;
                            }
                            break;
                        case "expectedmatches":
                            int matches;
                            if (int.TryParse(value, out matches))
                            {
                                currentPatch.ExpectedMatches = matches;
                            }
                            break;
                    }
                }
            }
            return patches;
        }

        public static void ParsePattern(string input, out byte[] pattern, out byte[] mask)
        {
            var parts = input.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            pattern = new byte[parts.Length];
            mask = new byte[parts.Length];

            for (int i = 0; i < parts.Length; i++)
            {
                if (parts[i] == "??" || parts[i] == "?")
                {
                    pattern[i] = 0x00;
                    mask[i] = 0x00; // Wildcard
                }
                else
                {
                    pattern[i] = byte.Parse(parts[i], NumberStyles.HexNumber);
                    mask[i] = 0xFF; // Exact match
                }
            }
        }

        public static byte[] HexStringToBytes(string hex)
        {
            hex = hex.Replace(" ", "");
            if (hex.Length % 2 != 0) throw new ArgumentException("Hex string must have an even number of characters");
            byte[] bytes = new byte[hex.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = byte.Parse(hex.Substring(i * 2, 2), NumberStyles.HexNumber);
            }
            return bytes;
        }

        public static void CreateSampleConfig(string path)
        {
            string content = @"; ByteSplice Sample Config File
; Define one or more patches below.

[Example Patch Name]
; Pattern to match. use '??' for wildcards.
Pattern = AA BB ?? DD EE
; Replacement bytes. Must match length of pattern if finding exact sequence, 
; but typically we replace what we find. 
; If you are replacing a smaller sequence inside a larger match, verify your logic.
; In this tool, replacement size determines how many bytes are overwritten starting at offset.
Replacement = FF FF 90 90 90

; Optional: Offset from the start of the match to apply the replacement. Default is 0.
Offset = 0

; Optional: Number of matches expected. If different, patching aborts.
ExpectedMatches = 1
";
            File.WriteAllText(path, content);
            Console.WriteLine("Sample config created at: " + path);
        }
    }
}
