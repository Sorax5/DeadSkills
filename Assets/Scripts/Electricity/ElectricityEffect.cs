using System.Collections;
using UnityEngine;

[RequireComponent(typeof(ElectrifiableObject))]
public class ElectricityEffect : MonoBehaviour
{
    [SerializeField] private GameObject renderObject;
    private ElectrifiableObject electrifiableObject;

    private void Awake()
    {
        electrifiableObject = GetComponent<ElectrifiableObject>();
        electrifiableObject.OnObjectElectrified.AddListener(PlayEffect);
    }

    public void PlayEffect()
    {
        StartCoroutine(ElectricityCoroutine());
    }

    private IEnumerator ElectricityCoroutine()
    {
        Renderer renderer = renderObject.GetComponent<Renderer>();
        if (renderer != null)
        {
            Color originalColor = renderer.material.color;
            renderer.material.color = Color.blue;
            yield return new WaitForSeconds(0.8f);
            renderer.material.color = Color.green;
        }
    }
}
