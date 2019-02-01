using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using AnimatorController = UnityEditor.Animations.AnimatorController;

public class SkelectonSampler : MonoBehaviour
{
    public Animator Ani;
    public AnimationClip AniClip;
    public RuntimeAnimatorController Rac;
    public float pbTime = 0;

    [Range(0, 1)] public float Time = 0;
    public GameObject TestObj;

    public void Sample()
    {
        AnimatorOverrideController aoc = new AnimatorOverrideController(Rac);
        List<KeyValuePair<AnimationClip, AnimationClip>> overrides =
            new List<KeyValuePair<AnimationClip, AnimationClip>>();
        List<KeyValuePair<AnimationClip, AnimationClip>> overrides1 =
            new List<KeyValuePair<AnimationClip, AnimationClip>>();
        aoc.GetOverrides(overrides);
        foreach (KeyValuePair<AnimationClip, AnimationClip> pair in overrides)
        {
            overrides1.Add(new KeyValuePair<AnimationClip, AnimationClip>(pair.Key, AniClip));
        }

        aoc.ApplyOverrides(overrides1);
        Ani.runtimeAnimatorController = aoc;

        Ani.Rebind();
        Ani.StopPlayback();
        Ani.recorderStartTime = 0;


        int frameCount = (int) (AniClip.frameRate * AniClip.length);
        Ani.StartRecording(frameCount);
        for (int i = 0; i < frameCount; i++)
        {
            Ani.Update(1.0f / frameCount);
        }

        Ani.StopRecording();
        Ani.playbackTime = 0;
        Ani.StartPlayback();
    }

    public void UpdateTest()
    {
        Ani.playbackTime = pbTime;
        Ani.Update(0);

        SkinnedMeshRenderer smr = GetComponentInChildren<SkinnedMeshRenderer>();
        Mesh mesh = smr.sharedMesh;

        List<BoneData> bDataL = BuildBoneData(smr.bones, mesh.bindposes);

        int frameCount = (int) (AniClip.frameRate * AniClip.length);
        List<List<Matrix4x4>> frameML = new List<List<Matrix4x4>>();
        float min = 0;
        float max = 0;
        for (int i = 0; i < frameCount; i++)
        {
            //float nowTime = i * 1.0f / frameCount;
            float nowTime = i * 1.0f / AniClip.frameRate;
            Ani.playbackTime = nowTime;
            Ani.Update(0);
            List<Matrix4x4> transM = new List<Matrix4x4>();
            foreach (var boneData in bDataL)
            {
                Matrix4x4 m = calcMatrix(boneData, boneData.BindPos, bDataL);
                for (int j = 0; j < 16; j++)
                {
                    if (m[j] > max)
                    {
                        max = m[j];
                    }
                    else if (m[j] < min)
                    {
                        min = m[j];
                    }
                }

                transM.Add(m);
            }

            frameML.Add(transM);
        }

        Mesh mm = CopyMesh(smr.sharedMesh);
        List<Vector4> tl = new List<Vector4>();
        for (int i = 0; i < mm.vertexCount; i++)
        {
            BoneWeight boneWeight = mm.boneWeights[i];
            tl.Add(new Vector4(boneWeight.boneIndex0, boneWeight.weight0, boneWeight.boneIndex1, boneWeight.weight1));
        }


        mm.SetTangents(tl);
        //TODO:DELETE ADDBYZJ
        Debug.Log("=====min====>>" + min);
        //TODO:DELETE ADDBYZJ
        Debug.Log("====max=====>>" + max);
        //TestObj.GetComponent<MeshFilter>().mesh = mm;
        AssetDatabase.CreateAsset(mm, "Assets/TestMesh.asset");

        CrateTexture(frameML, bDataL.Count);
        //TODO:DELETE ADDBYZJ
        Debug.Log("======boneCount===>>" + bDataL.Count);
    }

    void CrateTexture(List<List<Matrix4x4>> fmL, int boneCount)
    {
        int pixelSize = boneCount * fmL.Count * 3;
        Vector2 size = GetTextureSize(pixelSize);
        Texture2D t = new Texture2D((int) size.x, (int) size.y, TextureFormat.RGBAHalf, false, true);
        t.filterMode = FilterMode.Point;
        Color[] ps = t.GetPixels();
        for (int i = 0; i < fmL.Count; i++)
        {
            for (int j = 0; j < fmL[i].Count; j++)
            {
                Matrix4x4 m = fmL[i][j];
                int startIdx = i * boneCount * 3 + j * 3;
                ps[startIdx] = new Color(m.m00, m.m01, m.m02, m.m03);
                ps[startIdx + 1] = new Color(m.m10, m.m11, m.m12, m.m13);
                ps[startIdx + 2] = new Color(m.m20, m.m21, m.m22, m.m23);
            }
        }

        t.SetPixels(ps);
        t.Apply();
        AssetDatabase.CreateAsset(t, "Assets/TestTex.asset");
    }

    Vector2 GetTextureSize(int pixelSize)
    {
        Vector2 ret = Vector2.one;
        while (true)
        {
            if (ret.x * ret.y >= pixelSize)
            {
                break;
            }

            ret.x *= 2;
            if (ret.x * ret.y >= pixelSize)
            {
                break;
            }

            ret.y *= 2;
        }

        return ret;
    }

    Mesh CopyMesh(Mesh m)
    {
        Mesh retMesh = new Mesh();
        retMesh.vertices = m.vertices;
        retMesh.uv = m.uv;
        retMesh.normals = m.normals;
        retMesh.triangles = m.triangles;
        retMesh.bindposes = m.bindposes;
        retMesh.boneWeights = m.boneWeights;
        return retMesh;
    }

    private Matrix4x4 calcMatrix(BoneData bData, Matrix4x4 m, List<BoneData> bDataL)
    {
        Matrix4x4 rts = Matrix4x4.TRS(bData.BoneTrans.localPosition, bData.BoneTrans.localRotation,
            bData.BoneTrans.localScale);
        Matrix4x4 m1 = rts * m;

        foreach (Transform pt in bData.Parent)
        {
            Matrix4x4 rts1 = Matrix4x4.TRS(pt.localPosition, pt.localRotation, pt.localScale);
            m1 = rts1 * m1;
        }

        return m1;
    }

    public struct BoneData
    {
        public List<Transform> Parent;
        public int Indx;
        public Matrix4x4 BindPos;
        public Transform BoneTrans;
    }


    private List<BoneData> BuildBoneData(Transform[] bones, Matrix4x4[] bindpos)
    {
        List<BoneData> bDataL = new List<BoneData>();
        for (int i = 0; i < bones.Length; i++)
        {
            BoneData boneData = new BoneData();

            Transform parent = bones[i].parent;
            boneData.Parent = new List<Transform>();
            while (true)
            {
                boneData.Parent.Add(parent);
                if (parent == this.transform)
                {
                    break;
                }

                parent = parent.parent;
            }


            boneData.Indx = i;
            boneData.BindPos = bindpos[i];
            boneData.BoneTrans = bones[i];
            bDataL.Add(boneData);
        }

        return bDataL;
    }
}