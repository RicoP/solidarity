using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class charControl : MonoBehaviour
{
    public Animator anim;

    [SerializeField]
    float speed = 1f;

    [SerializeField]
    float rotspeed = 50f;

    bool inhand = false;
    

    private void Start()
    {
        anim = GetComponent<Animator>();
        anim.SetBool("walking", false);
        
    }

    private void Update()
    {
    return;
        if (Input.GetKey(KeyCode.Space))
        {
            anim.SetBool("walking", false);
            anim.SetBool("pickup", true);
            inhand = true;
        }
        
        





        if (!inhand)
        {
            if (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0)
            {
                transform.Translate(Input.GetAxis("Vertical") * Vector3.forward * Time.deltaTime * speed);
                transform.Rotate(Vector3.up * Input.GetAxis("Horizontal") * rotspeed);
                anim.SetBool("walking", true);
            }
            else
            {
                anim.SetBool("walking", false);
            }
        }
        else if(inhand)
        {
            if (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0)
            {
                transform.Translate(Input.GetAxis("Vertical") * Vector3.forward * Time.deltaTime * speed);
                transform.Rotate(Vector3.up * Input.GetAxis("Horizontal") * rotspeed);
                anim.SetBool("carry_walking", true);
            }
            else
            {
                anim.SetBool("carry_walking", false);
            }
        }

        
        

        

        
    }


}
