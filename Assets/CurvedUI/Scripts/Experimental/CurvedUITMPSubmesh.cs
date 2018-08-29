using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if CURVEDUI_TMP || TMP_PRESENT
using TMPro;
#endif

namespace CurvedUI
{
    [ExecuteInEditMode]
    public class CurvedUITMPSubmesh : MonoBehaviour
    {
#if CURVEDUI_TMP || TMP_PRESENT


        VertexHelper vh;
        Mesh straightMesh;
        Mesh curvedMesh;
        CurvedUIVertexEffect crvdVE;
        TMP_SubMeshUI TMPsub;

        public void UpdateSubmesh(bool tesselate, bool curve)
        {
            if (TMPsub == null)
                TMPsub = gameObject.GetComponent<TMP_SubMeshUI>();

            if (TMPsub == null) return;

            if (crvdVE == null)
                 crvdVE = gameObject.AddComponentIfMissing<CurvedUIVertexEffect>();


            if (tesselate || straightMesh == null || vh == null || (!Application.isPlaying))
            {
                //Debug.Log("Submesh: tesselate", this.gameObject);
                vh = new VertexHelper(TMPsub.mesh);

                //save straight mesh - it will be curved then every time the object moves on the canvas.
                straightMesh = new Mesh();
                vh.FillMesh(straightMesh);

                //we do it differently now
                //curve and save mesh 
                //crvdVE.ModifyMesh(vh);
                //curvedMesh = new Mesh();
                //vh.FillMesh(curvedMesh);

               curve = true;
            }


            if (curve)
            {
                //Debug.Log("Submesh: Curve", this.gameObject);
                vh = new VertexHelper(straightMesh);
                crvdVE.ModifyMesh(vh);
                curvedMesh = new Mesh();
                vh.FillMesh(curvedMesh);
                crvdVE.CurvingRequired = true;

            }

            TMPsub.canvasRenderer.SetMesh(curvedMesh);
        }

#endif
    }

}


