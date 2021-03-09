using UnityEngine;

public class AgentSelector : MonoBehaviour
{
    [SerializeField] private AgentPolicy policyAgent;
    [SerializeField] private AgentMonteCarlo monteCarloAgent;

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
            case AgentType.ValueIteration:
                policyAgent.LaunchAgent(_type);
                break;
            case AgentType.MonteCarloES:
            case AgentType.MonteCarloESEvery:
            case AgentType.MonteCarloOffPolicy:
            case AgentType.MonteCarloOffPolicyEvery:
            case AgentType.MonteCarloOnPolicy:
            case AgentType.MonteCarloOnPolicyEvery:
                monteCarloAgent.LaunchAgent(_type);
                break;
            case AgentType.Sarsa:
                break;
            case AgentType.QLearning:
                break;
        }
    }
}
