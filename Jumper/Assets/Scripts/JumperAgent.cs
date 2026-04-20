using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;

public class JumperAgent : Agent
{
    public float jumpForce = 5.6f;
    public float groundCheckDistance = 0.55f;
    private Rigidbody agentRigidbody;
    private bool isGrounded;

    public override void Initialize()
    {
        agentRigidbody = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(0, 1f, 0);
        agentRigidbody.linearVelocity = Vector3.zero;
        agentRigidbody.angularVelocity = Vector3.zero;

        ClearObjectsByTag("Obstacle");
        ClearObjectsByTag("GroundBonus");
        ClearObjectsByTag("FlyingBonus");
    }

    private void ClearObjectsByTag(string targetTag)
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag(targetTag);
        for (int i = 0; i < objects.Length; i++)
        {
            if (objects[i] != null && objects[i].transform.IsChildOf(transform.parent))
            {
                objects[i].SetActive(false);
                Destroy(objects[i]);
            }
        }
    }

    private void FixedUpdate()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        int jumpAction = actions.DiscreteActions[0];

        if (jumpAction == 1 && isGrounded && Mathf.Abs(agentRigidbody.linearVelocity.y) <= 0.1f)
        {
            agentRigidbody.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
            AddReward(-0.25f);
        }

    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        discreteActions[0] = Input.GetKey(KeyCode.Space) ? 1 : 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.activeInHierarchy) return;

        if (other.CompareTag("Obstacle"))
        {
            other.gameObject.SetActive(false);
            AddReward(-1.0f);
            EndEpisode();
        }
        else if (other.CompareTag("GroundBonus"))
        {
            AddReward(1.0f);
            other.gameObject.SetActive(false);
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("FlyingBonus"))
        {
            AddReward(2.7f);
            other.gameObject.SetActive(false);
            Destroy(other.gameObject);
        }
    }
}