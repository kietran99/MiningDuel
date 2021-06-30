using Mirror;
using UnityEngine;
using MD.Character;

namespace MD.Quirk
{
    public class UmbraMod : BaseQuirk
    {
        private class ModMultCalculator : IAtkMultCalculator
        {
            private readonly float MODIFIER;
            private IAtkMultCalculator _baseCalculator;

            public ModMultCalculator(IAtkMultCalculator baseCalculator, float modifier, float hpLossPercentage)
            {
                MODIFIER = modifier;
                _baseCalculator = baseCalculator;
                HPLossPercentage = hpLossPercentage;
            }

            public float HPLossPercentage { get; set; }    

            public float GetResult(float criticalMult, bool isCritical = false)
            {
                return _baseCalculator.GetResult(criticalMult, isCritical) * (1f + HPLossPercentage * MODIFIER);
            }
        }

        [SerializeField]
        private float _time = 3f;

        [SerializeField]
        private float _boostModifier = 1f;

        [Range(0f, 1f)]
        [SerializeField]
        private float _recoilModifier = .2f;

        private bool _active = false;
        private float _elapsed = 0f;
        private IAtkMultCalculator _curMultCalculator;
        private ModMultCalculator _modMultCalculator;
        private NetworkIdentity _user;
        private BasicAttackAction _atkAction;
        private IDamagable _userDamagable;

        public override void ServerActivate(NetworkIdentity user)
        {
            if (!user.TryGetComponent<BasicAttackAction>(out _atkAction))
            {
                Debug.LogWarning("No BasicAttackAction script attached on " + user.name + " GameObject");
                return;
            }

            if (!user.TryGetComponent<HitPoints>(out var hp))
            {
                Debug.LogWarning("No HitPoints script attached on " + user.name + " GameObject");
                return;
            }

            _user = user;
            _userDamagable = hp;
            _curMultCalculator = _atkAction.MultCalculator;
            _modMultCalculator = new ModMultCalculator(_atkAction.MultCalculator, _boostModifier, hp.GetLossPercentage());
            _atkAction.MultCalculator = _modMultCalculator;
            _active = true;
            _elapsed = _time;
            base.ServerActivate(user);
        }

        public override void SingleActivate(NetworkIdentity user)
        {
            var consumer = EventSystems.EventConsumer.Attach(gameObject);
            consumer.StartListening<HPChangeData>(OnHPChange);
            consumer.StartListening<DamageGivenData>(OnDamageGiven);
        }

        [Client]
        private void OnHPChange(HPChangeData data) => CmdUpdateModCalc(data.curHP, data.maxHP);   

        [Command]
        private void CmdUpdateModCalc(int curHP, int maxHP)
        {
            _modMultCalculator.HPLossPercentage = 1f - ((float) curHP / (float) maxHP);
        }

        [Client]
        private void OnDamageGiven(DamageGivenData data) => CmdGiveRecoilDamage(data.dmg);

        [Command]
        private void CmdGiveRecoilDamage(int dmg)
        {
            _userDamagable.TakeDamage(_user, Mathf.FloorToInt(dmg * _recoilModifier), false);
        }

        [ServerCallback]
        private void Update()
        {
            if (!_active)
            {
                return;
            }

            if (_elapsed <= 0f)
            {
                // Debug.Log("Effect worn out");
                _atkAction.MultCalculator = _curMultCalculator;
                _active = false;
                NetworkServer.Destroy(gameObject);
                return;
            }

            _elapsed -= Time.deltaTime;
        }
    }
}
