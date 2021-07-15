namespace MD.AI
{
    public class BotHitPoints : Character.HitPoints
    {
        public System.Action<uint> OnBotDeath { get; set; }

        protected override void RaiseDeathEvent()
        {
            OnBotDeath?.Invoke(netId);
        }

        protected override void OnAuthorityCurrentHPSync(int oldCurHP, int newCurHP)
        {}
    }
}
