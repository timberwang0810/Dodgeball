using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audience : MonoBehaviour
{
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void cheer()
    {
        // play big sound here?
        animator.SetTrigger("cheer");
    }

    public void resetCheer()
    {
        animator.ResetTrigger("cheer");
    }
}
