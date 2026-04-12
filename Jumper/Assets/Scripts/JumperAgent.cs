using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;

public class JumperAgent : Agent
{
    [Header("Movement Settings")]
    public float jumpForce = 6f;
    public float groundCheckDistance = 0.55f; // Slightly longer than half the agent's height

    private Rigidbody agentRigidbody;
    private bool isGrounded;

    public override void Initialize()
    {
        agentRigidbody = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        // Reset position and kill momentum instantly
        transform.localPosition = new Vector3(0, 1f, 0);
        agentRigidbody.linearVelocity = Vector3.zero;
        agentRigidbody.angularVelocity = Vector3.zero;

        // Clean up any remaining objects
        ClearObjectsByTag("Obstacle");
        ClearObjectsByTag("GroundBonus");
        ClearObjectsByTag("FlyingBonus");
    }

    private void ClearObjectsByTag(string targetTag)
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag(targetTag);
        foreach (GameObject obj in objects)
        {
            // Only destroy the object if it is a child of the same TrainingArea as this specific agent
            if (obj.transform.IsChildOf(transform.parent))
            {
                Destroy(obj);
            }
        }
    }

    private void FixedUpdate()
    {
        // Continuously check if the agent is exactly on the ground using a downward Raycast.
        // This prevents the physics engine from glitching out and allowing double jumps.
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        int jumpAction = actions.DiscreteActions[0];

        if (jumpAction == 1 && isGrounded)
        {
            agentRigidbody.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
        }

        // Small survival reward
        AddReward(0.001f);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        discreteActions[0] = Input.GetKey(KeyCode.Space) ? 1 : 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            AddReward(-1.0f);
            EndEpisode();
        }
        else
        {
            if (other.CompareTag("GroundBonus"))
            {
                AddReward(1.0f);
                Destroy(other.gameObject);
            }
            if (other.CompareTag("FlyingBonus"))
            {
                AddReward(3.0f);
                Destroy(other.gameObject);
            }
        }
    }
}