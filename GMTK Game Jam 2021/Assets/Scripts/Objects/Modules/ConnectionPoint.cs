using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionPoint : MonoBehaviour
{
    public bool attached = false;

    private void Start() {
        if (transform.parent.gameObject.GetComponent<ModuleController>().isBot) attached = true;
    }
}
