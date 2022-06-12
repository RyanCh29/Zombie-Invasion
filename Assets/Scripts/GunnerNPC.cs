using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunnerNPC : MonoBehaviour
{
    GameController game;
    [SerializeField] Animator anim;

    [SerializeField] GameObject prefabZombie;
    [SerializeField] GameObject prefabBullet;
    [SerializeField] GameObject prefabBulletVariant;


    bool spawned = false;

    bool alive = true;

    [SerializeField] int health = 1;
    bool takingDamage = false;
    float[] hitTimer = new float[2];


    float[] shotTimer = { 0f, 0f };

    float[] timer = new float[2];

    Vector2 direction = new Vector2(0, 0);
    [SerializeField] float movementSpeed = 1f;

    bool spriteFlipX = false;

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
        if (direction.x < 0)
        {
            spriteFlipX = true;
            this.transform.GetChild(0).GetComponentInChildren<BoxCollider2D>().offset = new Vector2(-1.5f, 0);
        }
        else if (direction.x > 0)
        {
            spriteFlipX = false;
            this.transform.GetChild(0).GetComponentInChildren<BoxCollider2D>().offset = new Vector2(1.5f, 0);
        }
    }

    void TakeDamage(int dmg, string tag)
    {
        // Decrement health
        health -= dmg;
        if (health <= 0)
        {
            //Debug.Log("gunner collide with " + col.collider.name);

            if (tag.Equals("Zombie"))
            {
                // create a new zombie object in the scene
                GameObject instantiator = Instantiate<GameObject>(prefabZombie, new Vector2(transform.position.x + 0.5f, transform.position.y - 0.5f), Quaternion.identity);
                instantiator.name = "zombie_" + Random.Range(0, 10000);

                game.UpdateScore(20, "Gunner");
            }
            // destroy the gunner object
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
        if (!colTag.Equals("GunnerRangeDetector"))
        {
            if ((colTag.Equals("Zombie") || colTag.Equals("Bullet")) && alive)
            {
                hitTimer[0] = Time.time;

                takingDamage = true;

                TakeDamage(10, colTag);
            }
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
        if (!colTag.Equals("GunnerRangeDetector"))
        {
            if ((colTag.Equals("Zombie") || colTag.Equals("Bullet")) && alive)
            {
                hitTimer[0] = Time.time;
                takingDamage = true;

                TakeDamage(1, colTag);
            }
        }
    }

    void OnCollisionExit2D(Collision2D col)
    {
        
        takingDamage = false;
        spawned = true;

    }

    void Shoot(string tag)
    {
        if (tag.Equals("Zombie"))
        {
            shotTimer[1] = Time.time;
            if (shotTimer[1] - shotTimer[0] > 1)
            {
                shotTimer[0] = Time.time;

                Vector2 tempPos = transform.position;
                // Create a new bullet object in the scene
                if (spriteFlipX)
                {
                    tempPos.x -= 0.8f;
                    GameObject instantiator = Instantiate<GameObject>(prefabBulletVariant, tempPos, Quaternion.identity);


                }
                else
                {
                    tempPos.x += 0.8f;
                    GameObject instantiator = Instantiate<GameObject>(prefabBullet, tempPos, Quaternion.identity);
                }
            }

        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        //shotTimer[0] = Time.time;

        //Debug.Log("col = " + col.name);
        Shoot(col.tag);

    }
    void OnTriggerStay2D(Collider2D col)
    {
        //Debug.Log("col = " + col.name);
        Shoot(col.tag);
    }

    void SetAnimation()
    {
        bool move = true;

        if (direction.x == 0 && direction.y == 0)
        {
            move = false;
        }

        if (!alive)
        {
            anim.SetTrigger("Death");
        }
        
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

            //Debug.Log("X: " + direction[0]);
            //Debug.Log("Y: " + direction[1]);
            if ((timer[1] - timer[0]) > Random.Range(1, 3))
            {
                // change movement direction
                GenerateDirection();
            }

            
            if (hitTimer[1] - hitTimer[0] > 0.25)
            {
                takingDamage = false;

            }

            // flip X based on direction of movement
            this.GetComponent<SpriteRenderer>().flipX = spriteFlipX;

            SetAnimation();

            // Apply movement
            transform.Translate(Vector2.right * (direction.x * movementSpeed * Time.deltaTime));
            transform.Translate(Vector2.up * (direction.y * movementSpeed * Time.deltaTime));
        }
        else
        {

            SetAnimation();
            Destroy(this.gameObject, 0.5f);
        }

    }
}
