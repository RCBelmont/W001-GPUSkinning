using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RaphealBelmont.GPUSkinning
{
    public class GPUSkinningSamplerData : ScriptableObject
    {
        public static GPUSkinningSamplerData CreateSmplerData(string fullPath)
        {
            GPUSkinningSamplerData ins = CreateInstance<GPUSkinningSamplerData>();
            GPUSkinningUtils.SaveAsset(ins, fullPath);
            return ins;
        }
    }


}