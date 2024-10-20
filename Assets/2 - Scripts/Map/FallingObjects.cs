using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingObjects : MonoBehaviour
{
    public float speed;
    public float goal;
    private bool down = false;
    public float currentTime = 0;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            down = true;
        }
    }

    void Update()
    {
        if (down)
        {
            currentTime += Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, new Vector2(transform.position.x, goal), speed * Time.deltaTime);
        }
        if (currentTime > 1)
        {
            foreach (Transform child in gameObject.transform)
            {
                if (child.tag == "Ground")
                {
                    child.gameObject.SetActive(true);
                }
                else if(child.tag == "Rock")
                {
                    Destroy(child.GetComponent<BoxCollider2D>());
                }
            }
        }
    }
}