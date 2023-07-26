using System;
using System.Collections.Generic;
using UnityEngine;
namespace Kurisu.AkiBT.Editor
{
    public class BehaviorTreeSearchCache : ScriptableObject
    {
        public string searchName = string.Empty;
        public Type searchType;
        public List<BehaviorTreeSerializationPair> searchResult;
        public List<BehaviorTreeSO> allBehaviorTreeSOCache;
    }
}
