using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.UIElements;
using UnityEngine;

[CustomEditor(typeof(SkelectonSampler))]
public class SkelectionSamplerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        SkelectonSampler t = target as SkelectonSampler;
        t.Ani = (Animator) EditorGUILayout.ObjectField("Ani11", t.Ani, typeof(Animator), true);
        t.AniClip = (AnimationClip) EditorGUILayout.ObjectField("AniClip", t.AniClip, typeof(AnimationClip), true);
        t.Rac = (RuntimeAnimatorController) EditorGUILayout.ObjectField("RuntimeAniClip", t.Rac,
            typeof(RuntimeAnimatorController), true);

         t.TestObj = (GameObject) EditorGUILayout.ObjectField("TestObj", t.TestObj,
            typeof(GameObject), true);

        if (GUILayout.Button("Test"))
        {
            t.Sample();
        }
        EditorGUI.BeginChangeCheck();
        t.pbTime = EditorGUILayout.Slider("pbTime", t.pbTime, 0, t.AniClip.length);
        if (GUILayout.Button("Test1"))
        {
            t.UpdateTest();
        }
        if (EditorGUI.EndChangeCheck())
        {
            //t.UpdateTest();
        }
    }
}