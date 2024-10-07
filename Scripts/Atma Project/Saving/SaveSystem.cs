using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Atma.Saving
{
    public static class SaveSystem
    {
        /// <summary>
        /// For setting the folder we will be saving in.
        /// If the folder exists, we do nothing.
        /// We create a new folder with the given name.
        /// </summary>
        /// <param name="folderName">String name of the folder we will be saving in. Does not expect / before it.</param>
        public static void CreateMainSaveFolder(string folderName)
        {
            if (Application.isEditor)
            {
                // Create editor save folder
                string t_editorSavesParentFolder = Application.persistentDataPath + "/Editor";
                if (!Directory.Exists(t_editorSavesParentFolder))
                {
                    // Create it if it doesn't exist.
                    Directory.CreateDirectory(t_editorSavesParentFolder);
                    Debug.Log($"Created folder {t_editorSavesParentFolder}");
                }
            }

            // Get the fullPath
            string t_fullPath = CreateFullPath(folderName);
            // Delete any already existing directory
            if (Directory.Exists(t_fullPath))
            {
                // No need to reset the directory if it exists already.
                return;
            }
            // Create a new folder with the path
            Directory.CreateDirectory(t_fullPath);
        }
        /// <summary>
        /// Saves the passed in data in a binary file at the specified path.
        /// </summary>
        /// <param name="data">Data to save.</param>
        /// <param name="additionalPath">What to save the file as. Does not expect a '/'.</param>
        public static void SaveData(object data, string additionalPath)
        {
            BinaryFormatter t_formatter = new BinaryFormatter();
            // Location of the file
            string t_path = CreateFullPath(additionalPath);
            // Open a connection to the file
            FileStream t_stream = new FileStream(t_path, FileMode.Create);

            // Write to the file
            t_formatter.Serialize(t_stream, data);
            // Close the connection to the file
            t_stream.Close();
        }
        /// <summary>
        /// Loads data from a binary file with the specified path.
        /// </summary>
        /// <param name="additionalPath">Name of the file to load.</param>
        /// <returns>Data as of the specified type.</returns>
        public static T LoadData<T>(string additionalPath)
        {
            // The attempted path
            string t_path = CreateFullPath(additionalPath);
            // If there is a file there
            if (File.Exists(t_path))
            {
                BinaryFormatter t_formatter = new BinaryFormatter();
                // Open a connection to the file
                FileStream t_stream = new FileStream(t_path, FileMode.Open);

                // Create data from the file
                T t_data = (T)t_formatter.Deserialize(t_stream);
                // Close the connection to the file
                t_stream.Close();

                return t_data;
            }
            else
            {
                Debug.LogWarning("Save file not found in " + t_path);
                return default;
            }
        }
        /// <summary>
        /// Checks if the given path exists.
        /// </summary>
        /// <param name="additionalPath">Additional path from the base folder.</param>
        /// <returns>True if path exists, false otherwise.</returns>
        public static bool CheckIfDataExists(string additionalPath)
        {
            // The attempted path
            string t_path = CreateFullPath(additionalPath);
            return File.Exists(t_path);
        }
        /// <summary>
        /// Deletes the file at the end of the given path.
        /// </summary>
        /// <param name="additionalPath">Additional path from the base folder.</param>
        public static void DeleteData(string additionalPath)
        {
            string t_path = CreateFullPath(additionalPath);
            if (File.Exists(t_path))
            {
                File.Delete(t_path);
            }
        }


        public static void TransferDataFromOldSaveLocation(params string[] additionalPaths)
        {
            for (int i = 0; i < additionalPaths.Length; ++i)
            {
                string t_curAdditionalPath = additionalPaths[i];
                string t_oldPath = Application.persistentDataPath + '/' + t_curAdditionalPath;
                string t_newPath = CreateFullPath(t_curAdditionalPath);

                if (File.Exists(t_oldPath))
                {
                    if (File.Exists(t_newPath))
                    {
                        // Uh oh, both exist.
                        DateTime t_oldCreateTime = File.GetCreationTime(t_oldPath);
                        DateTime t_newCreateTime = File.GetCreationTime(t_newPath);
                        if (t_oldCreateTime.CompareTo(t_newCreateTime) > 0)
                        {
                            // Old location one is newer than new location one, replace new location one.
                            File.Delete(t_newPath);
                            File.Move(t_oldPath, t_newPath);
                        }
                        else
                        {
                            // New location one is newer than old location one, just delete old one.
                            File.Delete(t_oldPath);
                        }
                    }
                    else
                    {
                        // Only the old one exists, so just move it.
                        File.Move(t_oldPath, t_newPath);
                    }
                }
            }
        }



        private static string CreateFullPath(string additionalPath)
        {
            if (Application.isEditor)
            {
                return Application.persistentDataPath + "/Editor/" + additionalPath;
            }
            return Application.dataPath + '/' + additionalPath;
        }
    }
}