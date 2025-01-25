using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListAnimationController : MonoBehaviour
{
    public Animator anim;
    public AnimationClip openClip;
    public AnimationClip closeClip;

    public MeshRenderer creatureListMesh;
    public Canvas creatureListUI;
    private bool opened = false;

    // Start is called before the first frame update
    void Start()
    {
        creatureListMesh.enabled = false;
        creatureListUI.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Tab) && opened == false)
        {
            creatureListMesh.enabled = true;
            creatureListUI.enabled = true;

            anim.SetBool("Open", true);
            opened = true;
        }
        else if (Input.GetKeyUp(KeyCode.Tab) && opened == true)
        {
            anim.SetBool("Open", false);

            CloseTime();

            opened = false;
        }
    }

    IEnumerator CloseTime()
    {
        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(1);

        //After we have waited 5 seconds print the time again.
        creatureListMesh.enabled = false;
        creatureListUI.enabled = false;
    }
}
