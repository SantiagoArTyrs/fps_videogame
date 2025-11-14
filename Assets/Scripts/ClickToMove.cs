using UnityEngine;

public class ClickToMove : MonoBehaviour
{
    private UnityEngine.AI.NavMeshAgent navAgent;

    private void Start()
    {
        navAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();

    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, UnityEngine.AI.NavMesh.AllAreas))
            {
                navAgent.SetDestination(hit.point);
            }
        }
    }
}
