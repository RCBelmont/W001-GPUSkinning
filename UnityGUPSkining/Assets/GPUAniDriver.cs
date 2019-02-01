using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPUAniDriver : MonoBehaviour
{
    public AnimationClip AniClip;

    private float _time = 0;
	// Use this for initialization
	void Start () {
	    int frameCount = (int)(AniClip.frameRate * AniClip.length);
	   
    }
	
	// Update is called once per frame
	void Update ()
	{
	    float dTime = Time.deltaTime;
	    int frame = (int) (_time * AniClip.frameRate);
	    int frameCount = (int) (AniClip.frameRate * AniClip.length);
	    int frameIdx = frame % frameCount;
	    Material m = this.GetComponent<Renderer>().sharedMaterial;
        m.SetFloat("_FrameIdx", frameIdx);
	    _time += dTime;
	}
}
