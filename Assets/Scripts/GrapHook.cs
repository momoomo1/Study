using UnityEngine;

public class GrapHook : MonoBehaviour
{

    public LineRenderer line;
    public Transform hook;
    Vector2 mousedir;

    public float maxDistance, hookSpeed, pullSpeed; // 수치설정 순서대로 로프길이, 로프 속도, 끌려가는 속도

    public bool isHookActive; //로프 작동중?
    public bool isAttach; //로프 박힘?
    public bool isRetract; // 회수중?
    public bool isPull; //오브젝트 연결됨?

    Rigidbody2D rigid;
    float gravity; //벽으로 날라갈 때 중력무시용
    DistanceJoint2D ropeJoint; 

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        gravity = rigid.gravityScale;

        ropeJoint = GetComponent<DistanceJoint2D>();
        if(ropeJoint != null) ropeJoint.enabled = false;

        line.positionCount = 2;
        line.endWidth = line.startWidth = 0.05f;
        line.useWorldSpace = true;  

        hook.gameObject.SetActive(false);
        line.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        //발사
        if (Input.GetMouseButtonDown(0) && !isHookActive)
        {
            hook.position = transform.position;
            hook.SetParent(null);

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            mousedir = (mousePos - transform.position).normalized;

            //상태 초기화 
            isHookActive = true;
            isAttach = false;
            isRetract = false;
            isPull = false;

            hook.gameObject.SetActive(true);
            line.enabled = true;
        }

        //회수
        if (Input.GetMouseButtonUp(0) && isHookActive)
        {
            StartRetract();
        }

      //로프 로직
        if (isHookActive)
        {
            line.SetPosition(0, transform.position);
            line.SetPosition(1, hook.position);

          //벽에 박힘
            if (isAttach)
            {
                //
                if (Vector2.Distance(transform.position, hook.position) > 0.6f) 
                {
                    Vector2 pullDir = (hook.position - transform.position).normalized;
                    rigid.linearVelocity = pullDir * pullSpeed;
                }
                else
                { //목적지에 도달하면 그자리 고정 
                    rigid.linearVelocity = Vector2.zero;
                }
            }
            //회수중 
            else if (isRetract)
            {   
                hook.position = Vector2.MoveTowards(hook.position, transform.position, Time.deltaTime * hookSpeed * 2f);

                if (Vector2.Distance(transform.position, hook.position) < 0.1f)
                {
                    isHookActive = false;
                    hook.gameObject.SetActive(false);
                    line.enabled = false;
                }
            }
            //오브젝트 견인 중
            else if (isPull)
            {
             
                if (Vector2.Distance(transform.position, hook.position) > maxDistance + 2f)
                {
                    StartRetract();
                }
            }
          
            else
            {
                hook.Translate(mousedir * Time.deltaTime * hookSpeed);

                Collider2D wallHit = Physics2D.OverlapCircle(hook.position, 0.2f, LayerMask.GetMask("Ground"));
                Collider2D pullHit = Physics2D.OverlapCircle(hook.position, 0.2f, LayerMask.GetMask("Target"));
                //로프가 벽에 맞았을 때
                if (wallHit != null)
                {
                    isAttach = true;
                    rigid.gravityScale = 0f;
                    rigid.linearVelocity = Vector2.zero;
                }
                //로프가 오브젝트에 맞았을때
                else if (pullHit != null)
                {
                    isPull = true;
                    hook.SetParent(pullHit.transform);

                    ropeJoint.enabled = true;
                    ropeJoint.connectedBody = pullHit.attachedRigidbody;
                    ropeJoint.distance = Vector2.Distance(transform.position, hook.position);
                }
                //헛쳤을때
                else if (Vector2.Distance(transform.position, hook.position) > maxDistance)
                {
                    StartRetract(); 
                }
            }
        }
    }

    //회수함수
    void StartRetract()
    {
        isRetract = true; 
        isAttach = false; 
        isPull = false;

        rigid.gravityScale = gravity; //중력정상화

        //오브젝트랑 연결이 끊기면
        if (ropeJoint != null)
        {
            ropeJoint.enabled = false;
            ropeJoint.connectedBody = null;
        }
        hook.SetParent(null);
    }
}
