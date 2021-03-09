using UnityEngine;

namespace MD.Tutorial
{
    public class ProjectileTutorialMaterial : TutorialMaterial<DiggableVisibleData, Diggable.Projectile.ProjectileObtainData, Character.ScoreChangeData>
    {
        [SerializeField]
        private GameObject practiceTarget = null;

        [SerializeField]
        private int showTargetIndex = 0;

        [SerializeField]
        private TutorialProjectileLauncher launcher = null;

        [SerializeField]
        private int addThrowActionIndex = 0;

        private GameObject thrower;

        protected override void Start()
        {
            base.Start();
            thrower = GameObject.FindGameObjectWithTag(Constants.PLAYER_TAG);
            GetComponent<EventSystems.EventConsumer>().StartListening<TutorialStateChangeData>(HandleTutorialStateChange);
        }

        private void HandleTutorialStateChange(TutorialStateChangeData stateChangeData)
        {
            if (stateChangeData.lineIdx == showTargetIndex)
            {
                practiceTarget.SetActive(true);
                return;
            }

            if (stateChangeData.lineIdx == addThrowActionIndex)
            {
                thrower.AddComponent<TutorialThrowAction>().SetLauncher(launcher);
                return;
            }
        }
    }
}