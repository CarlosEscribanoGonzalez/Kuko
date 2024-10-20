using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisappearingPlatform : MonoBehaviour
{

    public float timeToTogglePlatform = 5;
    public float currentTime;
    private bool enabled = true;
    private bool canCollide = true;

    // Start is called before the first frame update
    void Start()
    {
        enabled = true;
    }

    // Update is called once per frame



    void Update()
    {
        if (!canCollide)
        {
            currentTime += Time.deltaTime;
            if (currentTime >= timeToTogglePlatform)
            {
                currentTime = 0;
                TogglePlatform();
            }
        }
        if (!enabled)
        {
            StartCoroutine(Enable());
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && canCollide)
        {
            StartCoroutine(PlatformFall());
        }
    }

    void TogglePlatform()
    {
        enabled = false;
        foreach (Transform child in gameObject.transform)
        {
            if (child.tag != "Player")
            {
                child.gameObject.SetActive(enabled);
            }
        }
    }

    IEnumerator PlatformFall()
    {
        canCollide = false;
        yield return new WaitForSeconds(2f);
        canCollide = true;
    }

    IEnumerator Enable()
    {
        yield return new WaitForSeconds(3f);
        enabled = true;
        foreach (Transform child in gameObject.transform)
        {
            child.gameObject.SetActive(enabled);
        }
    }
}
