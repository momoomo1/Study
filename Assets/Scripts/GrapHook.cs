using UnityEngine;

public class GrapHook : MonoBehaviour
{

    public LineRenderer line;
    public Transform hook;
    Vector2 mousedir;

    public float maxDistance, hookSpeed, pullSpeed;

    public bool isHookActive;
    public bool isAttach;
    public bool isRetract;

    Rigidbody2D rigid;
    float gravity;

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        gravity = rigid.gravityScale;

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
       if(Input.GetMouseButtonDown(0) && !isHookActive)
        {
            hook.position = transform.position; 

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            mousedir = (mousePos - transform.position).normalized;

            isHookActive = true;
            isAttach = false;
            isRetract = false;

            hook.gameObject.SetActive(true);
            line.enabled = true;
        }
        // 2. 좌클릭 뗌 (해제 및 자유낙하)
        if (Input.GetMouseButtonUp(0) && isHookActive)
        {
            StartRetract();
        }

  
        if (isHookActive)
        {
            line.SetPosition(0, transform.position);
            line.SetPosition(1, hook.position);

           
            if (isAttach)
            {
                // 플레이어를 훅 위치로 당기기 (유니티 6 최신 방식 적용)
                Vector2 pullDir = (hook.position - transform.position).normalized;
                rigid.linearVelocity = pullDir * pullSpeed;

                // 훅에 거의 도착하면 자동 회수 (충돌 방지)
                if (Vector2.Distance(transform.position, hook.position) < 0.5f)
                {
                    StartRetract();
                }
            }
            // [상태 B] 마우스를 떼거나 사거리가 다 돼서 회수 중
            else if (isRetract)
            {
                // 돌아올 때는 날아갈 때보다 2배 빠르게 슉! 돌아오도록 연출
                hook.position = Vector2.MoveTowards(hook.position, transform.position, Time.deltaTime * hookSpeed * 2f);

                if (Vector2.Distance(transform.position, hook.position) < 0.1f)
                {
                    isHookActive = false;
                    hook.gameObject.SetActive(false);
                    line.enabled = false;
                }
            }
            // [상태 C] 갈고리가 목표를 향해 날아가는 중
            else
            {
                hook.Translate(mousedir * Time.deltaTime * hookSpeed);

               
                Collider2D hit = Physics2D.OverlapCircle(hook.position, 0.2f, LayerMask.GetMask("Ground"));
                if (hit != null)
                {
                    isAttach = true;
                    // 벽에 붙는 순간 일직선으로 날아가도록 중력을 끄고 기존 관성 제거
                    rigid.gravityScale = 0f;
                    rigid.linearVelocity = Vector2.zero;
                }
                // 벽에 안 닿고 최대 사거리를 벗어나면 회수
                else if (Vector2.Distance(transform.position, hook.position) > maxDistance)
                {
                    StartRetract();
                }
            }
        }
    }

    void StartRetract()
    {
        isRetract = true;
        isAttach = false;
     
        rigid.gravityScale = gravity;
    }
}
