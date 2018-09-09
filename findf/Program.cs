using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace findf {
    class Program {
        static void Main(string[] args) {
            try {
                string searchStringParameter = args.FirstOrDefault(a => a.StartsWith("-s:"));
                if (String.IsNullOrWhiteSpace(searchStringParameter)) {
                    PrintHelp();
                }
                else {
                    Stack<string> directories = new Stack<string>();
                    directories.Push(Directory.GetCurrentDirectory());

                    string searchText = searchStringParameter.Substring(3);

                    string patternText = ".*";
                    string patternParameter = args.FirstOrDefault(a => a.StartsWith("-p:"));
                    if (!String.IsNullOrWhiteSpace(patternParameter))
                        patternText = patternParameter.Substring(3);
                    Regex pattern = new Regex(patternText);

                    bool printLines = !args.Any(a => a == "-n");

                    while (directories.Count > 0) {
                        try {
                            string currentDirectory = directories.Pop();
                            string[] subFolders = Directory.GetDirectories(currentDirectory);
                            for (int i = subFolders.Length - 1; i >= 0; i--)
                                directories.Push(subFolders[i]);

                            string[] files = Directory.GetFiles(currentDirectory);
                            foreach (string currentFile in files) {
                                try {
                                    if (pattern.IsMatch(currentFile)) {
                                        using (StreamReader reader = new StreamReader(currentFile)) {
                                            int lineNumber = 0;
                                            while (!reader.EndOfStream) {
                                                if (Console.KeyAvailable) {
                                                    while (Console.KeyAvailable)
                                                        Console.ReadKey();
                                                    Console.WriteLine(String.Format("Currently checking file {0}", currentFile));
                                                }
                                                lineNumber++;
                                                string line = reader.ReadLine();
                                                if (line.IndexOf(searchText, StringComparison.InvariantCultureIgnoreCase) > -1) {
                                                    Console.WriteLine("{0} - {1}", new object[] { currentFile, lineNumber });
                                                    if (printLines)
                                                        Console.WriteLine(line);
                                                }
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex) {
                                    Console.WriteLine(ex.Message);
                                }
                            }
                        }
                        catch (Exception ex) {
                            Console.WriteLine(ex.Message);
                        }
                    }
                    Console.WriteLine();
                }
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }
        }

        public static void PrintHelp() {
            Console.WriteLine("Searches through all files in this directory and all sub-directories for a string.");
            Console.WriteLine();
            Console.WriteLine("Parameters");
            Console.WriteLine("Mandatory:");
            Console.WriteLine("\t\"-s:{searchText}\" - The text for which should be searched in the files.");
            Console.WriteLine("Optional:");
            Console.WriteLine("\t\"-p:{regexPattern}\" - A Regex pattern to validate file paths. Only files where the path matches with this pattern will be searched. (Default: .* (All))");
            Console.WriteLine("\t\"-n\" - If this parameter is given, only the files and line numbers will be outputted, but not the lines themselves.");
        }
    }
}