namespace HapticSystem
{
    public static class HapticClipInstanceExtentions
    {
        /// <summary>
        /// Check if a clip instance is playing
        /// </summary>
        /// <param name="clipInstance">Clip instance</param>
        /// <returns>True if the clip instance is not null and is playing, false otherwise</returns>
        public static bool IsPlaying(this HapticClipInstance clipInstance)
        {
            if (clipInstance == null)
                return false;
            return clipInstance.isPlaying;
        }
    }
}
