using System.Collections;
using UnityEngine;

public class TreeBranch : MonoBehaviour {

    void Start()
    {
        //audioManager = AudioManager.instance;

        //woodCrackClips = new string[7];
        //for (int i = 0; i < woodCrackClips.Length; i++)
        //{
        //    woodCrackClips[i] = "woodcrack0" + (i + 1);
        //}
    }

    public virtual void OnHitBranch()
    {
        //int i = Random.Range(1, woodCrackClips.Length);
        //audioManager.PlaySound(woodCrackClips[i]);

        transform.eulerAngles += new Vector3(0, 0, 1);
        transform.eulerAngles += new Vector3(0, 0, -1);
    }
}
