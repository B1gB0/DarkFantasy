namespace _Project.Scripts.UI.View
{
    public class HealthBar : Bar
    {
        private Health _health;
        
        private void OnEnable()
        {
            Subscribe();
        }

        private void OnDisable()
        {
            Unsubscribe();
        }

        public void Construct(Health health)
        {
            Unsubscribe();
            
            _health = health;
            
            Subscribe();
        }
        
        private void Subscribe()
        {
            if (_health == null) return;
            
            _health.Die -= OnDie;
            _health.Die += OnDie;
            
            _health.HealthChanged -= OnChangedValues;
            _health.HealthChanged += OnChangedValues;
        }

        private void Unsubscribe()
        {
            if (_health == null) return;
            
            _health.Die -= OnDie;
            _health.HealthChanged -= OnChangedValues;
        }

        private void OnDie()
        {
            Hide();
        }

        public override void Show()
        {
            base.Show();
            if (_health != null)
            {
                OnChangedValues(_health.CurrentHealth, _health.MaxHealth, _health.TargetHealth);
            }
        }

        private void OnChangedValues(float currentHealth, float maxHealth, float targetHealth)
        {
            SetValues(currentHealth, maxHealth, targetHealth);
        }
    }
}