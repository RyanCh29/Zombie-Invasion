using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieUnit : MonoBehaviour
{
    GameController game;
    [SerializeField] Animator anim;

    bool takingDamage = false;
    [SerializeField] int health = 1;
    float[] hitTimer = new float[2];
    bool alive = true;

    public bool moving = false;
    Vector2 direction;
    float[] idleTimer = new float[2];
    public Vector2 desiredPosition = new Vector2(1, 1);
    [SerializeField] float movementSpeed = 1f;
    Vector2 currentPosition;
    Vector2 lastPosition;
    [SerializeField] float movementThreshold = 1f;


    // Start is called before the first frame update
    void Start()
    {
        if (GameObject.Find("GameController"))
        {
            game = GameObject.Find("GameController").GetComponent<GameController>();

        }
    }

    public void TakeDamage(int dmg)
    {
        health -= dmg;

        if (health <= 0 && alive)
        {
            // change alive status
            alive = false;
            game.UpdateScore(-5, "Zombie");

        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        //Debug.Log(col.collider.name);

        // Collision with a gunner or bullets 
        if (col.collider.tag.Equals("Bullet"))
        {
            // Start hit timer
            hitTimer[0] = Time.time;

            takingDamage = true;

            TakeDamage(15);

            // Destroy bullet once it hits a zombie
            //Destroy(col.collider.gameObject);

        }
    }

    void OnCollisionStay2D(Collision2D col)
    {
        // Collision with a gunner or bullets 
        if (col.collider.tag.Equals("Bullet"))
        {
            hitTimer[0] = Time.time;
            takingDamage = true;

            TakeDamage(1);
        }
    }

    void OnCollisionExit2D(Collision2D col)
    {
        //takingDamage = false;

    }


    void GenerateDirection()
    {
        // Start timer
        idleTimer[0] = Time.time;
        // Randomize movement direction
        direction.x = Random.Range(-1, 2);
        direction.y = Random.Range(-1, 2);
    }

    void SetAnimation()
    {
        int dir;
        bool move = true;

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            if (direction.x > 0)
            {
                dir = 3;
            }
            else
            {
                dir = 1;
            }
        }
        else if (direction.x == 0  && direction.y == 0)
        {
            move = false;
            dir = 0;
        }
        else
        {
            if (direction.y > 0)
            {
                dir = 2;
            }
            else
            {
                dir = 0;
            }
        }

        // Set direction animation parameter
        anim.SetInteger("Direction", dir);
        // Set Moving animation parameter
        anim.SetBool("Moving", move);
    }

    // Update is called once per frame
    void Update()
    {
        // Change sprite colour when taking damage (colliding with bullet)
        if (takingDamage)
        {
            // Update hit timer
            hitTimer[1] = Time.time;
            this.GetComponent<SpriteRenderer>().color = Color.red;
        }
        else
        {
            this.GetComponent<SpriteRenderer>().color = Color.white;
        }

        if (hitTimer[1] - hitTimer[0] > 0.25)
        {
            takingDamage = false;

        }

        if (alive)
        {
            currentPosition = (Vector2)transform.position;
            if (moving)
            {
                
                direction = desiredPosition - currentPosition;
                //Debug.Log(direction);
                if (Mathf.Abs(direction.x) > 3)
                {
                    if (direction.x < 0)
                    {
                        direction.x = -3;
                    }
                    else
                    {
                        direction.x = 3;
                    }
                }

                if (Mathf.Abs(direction.y) > 3)
                {
                    if (direction.y < 0)
                    {
                        direction.y = -3;
                    }
                    else
                    {
                        direction.y = 3;
                    }
                }

                // move the unit to towards the desired position
                transform.Translate(Vector2.right * (direction.x * movementSpeed * Time.deltaTime));
                transform.Translate(Vector2.up * (direction.y * movementSpeed * Time.deltaTime));

                SetAnimation();

                // check if destination is reached or if very slowly moving
                if (currentPosition == desiredPosition ||
                    (Mathf.Abs(currentPosition.x - lastPosition.x) < movementThreshold &&
                    Mathf.Abs(currentPosition.y - lastPosition.y) < movementThreshold))
                {
                    //Debug.Log("slow moving");
                    //Debug.Log(currentPosition.x - lastPosition.x);
                    // stop movement
                    moving = false;

                }
                lastPosition = currentPosition;

              
            }
            else
            {
                // Idle movement generator
                idleTimer[1] = Time.time;

                if ((idleTimer[1] - idleTimer[0]) > Random.Range(2, 3))
                {
                    // change movement direction
                    GenerateDirection();
                }
                transform.Translate(Vector2.right * (direction.x * movementSpeed * Time.deltaTime));
                transform.Translate(Vector2.up * (direction.y * movementSpeed * Time.deltaTime));

                SetAnimation();

            }

            

        }
        else
        {
            Destroy(gameObject, 1);
        }


    }
}
