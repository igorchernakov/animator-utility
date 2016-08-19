using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

[CustomEditor(typeof(RuntimeAnimatorController))]
public class SkeletonDataAssetInspector : UnityEditor.Editor 
{
	[MenuItem("Assets/Animator/Populate Animator")]
	public static void PopulateAnimator()
	{
		List<AnimationClip> animations = new List<AnimationClip>(); 
		RuntimeAnimatorController controller = null;

		foreach (var guid in Selection.assetGUIDs) {
			var path = AssetDatabase.GUIDToAssetPath(guid);
			var ctrl = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(path);
			if (ctrl) {
				controller = ctrl;
				continue;
			}
			var anim = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
			if (anim) {
				animations.Add(anim);
				continue;
			}
		}
		if (controller) {
			for (int i = 0; i < animations.Count; i++) {
				var anim = animations[i];
				var inst = AnimationClip.Instantiate<AnimationClip>(anim);
				inst.name = anim.name;
				AssetDatabase.AddObjectToAsset(inst, controller);
				AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(anim));
			}
		}
		AssetDatabase.SaveAssets();
	}

	[MenuItem("Assets/Animator/Extract Animator")]
	public static void ExtractAnimator()
	{
		foreach (var guid in Selection.assetGUIDs) {
			var path = AssetDatabase.GUIDToAssetPath(guid);
			var ctrl = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(path);
			if (ctrl) {
				foreach (var obj in AssetDatabase.LoadAllAssetsAtPath(path)) {
					if (obj is AnimationClip) {
						var copy = Instantiate(obj);
						AssetDatabase.CreateAsset(copy, string.Format("{0}/{1}.anim", 
							Path.GetDirectoryName(path),
							obj.name));
						DestroyImmediate(obj, true);
					}
				}
			}
		}
		AssetDatabase.SaveAssets();
	}
}
