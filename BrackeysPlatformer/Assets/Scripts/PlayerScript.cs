using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public float speed = 5f;
    public float jump_power = 1f;
    public int curr_health;
    public int max_health;
    public Image[] hearts;
    public Sprite full_heart;
    public Sprite empty_heart;

    private Rigidbody2D rb;
    private Collider2D coll;
    private Animator anim;

    //states
    private enum states {idle, running, jumping, die, falling, landing, attacking}
    private states state = states.idle;

    private int jump_count;
    [SerializeField] private LayerMask ground;
    

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        coll = GetComponent<CircleCollider2D>();
        curr_health = max_health;
    }

    // Update is called once per frame
    void Update()
    {
        
        //health system
        for(int i = 0; i < hearts.Length; i++){
            if(i < curr_health){
                hearts[i].sprite = full_heart;
            }
            else hearts[i].sprite = empty_heart;

            if(i< max_health){
                hearts[i].enabled = true;
            }
            else hearts[i].enabled = false;
            
        }

        if(curr_health == 0) state = states.die;


        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0f, 0f);
        transform.position += movement * Time.deltaTime * speed;

        //sprite flip
        if(movement.x < 0){
            transform.localScale = new Vector2(-1, 1);
        }

        else if(movement.x > 0){
            transform.localScale = new Vector2(1, 1);
        }

        //jumps, currently allows double jump
        if(Input.GetButtonDown("Jump") && jump_count < 2){
            rb.AddForce(new Vector2(0, jump_power), ForceMode2D.Impulse);
            state = states.jumping;
            jump_count++;
        }

        StateSwitch(movement);
        anim.SetInteger("state", (int) state);
        
    }

    private void StateSwitch(Vector3 movement){
        
        if(state == states.jumping){
            if(rb.velocity.y < .1f){
                state = states.falling;
            }
        }

        else if(state == states.falling){
            
            if (coll.IsTouchingLayers(ground)){
                state = states.landing;
                jump_count = 0;
            }
        }

        else if(Mathf.Abs(movement.x) > 0.01f){
            state = states.running;
        }

        
        else {
            state = states.idle;
        }
        
    }
}
