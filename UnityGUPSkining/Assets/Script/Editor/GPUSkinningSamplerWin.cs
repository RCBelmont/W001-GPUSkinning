using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RaphealBelmont.GPUSkinning
{
    public class GPUSkinningSamplerWin : EditorWindow
    {
        [MenuItem("Tools/BoneAnimSampler")]
        public static void OpenWindow()
        {
            EditorWindow win = EditorWindow.GetWindow<GPUSkinningSamplerWin>();
            win.maxSize = new Vector2(400, 500);
            win.minSize = new Vector2(400, 500);
            win.Show();
        }

        //变量定义
        private Transform _targetTarns;
        private Transform _rootTrans;
        private bool _infoCollected;
        private InfoData _infoData;
        private Vector2 _scrollPos1;


        public void OnGUI()
        {
            if (GUILayout.Button("Test"))
            {
               
            }

            EditorGUI.BeginChangeCheck();
            _targetTarns =
                (Transform) EditorGUILayout.ObjectField("SamplerTarget", _targetTarns, typeof(Transform), true);
            if (EditorGUI.EndChangeCheck())
            {
                _infoCollected = false;
                _rootTrans = _targetTarns;
                _infoData = null;
            }

            if (_targetTarns != null)
            {
                EditorGUILayout.Space();
                if (GUILayout.Button("CollectInfo"))
                {
                    CollectInfoData();
                }

                if (!_infoCollected)
                {
                    CollectInfoData();
                }
                else if (_infoCollected && _infoData != null)
                {
                    if (_infoData.Animator == null)
                    {
                        GUILayout.Label("Cannot Find Animator, Please Add Animator To Target");
                        return;
                    }

                    if (_infoData.SmrL.Count <= 0)
                    {
                        GUILayout.Label("Cannot Find Any SkinnedMeshRenderer, Please Check Your Asset");
                        return;
                    }

                    EditorGUILayout.Space();
                    GUILayout.Label("Info:");
                    _scrollPos1 = EditorGUILayout.BeginScrollView(_scrollPos1);
                    ;
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.ObjectField("Animator", _infoData.Animator, typeof(Animator), false);
                    int i = 0;
                    EditorGUILayout.Space();
                    GUILayout.Label("SkinnedMeshRendererList:");
                    foreach (SkinnedMeshRenderer renderer in _infoData.SmrL)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.ObjectField("SMR:" + i, renderer, typeof(SkinnedMeshRenderer), false);
                        GUILayout.Label("BoneCount: " + renderer.bones.Length);
                        EditorGUILayout.EndHorizontal();
                        i++;
                    }

                    EditorGUI.EndDisabledGroup();

                    EditorGUILayout.EndScrollView();
                }
            }
        }

        void CollectInfoData()
        {
            _infoData = new InfoData();
            Animator anim = _targetTarns.GetComponent<Animator>();
            if (anim == null)
            {
                anim = _targetTarns.GetComponentInChildren<Animator>();
            }

            _infoData.Animator = anim;

            SkinnedMeshRenderer smr = _targetTarns.GetComponent<SkinnedMeshRenderer>();
            SkinnedMeshRenderer[] smrL = _targetTarns.GetComponentsInChildren<SkinnedMeshRenderer>();

            if (smr != null)
            {
                _infoData.SmrL.Add(smr);
            }

            foreach (SkinnedMeshRenderer renderer in smrL)
            {
                _infoData.SmrL.Add(renderer);
            }

            _infoCollected = true;
        }

        private class InfoData
        {
            public List<SkinnedMeshRenderer> SmrL = new List<SkinnedMeshRenderer>();
            public Animator Animator;
        }
    }
}