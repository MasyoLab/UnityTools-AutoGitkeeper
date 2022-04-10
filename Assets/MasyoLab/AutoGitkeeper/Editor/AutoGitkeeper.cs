#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

//=========================================================
//
//  developer : MasyoLab
//  github    : https://github.com/MasyoLab/UnityTools-AutoGitkeeper
//
//=========================================================

namespace MasyoLab.Editor.AutoGitkeeper {

    public sealed class AutoGitkeeper : AssetPostprocessor {
        private const string GIT_KEEP = ".gitkeep";
        private const string OUTPUT_FORMAT = "<color=#00FF00>{0}</color> {1} in {2}";

        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths) {

            if (0 < importedAssets.Length) {
                CreateKeeperEmptyDirectory(importedAssets, out string[] parentPaths);
                DeleteKeeperFillDirectory(parentPaths);
            }

            if (0 < deletedAssets.Length) {
                var deletedParents = GetParentPaths(deletedAssets);
                CreateKeeperEmptyDirectory(deletedParents);
            }

            if (0 < movedFromAssetPaths.Length) {
                CreateKeeperEmptyDirectory(movedFromAssetPaths);
                var movedParents = GetParentPaths(movedAssets);
                DeleteKeeperFillDirectory(movedParents);
            }

            AssetDatabase.Refresh();
        }

        private static void CreateKeeperEmptyDirectory(string[] filePaths) {
            foreach (var filePath in filePaths) {
                var isDirectory = Directory.Exists(filePath);
                if (!isDirectory) {
                    continue;
                }

                var hasDirectory = 0 < Directory.GetDirectories(filePath).Length;
                var hasFile = 0 < Directory.GetFiles(filePath).Length;
                var isEmpty = !hasDirectory && !hasFile;

                var gitkeepPath = filePath + "/" + GIT_KEEP;
                var hasGitKeep = File.Exists(gitkeepPath);

                if (isEmpty && !hasGitKeep) {
                    File.Create(gitkeepPath).Close();
                    DebugLog("Create", filePath);
                }
            }
        }

        private static void CreateKeeperEmptyDirectory(string[] filePaths, out string[] parentPaths) {
            var parentDirectorys = new List<string>();

            foreach (var filePath in filePaths) {
                // 親ディレクトリの取得
                var parentPath = Directory.GetParent(filePath).FullName;
                if (parentPath != null && !parentDirectorys.Contains(parentPath)) {
                    parentDirectorys.Add(parentPath);
                }

                var isDirectory = Directory.Exists(filePath);
                if (!isDirectory) {
                    continue;
                }

                var hasDirectory = 0 < Directory.GetDirectories(filePath).Length;
                var hasFile = 0 < Directory.GetFiles(filePath).Length;
                var isEmpty = !hasDirectory && !hasFile;

                var gitkeepPath = filePath + "/" + GIT_KEEP;
                var hasGitKeep = File.Exists(gitkeepPath);

                if (isEmpty && !hasGitKeep) {
                    File.Create(gitkeepPath).Close();
                    DebugLog("Create", filePath);
                }
            }

            parentPaths = parentDirectorys.ToArray();
        }

        private static string[] GetParentPaths(string[] filePaths) {
            var prms = new List<string>();

            foreach (var filePath in filePaths) {
                var parentPath = Directory.GetParent(filePath).FullName;
                var isExist = Directory.Exists(parentPath);
                if (isExist && !prms.Contains(parentPath)) {
                    prms.Add(parentPath);
                }
            }
            return prms.ToArray();
        }

        private static void DeleteKeeperFillDirectory(string[] filePaths) {

            foreach (var filePath in filePaths) {
                var gitkeepPath = filePath + "/" + GIT_KEEP;
                var hasGitKeep = File.Exists(gitkeepPath);
                if (!hasGitKeep) {
                    continue;
                }

                var hasDirectory = 0 < Directory.GetDirectories(filePath).Length;
                var hasFile = 0 < Directory.GetFiles(filePath).Length - 1;
                var isEmpty = !hasDirectory && !hasFile;
                if (!isEmpty) {
                    File.Delete(gitkeepPath);
                    DebugLog("Delete", filePath);
                }
            }
        }

        private static void DebugLog(string action, string filePath) {
            Debug.Log(string.Format(OUTPUT_FORMAT, action, GIT_KEEP, filePath));
        }
    }
}
#endif
