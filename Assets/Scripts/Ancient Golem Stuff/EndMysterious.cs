using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndMysterious : MonoBehaviour {

    public delegate void OnEndTransition();
    public static event OnEndTransition onEndTransition;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if(onEndTransition != null)
            {
                onEndTransition();
            }
        }
    }
}
