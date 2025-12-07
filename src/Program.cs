using System;
using System.Collections.Generic;
using System.Globalization;
using ByteSplice.Config;
using ByteSplice.Core;

namespace ByteSplice
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("ByteSplice v1.0");

            if (args.Length == 0)
            {
                ShowUsage();
                return;
            }

            string configFile = null;
            string patternHex = null;
            string replacementHex = null;
            int offset = 0;
            bool dryRun = false;
            bool skipBackup = false;
            bool verbose = false;
            bool createSample = false;
            string targetFile = null;

            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];
                if (arg == "--config" && i + 1 < args.Length)
                {
                    configFile = args[++i];
                }
                else if (arg == "--pattern" && i + 1 < args.Length)
                {
                    patternHex = args[++i];
                }
                else if (arg == "--replace" && i + 1 < args.Length)
                {
                    replacementHex = args[++i];
                }
                else if (arg == "--offset" && i + 1 < args.Length)
                {
                    string val = args[++i];
                    if (val.StartsWith("0x") || val.StartsWith("0X"))
                        offset = int.Parse(val.Substring(2), NumberStyles.HexNumber);
                    else
                        offset = int.Parse(val);
                }
                else if (arg == "--dry-run")
                {
                    dryRun = true;
                }
                else if (arg == "--no-backup")
                {
                    skipBackup = true;
                }
                else if (arg == "--verbose")
                {
                    verbose = true;
                }
                else if (arg == "--create-sample")
                {
                    createSample = true;
                }
                else if (arg == "--help" || arg == "/?")
                {
                    ShowUsage();
                    return;
                }
                else if (!arg.StartsWith("-"))
                {
                    targetFile = arg;
                }
            }

            if (createSample)
            {
                ConfigParser.CreateSampleConfig("sample_patches.txt");
                return;
            }

            if (targetFile == null)
            {
                Console.WriteLine("Error: No target file specified.");
                ShowUsage();
                return;
            }

            List<PatchDefinition> patches = new List<PatchDefinition>();

            try
            {
                if (configFile != null)
                {
                    Console.WriteLine("Loading config: " + configFile);
                    patches = ConfigParser.Parse(configFile);
                }
                else if (patternHex != null && replacementHex != null)
                {
                    Console.WriteLine("Using standalone mode.");
                    PatchDefinition p = new PatchDefinition();
                    p.Name = "Standalone Patch";
                    byte[] pattern, mask;
                    ConfigParser.ParsePattern(patternHex, out pattern, out mask);
                    p.Pattern = pattern;
                    p.Mask = mask;
                    p.Replacement = ConfigParser.HexStringToBytes(replacementHex);
                    p.Offset = offset;
                    patches.Add(p);
                }
                else
                {
                    Console.WriteLine("Error: Must specify either --config or both --pattern and --replace.");
                    ShowUsage();
                    return;
                }

                Patcher.ApplyPatches(targetFile, patches, dryRun, skipBackup, verbose);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                if (verbose)
                    Console.WriteLine(ex.StackTrace);
            }
        }

        static void ShowUsage()
        {
            Console.WriteLine("Usage: ByteSplice.exe <target_file> [options]");
            Console.WriteLine();
            Console.WriteLine("General Options:");
            Console.WriteLine("  --dry-run           Don't write changes");
            Console.WriteLine("  --no-backup         Skip creating .bak file (Default is to create backup)");
            Console.WriteLine("  --verbose           Show detailed output");
            Console.WriteLine("  --create-sample     Create a sample config file (sample_patches.txt)");
            Console.WriteLine();
            Console.WriteLine("Config Mode:");
            Console.WriteLine("  --config <file>     Load patches from file");
            Console.WriteLine();
            Console.WriteLine("Standalone Mode:");
            Console.WriteLine("  --pattern <hex>     Single pattern to find (e.g. \"AA BB ?? DD\")");
            Console.WriteLine("  --replace <hex>     Replacement bytes (e.g. \"CC DD EE\")");
            Console.WriteLine("  --offset <int>      Offset from pattern match to apply replacement");
        }
    }
}
