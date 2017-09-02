using System.Collections;
using UnityEngine;

public class TreeBranch : MonoBehaviour {

    Rigidbody2D rb2D;
    public LayerMask treeBranchLayerMask;
    bool checking = true;

    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        FindObjectOfType<Controller2D>().hitBranchEvent += StartCheckAngle;
    }

    void StartCheckAngle()
    {
        StartCoroutine(CheckAngle());
        FindObjectOfType<Controller2D>().hitBranchEvent -= StartCheckAngle;
    }

    IEnumerator CheckAngle()
    {
        while (checking == true)
        {
            if (transform.eulerAngles.z < 3.5f)
            {
                OnHitBranch();
            }
            else if (transform.eulerAngles.z > 4.5f && transform.eulerAngles.z < 7)
            {
                OnHitBranch();
            }
            else if (transform.eulerAngles.z > 8.3f)
            {
                OnBranchBreak();
            }
            yield return null;
        }
    }

    public void OnHitBranch()
    {
        transform.eulerAngles += new Vector3(0, 0, 1);
        rb2D.constraints = RigidbodyConstraints2D.None;
        //FindObjectOfType<Controller2D>().hitBranchEvent -= OnHitBranch;
    }

    public void OnBranchBreak()
    {
        checking = false;
        rb2D.gravityScale = 30;
        rb2D.mass = 5;
        FindObjectOfType<Controller2D>().hitBranchEvent -= OnHitBranch;
        FindObjectOfType<Controller2D>().branchBreakEvent -= OnBranchBreak;
    }
}
