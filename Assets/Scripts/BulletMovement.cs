using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMovement : MonoBehaviour
{
    [SerializeField] float movementSpeed = 1;
    [SerializeField] int direction = 1; // true = positive direction

    float airTime; 

    // Start is called before the first frame update
    void Start()
    {
        airTime = 3f;
    }


    void OnCollisionEnter2D(Collision2D col)
    {

        // Destroy bullet once it hits a zombie
        Destroy(this.gameObject);
        
    }

    // Update is called once per frame
    void Update()
    {
        airTime -= Time.deltaTime;

        if (airTime < 0f)
        {
            Destroy(this.gameObject);
        }
        transform.Translate(Vector2.right * (movementSpeed * direction * Time.deltaTime));
    }
}
