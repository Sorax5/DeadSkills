using System.Collections;
using UnityEngine;

[RequireComponent(typeof(ElectrifiableObject))]
public class ElectricityEffect : MonoBehaviour
{
    [SerializeField] private GameObject renderObject;
    [SerializeField] private ParticleSystem electricityParticles;
    private ElectrifiableObject electrifiableObject;
    
    // Cache renderer and material to avoid GetComponent and material instantiation on every effect play
    private Renderer cachedRenderer;
    private Material cachedMaterial;
    private Color originalColor;

    private void Awake()
    {
        electrifiableObject = GetComponent<ElectrifiableObject>();
        electrifiableObject.OnObjectElectrified.AddListener(PlayEffect);
        
        // Cache renderer and material references
        if (renderObject != null)
        {
            cachedRenderer = renderObject.GetComponent<Renderer>();
            if (cachedRenderer != null)
            {
                cachedMaterial = cachedRenderer.material;
                originalColor = cachedMaterial.color;
            }
        }
    }

    public void PlayEffect()
    {
        StartCoroutine(ElectricityCoroutine());
    }

    private IEnumerator ElectricityCoroutine()
    {
        if (electricityParticles != null)
        {
            electricityParticles.Play();
        }

        if (cachedMaterial != null)
        {
            cachedMaterial.color = Color.blue;
            yield return new WaitForSeconds(0.8f);
            cachedMaterial.color = Color.green;
        }
    }
}
