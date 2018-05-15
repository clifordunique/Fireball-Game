using UnityEngine;

public class GroundFade : MonoBehaviour {

    Player player;
    Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.GetComponent<Player>() != null)
        {
            anim.SetFloat("Distance", 1);
        }
    }
}
