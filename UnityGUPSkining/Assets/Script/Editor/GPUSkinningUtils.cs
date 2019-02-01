using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RaphealBelmont.GPUSkinning
{
    public class GPUSkinningUtils
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="fullPath"></param>
        public static void SaveAsset(Object obj, string fullPath)
        {
            string savePath = GetAssetsRelativePath(fullPath);
            AssetDatabase.CreateAsset(obj, savePath);
            AssetDatabase.Refresh();

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        public static bool CheckPathInAssets(string fullPath)
        {
            return fullPath.Contains(Application.dataPath);
        }

        public static string GetAssetsRelativePath(string fullPath)
        {
            string[] parts = fullPath.Split('/');
            string retString = "";
            bool push = false;
            foreach (string s in parts)
            {
                if (s == "Assets")
                {
                    push = true;
                }

                if (push)
                {
                    retString += s + "/";
                }
            }

            if (retString.Length > 0)
            {
                return retString.Remove(retString.Length - 1);
            }

            return "";
        }

    }
}
