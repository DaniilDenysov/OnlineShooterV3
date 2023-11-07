using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshTrail : MonoBehaviour
{
    [Range(0,100)]
    [SerializeField] private float duration=2f,refreshRate=0.1f,lifetime=0.01f, shaderVariableRefreshRate = 0.05f,shaderVarRate=0.1f;
    [SerializeField] private string shaderVariable;
    [SerializeField] private Material material;

    private bool isActive;
    private SkinnedMeshRenderer[] skinnedMeshRenderers;


   public void Activate ()
   {
        StartCoroutine(ActivateTrail());
   }

   IEnumerator ActivateTrail ()
   {
     if (!isActive)
     { 
        isActive = true;
        float time = duration;
        while (time > 0)
        {
            time -= refreshRate;
            if (skinnedMeshRenderers == null)
                    skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach(SkinnedMeshRenderer smr in skinnedMeshRenderers)
            {
                    GameObject skin = new GameObject();
                    skin.transform.position = transform.position;
                    skin.transform.rotation = transform.rotation;
                    MeshRenderer mr = skin.AddComponent<MeshRenderer>();
                    mr.material = material;
                    MeshFilter mf = skin.AddComponent<MeshFilter>();
                    Mesh m = new Mesh();
                    smr.BakeMesh(m);
                    mf.mesh = m;
                    StartCoroutine(FadeMaterial(skin,mr.material,0,shaderVarRate, shaderVariableRefreshRate));
            }
            yield return new WaitForSeconds(refreshRate);
        }
        isActive = false;
     }
   }
   IEnumerator FadeMaterial (GameObject obj,Material mat, float goal,float rate, float refresh)
   {
        float value = mat.GetFloat(shaderVariable);
        while (value > goal)
        {
            value -= rate;
            mat.SetFloat(shaderVariable,value);
            yield return new WaitForSeconds(refresh);
        }
        Destroy(obj);
   }
}
