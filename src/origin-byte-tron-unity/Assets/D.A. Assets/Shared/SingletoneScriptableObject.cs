using UnityEditor;
using UnityEngine;

namespace DA_Assets.Shared
{
    //[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SingletoneScriptableObject", order = 1)]
    /// <summary>
    /// Creates a single instance of your ScriptableObject in "Assets/Resources" folder and makes it accessible via YourClass.Instance.
    /// </summary>
    public class SingletoneScriptableObject<T> : ScriptableObject where T : ScriptableObject
    {
        public static T instance;
        public static T Instance
        {
            get
            {
#if UNITY_EDITOR
                if (instance == null)
                {
                    string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");

                    if (guids.Length == 0)
                    {
                        throw new System.Exception($"ScriptableObject '{typeof(T).Name}' not found in your project.");
                    }

                    string assetPath = AssetDatabase.GUIDToAssetPath(guids[0]);

                    instance = (T)AssetDatabase.LoadAssetAtPath(assetPath, typeof(T));
                }

                return instance;
#else
                throw new System.Exception("Not implemented.");
#endif
            }
        }
    }
}