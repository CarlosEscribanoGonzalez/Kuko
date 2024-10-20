using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class TutorialDialogues : MonoBehaviour
{
    public GameObject canvas;
    public TextMeshProUGUI dialogue;
    public int contador = 0;
    private Animator anim;
    private bool executeSwitch = true;

    [SerializeField] private GameObject transition;
    private Animator transAnim;

    [SerializeField] private AudioSource growl;

    //GUIs de teclas:
    [SerializeField] private Sprite keyAD;
    [SerializeField] private Sprite keySpace;
    [SerializeField] private Sprite keyShift;
    [SerializeField] private Sprite keyLeftClick;
    [SerializeField] private Sprite keyRightClick;

    public GameObject image;


    // Start is called before the first frame update
    void Start()
    {
        anim = canvas.GetComponent<Animator>();
        transAnim = transition.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

        if (executeSwitch)
        {
            switch (contador)
            {
                case 0:
                    if (executeSwitch)
                    {
                        StartCoroutine(NextText());
                    }
                    break;
                case 1:
                    if (executeSwitch)
                    {
                        dialogue.text = "We are going to start \n first with the basics. \n Try to move around.";
                        StartCoroutine(NextText());
                        image.GetComponent<SpriteRenderer>().sprite = keyAD;
                    }
                    break;
                case 2:
                    if (executeSwitch && (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D)))
                    {
                        dialogue.text = "Perfect! Now try to run. \n It will help you escape \n from dangerous situations.";
                        StartCoroutine(NextText());
                        image.GetComponent<SpriteRenderer>().sprite = keyShift;
                    }
                    break;
                case 3:
                    if (executeSwitch && (Input.GetKey(KeyCode.LeftShift)))
                    {
                        dialogue.text = "Well done! Now let's jump. \n Remember, you can double jump \n if you jump in mid-air.";
                        StartCoroutine(NextText());
                        image.GetComponent<SpriteRenderer>().sprite = keySpace;
                    }
                    break;
                case 4:
                    if (executeSwitch && (Input.GetButtonDown("Jump")))
                    {
                        dialogue.text = "Now I'll teach you some attacks. \n Be cautious and use them only \n when it's necessary!";
                        StartCoroutine(NextText());
                        image.GetComponent<SpriteRenderer>().sprite = null;
                    }
                    break;
                case 5:
                    if (executeSwitch) 
                    {
                        dialogue.text = "You can use your \n tail to push enemies.\n back and keep you safe.";
                        StartCoroutine(NextText());
                        image.GetComponent<SpriteRenderer>().sprite = keyLeftClick;
                    }
                    break;
                case 6:
                    if (executeSwitch && Input.GetButtonDown("Fire1"))
                    {
                        dialogue.text = "You can perform a \n headbutt to inflict damage.\n It can also be used as a dash.";
                        StartCoroutine(NextText());
                        image.GetComponent<SpriteRenderer>().sprite = keyRightClick;

                    }
                    break;
                case 7:
                    if (executeSwitch && Input.GetButtonDown("Fire2"))
                    {
                        dialogue.text = "Perfect! That's all for today.\n I have to go. Practice \n today's lesson with your ball. \n Goodbye, darling.";
                        StartCoroutine(NextText());
                        StartCoroutine(ChangeOfScene());
                        Destroy(image);
                    }
                    break;
            }
        }
    }

    IEnumerator NextText()
    {
        executeSwitch = false;
        growl.Play();
        yield return new WaitForSeconds(1.5f);
        anim.SetBool("enter", true);
        anim.SetBool("opened", true);
        yield return new WaitForSeconds(5f);
        anim.SetBool("enter", false);
        anim.SetBool("opened", false);
        contador++;
        yield return new WaitForSeconds(1f);
        executeSwitch = true;
    }

    IEnumerator WaitFirstDialogue()
    {
        yield return new WaitForSeconds(2.5f);
        StartCoroutine(NextText());
    }

    IEnumerator ChangeOfScene()
    {
        yield return new WaitForSeconds(8.5f);
        transAnim.SetBool("start", true);
        yield return new WaitForSeconds(0.5f);
        transAnim.SetBool("start", false);
        SceneManager.LoadScene(1);
    }
}
