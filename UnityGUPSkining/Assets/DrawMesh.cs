using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class DrawMesh : MonoBehaviour
{
    public Mesh TargetMesh;

    public int Row = 5;

    public int Column = 5;

    public AnimationClip Clip;

    public float Gap = 0.5f;
    private float _offset = 2.0f;

    public Material m;

    private List<Matrix4x4> ml;

    private float _time = 0;

    // Use this for initialization
    void Start()
    {
        _offset = Random.Range(0, 10);
        ml = new List<Matrix4x4>();
        for (int i = 0; i < Row; i++)
        {
            for (int j = 0; j < Column; j++)
            {
                Vector3 pos = new Vector3((i - Row / 2.0f) * Gap, 0, (j - Column / 2.0f) * Gap);

                Matrix4x4 matrix = Matrix4x4.TRS(this.transform.worldToLocalMatrix.MultiplyPoint(pos),
                    Quaternion.identity, Vector3.one *20); 
                ml.Add(matrix);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        float dTime = Time.deltaTime;
        int frame = (int) ((_time + _offset) * Clip.frameRate);
        int frameCount = (int) (Clip.frameRate * Clip.length);
        int frameIdx = frame % frameCount;

        _time += dTime;


        MaterialPropertyBlock mb = new MaterialPropertyBlock();
        mb.SetFloat("_FrameIdx", frameIdx);
        ;
        Graphics.DrawMeshInstanced(TargetMesh, 0, m, ml, mb);
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(this.transform.position, new Vector3(Row * Gap, 1, Column * Gap));
    }
}