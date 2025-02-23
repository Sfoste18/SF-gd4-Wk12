using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class playerMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    public Vector2 inputDirection,lookDirection;
    Animator anim;

    Vector3 touchStart, touchEnd;
    public Image dpad, dpadBG;
    public float dpadRadius;

    Touch theTouch;




    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();

        //makes the character look down by default
        lookDirection = new Vector2(0, -1);
    }

    // Update is called once per frame
    void Update()
    {
        
#if UNITY_WEBGL && UNITY_2022_1_OR_NEWER
        calculateDesktopInputs(); //getting input from keyboard controls

#elif UNITY_EDITOR || UNITY_STANDALONE
        calculateMobileInputs(); //getting inputs from mouse

#else 
        calculateTouchInputs(); //getting input from mobile touch screen
#endif

        //sets up the animator
        animationSetup();

        //moves the player
        transform.Translate(inputDirection * moveSpeed * Time.deltaTime);
    }


    #region Anim Setup
    void animationSetup()
    {
        //checking if the player wants to move the character or not
        if (inputDirection.magnitude > 0.1f)
        {
            //changes look direction only when the player is moving, so that we remember the last direction the player was moving in
            lookDirection = inputDirection;

            //sets "isWalking" true. this triggers the walking blend tree
            anim.SetBool("isWalking", true);
        }
        else
        {
            // sets "isWalking" false. this triggers the idle blend tree
            anim.SetBool("isWalking", false);

        }

        //sets the values for input and lookdirection. this determines what animation to play in a blend tree
        anim.SetFloat("inputX", lookDirection.x);
        anim.SetFloat("inputY", lookDirection.y);
        anim.SetFloat("lookX", lookDirection.x);
        anim.SetFloat("lookY", lookDirection.y);
    }

    public void attack()
    {
        anim.SetTrigger("Attack");
    }
    #endregion Anim setup

    #region Desktop Inputs
    void calculateDesktopInputs()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        inputDirection = new Vector2(x, y).normalized;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            attack();
        }

    }
    #endregion Desktop Inputs

    #region Touch Input
    void calculateTouchInputs()
    {
        if (Input.touchCount > 0)
        {

            dpad.gameObject.SetActive(true);
            dpadBG.gameObject.SetActive(true);
            theTouch = Input.GetTouch(0);

            if (theTouch.phase == TouchPhase.Began)
            {
                touchStart = theTouch.position;
            }
            else if (theTouch.phase == TouchPhase.Moved || theTouch.phase == TouchPhase.Ended)
            {
                touchEnd = theTouch.position;

                float x = touchEnd.x - touchStart.x;
                float y = touchEnd.y - touchStart.y;

                inputDirection = new Vector2(x, y).normalized;

                if ((touchEnd - touchStart).magnitude > dpadRadius)
                {
                    dpad.transform.position = touchStart + (touchEnd - touchStart).normalized * dpadRadius;
                }
                else
                {
                    dpad.transform.position = touchEnd;
                }
            }
            

        }
        else
        {
            inputDirection = Vector2.zero;
            dpad.gameObject.SetActive(false);
            dpadBG.gameObject.SetActive(false);
        }

    }
    #endregion Touch Inputs

    #region Mobile Inputs
    void calculateMobileInputs()
    {

        if (Input.GetMouseButton(0))
        {
            dpad.gameObject.SetActive(true);
            dpadBG.gameObject.SetActive(true);


            if (Input.GetMouseButtonDown(0))
            {
                touchStart = Input.mousePosition;
                dpadBG.transform.position = touchStart;
            }

            touchEnd = Input.mousePosition;

            float x = touchEnd.x - touchStart.x;
            float y = touchEnd.y - touchStart.y;

            inputDirection = new Vector2(x, y).normalized;

            if ((touchEnd - touchStart).magnitude > dpadRadius)
            {
                dpad.transform.position = touchStart + (touchEnd - touchStart).normalized * dpadRadius;

            }
            else
            {
                dpad.transform.position = touchEnd;

            }

        }
        else
        {
            inputDirection = Vector2.zero;
            dpad.gameObject.SetActive(false);
            dpadBG.gameObject.SetActive(false);

        }

    }
    #endregion Mobile Inputs
}
