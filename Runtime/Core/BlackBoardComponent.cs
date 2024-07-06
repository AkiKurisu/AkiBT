using System.Collections.Generic;
using UnityEngine;
namespace Kurisu.AkiBT
{
    /// <summary>
    /// BlackBoardComponent provides a <see cref="BlackBoard"/> with a MonoBehavior lifecycle 
    /// that can cross components within GameObject, so that multiple <see cref="BehaviorTree"/> 
    /// instances can be bound to the same host blackboard. 
    /// This is convenient for switching behavior tree instances for one character.
    /// </summary>
    public class BlackBoardComponent : MonoBehaviour
    {
        [SerializeReference]
        private List<SharedVariable> sharedVariables = new();
        private BlackBoard instance;
        public BlackBoard GetBlackBoard()
        {
            if (Application.isPlaying)
            {
                instance ??= BlackBoard.Create(sharedVariables);
                return instance;
            }
            return BlackBoard.Create(sharedVariables);
        }
        internal void SetBlackBoardVariables(List<SharedVariable> variables)
        {
            sharedVariables.Clear();
            sharedVariables.AddRange(variables);
        }
    }
}