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
    int offsetH = 0;
    int offsetF = 0;
    int maxCovers = 35;
    Stack<GameObject> healthStack;
    Stack<GameObject> fireHealthStack;
    PlayerStats stats;

    void Start()
    {
        // initialization
        healthStack = new Stack<GameObject>();
        fireHealthStack = new Stack<GameObject>();
        stats = PlayerStats.instance;
    }


    void Update()
    {
        healthAmount = maxCovers * (stats.MaxHealth - stats.CurHealth) / stats.MaxHealth;
        fireHealthAmount = maxCovers * (stats.MaxFireHealth - stats.CurFireHealth) / stats.MaxFireHealth;
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
            GameObject temp = Instantiate(coverItem, new Vector2(posHealth.position.x - offsetH, posHealth.position.y), coverItem.transform.rotation, posHealth);
            healthStack.Push(temp);
            offsetH += offset;
        }
        // Player is gaining health
        if (healthStack.Count > healthAmount && healthStack.Count > 0)
        {
            GameObject temp = healthStack.Pop();
            Destroy(temp);
            offsetH -= offset;
        }
        // Player is losing fire health
        if (fireHealthStack.Count < fireHealthAmount && fireHealthStack.Count < maxCovers)
        {
            GameObject temp = Instantiate(coverItem, new Vector2(posFireHealth.position.x - offsetF, posFireHealth.position.y), coverItem.transform.rotation, posFireHealth);
            fireHealthStack.Push(temp);
            offsetF += offset;
        }
        // Player is gaining fire health
        if (fireHealthStack.Count > fireHealthAmount && fireHealthStack.Count > 0)
        {
            GameObject temp = fireHealthStack.Pop();
            Destroy(temp);
            offsetF -= offset;
        }
    }
}
