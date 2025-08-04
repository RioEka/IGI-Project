using System;

namespace IGI.Manager
{
    public enum GameResult { Win, Lose }

    public static class EventCallback
    {
        public static Action<GameResult> OnGameOver { get; set; } = delegate { };
        public static Action OnPlayerHit { get; set; } = delegate { };
    }
}