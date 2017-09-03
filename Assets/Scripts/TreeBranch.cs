using System.Collections;
using UnityEngine;

public class TreeBranch : MonoBehaviour {

    AudioManager audioManager;

    Rigidbody2D rb2D;
    public LayerMask treeBranchLayerMask;
    bool checking = true;
    string[] woodCrackClips;

    void Start()
    {
        audioManager = AudioManager.instance;

        rb2D = GetComponent<Rigidbody2D>();
        FindObjectOfType<Controller2D>().hitBranchEvent += StartCheckAngle;

        woodCrackClips = new string[7];
        for (int i = 0; i < woodCrackClips.Length; i++)
        {
            woodCrackClips[i] = "woodcrack0" + (i + 1);
        }
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
        int i = Random.Range(1, woodCrackClips.Length);
        audioManager.PlaySound(woodCrackClips[i]);

        transform.eulerAngles += new Vector3(0, 0, 1);
        rb2D.constraints = RigidbodyConstraints2D.None;
    }

    public void OnBranchBreak()
    {
        audioManager.PlaySound(woodCrackClips[0]);
        new WaitForSeconds(1f);
        audioManager.PlaySound(woodCrackClips[5]);

        checking = false;
        rb2D.gravityScale = 30;
        rb2D.mass = 5;
    }
}
