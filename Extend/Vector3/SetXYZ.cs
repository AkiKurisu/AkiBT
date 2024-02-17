using UnityEngine;
namespace Kurisu.AkiBT.Extend.Vector3
{
    [AkiInfo("Action: Set XYZ value of Vector3")]
    [AkiLabel("Vector3: SetXYZ")]
    [AkiGroup("Vector3")]
    public class SetXYZ : Action
    {
        public enum Filter
        {
            X, Y, Z, XY, XZ, YZ, XYZ
        }
        [Tooltip("Set value")]
        public SharedVector3 vector3;
        [Tooltip("Value filter")]
        public Filter filter = Filter.XYZ;
        [Tooltip("The X value.")]
        public SharedFloat xValue;
        [Tooltip("The Y value.")]
        public SharedFloat yValue;
        [Tooltip("The Z value.")]
        public SharedFloat zValue;
        protected override Status OnUpdate()
        {
            var vector3Value = vector3.Value;
            if (filter is Filter.X or Filter.XY or Filter.XZ or Filter.XYZ)
            {
                vector3Value.x = xValue.Value;
            }
            if (filter is Filter.Y or Filter.XY or Filter.YZ or Filter.XYZ)
            {
                vector3Value.y = yValue.Value;
            }
            if (filter is Filter.Z or Filter.XZ or Filter.YZ or Filter.XYZ)
            {
                vector3Value.z = zValue.Value;
            }
            vector3.Value = vector3Value;
            return Status.Success;
        }
    }
}
