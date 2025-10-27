using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class ElectrifiableObject : MonoBehaviour
{
    [SerializeField] private string lightningTag = "Lightning";
    private bool isElectrified = false;

    public UnityEvent OnObjectElectrified;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("ElectrifiableObject detected collision with: " + other.gameObject.name);
        if (other.CompareTag(lightningTag) && !isElectrified)
        {
            isElectrified = true;
            OnObjectElectrified?.Invoke();
        }
    }
}
