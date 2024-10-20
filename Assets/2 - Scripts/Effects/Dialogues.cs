using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialogues : MonoBehaviour
{
    public GameObject dialogue;
    private Animator anim;
    [SerializeField] private AudioSource creature;
    private bool canOpen = true;
    private bool exited = false;

    void Start()
    {
        anim = dialogue.GetComponent<Animator>();
    }
    
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (canOpen)
        {
            StartCoroutine(OpenControl());
            anim.SetBool("enter", true);
            anim.SetBool("opened", true);
            creature.Play();
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        StartCoroutine(ExitDialogue());
        anim.SetBool("enter", false);
        exited = true;
    }

    IEnumerator ExitDialogue()
    {
        anim.SetBool("opened", true);
        yield return new WaitForSeconds(0.5f);
        anim.SetBool("exit", true);
        yield return new WaitForSeconds(0.3f);
        anim.SetBool("exit", false);
    }

    IEnumerator OpenControl()
    {
        canOpen = false;
        yield return new WaitForSeconds(2f);
        canOpen = true;
    }

    void Update()
    {
        if (exited)
        {
            exited = false;
            ExitDialogue();
        }
    }
}
