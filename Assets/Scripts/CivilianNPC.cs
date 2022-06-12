using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CivilianNPC : MonoBehaviour
{
    GameController game;
    [SerializeField] GameObject prefabZombie;
    [SerializeField] Animator anim;


    bool spawned = false;

    bool alive = true;
    [SerializeField] int health = 1;
    bool takingDamage = false;
    float[] hitTimer = new float[2];

    float[] timer = new float[2];

    Vector2 direction = new Vector2(0, 0);
    [SerializeField] float movementSpeed = 1f;

    // Start is called before the first frame update
    void Start()
    {
        if (GameObject.Find("GameController"))
        {
            game = GameObject.Find("GameController").GetComponent<GameController>();

        }
        GenerateDirection();

    }

    void GenerateDirection()
    {
        // Start timer
        timer[0] = Time.time;
        // Randomize movement direction
        direction.x = Random.Range(-1, 2);
        direction.y = Random.Range(-1, 2);
    }

    void TakeDamage(int dmg, string tag)
    {
        // Decrement health
        health -= dmg;
        if (health <= 0)
        {
            if (tag.Equals("Zombie"))
            {
                // create a new zombie object in the scene
                GameObject instantiator = Instantiate<GameObject>(prefabZombie, transform.position, Quaternion.identity);
                instantiator.name = "zombie_" + Random.Range(0, 10000);

                game.UpdateScore(10, "Civilian");
            }

            // destroy the civilian object
            alive = false;
        }
    }

    void SpawnPosChange(string tag)
    {
        //Debug.Log(tag);

        if (!spawned && tag.Equals("Terrain"))
        {
            this.GetComponent<SpriteRenderer>().enabled = spawned;

            // translate position out of terrain and "spawn"(enable the sprite renderer) object
            int rand = Random.Range(-1, 2);

            while (rand == 0)
            {
                rand = Random.Range(-1, 2);
            }

            transform.Translate(Vector2.right * 10 * rand);
            transform.Translate(Vector2.up * 10 * rand);

            
        }
        spawned = true;
        this.GetComponent<SpriteRenderer>().enabled = spawned;


    }


    void OnCollisionEnter2D(Collision2D col)
    {

        if (!spawned)
        {
            if (Time.time < 1)
            {
                SpawnPosChange(col.collider.tag);
            }
            spawned = true;

        }

        string colTag = col.collider.tag;


        // Collision with a zombie or bullet
        if ((colTag.Equals("Zombie") || colTag.Equals("Bullet")) && alive)
        {
            hitTimer[0] = Time.time;

            takingDamage = true;

            TakeDamage(15, colTag);
        }
    }

    void OnCollisionStay2D(Collision2D col)
    {

        if (!spawned)
        {
            if (Time.time < 1)
            {
                SpawnPosChange(col.collider.tag);
            }
            spawned = true;

        }

        string colTag = col.collider.tag;
        // Collision with a zombie or bullet
        if ((colTag.Equals("Zombie") || colTag.Equals("Bullet")) && alive)
        {
            hitTimer[0] = Time.time;
            takingDamage = true;

            TakeDamage(1, colTag);
        }

    }

    void OnCollisionExit2D(Collision2D col)
    {
        takingDamage = false;
        spawned = true;


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
        else if (Mathf.Abs(direction.x) == 0 && Mathf.Abs(direction.y) == 0)
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

        // Change sprite colour when taking damage(colliding with zombie)
        if (takingDamage)
        {
            hitTimer[1] = Time.time;

            this.GetComponent<SpriteRenderer>().color = Color.red;
        }
        else
        {
            this.GetComponent<SpriteRenderer>().color = Color.white;
        }

        if (alive)
        {
            // update timer
            timer[1] = Time.time;

            if ((timer[1] - timer[0]) > Random.Range(1, 3))
            {
                // change movement direction
                GenerateDirection();
            }

            if (hitTimer[1] - hitTimer[0] > 0.25)
            {
                takingDamage = false;

            }

            SetAnimation();

            // Apply movement
            transform.Translate(Vector2.right * (direction.x * movementSpeed * Time.deltaTime));
            transform.Translate(Vector2.up * (direction.y * movementSpeed * Time.deltaTime));
        }
        else
        {
            Destroy(this.gameObject);

        }

    }
}
