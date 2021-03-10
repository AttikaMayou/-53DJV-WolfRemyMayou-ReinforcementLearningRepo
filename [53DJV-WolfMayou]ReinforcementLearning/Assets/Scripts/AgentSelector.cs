using UnityEngine;
using UnityEngine.UI;

public class AgentSelector : MonoBehaviour
{
    [SerializeField] private AgentPolicy policyAgent;
    [SerializeField] private AgentMonteCarlo monteCarloAgent;

    [SerializeField] private Toggle stepByStep;

    public enum AgentType
    {
        PolicyIteration,
        ValueIteration,
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
                policyAgent.LaunchAgent(_type, stepByStep.isOn);
                break;
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
