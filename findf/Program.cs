using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace findf {
    class Program {
        static void Main(string[] args) {
            try {
                if (args.Length == 0) {
                    Console.WriteLine("Searches through all files in this directory and all sub-directories for a string.");
                    Console.WriteLine("Parameters:");
                    Console.WriteLine("1. Parameter - Search Text: The text for which should be searched in the files.");
                    Console.WriteLine("2. Parameter - File Path Regex (Optional): A Regex pattern to validate file paths. Only files where the path matches with this pattern will be searched. (Default: .* (All))");
                }
                else {
                    Stack<string> directories = new Stack<string>();
                    directories.Push(Directory.GetCurrentDirectory());

                    string searchText = args[0];

                    string patternText = "";
                    if (args.Length > 1)
                        patternText = args[1];
                    if (String.IsNullOrWhiteSpace(patternText))
                        patternText = ".*";
                    Regex pattern = new Regex(patternText);

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
                                                    Console.WriteLine("{0} - {1}:\n\r{2}\n\r", new object[] { currentFile, lineNumber, line });
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
    }
}