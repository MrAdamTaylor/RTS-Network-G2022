using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.AI;

public class PlayerUnityMovement : NetworkBehaviour
{
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private Targeter _targeter;

    /*[ServerCallback]
    private void Update()
    {
        if (!_agent.hasPath) { return; }

        if (_agent.remainingDistance <= _agent.stoppingDistance) { return; }
        
        _agent.ResetPath();
    }*/

    public void MoveUnit(Vector3 position)
    {
        _targeter.ClearTarget();
        
        if (!NavMesh.SamplePosition(position, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
        {
            return;
        }

        _agent.SetDestination(hit.position);
    }
    
}
