namespace Kurisu.AkiBT.Example
{
    [AkiInfo("Conditional:判断AIModel是否可以移动即canMove是否被勾选")]
    [AkiLabel("Example:判断是否可以移动")]
    [AkiGroup("Example")]
    public class CanMoveCondition : Conditional
    {
        private AIModel model;
        protected override void OnAwake()
        {
            model = GameObject.GetComponent<AIModel>();
        }
        protected override bool IsUpdatable()
        {
            return model.canMove;
        }
    }
}
