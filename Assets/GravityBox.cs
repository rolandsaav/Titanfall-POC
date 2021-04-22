using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityBox : MonoBehaviour
{
    public float gravity;
    public float jump;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            print("Touching player");
            other.GetComponent<ThirdPersonMovement>().gravityMultiplier = gravity;
            other.GetComponent<ThirdPersonMovement>().jumpHeight = jump;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            print("Touching player");
            other.GetComponent<ThirdPersonMovement>().gravityMultiplier = 1;
            other.GetComponent<ThirdPersonMovement>().jumpHeight = 3;
        }
    }
}
