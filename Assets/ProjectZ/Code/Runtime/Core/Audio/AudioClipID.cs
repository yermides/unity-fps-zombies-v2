using NaughtyAttributes;
using UnityEditor;
using UnityEngine;

namespace ProjectZ.Code.Runtime.Core.Audio
{
    [CreateAssetMenu(fileName = "ID_CLIP_", menuName = "ProjectZ/Audio/AudioClipID", order = 0)]
    public class AudioClipID : ScriptableObject
    {
        /// <summary>
        /// This little tool helps us create AudioClipConfigurations automatically <br/><br/>
        /// The workflow to create clips is the following: <br/>
        /// 1. Create the AudioClipID <br/>
        /// 2. Create an AudioClipConfiguration and assign to it the id and the clip <br/>
        /// 3. Drag AudioClipConfiguration to the AudioClipFactoryConfiguration <br/>
        /// Why? To avoid breaking stuff if an AudioClip goes missing
        /// and to replace clips usage across the board with just a Clip replacement inside AudioClipConfiguration
        /// </summary>
#if UNITY_EDITOR
        private const string Path = "Assets/ProjectZ/Level/Scriptables/Audio/ACC_CLIP_.asset";
        
        [Button]
        private void CreateAndAssignConfiguration()
        {
            AudioClipConfiguration asset = ScriptableObject.CreateInstance<AudioClipConfiguration>();
            asset.SetClipID(this);
            AssetDatabase.CreateAsset(asset, Path);
            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }
#endif
    }
}