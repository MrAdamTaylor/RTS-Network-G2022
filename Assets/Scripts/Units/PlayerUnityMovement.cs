using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.AI;

public class PlayerUnityMovement : NetworkBehaviour
{
    [SerializeField] private NavMeshAgent _agent;

    public void MoveUnit(Vector3 position)
    {
        if (!NavMesh.SamplePosition(position, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
        {
            return;
        }

        _agent.SetDestination(hit.position);
    }
    
}
