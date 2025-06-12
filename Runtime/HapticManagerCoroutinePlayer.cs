using UnityEngine;

namespace HapticSystem
{
    public class HapticManagerCoroutinePlayer : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            gameObject.AddComponent<HapticManagerProcessor>();
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            HapticManager.StopAllClipInstances();
        }
    }
}