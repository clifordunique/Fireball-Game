using System.Collections.Generic;
using UnityEngine;

public class Healthbar : MonoBehaviour
{

    public GameObject coverItem;
    public Transform pos;
    public int healthAmount;

    int offset = 3;
    int maxCovers = 35;
    Vector2 position;
    Stack<GameObject> coverStack;
    PlayerStats stats;

    void Start()
    {
        // initialization
        position = pos.position;
        coverStack = new Stack<GameObject>();
        stats = PlayerStats.instance;
    }


    void Update()
    {
        healthAmount = maxCovers * (stats.maxHealth - stats.curHealth) / stats.maxHealth;
        UpdateHealthBar();
    }

    /* This function updates the healthbar UI element by adding
     * a small black sprite - coverItem to the stack - coverStack
     * and positioning it in the proper place over the healthbar.
     */
    void UpdateHealthBar()
    {
        // Player is losing health
        if (coverStack.Count < healthAmount)
        {
            GameObject temp = Instantiate(coverItem, position, coverItem.transform.rotation, this.transform);
            coverStack.Push(temp);
            position = new Vector2(position.x - offset, position.y);            // offset is the distance from the old sprite to where the new sprite will be instantiated
        }
        // Player is gaining health
        if (coverStack.Count > healthAmount)
        {
            GameObject temp = coverStack.Pop();
            Destroy(temp);
            position = new Vector2(position.x + offset, position.y);
        }
    }

    public void SetHealth(int health)
    {

    }

    public void SetFireHealth(int health)
    {

    }

    public void SetMax(int maxHealth, int maxFireHealth)
    {

    }
}
