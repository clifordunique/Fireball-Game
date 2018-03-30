using System.Collections.Generic;
using UnityEngine;

public class Healthbar : MonoBehaviour
{
    public GameObject coverItem;
    public Transform posHealth;
    public Transform posFireHealth;
    public int healthAmount = 0;
    public int fireHealthAmount = 0;

    int offset = 3;
    int maxCovers = 35;
    Vector2 positionH;
    Vector2 positionF;
    Stack<GameObject> healthStack;
    Stack<GameObject> fireHealthStack;
    PlayerStats stats;

    void Start()
    {
        // initialization
        positionH = posHealth.position;
        positionF = posFireHealth.position;
        healthStack = new Stack<GameObject>();
        fireHealthStack = new Stack<GameObject>();
        stats = PlayerStats.instance;
    }


    void Update()
    {
        healthAmount = maxCovers * (stats.maxHealth - stats.curHealth) / stats.maxHealth;
        fireHealthAmount = maxCovers * (stats.maxFireHealth - stats.curFireHealth) / stats.maxFireHealth;
        UpdateHealthBar();
    }

    /* This function updates the healthbar UI element by adding
     * a small black sprite - coverItem to the stack - coverStack
     * and positioning it in the proper place over the healthbar.
     */
    void UpdateHealthBar()
    {
        // Player is losing health
        if (healthStack.Count < healthAmount && healthStack.Count < maxCovers)
        {
            GameObject temp = Instantiate(coverItem, positionH, coverItem.transform.rotation, posHealth);
            healthStack.Push(temp);
            positionH = new Vector2(positionH.x - offset, positionH.y);            // offset is the distance from the old sprite to where the new sprite will be instantiated
        }
        // Player is gaining health
        if (healthStack.Count > healthAmount && healthStack.Count > 0)
        {
            GameObject temp = healthStack.Pop();
            Destroy(temp);
            positionH = new Vector2(positionH.x + offset, positionH.y);
        }
        // Player is losing fire health
        if (fireHealthStack.Count < fireHealthAmount && fireHealthStack.Count < maxCovers)
        {
            GameObject temp = Instantiate(coverItem, positionF, coverItem.transform.rotation, posFireHealth);
            fireHealthStack.Push(temp);
            positionF = new Vector2(positionF.x - offset, positionF.y);            // offset is the distance from the old sprite to where the new sprite will be instantiated
        }
        // Player is gaining fire health
        if (fireHealthStack.Count > fireHealthAmount && fireHealthStack.Count > 0)
        {
                GameObject temp = fireHealthStack.Pop();
                Destroy(temp);
                positionF = new Vector2(positionF.x + offset, positionF.y);
        }
    }
}
