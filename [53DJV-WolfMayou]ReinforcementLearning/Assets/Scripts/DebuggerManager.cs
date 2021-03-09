using UnityEngine;

public class DebuggerManager : MonoBehaviour
{
    public void ClearIntents()
    {
        foreach (Transform child in this.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
