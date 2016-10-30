using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ScalaExercisesMover
{
    class Program
    {
        // Directory the program was called from
        static string currentDirectory;
        // Fields for the folders used
        static string myFolder;
        static string reviewFolder;
        static string[] reviewFoldersUsers;
        // Project (teden) folder
        static string projectFolder;
        // Whether the target argument was supplied or not
        static bool continousMode;

        // Testing
        // A folder that is used instead of real current directory; for testing.
        static string testingDirectory = @"C:\Users\jakob\OneDrive\FRI II\Scala\W4";

        static void Main(string[] args)
        {
            continousMode = args.Length == 0;

            // Initialize
            try {
                InitializeFolderVariables();
            } catch (Exception e) {
                WriteMessageAndError(e);
                return;
            }

            // Make sure the folders are created
            try {
                CheckForFolders();
            } catch (Exception e) {
                WriteMessageAndError(e, "You should have a \"my\" folder for backup!!!");
                return;
            }


            // If the program is called without arguments, the user
            // can leave the app opened and write the next number - hence the loop
            do {
                // Get user input
                string stringInput;
                if (continousMode) {
                    Console.WriteLine("Please write the number of user to move");
                    stringInput = Console.ReadLine();
                    if (stringInput == "q" || stringInput == "") {
                        return;
                    }
                } else {
                    stringInput = args[0];
                }

                // Validate user input
                int currentTarget = int.Parse(stringInput) - 1;
                if (currentTarget < 0 || currentTarget > reviewFoldersUsers.Length - 1) {
                    Exception e = new Exception("The given parameter for current target is out of bounds!");
                    WriteMessageAndError(e);
                    return;
                }

                // Try to copy the files
                try {
                    CopyForTarget(currentTarget);
                } catch (Exception e) {
                    WriteMessageAndError(e);
                    return;
                }

                if (continousMode) {
                    Console.WriteLine("\nIf you wish to quit, press enter or \'q\'");
                }
            } while (continousMode);
        }

        /// <summary>
        /// Prints the message from exception and the additional message to the console.
        /// </summary>
        /// <param name="error">Exception, containing the detail of the error</param>
        /// <param name="message">Additional message with explanation</param>
        private static void WriteMessageAndError(Exception error, string message = "There was an error!")
        {
            Console.WriteLine(message);
            Console.WriteLine(error.Message);
            if (continousMode) Console.ReadKey();
        } 

        /// <summary>
        /// Set the values in the global fields. Pretty much the constructor.
        /// </summary>
        private static void InitializeFolderVariables()
        {
            currentDirectory = Environment.CurrentDirectory;
            if (testingDirectory.Length > 0) currentDirectory = testingDirectory;

            // Initialize the strings for the other folders
            myFolder = currentDirectory + @"\my";
            reviewFolder = currentDirectory + @"\review";
            reviewFoldersUsers = new string[3];
            reviewFoldersUsers[0] = reviewFolder + @"\1st";
            reviewFoldersUsers[1] = reviewFolder + @"\2nd";
            reviewFoldersUsers[2] = reviewFolder + @"\3rd";

            try {
                projectFolder = Directory.GetDirectories(currentDirectory).First(x => {
                    return Path.GetFileName(x).StartsWith("teden");
                });
            } catch (Exception e) {
                throw new Exception("No folder starting with \"teden\" found!", e);
            }
        }

        /// <summary>
        /// Checks if all the needed folders for the utility are present.
        /// If they aren't, it creates them.
        /// </summary>
        private static void CheckForFolders()
        {
            // Checks for the "my" folder
            if (!Directory.Exists(myFolder)) throw new Exception("\"my\" folder does not exist!");
            int myFileCount = Directory.GetFiles(myFolder).Length;
            if (myFileCount == 0) throw new Exception("\"my\" folder does not have any files!");

            // Checks if review and those folders exits. If not, creates them
            Directory.CreateDirectory(reviewFolder);
            foreach (String folder in reviewFoldersUsers) {
                Directory.CreateDirectory(folder);
            }
        }

        /// <summary>
        /// Returns the names of the files that should be copied.
        /// </summary>
        /// <returns>Only the names of the files</returns>
        private static string[] FilesToCopy()
        {
            return Directory.GetFiles(myFolder).Select(x => Path.GetFileName(x)).ToArray();
        }

        /// <summary>
        /// Performs the action of copying the files; point of the program.
        /// </summary>
        /// <param name="target">The number of the user</param>
        /// <param name="log">Should we log each action to the console?</param>
        private static void CopyForTarget(int target, bool log = true)
        {
            string[] fileNames = FilesToCopy();
            string targetFolder = reviewFoldersUsers[target];
            foreach (string fileName in fileNames) {
                string sourceFile = targetFolder + @"\" + fileName;
                string destinationFile = projectFolder;

                string insideSrc = fileName.Contains("Test") ? "test" : "main";
                destinationFile += String.Format("\\src\\{0}\\scala\\{1}", insideSrc, fileName);

                File.Copy(sourceFile, destinationFile, true);
                if (log) LogCopyToConsole(sourceFile, destinationFile);
            }
        }

        /// <summary>
        /// Logs the copy action to the console.
        /// </summary>
        /// <param name="source">The path of the source file</param>
        /// <param name="dest">The path of the destination file</param>
        private static void LogCopyToConsole(string source, string dest)
        {
            string sourceFile = source.Replace(currentDirectory, " ...");
            string destinationFile = dest.Replace(currentDirectory, " ...");
            Console.WriteLine(sourceFile + " => " + destinationFile);
        }
    }
}
