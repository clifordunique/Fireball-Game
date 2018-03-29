using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healthbar : MonoBehaviour
{

    public GameObject coverItem;
    public Transform pos;
    public int percent=0;
    public int offset;

    int count=0;
    Vector2 position;
    Stack<GameObject> coverStack;

    void Start()
    {
        // initialization
        position = pos.position;
        coverStack = new Stack<GameObject>();
    }


    void Update()
    {
        UpdateHealthBar();
    }

    /* This function updates the healthbar UI element by adding
     * a small black sprite - coverItem to the stack - coverStack
     * and positioning it in the proper place over the healthbar.
     */
    void UpdateHealthBar()
    {
        // Player is losing health
        if (count < percent)
        {
            GameObject temp = Instantiate(coverItem, position, coverItem.transform.rotation, this.transform);
            coverStack.Push(temp);
            count++;
            position = new Vector2(position.x - offset, position.y);            // offset is the distance from the olde sprite to where the new sprite will be instantiated
        }
        // Player is gaining health
        if (count > percent)
        {
            GameObject temp = coverStack.Pop();
            Destroy(temp);
            count--;
            position = new Vector2(position.x + offset, position.y);
        }
    }
}
