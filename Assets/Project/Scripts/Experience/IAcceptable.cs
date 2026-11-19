namespace Project.Scripts.Experience
{
    public interface IAcceptable
    {
        public void AcceptScore(IScoreActorVisitor visitor)
        {
        }
    }
}