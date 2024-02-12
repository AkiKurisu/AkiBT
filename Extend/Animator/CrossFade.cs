namespace Kurisu.AkiBT.Extend.Animator
{
    [AkiInfo("Action: Creates a crossfade from the current state to any other state using times in seconds")]
    [AkiLabel("Animator: CrossFade")]
    [AkiGroup("Animator")]
    public class CrossFade : AnimatorAction
    {
        public SharedString stateName;
        public SharedFloat normalizedTransitionDuration = new(0);
        public SharedInt layer = new(-1);
        public SharedFloat normalizedTimeOffset = new(float.NegativeInfinity);
        protected override Status OnUpdate()
        {
            Animator.CrossFade(stateName.Value, normalizedTransitionDuration.Value, layer.Value, normalizedTimeOffset.Value);
            return Status.Success;
        }
    }
}
