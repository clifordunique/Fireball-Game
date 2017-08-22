using System.Collections;
using UnityEngine;

public class TreeBranch : MonoBehaviour {

    //Player player;
    //Vector3 currentV;
    Rigidbody2D rb2D;
    public LayerMask treeBranchLayerMask;
    //float currentVelocity;
    //float smoothTime = .125f;
    //float rotateAmount = 0;

    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        FindObjectOfType<Controller2D>().hitBranchEvent += OnHitBranch;
        FindObjectOfType<Controller2D>().branchBreakEvent += OnBranchBreak;
    }

    public void OnHitBranch()
    {
        transform.eulerAngles += new Vector3(0, 0, 1);
        rb2D.constraints = RigidbodyConstraints2D.None;
        //FindObjectOfType<Controller2D>().hitBranchEvent -= OnHitBranch;
    }

    public void OnBranchBreak()
    {
        rb2D.gravityScale = 30;
        rb2D.mass = 5;
        FindObjectOfType<Controller2D>().hitBranchEvent -= OnHitBranch;
        FindObjectOfType<Controller2D>().branchBreakEvent -= OnBranchBreak;
    }

    //void OnCollisionStay2D(Collision2D col)
    //{
    //    if (col.collider.CompareTag("Player"))
    //    {
    //        rb2D.gravityScale = 0;
    //        rb2D.mass = 0;
    //    }
    //}
    /*
    void OnCollisionEnter2D(Collision2D col)
    {
        if(col.gameObject.GetComponent<Player>() != null)
        {
            
            float rotateAmt = Mathf.Abs(player.velocityYOld) * .0000007f;
            Debug.Log(rotateAmt);
            Vector3 targetRotation = new Vector3(transform.position.x, transform.position.y, transform.position.z + rotateAmt);
            Vector3 originalRotation = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            StartCoroutine(rotate(originalRotation, targetRotation));

            //Vector3 rotateAmount = Vector3.SmoothDamp(new Vector3(transform.position.x, transform.position.y, transform.position.z), targetRotation, ref currentVelocity, smoothTime);
            //transform.Rotate(rotateAmount);
        }
    }

    IEnumerator rotate(Vector3 originalRotation, Vector3 targetRotation)
    {
        while (originalRotation != targetRotation)
        {
            Debug.Log(originalRotation.z + " " + targetRotation.z);
            originalRotation = Vector3.SmoothDamp(originalRotation, targetRotation, ref currentV, smoothTime);
            rotateAmount = Mathf.SmoothDamp(originalRotation.z, targetRotation.z, ref currentVelocity, smoothTime);
            Debug.Log(rotateAmount);
            
            yield return null;
        }
        rotateAmount = 0;
    }
    */
}
