using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using DailySolutions.AssetFinder.Project.Scriptable;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;

namespace DailySolutions.AssetFinder.Project
{
    public class ProjectFinderEditorCache : EditorWindow
    {
        private static string RootPath
        {
            get
            {
                var guid = AssetDatabase.FindAssets("t:Script ProjectFinderEditorCache");
                return AssetDatabase.GUIDToAssetPath(guid[0]).Replace("/ProjectFinderEditorCache.cs", "");
            }
        }

        public static string CachePath => $"{RootPath}/CacheReference/CacheReference.asset";

        private const string RegexFileReplacePattern = @"\/[a-zA-Z0-9\-._]*\.[a-zA-Z0-9\-_]*";
        private const string RegexGuidFindPattern = "guid: ([a-z0-9]*)";
        private const string YamlPrefix = "%YAML 1.1";

        private static int[] assetProcessedGCCount;
        private static int[] threadCounters;

        private static readonly Regex RegexGuid = new Regex(RegexGuidFindPattern);
        private static readonly Regex RegexFileReplace = new Regex(RegexFileReplacePattern);
        

        [MenuItem("Tools/Daily Solutions/Daily Asset Finder/Cache References")]
        public static void CacheReferences()
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            ScriptableFinderCacheScriptable cachedValues = AssetDatabase.LoadAssetAtPath<ScriptableFinderCacheScriptable>(CachePath);
            if (cachedValues == null)
            {
                cachedValues = ScriptableObject.CreateInstance<ScriptableFinderCacheScriptable>();
                PrepareFolder();
                AssetDatabase.CreateAsset(cachedValues, CachePath);
                AssetDatabase.SaveAssets();
            }
            EditorCoroutineUtility.StartCoroutineOwnerless(CacheReferences_Process(cachedValues));
            ProjectWindowFinder.RefreshScriptable();

        }

        [MenuItem("Assets/Daily Solutions/Daily Asset Finder/Cache This")]
        private static void CacheReferences_MenuItem()
        {
            var selected = Selection.activeObject;

            var path = AssetDatabase.GetAssetPath(selected);

            path = RegexFileReplace.Replace(path, string.Empty);

            Debug.Log(path);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            ScriptableFinderCacheScriptable cachedValues = AssetDatabase.LoadAssetAtPath<ScriptableFinderCacheScriptable>(CachePath);
            if (cachedValues == null)
            {
                cachedValues = ScriptableObject.CreateInstance<ScriptableFinderCacheScriptable>();
                PrepareFolder();
                AssetDatabase.CreateAsset(cachedValues, CachePath);
                AssetDatabase.SaveAssets();
            }
            EditorCoroutineUtility.StartCoroutineOwnerless(CacheReferences_Process(cachedValues, path, true));
            ProjectWindowFinder.RefreshScriptable();


        }

        private static void PrepareFolder()
        {
            if (System.Array.IndexOf(AssetDatabase.GetSubFolders(RootPath), "CacheReference") == -1)
            {
                AssetDatabase.CreateFolder(RootPath, "CacheReference");
            }
        }



        private static IEnumerator CacheReferences_Process(ScriptableFinderCacheScriptable scriptableCache, string path = "Assets/", bool isAdditive = false)
        {

            while(EditorApplication.isCompiling) { yield return null; }

            var assetsPath = Directory.GetFiles(Directory.GetCurrentDirectory() + "/" + path, "*.*", SearchOption.AllDirectories).
                                                Where(file => file.ToLower().EndsWith("asset") || file.ToLower().EndsWith("prefab") || file.ToLower().EndsWith("scene")).ToArray();
            int maxSize = assetsPath.Length;
            int threadCount = SystemInfo.processorCount;
            int threadProcessBlock = maxSize / threadCount;

            CacheAssetsExecutor[] executors = new CacheAssetsExecutor[threadCount];
            ManualResetEvent[] doneEvents = new ManualResetEvent[threadCount];

            

            threadCounters = new int[threadCount];
            assetProcessedGCCount = new int[threadCount];
            Dictionary<string, HashSet<string>>[] tempCachedValues = new Dictionary<string, HashSet<string>>[threadCount];

            for (int i = 0; i < threadCount; i++)
            {
                doneEvents[i] = new ManualResetEvent(false);

                executors[i] = new CacheAssetsExecutor
                {
                    allAssetsArray = assetsPath,
                    ThreadCount = threadCount,
                    ThreadProcessAmount = threadProcessBlock,
                    DoneEvent = doneEvents[i],
                    OnEachProcessCallback = (threadIndex)=>
                    {
                        threadCounters[threadIndex]++;
                        assetProcessedGCCount[threadIndex]++;
                    },
                    OnFinishedCallback = (threadIndex, cachedValDict) =>
                    {
                        tempCachedValues[threadIndex] = cachedValDict;
                    }
                };
                ThreadPool.QueueUserWorkItem(executors[i].ThreadPoolCallback, i);
            }

            while (!WaitHandle.WaitAll(doneEvents, 100))
            {
                int sum = 0;
                for(int i = 0; i < threadCounters.Length; i++)
                {
                    sum += threadCounters[i];
                }
                EditorUtility.DisplayProgressBar("Cache References", $"{sum} of {maxSize} assets processed.", sum / maxSize);
                GarbageCollectorIfNeed();
                yield return null;
            }

            EditorUtility.ClearProgressBar();

            scriptableCache.BatchAdd(tempCachedValues, isAdditive);

            Clear(assetsPath, tempCachedValues);


            EditorUtility.SetDirty(scriptableCache);
            AssetDatabase.SaveAssets();
            yield return null;
        }

        private static void GarbageCollectorIfNeed()
        {
            int sum = 0;
            for (int i = 0; i < assetProcessedGCCount.Length; i++)
            {
                sum += assetProcessedGCCount[i];
            }
            if (sum >= 100)
            {
                System.GC.Collect();
            }
        }

        private static void Clear(string[] assets, Dictionary<string, HashSet<string>>[] threadResults)
        {
            System.Array.Clear(assets, 0, assets.Length);

            foreach (var values in threadResults)
            {
                foreach (var cachedValue in values)
                {
                    cachedValue.Value.Clear();
                }

                values.Clear();
            }
            System.GC.Collect();

        }

        private static List<string> GetReferencesOnAsset(string pAssetPath)
        {
            List<string> references = new List<string>();

            using (StreamReader sr = File.OpenText(pAssetPath))
            {
                if (sr.ReadLine().Contains(YamlPrefix))
                {
                    while (!sr.EndOfStream)
                    {
                        var match = RegexGuid.Match(sr.ReadLine());
                        var groups = match.Groups;
                        if (groups.Count > 1)
                        {
                            references.Add(groups[1].Value);
                        }
                    }
                }
            }

            return references;
        }

        private static void CacheReferencesOnAsset(string assetPath, ref Dictionary<string, HashSet<string>> cachedValuesRef)
        {
            string assetGUID = RegexGuid.Match(File.ReadAllText($"{assetPath}.meta")).Groups[1].Value;

            if (string.IsNullOrEmpty(assetGUID))
                return;

            var tempReferencesArray = GetReferencesOnAsset(assetPath);

            for (int j = 0; j < tempReferencesArray.Count; j++)
            {
                string referencedGuid = tempReferencesArray[j];
                if (!string.IsNullOrEmpty(referencedGuid))
                {
                    if (cachedValuesRef.TryGetValue(referencedGuid, out var references))
                    {
                        references.Add(assetGUID);
                    }
                    else
                    {
                        cachedValuesRef.Add(referencedGuid, new HashSet<string>() { assetGUID });
                    }
                }
            }

            tempReferencesArray.Clear();

        }

        private struct CacheAssetsExecutor
        {
            public ManualResetEvent DoneEvent;
            public System.Action<int> OnEachProcessCallback;
            public System.Action<int, Dictionary<string, HashSet<string>>> OnFinishedCallback;
            public string[] allAssetsArray;
            public int ThreadCount;
            public int ThreadProcessAmount;

            public void ThreadPoolCallback(object threadContext)
            {
                Dictionary<string, HashSet<string>> threadCachedValues = new Dictionary<string, HashSet<string>>();

                int threadIndex = (int)threadContext;
                int startIndex = threadIndex * ThreadProcessAmount;
                int endIndex = (threadIndex + 1) * ThreadProcessAmount;

                if (threadIndex == ThreadCount - 1)
                {
                    endIndex = allAssetsArray.Length;
                }

                for (int i = startIndex; i < endIndex && i < allAssetsArray.Length; i++)
                {
                    CacheReferencesOnAsset(allAssetsArray[i], ref threadCachedValues);
                    OnEachProcessCallback?.Invoke(threadIndex);
                }


                OnFinishedCallback?.Invoke(threadIndex, threadCachedValues);
                DoneEvent.Set();

            }
        }

    }
}
