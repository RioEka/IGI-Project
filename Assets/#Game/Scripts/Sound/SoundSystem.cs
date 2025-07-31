using System.Collections.Generic;

using UnityEngine;

namespace IGI.Sound
{
    public static class SoundSystem
    {
        private static List<Enemy.SoundDetection> listeners { get; set; } = new();

        public static void RegisterListener(Enemy.SoundDetection hearing)
        {
            if (!listeners.Contains(hearing))
                listeners.Add(hearing);
        }

        public static void UnregisterListener(Enemy.SoundDetection hearing)
        {
            if (listeners.Contains(hearing))
                listeners.Remove(hearing);
        }

        public static void EmitSound(AudioClip clip, Vector3 position, float volume, float intensity = 1f, Enemy.SoundDetection source = null)
        {
            foreach (var listener in listeners)
            {
                if (listener == source) continue;

                listener.HearSound(position, intensity);
            }

            AudioSource.PlayClipAtPoint(clip, position, volume);
        }
    }
}