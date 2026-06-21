using UnityEngine;


public class Player : MonoBehaviour

{

    public float maxSpeed, jumpPower;
    bool isGrounded;
    Rigidbody2D rigid;

   

    public Transform[] testZone; 




    void Awake()

    {

        rigid = GetComponent<Rigidbody2D>();

    }

     void Update()
    {
        //Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            isGrounded = false;
        }

        //stop
        if(Input.GetButtonUp("Horizontal"))
        {
            rigid.linearVelocity = new Vector2(rigid.linearVelocity.normalized.x * 0.6f, rigid.linearVelocity.y);
        }

        //Teleport
        if (Input.GetKeyDown(KeyCode.Alpha1)) TeleportToZone(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) TeleportToZone(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) TeleportToZone(2);

    }



    void FixedUpdate()

    {
        GrapHook hook = GetComponent<GrapHook>();

        if (hook != null && hook.isAttach)
        {
            
        }
        else
        {
            float h = Input.GetAxisRaw("Horizontal");

            rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

            //maxSpeed
            if (rigid.linearVelocity.x > maxSpeed)

                rigid.linearVelocity = new Vector2(maxSpeed, rigid.linearVelocity.y);

            else if (rigid.linearVelocity.x < maxSpeed * (-1))

                rigid.linearVelocity = new Vector2(maxSpeed * (-1), rigid.linearVelocity.y);
        }


            //move
            

     
        RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Ground"));

        if(rigid.linearVelocity.y < 0 )
        {
            if (rayHit.collider != null)
            {
                if (rayHit.distance < 0.5f)
                {
                    isGrounded = true;
                }
                else
                    isGrounded = false;
                   
            }
        }     
    }

    void TeleportToZone(int zoneIndex)
    {
       if(zoneIndex < testZone.Length && testZone[zoneIndex] != null)
        {
            transform.position = testZone[zoneIndex].position;

            rigid.linearVelocity = Vector2.zero;
        }
    }

}
