using System.Collections.Generic;

namespace _Project.Scripts.Experience
{
    public class Score : IExperiencePoints
    {
        private readonly ScoreActorVisitor _scoreActorVisitor = new();
        private readonly List<int> _playerLevels;

        public int AccumulatedKills => _scoreActorVisitor.AccumulatedEnemyKills;
        public int AccumulatedScore => _scoreActorVisitor.AccumulatedScore;

        public void OnKill(IAcceptable experience)
        {
            experience.AcceptScore(_scoreActorVisitor);
        }

        public void ResetAccumulatedValues()
        {
            _scoreActorVisitor.ResetAccumulatedValues();
        }
    }
}