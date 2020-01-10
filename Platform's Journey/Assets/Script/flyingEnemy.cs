using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class flyingEnemy : MonoBehaviour
{
    public int enemySpeed;
    public int xMoveDirection;
    private bool facingRight = true;

    //New movement code
    public bool MoveRight;

    //This is for the player 
    GameObject player;

    //This is for the animator
    public Animator animator;

    //Sounds
    public AudioSource eagleDead;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        //Enemy movement
        flipEnemyDirection();

        //This checks if the player hits the enemy and if the player hits the enemies side then it kills the player
        enemyRayCast();

        //This checks if the player stomps the enemy
        enemyKilled();

        //Death by falling
        if (gameObject.transform.position.y <= -7)
        {
            Die();
        }
    }

    //This cause the enemy to turn when it comes close to a wall and continue his path
    void flipEnemyDirection()
    {
        if (MoveRight)
        {
            transform.Translate(2 * Time.deltaTime * enemySpeed, 0, 0);
            transform.localScale = new Vector2(5, 5);//3.818752, 3.712972);
        }
        else
        {
            transform.Translate(-2 * Time.deltaTime * enemySpeed, 0, 0);
            transform.localScale = new Vector2(-5, 5);//-3.818752, 3.712972);
        }
    }

    //This flips enemy's skins direction
    void flipEnemy()
    {
        facingRight = !facingRight; //If you are facing right and turn then you are not facing right
        Vector2 localScale = gameObject.transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    private void OnTriggerEnter2D(Collider2D trig)
    {
        if (trig.gameObject.CompareTag("Turn"))
        {
            if (MoveRight)
            {
                MoveRight = false;
            }
            else
            {
                MoveRight = true;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        RaycastHit2D hitRight = Physics2D.Raycast(transform.position, Vector2.right);
        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, Vector2.left);

        if (collision.gameObject.tag == "Player")
        {
            if (MoveRight)
            {
                MoveRight = false;
                Destroy(hitRight.collider.gameObject);
                player.SendMessage("Die", SendMessageOptions.DontRequireReceiver);
            }
            else
            {
                MoveRight = true;
                Destroy(hitRight.collider.gameObject);
                player.SendMessage("Die", SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    //This is used to detect if the player is to the right or left and then destroy the player
    void enemyRayCast()
    {
        RaycastHit2D hitRight = Physics2D.Raycast(transform.position, Vector2.right);
        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, Vector2.left);

        //This is for the player
        player = GameObject.FindWithTag("Player");

        if (hitRight.distance < 0.7f)
        {
            if (hitRight.collider.tag == "Player")
            {
                Destroy(hitRight.collider.gameObject);
                player.SendMessage("Die", SendMessageOptions.DontRequireReceiver);
            }
        }
        else if (hitLeft.distance < 0.7f)
        {
            if (hitLeft.collider.tag == "Player")
            {
                Destroy(hitLeft.collider.gameObject);
                player.SendMessage("Die", SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    void enemyKilled()
    {
        RaycastHit2D hitFromAbove = Physics2D.Raycast(transform.position, Vector2.up);

        //This is for the player
        player = GameObject.FindWithTag("Player");

        if (/*hitFromAbove != null &&*/ hitFromAbove.collider != null && hitFromAbove.distance < 1f)
        {
            if (hitFromAbove.collider.tag == "Player")
            {
                player.GetComponent<Rigidbody2D>().AddForce(Vector2.up * 1000);

                animator.SetBool("isDead", true);

                enemySpeed = 0;

                eagleDead.Play();

                Destroy(this.gameObject, 0.2f);
                player.SendMessage("enemyKilledScoreUp", SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    void Die()
    {
        Destroy(this.gameObject);
    }
}
