using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PhotoPicker : MonoBehaviour
{
    public static int photoIndexInc = 0;

    [SerializeField] private GameObject photoFront;
    [SerializeField] private GameObject photoBack;
    public Texture[] textures;    // Assign the textures you want to use in the Inspector
    private int photoIndex;
    void Start()
    {
        photoIndex = photoIndexInc++;
        Renderer renderer = photoFront.GetComponent<Renderer>();
        if (renderer != null && photoIndex < textures.Length && photoIndex>=0)
        {
            var baseMaterial = renderer.sharedMaterial;
            Material newMaterial = new Material(baseMaterial);
            newMaterial.mainTexture = textures[photoIndex];
            renderer.material = newMaterial;
            photoBack.GetComponent<Renderer>().material = newMaterial;
        }
    }
}
