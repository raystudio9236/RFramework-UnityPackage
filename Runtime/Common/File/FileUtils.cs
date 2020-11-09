using System;
using System.Collections.Generic;
using System.IO;
using RFramework.Common.Log;

namespace RFramework.Common.File.Utils
{
    public class FileUtils
    {
        public static void Write(string content, string fileName)
        {
            CheckDirExists(GetFilePath(fileName), true);

            var fs = new FileStream(fileName, FileMode.Create);

            var data = System.Text.Encoding.UTF8.GetBytes(content);

            fs.Write(data, 0, data.Length);
            fs.Flush();
            fs.Close();
        }

        public static bool Write(byte[] content, string fileName)
        {
            try
            {
                if (string.IsNullOrEmpty(fileName))
                    return false;

                CheckDirExists(fileName, true);
                if (System.IO.File.Exists(fileName))
                    System.IO.File.SetAttributes(fileName, FileAttributes.Normal);

                System.IO.File.WriteAllBytes(fileName, content);
                return true;
            }
            catch (Exception e)
            {
                RLog.LogError($"Write failed! path = {fileName} with err = {e.Message}");
                return false;
            }
        }

        public static void WriteLines(List<string> lines, string fileName)
        {
            CheckDirExists(GetFilePath(fileName), true);

            var fs = new FileStream(fileName, FileMode.Create);

            var result = "";

            foreach (var line in lines)
            {
                result += line + "\n";
            }

            var data = System.Text.Encoding.UTF8.GetBytes(result);

            fs.Write(data, 0, data.Length);
            fs.Flush();
            fs.Close();
        }


        public static string Read(string path)
        {
            if (!System.IO.File.Exists(path))
                return "";

            string content = string.Empty;
            try
            {
                content = System.IO.File.ReadAllText(path);
            }
            catch (Exception e)
            {
                RLog.LogError("FileHelper Read file error: " + e);
            }

            return content;
        }

        public static string[] ReadLines(string path)
        {
            if (!System.IO.File.Exists(path))
                return null;

            string[] contents = null;
            try
            {
                contents = System.IO.File.ReadAllLines(path);
            }
            catch (Exception e)
            {
                RLog.LogError("FileHelper Read file all lines error: " + e);
            }

            return contents;
        }

        public static byte[] ReadAllBytes(string path)
        {
            try
            {
                if (string.IsNullOrEmpty(path))
                    return null;

                if (!System.IO.File.Exists(path))
                    return null;

                System.IO.File.SetAttributes(path, FileAttributes.Normal);
                return System.IO.File.ReadAllBytes(path);
            }
            catch (Exception e)
            {
                RLog.LogError($"ReadAllBytes failed! path = {path} with err = {e.Message}");
                return null;
            }
        }

        /// <summary>
        /// 得到目录下的所有文件信息
        /// </summary>
        /// <returns>所有文件信息</returns>
        /// <param name="path">目录</param>
        /// <param name="suffix">文件后缀</param>
        /// <param name="recursive">是否递归遍历</param>
        public static List<FileInfo> GetFiles(string path, string suffix = "", bool recursive = false)
        {
            var dir = new DirectoryInfo(path);

            var ret = new List<FileInfo>();

            InnerGetFiles(dir, ref ret, suffix, recursive);

            return ret;
        }

        private static void InnerGetFiles(DirectoryInfo root, ref List<FileInfo> fileList, string suffix,
            bool recursive = false)
        {
            var files = root.GetFiles(suffix);
            fileList.AddRange(files);

            if (recursive)
            {
                var dirs = root.GetDirectories();
                foreach (var dir in dirs)
                {
                    InnerGetFiles(dir, ref fileList, suffix, true);
                }
            }
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool DelFile(string path)
        {
            if (!CheckFileExists(path))
                return false;

            System.IO.File.Delete(path);
            return true;
        }

        /// <summary>
        /// 删除目录下所有满足条件文件
        /// </summary>
        /// <returns>如果有删除的文件则返回真，否则返回假</returns>
        /// <param name="path">路径</param>
        /// <param name="suffix">后缀</param>
        /// <param name="recursive">是否递归删除</param>
        public static bool DelFiles(string path, string suffix, bool recursive = false)
        {
            if (!CheckDirExists(path))
                return false;

            var files = GetFiles(path, suffix, recursive);
            if (files == null || files.Count == 0)
            {
                return false;
            }
            else
            {
                foreach (var file in files)
                {
                    System.IO.File.Delete(file.FullName);
                }

                return true;
            }
        }

        /// <summary>
        /// 删除目录下所有满足条件的文件
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="includeList">包含文件列表</param>
        /// <param name="excludeList">排除文件列表</param>
        /// <param name="recursive">是否递归删除</param>
        /// <returns></returns>
        public static bool DelFiles(string path, string suffix,
            List<string> includeList,
            List<string> excludeList = null,
            bool recursive = false)
        {
            var files = GetFiles(path, suffix, recursive);
            if (files == null || files.Count == 0)
            {
                return false;
            }
            else
            {
                foreach (var file in files)
                {
                    if (includeList != null && includeList.Contains(file.Name))
                    {
                        System.IO.File.Delete(file.FullName);
                        continue;
                    }

                    if (excludeList != null && excludeList.Contains(file.Name))
                        continue;

                    System.IO.File.Delete(file.FullName);
                }

                return true;
            }
        }

        public static bool MoveFile(string path, string newPath)
        {
            if (!CheckFileExists(path))
                return false;

            System.IO.File.Move(path, newPath);
            return true;
        }


        /// <summary>
        /// 检查目录是否存在
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="recursiveCreate">是否递归创建</param>
        /// <returns></returns>
        public static bool CheckDirExists(string path, bool recursiveCreate = false)
        {
            if (!recursiveCreate)
                return Directory.Exists(path);

            path = path.Replace("\\", "/");
            if (!Directory.Exists(path))
            {
                string[] pathParts = path.Split('/');
                string currentPath = pathParts[0];

                if (!Directory.Exists(currentPath))
                    Directory.CreateDirectory(currentPath);

                for (int i = 1; i < pathParts.Length; i++)
                {
                    currentPath += "/" + pathParts[i];
                    if (!Directory.Exists(currentPath))
                        Directory.CreateDirectory(currentPath);
                }
            }

            return true;
        }

        /// <summary>
        /// 检查文件是否存在
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="create">是否创建</param>
        /// <returns></returns>
        public static bool CheckFileExists(string path, bool create = false)
        {
            if (System.IO.File.Exists(path))
                return true;

            if (create)
            {
                var filePath = GetFilePath(path);

                if (!string.IsNullOrEmpty(filePath))
                {
                    CheckDirExists(filePath, true);
                }

                Write(string.Empty, path);
            }

            return false;
        }

        public static DirectoryInfo GetDirectoryInfo(string path, bool autoCreate = true)
        {
            if (string.IsNullOrEmpty(path))
                return null;

            if (CheckDirExists(path, autoCreate))
                return new DirectoryInfo(path);

            return null;
        }

        public static FileInfo GetFileInfo(string path, bool autoCreate = true)
        {
            if (string.IsNullOrEmpty(path))
                return null;

            if (CheckFileExists(path, autoCreate))
                return new FileInfo(path);

            return null;
        }

        /// <summary>
        /// 从路径中分离出文件名
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns>文件名</returns>
        public static string GetFileName(string path)
        {
            path = path.Replace("\\", "/");

            return path.Substring(path.LastIndexOf("/", StringComparison.Ordinal) + 1);
        }

        /// <summary>
        /// 从路径中分离文件名
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetFilePath(string path)
        {
            path = path.Replace("\\", "/");

            if (path.Contains("/"))
                return path.Substring(0, path.LastIndexOf("/", StringComparison.Ordinal));
            else
                return string.Empty;
        }
    }
}