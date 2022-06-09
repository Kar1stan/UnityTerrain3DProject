using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clock : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        
    }

    public void OnAnimationDisapearEnd()
    {
        animator.SetBool("IsCatched", false);
        transform.parent.Translate(Vector3.right * 2);
    }

    private void OnTriggerEnter(Collider other)
    {
        animator.SetBool("IsCatched", true);
    }
}
