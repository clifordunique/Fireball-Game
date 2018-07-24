using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextMeshToFront : MonoBehaviour {

    public string sortingLayer = "Foreground";
    public int sortingOrder = 15;

    void Awake () {
        GetComponent<MeshRenderer>().sortingLayerName = sortingLayer;
        GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
	}
}
