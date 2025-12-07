using System;
using System.Collections.Generic;
using System.IO;
using ByteSplice.Config;

namespace ByteSplice.Core
{
    public class Patcher
    {
        public static void ApplyPatches(string filePath, List<PatchDefinition> patches, bool dryRun, bool skipBackup, bool verbose)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine("Error: File not found: " + filePath);
                return;
            }

            Console.WriteLine("Reading file: " + filePath);
            byte[] fileData = File.ReadAllBytes(filePath);
            bool modified = false;

            foreach (var patch in patches)
            {
                Console.WriteLine("Processing patch: " + (patch.Name ?? "Unnamed"));

                var matches = FindMatches(fileData, patch.Pattern, patch.Mask);
                Console.WriteLine("  Found " + matches.Count + " matches.");

                if (patch.ExpectedMatches >= 0 && matches.Count != patch.ExpectedMatches)
                {
                    Console.WriteLine("  ERROR: Expected " + patch.ExpectedMatches + " matches but found " + matches.Count + ". Skipping patch.");
                    continue;
                }

                if (matches.Count == 0)
                {
                    Console.WriteLine("  No matches found for patch. Skipping.");
                    continue;
                }

                foreach (int matchOffset in matches)
                {
                    int patchOffset = matchOffset + patch.Offset;
                    
                    if (patchOffset < 0 || patchOffset + patch.Replacement.Length > fileData.Length)
                    {
                        Console.WriteLine("  ERROR: Patch offset out of bounds at match " + matchOffset.ToString("X") + ". Skipping.");
                        continue;
                    }

                    if (verbose)
                    {
                         Console.Write("  Patching at offset " + patchOffset.ToString("X") + ": ");
                         // Show old bytes
                         for(int k=0; k<patch.Replacement.Length; k++) Console.Write(fileData[patchOffset+k].ToString("X2") + " ");
                         Console.Write("-> ");
                         for(int k=0; k<patch.Replacement.Length; k++) Console.Write(patch.Replacement[k].ToString("X2") + " ");
                         Console.WriteLine();
                    }

                    // Apply replacement
                    for (int i = 0; i < patch.Replacement.Length; i++)
                    {
                        fileData[patchOffset + i] = patch.Replacement[i];
                    }
                    modified = true;
                }
            }

            if (modified)
            {
                if (dryRun)
                {
                    Console.WriteLine("Dry run: Changes defined but not written to disk.");
                }
                else
                {
                    // Backup logic
                    string backupPath = filePath + ".bak";
                    if (!skipBackup)
                    {
                        if (File.Exists(backupPath))
                        {
                            Console.WriteLine("Error: Backup file already exists: " + backupPath);
                            Console.WriteLine("Aborting to prevent data loss. Use --no-backup to skip or delete the backup file.");
                            return;
                        }
                        
                        Console.WriteLine("Creating backup: " + backupPath);
                        File.Copy(filePath, backupPath, false); // Fail if exists check already done
                    }

                    Console.WriteLine("Writing changes to disk...");
                    File.WriteAllBytes(filePath, fileData);
                    Console.WriteLine("Done.");
                }
            }
            else
            {
                Console.WriteLine("No changes required or all patches failed validation.");
            }
        }

        public static List<int> FindMatches(byte[] data, byte[] pattern, byte[] mask)
        {
            List<int> matches = new List<int>();
            if (data.Length < pattern.Length) return matches;

            for (int i = 0; i <= data.Length - pattern.Length; i++)
            {
                bool match = true;
                for (int j = 0; j < pattern.Length; j++)
                {
                    if (mask[j] == 0xFF) // Exact match required
                    {
                        if (data[i + j] != pattern[j])
                        {
                            match = false;
                            break;
                        }
                    }
                    // else wildcard, match anything
                }

                if (match)
                {
                    matches.Add(i);
                }
            }
            return matches;
        }
    }
}
