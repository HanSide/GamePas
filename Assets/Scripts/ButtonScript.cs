using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonScript : MonoBehaviour
{
    public void Retry()
    {
        ResetAllSystems();
        SceneManager.LoadScene(1);
    }

    public void Home()
    {
        ResetAllSystems();
        SceneManager.LoadScene(0);
    }

    private void ResetAllSystems()
    {
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.ResetInventory();

        if (MultiCollectibleCounter.Instance != null)
            ResetCollectibleCounter();
    }

    private void ResetCollectibleCounter()
    {
        var m = MultiCollectibleCounter.Instance;

        if (m == null) return;

        foreach (var c in m.collectibles)
        {
            c.currentAmount = 0;
            m.UpdateUI(c);
        }
    }
}
