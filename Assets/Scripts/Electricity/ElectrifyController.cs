using System.Collections;
using UnityEngine;

public class ElectrifyController : MonoBehaviour
{
    [SerializeField] private GameObject electrifyObject;
    public bool IsCurrentlyElectrified
    {
        get
        {
            return electrifyObject != null && electrifyObject.activeSelf;
        }
    }

    public void EnableElectrify()
    {
        if (electrifyObject != null)
        {
            electrifyObject.SetActive(true);
            StartCoroutine(DelaiBeforeHide());
        }
    }

    public void DisableElectrify()
    {
        if (electrifyObject != null)
        {
            electrifyObject.SetActive(false);
        }
    }

    private IEnumerator DelaiBeforeHide()
    {
        yield return new WaitForSeconds(0.5f);
        DisableElectrify();
    }
}
