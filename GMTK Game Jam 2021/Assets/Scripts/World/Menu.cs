using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public Animator anim;
    public bool tutorial;
    private void Awake() {
        anim = gameObject.GetComponent<Animator>();
    }

    public void openTutorial(){
        anim.SetBool("Main Menu", false);
    }
    public void Back(){
        anim.SetBool("Main Menu", true);
    }
}
