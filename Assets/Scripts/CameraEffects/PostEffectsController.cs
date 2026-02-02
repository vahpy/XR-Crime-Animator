using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways, ImageEffectAllowedInSceneView]
public class PostEffectsController : MonoBehaviour
{
    [SerializeField] private Shader postEffectShader;
    [SerializeField] private Material postEffectMaterial;

    public Color screenTint = Color.white;
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (postEffectShader == null) return;
        if (postEffectShader == null)
        {
            postEffectMaterial = new Material(postEffectShader);
            return;
        }

        //postEffectMaterial.SetColor("_TintScreen", screenTint);

        // Apply effects here
        int width = source.width;
        int height = source.height;
        RenderTexture renTexIn = RenderTexture.GetTemporary(width, height, 0, source.format);
        Graphics.Blit(source, renTexIn);
        for (int i=0; i < 4; i++)
        {
            width = width / 2;
            height = height/2;
            RenderTexture renTexOut = RenderTexture.GetTemporary(width, height, 0, source.format);
            Graphics.Blit (renTexIn, renTexOut);
            RenderTexture.ReleaseTemporary(renTexIn);
            renTexIn = renTexOut;
        }

        Graphics.Blit(renTexIn, destination, postEffectMaterial, 1);
        RenderTexture.ReleaseTemporary(renTexIn);
        
    }
}
