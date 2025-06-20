using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMoving : MonoBehaviour
{
    public float detectionRadius = 5f;
    public float speed = 3f;
    public float figureEightRadius = 3f;

    public bool useFigureEight = true;

    private Rigidbody rb;
    private float angle = 0f;
    private bool isChasing = false;
    private Transform target;

    private int rotatingLeft = 1;
    private float angularSpeed;

    // 원형 순찰용 중심 (8자형이 아닐 때 사용)
    private Vector3 circleCenter;
    // 8자 순찰용 중심
    private Vector3 jointPosition;
    private Vector3 leftCenter;
    private Vector3 rightCenter;
    private Vector3 presentCenter;
    private GameObject player;
    
    // Strategy Pattern 추가
    private IMovementStrategy currentStrategy;
    private FigureEightMovementStrategy figureEightStrategy;
    private CircularMovementStrategy circularStrategy;
    private ChasingMovementStrategy chasingStrategy;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        player = GameObject.FindWithTag("Player");

        Vector3 startPos = transform.position;
        float y = Terrain.activeTerrain.SampleHeight(startPos);

        angularSpeed = speed / figureEightRadius;

        jointPosition = new Vector3(startPos.x, y, startPos.z);

        Vector3 left = jointPosition + new Vector3(-figureEightRadius, 0, 0);
        Vector3 right = jointPosition + new Vector3(figureEightRadius, 0, 0);

        left.y = Terrain.activeTerrain.SampleHeight(left);
        right.y = Terrain.activeTerrain.SampleHeight(right);

        leftCenter = left;
        rightCenter = right;

        // 원형 순찰용 초기 중심은 시작 위치
        circleCenter = jointPosition;
        presentCenter = leftCenter;
        
        // Strategy Pattern 초기화
        InitializeStrategies();
    }
    
    void InitializeStrategies()
    {
        figureEightStrategy = new FigureEightMovementStrategy();
        circularStrategy = new CircularMovementStrategy();
        chasingStrategy = new ChasingMovementStrategy();
        
        // 초기 전략 설정
        if (useFigureEight)
        {
            currentStrategy = figureEightStrategy;
        }
        else
        {
            currentStrategy = circularStrategy;
        }
        
        currentStrategy.Initialize(this, transform.position);
    }
    
    public void SetMovementStrategy(IMovementStrategy strategy)
    {
        currentStrategy = strategy;
        currentStrategy.Initialize(this, transform.position);
    }

    void FixedUpdate()
    {
        UpdateTarget();

        if (isChasing)
        {
            // 추격 전략 사용
            if (currentStrategy != chasingStrategy)
            {
                currentStrategy = chasingStrategy;
                currentStrategy.Initialize(this, transform.position);
            }
            currentStrategy.Move(this, rb);
            return;
        }

        // 일반 이동 전략 사용
        if (currentStrategy == chasingStrategy)
        {
            // 추격에서 돌아왔을 때 원래 전략으로 복귀
            if (useFigureEight)
            {
                currentStrategy = figureEightStrategy;
                // 8자 이동 중심점 재설정
                if (figureEightStrategy != null)
                {
                    figureEightStrategy.UpdateCentersAfterChasing(transform.position, figureEightRadius);
                }
            }
            else
            {
                currentStrategy = circularStrategy;
                // 원형 이동 중심점 재설정
                if (circularStrategy != null)
                {
                    circularStrategy.UpdateCenterAfterChasing(transform.position, angle, figureEightRadius);
                }
            }
            currentStrategy.Initialize(this, transform.position);
        }
        
        currentStrategy.Move(this, rb);
    }

    // 기존 메서드들 - 하위 호환성을 위해 유지
    void MoveInFigureEight()
    {
        angle += angularSpeed * Time.fixedDeltaTime * rotatingLeft;

        float x = presentCenter.x + Mathf.Cos(angle) * figureEightRadius;
        float z = presentCenter.z + Mathf.Sin(angle) * figureEightRadius;
        Vector3 targetPos = new Vector3(x, rb.position.y, z);
        Vector3 move = targetPos - rb.position;
        if (angle >= 2f * Mathf.PI)
        {
            angle = Mathf.PI;
            rotatingLeft = -1;
            presentCenter = rightCenter;
        }
        else if (angle <= -Mathf.PI)
        {
            angle = 0;
            rotatingLeft = 1;
            presentCenter = leftCenter;
        }
        rb.MovePosition(rb.position + SlideAlongObstacle(move));
    }

    void UpdateTarget()
    {
        float dist = Vector3.Distance(transform.position, player.transform.position);
        if (dist <= detectionRadius)
        {
            isChasing = true;
            target = player.transform;
        }
        else
        {
            if (isChasing)
            {
                Vector3 currentPos = transform.position;
                if (!useFigureEight)
                {
                    float currentAngle = angle;
                    float radius = figureEightRadius;

                    Vector3 newCenter = currentPos - new Vector3(Mathf.Cos(currentAngle), 0, Mathf.Sin(currentAngle)) * radius;
                    newCenter.y = Terrain.activeTerrain.SampleHeight(newCenter);

                    circleCenter = newCenter;
                }
                else
                {
                    Vector3 left = currentPos + new Vector3(-figureEightRadius, 0, 0);
                    Vector3 right = currentPos + new Vector3(figureEightRadius, 0, 0);

                    left.y = Terrain.activeTerrain.SampleHeight(left);
                    right.y = Terrain.activeTerrain.SampleHeight(right);

                    leftCenter = left;
                    rightCenter = right;
                    presentCenter = leftCenter;
                    rotatingLeft = 1;
                    angle = 0;
                }
            }
            isChasing = false;
        }
    }

    public Vector3 SlideAlongObstacle(Vector3 move)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, move.normalized, out hit, move.magnitude + 0.1f))
        {
            Vector3 slideDirection = Vector3.ProjectOnPlane(move, hit.normal);
            return slideDirection;
        }
        return move;
    }
    
    // Strategy Pattern 공개 메서드
    public string GetCurrentStrategyName()
    {
        return currentStrategy?.GetStrategyName() ?? "None";
    }
}
