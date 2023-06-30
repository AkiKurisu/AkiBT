using System;
using System.Collections.Generic;
using UnityEngine;
namespace Kurisu.AkiBT.Editor
{
    public class BehaviorTreeSearchCache : ScriptableObject
    {
        public Type searchType;
        public List<BehaviorTreeSerializationPair> searchResult;
    }
}
