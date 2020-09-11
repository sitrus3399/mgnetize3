using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb2D;

    public GameObject startPosition;
    public float moveSpeed = 5f;

    //Variable Tower
    public float pullForce = 100f;
    public float rotateSpeed = 360f;
    private GameObject closestTower;
    private GameObject hookedTower;
    private bool isPulled = false;
    private UIControllerScript uiControl;

    //Audio
    private AudioSource myAudio;
    private bool isCrashed = false;

    void Start()
    {
        rb2D = this.gameObject.GetComponent<Rigidbody2D>();
        uiControl = GameObject.Find("Canvas").GetComponent<UIControllerScript>();
        myAudio = this.gameObject.GetComponent<AudioSource>();
    }

    void Update()
    {
        //Move the object
        rb2D.velocity = -transform.up * moveSpeed;

        if (Input.GetKey(KeyCode.Z) || Input.GetMouseButton(0) && !isPulled)
        {
            if(closestTower != null && hookedTower == null)
            {
                hookedTower = closestTower;
            }

            if(hookedTower)
            {
                float distance = Vector2.Distance(transform.position, hookedTower.transform.position);

                //Gravitation toward tower
                Vector3 pullDirection = (hookedTower.transform.position - transform.position).normalized;
                float newPullForce = Mathf.Clamp(pullForce / distance, 20, 50);
                rb2D.AddForce(pullDirection * newPullForce);

                //Angular velocity
                rb2D.angularVelocity = -rotateSpeed / distance;
                isPulled = true;
            }
        }

        if (Input.GetKeyUp(KeyCode.Z) || Input.GetMouseButtonUp(0))
        {
            isPulled = false;
            hookedTower = null;
            rb2D.velocity = -transform.up * moveSpeed;
            rb2D.angularVelocity = 0f;
        }

        if (isCrashed)
        {
            if (!myAudio.isPlaying)
            {
                restartPosition();
            }
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            if (!isCrashed)
            {
                //Play SFX
                myAudio.Play();
                rb2D.velocity = new Vector3(0f, 0f, 0f);
                rb2D.angularVelocity = 0f;
                isCrashed = true;
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Goal")
        {
            Debug.Log("Levelclear!");
            uiControl.endGame();
        }
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Tower")
        {
            closestTower = collision.gameObject;

            //Changee tower color back to green as indicator
            collision.gameObject.GetComponent<SpriteRenderer>().color = Color.green;

        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (isPulled) return;

        if(collision.gameObject.tag == "Tower")
        {
            closestTower = null;
            hookedTower = null;

            //Change tower color back to normal
            collision.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
        }
    }

    public void restartPosition()
    {
        //Set to start position
        this.transform.position = startPosition.transform.position;

        //Restart rotation
        this.transform.rotation = Quaternion.Euler(0f, 0f, 90f);

        rb2D.velocity = -transform.up * moveSpeed;
        rb2D.angularVelocity = 0f;
        //Set isCrashed to false
        isCrashed = false;

        if (closestTower)
        {
            closestTower.GetComponent<SpriteRenderer>().color = Color.white;
            closestTower = null;
        }
    }

}
