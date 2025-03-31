using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A code for eyes that follow the player as they move around the level

public class Eyes : MonoBehaviour
{
    public Transform player;
    public Transform baseOfEye;
    public float maxDistance = 0.5f;
    private bool stopMovement = false; 

    private void Update()
    {
        if (player == null || baseOfEye == null)
        {
            Debug.LogWarning("Player or baseOfEye references not set!");
            return;
        }

        if (!stopMovement)
        {
            Vector3 directionToPlayer = player.position - transform.position;

            Vector3 clampedPosition = baseOfEye.position + Vector3.ClampMagnitude(directionToPlayer, maxDistance);

            transform.position = clampedPosition;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(StopEyeMovementForOneSecond());
        }
    }

    private IEnumerator StopEyeMovementForOneSecond()
    {
        stopMovement = true;

        yield return new WaitForSeconds(1.0f);

        stopMovement = false;
    }
}

