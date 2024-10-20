using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeOfScene : MonoBehaviour
{
    [SerializeField] private GameObject transition;
    private Animator anim;
    private Scene scene;
    private bool bossIsDead = false;

    void Start()
    {
        anim = transition.GetComponent<Animator>();
        StartCoroutine(SceneLoad());
        scene = SceneManager.GetActiveScene();
    }

    void Update()
    {
        bossIsDead = GameObject.Find("BerserkerBoss").GetComponent<EnemyHealth>().dead;

        if (bossIsDead)
        {
            StartCoroutine(SceneChange());
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(SceneChange()); 
        }
    }

    IEnumerator SceneLoad()
    {
        anim.SetBool("end", true);
        yield return new WaitForSeconds(0.5f);
        anim.SetBool("end", false);
    }

    IEnumerator SceneChange()
    {
        yield return new WaitForSeconds(2f);
        anim.SetBool("start", true);
        yield return new WaitForSeconds(0.5f);
        anim.SetBool("start", false);
        SceneManager.LoadScene(scene.buildIndex + 1);
    }
}
