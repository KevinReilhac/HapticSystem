using UnityEngine;

namespace HapticSystem
{
    public class HapticManagerProcessor : MonoBehaviour
    {
        private void Update()
        {
            HapticManager.ProcessMotorUpdates();
        }

        private void OnDestroy()
        {
            HapticManager.StopUpdateThread();
        }
    }
} 