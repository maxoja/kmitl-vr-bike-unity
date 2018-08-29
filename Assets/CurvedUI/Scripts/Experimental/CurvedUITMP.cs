using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

#if CURVEDUI_TMP || TMP_PRESENT
using TMPro;
#endif 

//To use this class you have to add CURVEDUI_TMP to your define symbols. You can do it in project settings.
//To learn how to do it visit http://docs.unity3d.com/Manual/PlatformDependentCompilation.html and search for "Platform Custom Defines"
namespace CurvedUI
{
    [ExecuteInEditMode]
    public class CurvedUITMP : MonoBehaviour
    {

#if CURVEDUI_TMP || TMP_PRESENT

        //internal
        CurvedUIVertexEffect crvdVE;
        TextMeshProUGUI tmpText;
        CurvedUISettings mySettings;
        Mesh m_savedMesh;
        VertexHelper m_vh;
        List<UIVertex> m_flatSavedVerts = new List<UIVertex>();


        Vector2 savedSize;
        Vector3 savedUp;
        Vector3 savedPos;
        Vector3 savedLocalScale;
        List<CurvedUITMPSubmesh> subMeshes = new List<CurvedUITMPSubmesh>(); 

        public bool Dirty = false; // set this to true to force mesh update.

        bool curvingRequired = false;
        bool tesselationRequired = false;



        #region LIFECYCLE
        void Start()
        {
            if (mySettings == null)
                mySettings = GetComponentInParent<CurvedUISettings>();
        }


        void OnEnable()
        {
            FindTMP();

            if (tmpText)
            {
                tmpText.RegisterDirtyMaterialCallback(TesselationRequiredCallback);
                TMPro_EventManager.TEXT_CHANGED_EVENT.Add(TMPTextChangedCallback);
            }
        }


        void OnDisable()
        {
            if (tmpText)
            {
                tmpText.UnregisterDirtyMaterialCallback(TesselationRequiredCallback);
                TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(TMPTextChangedCallback);
            }   
        }


        void LateUpdate()
        {
            //if we're missing stuff, find it
            if(!tmpText) FindTMP();


            //Edit Mesh on TextMeshPro component
            if (tmpText)
            {

                //if (!Application.isPlaying)
                //    tesselationRequired = true;


                if (savedSize != (transform as RectTransform).rect.size)
                {
                    tesselationRequired = true;
                    //Debug.Log("size changed");

                }
                else if (savedLocalScale != mySettings.transform.localScale)
                {
                    tesselationRequired = true;
                    //Debug.Log("size changed");

                }
                else if (!savedPos.AlmostEqual(mySettings.transform.worldToLocalMatrix.MultiplyPoint3x4(transform.position)))
                {
                    curvingRequired = true;
                    // Debug.Log("pos changed");

                }
                else if (!savedUp.AlmostEqual(mySettings.transform.worldToLocalMatrix.MultiplyVector(transform.up)))
                {
                    curvingRequired = true;
                    // Debug.Log("up changed");
                }


                if (Dirty || tesselationRequired || m_savedMesh == null || m_vh == null || (curvingRequired && !Application.isPlaying))
                {

                    //Get the mesh from TMP object.
                    tmpText.renderMode = TMPro.TextRenderFlags.Render;
                    tmpText.ForceMeshUpdate();
                    if(m_vh != null) m_vh.Dispose();
                    m_vh = new VertexHelper(tmpText.mesh);

                    //store a copy of flat UIVertices for later so we dont have to retrieve the Mesh every framee.
                    m_vh.GetUIVertexStream(m_flatSavedVerts);

                    //Tesselate and Curve the flat UIVertices stored in Vertex Helper
                    crvdVE.TesselationRequired = true;
                    crvdVE.ModifyMesh(m_vh);


                    //fill the mesh with curved UIVertices
                    if (!m_savedMesh) m_savedMesh = new Mesh();
                    m_savedMesh.Clear();
                    m_vh.FillMesh(m_savedMesh);
                    tmpText.renderMode = TMPro.TextRenderFlags.DontRender;

                    //reset flags
                    tesselationRequired = false;
                    curvingRequired = false;
                    Dirty = false;

                    //save current data
                    savedLocalScale = mySettings.transform.localScale;
                    savedSize = (transform as RectTransform).rect.size;
                    savedUp = mySettings.transform.worldToLocalMatrix.MultiplyVector(transform.up);
                    savedPos = mySettings.transform.worldToLocalMatrix.MultiplyPoint3x4(transform.position);

                    //prompt submeshes to update
                    FindSubmeshes();
                    foreach (CurvedUITMPSubmesh mesh in subMeshes)
                        mesh.UpdateSubmesh(true, false);
                }


                if (curvingRequired)
                {
                    //fill the VertexHelper with stored flat mesh
                    m_vh.Clear();
                    m_vh.AddUIVertexTriangleStream(m_flatSavedVerts);

                    //curve Mesh stored in VertexHelper with CurvedUIVertexEffect
                    crvdVE.TesselationRequired = false;
                    crvdVE.CurvingRequired = true;
                    crvdVE.ModifyMesh(m_vh);

                    //Fill the mesh we're going to upload to TMP object with already curved UIVertices
                    m_savedMesh.Clear();
                    m_vh.FillMesh(m_savedMesh);

                    //reset flags
                    curvingRequired = false;

                    //save current data
                    savedLocalScale = mySettings.transform.localScale;
                    savedUp = mySettings.transform.worldToLocalMatrix.MultiplyVector(transform.up);
                    savedPos = mySettings.transform.worldToLocalMatrix.MultiplyPoint3x4(transform.position);

                    //prompt submeshes to update
                    foreach (CurvedUITMPSubmesh mesh in subMeshes)
                        mesh.UpdateSubmesh(false, true);     
                }

                //upload mesh to TMP Object
                tmpText.canvasRenderer.SetMesh(m_savedMesh);
            }
        }
        #endregion





        #region PRIVATE
        void FindTMP()
        {
            if (this.GetComponent<TextMeshProUGUI>() != null)
            {
                tmpText = this.gameObject.GetComponent<TextMeshProUGUI>();
                crvdVE = this.gameObject.GetComponent<CurvedUIVertexEffect>();
                mySettings = GetComponentInParent<CurvedUISettings>();
                transform.hasChanged = false;

                FindSubmeshes();
            }
        }

        void FindSubmeshes()
        {
            foreach (TMP_SubMeshUI sub in GetComponentsInChildren<TMP_SubMeshUI>())
            {
                CurvedUITMPSubmesh msh = sub.gameObject.AddComponentIfMissing<CurvedUITMPSubmesh>();
                if (!subMeshes.Contains(msh))
                    subMeshes.Add(msh);
            }
        }
        #endregion




        #region EVENTS AND CALLBACKS
        void TMPTextChangedCallback(object obj)
        {
            if (obj != (object)tmpText) return;

            tesselationRequired = true;
            //Debug.Log("tmp prop changed on "+this.gameObject.name, this.gameObject);
        }

        void TesselationRequiredCallback()
        {
            tesselationRequired = true;
            curvingRequired = true;
        }
        #endregion

#endif
    }
}



