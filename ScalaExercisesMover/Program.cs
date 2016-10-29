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
        // Create the strings for the current folder
        static string executionAssembly;
        static string executionFolder;
        // Create the strings for the custom folders
        static string myFolder;
        static string reviewFolder;
        static string[] reviewFoldersUsers;

        static string projectFolder;

        static bool calledExternally;

        static void Main(string[] args)
        {
            calledExternally = args.Length > 0;

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

            // Get user input
            user_input_point: // Using goto because I'm a prick!
            string stringInput;
            if (calledExternally) {
                stringInput = args[0];
            } else {
                Console.WriteLine("Please write the number of user to move");
                stringInput = Console.ReadLine();
                if (stringInput == "q" || stringInput == "") {
                    return;
                }
            }
            
            // Validate user input
            int currentTarget = int.Parse(stringInput) - 1;
            if (currentTarget < 0 || currentTarget > reviewFoldersUsers.Length - 1) {
                Console.WriteLine("The given parameter for current target is out of bounds!");
                if (!calledExternally) Console.ReadKey();
                return;
            }

            // Try to copy the files
            try {
                CopyForTarget(currentTarget);
            } catch (Exception e) {
                WriteMessageAndError(e);
                return;
            }

            if (!calledExternally) {
                Console.WriteLine("If you wish to quit, press enter or \'q\'");
                goto user_input_point;
            }


        }

        private static void WriteMessageAndError(Exception error, string message = "There was an error!")
        {
            Console.WriteLine(message);
            Console.WriteLine(error.Message);
            if (!calledExternally) Console.ReadKey();
        } 

        private static void InitializeFolderVariables()
        {
            executionAssembly = System.Reflection.Assembly.GetEntryAssembly().Location.ToString();
            executionFolder = Path.GetDirectoryName(executionAssembly);

            // Create the strings for the custom folders
            myFolder = executionFolder + @"\my";
            reviewFolder = executionFolder + @"\review";
            reviewFoldersUsers = new string[3];
            reviewFoldersUsers[0] = reviewFolder + @"\1st";
            reviewFoldersUsers[1] = reviewFolder + @"\2nd";
            reviewFoldersUsers[2] = reviewFolder + @"\3rd";

            try {
                projectFolder = Directory.GetDirectories(executionFolder).First(x => {
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
            bool myExists = Directory.Exists(myFolder);
            if (!myExists) throw new Exception("\"my\" folder does not exist!");
            int myFileCount = Directory.GetFiles(myFolder).Length;
            if (myFileCount == 0) throw new Exception("\"my\" folder does not have any files!");

            // Checks if review and those folders exits. If not, creates them
            Directory.CreateDirectory(reviewFolder);
            foreach (String folder in reviewFoldersUsers) {
                Directory.CreateDirectory(folder);
            }
        }

        /// <summary>
        /// Returns the names of the files that should be copied
        /// </summary>
        /// <returns></returns>
        private static string[] FilesToCopy()
        {
            return Directory.GetFiles(myFolder).Select(x => Path.GetFileName(x)).ToArray();
        }

        /// <summary>
        /// Performs the action of copying the files; point of the program
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
                switch (fileName) {
                    case "Main.scala":
                        destinationFile += @"\src\main\scala\" + fileName;
                        break;
                    case "MainTests.scala":
                        destinationFile += @"\src\test\scala\" + fileName;
                        break;
                    default:
                        destinationFile += @"\src\main\scala\" + fileName;
                        break;
                }

                File.Copy(sourceFile, destinationFile, true);
                if (log) Console.WriteLine(sourceFile + " => " + destinationFile);
            }
        }
        
        
    }
}
