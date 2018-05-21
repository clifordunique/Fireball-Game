using UnityEngine;

public class CampFire : MonoBehaviour {

    public GameObject fire;
    public float displacementX;
    public float displacementY;

    public delegate void OnLevelEnd();
    public event OnLevelEnd levelEndEvent;

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("PlayerWeapon"))
        {
            Instantiate(fire, new Vector2(transform.position.x + displacementX, transform.position.y + displacementY), transform.rotation);
            if (levelEndEvent != null)
            {
                levelEndEvent();
            }
            Destroy(this); // Added so you can't light two fires
        }
    }
}
