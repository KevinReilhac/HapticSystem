using UnityEngine;

namespace HapticSystem
{
    internal class HapticManagerCoroutinePlayer : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void OnApplicationQuit()
        {
            HapticManager.StopAllClipInstances();
        }
    }
}