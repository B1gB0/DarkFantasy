using _Project.Scripts.UI.Panel;

namespace _Project.Scripts.Experience
{
    public interface IScoreActorVisitor
    {
        public void Visit(IExperienceScoreActor experienceScoreActor);
#if CHEATS
        public void Visit(CheatPanel cheatPanel);
#endif
    }
}