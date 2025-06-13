using UnityEngine;
using UnityEngine.InputSystem;

namespace HapticSystem
{
    public static class HapticClipExtentions
    {
        public static HapticClipInstance Play(this HapticClip clip, int targetGamepadIndex, float strenghtMultiplier = 1f, float lowFrequencyMultiplier = 1f, float highFrequencyMultiplier = 1f)
        {
            return HapticManager.PlayClip(clip, targetGamepadIndex, strenghtMultiplier, lowFrequencyMultiplier, highFrequencyMultiplier);
        }

        public static HapticClipInstance Play(this HapticClip clip, float strenghtMultiplier = 1f, float lowFrequencyMultiplier = 1f, float highFrequencyMultiplier = 1f)
        {
            return HapticManager.PlayClip(clip, strenghtMultiplier, lowFrequencyMultiplier, highFrequencyMultiplier);
        }

        public static HapticClipInstance Play(this HapticClip clip, PlayerInput playerInput, float strenghtMultiplier = 1f, float lowFrequencyMultiplier = 1f, float highFrequencyMultiplier = 1f)
        {
            return HapticManager.PlayClip(clip, playerInput, strenghtMultiplier, lowFrequencyMultiplier, highFrequencyMultiplier);
        }

        public static HapticClipInstance Stop(this HapticClipInstance clipInstance)
        {
            HapticManager.StopClipInstance(clipInstance);
            return clipInstance;
        }
    }
}