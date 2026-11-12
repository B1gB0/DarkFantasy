using YG;

namespace _Project.Scripts.Experience
{
    public class ScoreActorVisitor : IScoreActorVisitor
    {
        private const int MinValue = 0;
        
        public int AccumulatedEnemyKills { get; private set; }
        public int AccumulatedScore { get; private set; }

        public void ResetAccumulatedValues()
        {
            AccumulatedEnemyKills = MinValue;
            AccumulatedScore = MinValue;
        }

        public void Visit(IScoreActor scoreActor)
        {
            AccumulatedScore += scoreActor.Score;
            YG2.saves.AcumulatedScore += scoreActor.Score;
        
            if (scoreActor.IsEnemy)
            {
                AccumulatedEnemyKills++;
            }
        }
    }
}