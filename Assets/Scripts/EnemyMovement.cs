using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    public enum EnemyType
    {
        Pursuer,   // Persigue al jugador
        Evader,    // Huye del jugador si está cerca, sino vaga
        Wanderer,  // Se mueve aleatoriamente por todo el mapa
        Sentinel   // Patrullaje entre dos puntos
    }

    public EnemyType enemyType;
    public Transform player;

    [Header("Evader Settings")]
    public float evadeDistance = 10f;

    [Header("Wander Settings")]
    public float wanderTimer = 3f;

    [Header("Sentinel Settings")]
    public Transform pointA;
    public Transform pointB;

    private Vector2 areaMin;
    private Vector2 areaMax;

    private NavMeshAgent navMeshAgent;
    private float timer;
    private Transform currentTarget;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        timer = wanderTimer;
        DetectMapBounds();

        // Inicializa el enemigo Sentinel para patrullar entre los puntos A y B
        if (enemyType == EnemyType.Sentinel && pointA && pointB)
        {
            currentTarget = pointA;
            navMeshAgent.SetDestination(currentTarget.position);
        }
    }

    void Update()
    {
        // Si el tipo de enemigo no es Sentinel, y no hay jugador asignado, se sale.
        if (enemyType != EnemyType.Sentinel && player == null) return;

        timer += Time.deltaTime;

        switch (enemyType)
        {
            case EnemyType.Pursuer:
                // Persigue al jugador
                navMeshAgent.SetDestination(player.position);
                break;

            case EnemyType.Evader:
                // Si está cerca del jugador, huye.
                float distance = Vector3.Distance(transform.position, player.position);
                if (distance < evadeDistance)
                {
                    Vector3 dirAway = (transform.position - player.position).normalized;
                    Vector3 fleeTarget = transform.position + dirAway * evadeDistance;

                    NavMeshHit hit;
                    if (NavMesh.SamplePosition(fleeTarget, out hit, 10f, NavMesh.AllAreas))
                    {
                        navMeshAgent.SetDestination(hit.position);
                    }
                }
                else if (!navMeshAgent.hasPath || navMeshAgent.remainingDistance < 0.5f || timer >= wanderTimer)
                {
                    Vector3 newPos = GetFarthestRandomPoint(areaMin, areaMax, transform.position);
                    navMeshAgent.SetDestination(newPos);
                    timer = 0;
                }
                break;

            case EnemyType.Wanderer:
                // Se mueve aleatoriamente por el mapa
                if (!navMeshAgent.hasPath || navMeshAgent.remainingDistance < 0.5f || timer >= wanderTimer)
                {
                    Vector3 newPos = GetFarthestRandomPoint(areaMin, areaMax, transform.position);
                    navMeshAgent.SetDestination(newPos);
                    timer = 0;
                }
                break;

            case EnemyType.Sentinel:
                // Patrullaje entre los dos puntos
                if (pointA == null || pointB == null) return;

                // Comprobar si ha llegado al destino de forma más robusta
                if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= 0.5f)
                {
                    // Cambiar al siguiente punto de destino (si estaba en pointA, cambiar a pointB, y viceversa)
                    currentTarget = currentTarget == pointA ? pointB : pointA;

                    // Establecer un nuevo destino, pero asegurarse de que el agente se mueva correctamente
                    navMeshAgent.SetDestination(currentTarget.position);
                }
                break;
        }
    }

    // Detectar las paredes y calcular los límites
    void DetectMapBounds()
    {
        GameObject westWall = GameObject.Find("West Wall");
        GameObject eastWall = GameObject.Find("East Wall");
        GameObject northWall = GameObject.Find("North Wall");
        GameObject southWall = GameObject.Find("South Wall");

        if (westWall && eastWall && northWall && southWall)
        {
            float minX = westWall.transform.position.x;
            float maxX = eastWall.transform.position.x;
            float minZ = southWall.transform.position.z;
            float maxZ = northWall.transform.position.z;

            areaMin = new Vector2(minX, minZ);
            areaMax = new Vector2(maxX, maxZ);
        }
        else
        {
            Debug.LogWarning("No se encontraron las paredes correctamente.");
        }
    }

    // Obtener un punto aleatorio dentro de los límites del mapa
    Vector3 RandomPointInArea(Vector2 min, Vector2 max)
    {
        float x = Random.Range(min.x, max.x);
        float z = Random.Range(min.y, max.y);
        return new Vector3(x, 0, z);
    }

    // Asegurarse de que el punto aleatorio esté dentro del NavMesh
    Vector3 GetValidNavMeshPosition(Vector3 target, float range)
    {
        NavMeshHit navHit;
        if (NavMesh.SamplePosition(target, out navHit, range, NavMesh.AllAreas))
        {
            return navHit.position;
        }
        return transform.position;
    }

    // Obtener el punto más alejado en el mapa
    Vector3 GetFarthestRandomPoint(Vector2 min, Vector2 max, Vector3 fromPosition)
    {
        Vector3 farthestPoint = fromPosition;
        float maxDistance = 0f;

        for (int i = 0; i < 10; i++)
        {
            Vector3 randomPoint = RandomPointInArea(min, max);
            float dist = Vector3.Distance(fromPosition, randomPoint);

            if (dist > maxDistance)
            {
                Vector3 validPoint = GetValidNavMeshPosition(randomPoint, 10f);
                if (validPoint != fromPosition)
                {
                    maxDistance = dist;
                    farthestPoint = validPoint;
                }
            }
        }

        return farthestPoint;
    }
}
