namespace Kurisu.AkiBT.Extend.Animator
{
    [AkiInfo("Action: Plays a state")]
    [AkiLabel("Animator: Play")]
    [AkiGroup("Animator")]
    public class Play : AnimatorAction
    {
        public SharedString stateName;
        public SharedInt layer = new(-1);
        protected override Status OnUpdate()
        {
            if (Animator == null)
            {
                return Status.Failure;
            }
            Animator.Play(stateName.Value, layer.Value);
            return Status.Success;
        }
    }
}
