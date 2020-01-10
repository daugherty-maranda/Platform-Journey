using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Player : MonoBehaviour
{
    //This is for moving and jumping
    public int playerSpeed = 10;
    private bool facingRight = true;
    public int jumpForce = 1250;
    private float moveX;

    //This for Ground detection, so that the character can jump
    public bool isGrounded;
    
    //Sprite itself
    private Rigidbody2D rb;  //<--- used with AddForce()
    private SpriteRenderer sprite; //Facing direction

    private bool jump = false; //Not used

    //This is for for player health
    public int health = 150; //<-- This is not used for now
    private bool dead;

    //This is for the player's score
    private float timeLeft = 120;
    public int playerScore = 0;
    
    //This is for the Text that will be displayed
    public Text collectableText;
    public int collectablesNum = 0;
    public int collectTotal;

    //This is the UI for score and time
    public GameObject time_leftUI;
    public GameObject player_scoreUI;
    public GameObject collectables_UI;

    //Win Screen  <-- Work in progress
    public GameObject playerWon;
    public bool won = false;

    //This is for the animator
    public Animator animator;

    //Sounds
    public AudioSource playerHurt;
    public AudioSource coinPickUp;
    public AudioSource gemPickUp;
    public AudioSource jumping;
    public AudioSource endOfLevel;


    // Start is called before the first frame update
    void Start()
    {
        //Player's Character
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        isGrounded = true;

        //Collectables Total on levels
        collectTotal = GameObject.FindGameObjectsWithTag("Gem").Length;
    }

    // Update is called once per frame
    void Update()
    {
        //Moves the player
        playerMove();

        //Debug.DrawRay(transform.position - new Vector3(0, sprite.bounds.extents.y + 0.01f, 0), Vector2.down * 0.2f, Color.red);

        //Animations - Specifically, the running and idle animations
        animator.SetFloat("Speed", Mathf.Abs(moveX));

        if (isGrounded == true)
        {
            animator.SetBool("IsJumping", false);
        }

        //Death by falling
        if(gameObject.transform.position.y <= -7)
        {
            Die();
        }

        //This is for the player's score
        if(won != true)
        {
            timeLeft -= Time.deltaTime; //This times the player
        }

        time_leftUI.gameObject.GetComponent<Text>().text = ("Time Left: " + (int)timeLeft);
        player_scoreUI.gameObject.GetComponent<Text>().text = ("Score: " + playerScore);

        collectables_UI.gameObject.GetComponent<Text>().text = ("Gems Collected: " + collectablesNum + "/" + collectTotal);

        //This restarts the scene if the player runs out of time
        if (timeLeft < 0.1f)
        {
            string sceneName = SceneManager.GetActiveScene().name;

            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }
    }

    //This allows the player to move the character
    void playerMove()
    {
        //Character controls
        moveX = Input.GetAxis("Horizontal");

        if (Input.GetButton("Jump") && isGrounded == true)
        { 
            playerJump();
        }

        //Debug.DrawRay(transform.position - new Vector3(0, sprite.bounds.extents.y + 0.0001f, 0), Vector2.down * 0.1f, Color.red);

        //Player's Direction
        if (moveX > 0.0f && facingRight == false)
        {
            //This flips the character, so that it is facing left
            flipPlayer();
        }
        else if (moveX < 0.0f && facingRight == true)
        {
            //This flips the character, so that it is facing right
            flipPlayer();
        }
        //Physics that apply to player movement
        gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(moveX * playerSpeed, gameObject.GetComponent<Rigidbody2D>().velocity.y); //GetComponent allows you to mess with a component of a gameObject that the script is attached to.

    }

    
    //This allows the player to jump
    void playerJump()
    {
        GetComponent<Rigidbody2D>().AddForce(Vector2.up * jumpForce);
        isGrounded = false;
        jump = true;

        jumping.Play();

        animator.SetBool("IsJumping", true);
    }

    //This just flips the character model
    void flipPlayer()
    {
        facingRight = !facingRight; //If you are facing right and turn then you are not facing right
        Vector2 localScale = gameObject.transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //RaycastHit2D hitDown = Physics2D.Raycast(transform.position, Vector2.down);
        RaycastHit2D hitDown = Physics2D.Raycast(transform.position - new Vector3(0, sprite.bounds.extents.y + 0.0001f, 0), Vector2.down, 0.1f); //sprite.bounds.extents.y + 0.01f
        Debug.Log(hitDown.distance);

        if (hitDown)
        {
            Debug.Log("Touching Ground!");
            if (collision.gameObject.tag == "Ground")
            {
                isGrounded = true;
                jumpForce = 1250;
                jump = false;
            }
            //I don't want the player to be able to jump as high if it is not the ground
            //This was a solution to the player getting extra lift when holding the 'jump' key and grazing the pillar's collison box.
            else if (collision.gameObject.tag == "Pillar")
            {
                isGrounded = true;
                jumpForce = 625;
                jump = false;
            }
        }
        else if(hitDown.distance == 0f && collision.gameObject.tag == "Ground" || hitDown.distance == 0f && collision.gameObject.tag == "Pillar")
        {
            if (collision.gameObject.tag == "Ground")
            {
                isGrounded = true;
                jumpForce = 125;
                jump = false;
            }
            //I don't want the player to be able to jump as high if it is not the ground
            //This was a solution to the player getting extra lift when holding the 'jump' key and grazing the pillar's collison box.
            else if (collision.gameObject.tag == "Pillar")
            {
                isGrounded = true;
                jumpForce = 125;
                jump = false;
            }
        }
        else
        {
            isGrounded = false;
        }
        
    }

    //This reloads the scene when the player dies
    void Die ()
    {
        animator.SetBool("isDead", true);
        playerHurt.Play();

        Destroy(gameObject, 1f);

        string sceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);//"Starter");
    }

    //Player wins if they make it to the end of the level
    private void OnTriggerEnter2D(Collider2D trig)
    {
        if (trig.gameObject.tag == "End Level")
        {
            CountScore();
            Destroy(trig.gameObject);
            winGame();

            endOfLevel.Play();

            StartCoroutine(WaitForScene());
        }

        //If the player runs into a 'coin' then it counts the score and destroys the coin
        if (trig.gameObject.tag == "Coin")
        {
            playerScore += 10;

            coinPickUp.Play();

            Destroy(trig.gameObject); //This destroys the coin, so that it will not be counted multiple times when the player runs into them
        }

        if (trig.gameObject.tag == "Gem")
        {
            playerScore += 100;
            collectablesNum += 1;

            gemPickUp.Play();

            Destroy(trig.gameObject); //This destroys the gem, so that it will not be counted multiple times when the player runs into them
        }
    }

    //This counts up the player's score
    void CountScore()
    {
        playerScore = playerScore + (int)(timeLeft * 10);
    }

    /*
    //This shoots a ray down from the player and everytime it hits an enemy, the player bounces of the enemy
    void playerRayCastJumps()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down);

        //Debug.Log(hit.distance);
        if (hit != null && hit.collider != null && hit.distance < 1.18f && hit.collider.tag == "Enemy")
        {
            //This cause the player to bounce if they jump on an enemy
            GetComponent<Rigidbody2D>().AddForce(Vector2.up * 1000);
            Destroy(hit.collider.gameObject);
            playerScore += 50;
        }
    }*/

    //This needs to be handled by the GameController
    void winGame()
    {
        won = true;
        playerSpeed = 0;
        playerWon.gameObject.GetComponent<Text>().text = ("YOU WIN!");
    }

    private IEnumerator WaitForScene()
    {
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

        /*
         string sceneName = SceneManager.GetActiveScene().name;
         if(sceneName != "Win" || sceneName != "Lose"){
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
         }
         if else (sceneName.equalTo("Final Level){
            SceneManager.LoadScene("Win", LoadSceneMode.Single);
         }
        */
    }

    //This recieves a message from the Enemy Script once the enemy is destroyed and adds 50 to the score
    void enemyKilledScoreUp()
    {
        playerScore += 50;
    }
}
