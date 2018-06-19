using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentMotionBlur : MonoBehaviour {

    MotionBlur[] blurObjects;

	// Use this for initialization
	void Start () {
        blurObjects = GetComponentsInChildren<MotionBlur>();
	}
	
    /// <summary>
    /// Blurs all children objects of type MotionBlur
    /// </summary>
    public void Blur()
    {
        for(int i = 0; i < blurObjects.Length; i++)
        {
            blurObjects[i].StartBlur();
        }
    }
}
