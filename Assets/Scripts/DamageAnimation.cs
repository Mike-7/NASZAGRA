using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageAnimation : MonoBehaviour
{
    public Renderer meshRenderer;
    [ColorUsage(false, true)]
    public Color damageColor;
    public float lerpSpeed = 5f;

    Material material;

    void Start()
    {
        material = meshRenderer.material;
    }

    void Update()
    {
        var desiredColor = Color.Lerp(material.GetColor("_EmissionColor"), Color.black, lerpSpeed * Time.deltaTime);
        material.SetColor("_EmissionColor", desiredColor);
    }

    void OnDestroy()
    {
        Destroy(material);
    }

    public void OnDamage()
    {
        material.SetColor("_EmissionColor", damageColor);
    }

}
