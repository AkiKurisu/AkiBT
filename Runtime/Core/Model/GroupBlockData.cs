using System;
using System.Collections.Generic;
using UnityEngine;
namespace Kurisu.AkiBT
{
    [Serializable]
    public class GroupBlockData
    {
        public List<string> ChildNodes = new();
        public Vector2 Position;
        public string Title = "Group Block";
    }
}
