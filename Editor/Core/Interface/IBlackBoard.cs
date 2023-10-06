using System;
namespace Kurisu.AkiBT.Editor
{
    public interface IBlackBoard
    {
        /// <summary>
        /// 共享变量名称修改事件
        /// </summary>
        event Action<SharedVariable> OnPropertyNameChange;
        /// <summary>
        /// 添加共享变量
        /// </summary>
        /// <param name="variable"></param>
        void AddExposedProperty(SharedVariable variable);
    }
}