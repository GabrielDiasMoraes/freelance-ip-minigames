using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DailySolutions.AssetFinder.Project.Scriptable
{
    public class ScriptableFinderCacheScriptable : ScriptableObject
    {
        [SerializeField] public ProjectReference[] projectReferences;

        #region Project Window Related Function

        public void Add(string objGuid, string[] references)
        {
            if (TryGetValue(objGuid, out ProjectReference value))
            {
                value.AppendReferenceArray(references);
            }
            else
            {
                int newSize = projectReferences.Length + 1;
                Array.Resize(ref projectReferences, newSize);
                projectReferences[newSize - 1] = new ProjectReference
                {
                    objectGuid = objGuid,
                    referencesGuid = references
                };
            }
        }

        public void BatchAdd(Dictionary<string, HashSet<string>>[] threadResult, bool isAdditive = false)
        {
            if (isAdditive)
            {
                foreach (var values in threadResult)
                {
                    foreach (var keyVal in values)
                    {
                        Add(keyVal.Key, keyVal.Value.ToArray());
                    }
                }
            }
            else
            {
                int maxSize = 0;
                foreach (var values in threadResult)
                {
                    maxSize += values.Count;
                }
                projectReferences = new ProjectReference[maxSize];
                int count = 0;

                foreach (var values in threadResult)
                {
                    foreach (var keyVal in values)
                    {
                        projectReferences[count] = new ProjectReference
                        {
                            objectGuid = keyVal.Key,
                            referencesGuid = keyVal.Value.ToArray()
                        };
                        count++;
                    }
                }
            }
        }

        public bool TryGetValue(string objGuid, out ProjectReference projectRef)
        {
            projectRef = new ProjectReference();
            if (projectReferences == null)
                return false;

            for (int i = 0; i < projectReferences.Length; i++)
            {
                if (projectReferences[i].objectGuid == objGuid)
                {
                    projectRef = projectReferences[i];
                    return true;
                }
            }

            return false;
        }

        #endregion
    }

    [Serializable]
    public struct ProjectReference
    {
        [SerializeField]
        public string objectGuid;
        [SerializeField]
        public string[] referencesGuid;

        public void AppendReferenceArray(string[] newArray)
        {
            referencesGuid = referencesGuid.Union(newArray).ToArray();

        }
    }
    
}