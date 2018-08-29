using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LODFixer : MonoBehaviour
{
    private LODGroup lodGroup;

    private void Awake()
    {
        lodGroup = GetComponent<LODGroup>();
        //LOD[] lods = new LOD[] { lodGroup.GetLODs()[0] };
        //lodGroup.SetLODs(lods);
        lodGroup.ForceLOD(1);
    }
}
