using UnityEngine;

public class AgentSelector : MonoBehaviour
{
    [SerializeField] private AgentPolicy policyAgent;

    public enum AgentType
    {
        PolicyIteration,
        ValueIteration,
        MonteCarloES,
        MonteCarloESEvery,
        MonteCarloOffPolicy,
        MonteCarloOffPolicyEvery,
        MonteCarloOnPolicy,
        MonteCarloOnPolicyEvery,
        Sarsa,
        QLearning
    }
    
    private AgentType _type;

    public void ChangeAgentType(int value)
    {
        _type = (AgentType) value;
    }

    public void LaunchAgent()
    {
        switch (_type)
        {
            case AgentType.PolicyIteration:
                policyAgent.LaunchAgent(AgentPolicy.PolicyAlgorithm.PolicyIteration);
                break;
            case AgentType.ValueIteration:
                policyAgent.LaunchAgent(AgentPolicy.PolicyAlgorithm.ValueIteration);
                break;
            case AgentType.MonteCarloES:
                break;
            case AgentType.MonteCarloESEvery:
                break;
            case AgentType.MonteCarloOffPolicy:
                break;
            case AgentType.MonteCarloOffPolicyEvery:
                break;
            case AgentType.MonteCarloOnPolicy:
                break;
            case AgentType.MonteCarloOnPolicyEvery:
                break;
            case AgentType.Sarsa:
                break;
            case AgentType.QLearning:
                break;
        }
    }
}
