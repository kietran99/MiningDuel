using UnityEngine;

namespace MD.Quirk
{
    public class CamoPerse : BaseQuirk
    {
        [SerializeField]
        private ProjectilizedCamoPerse projectilizedCamoPersePrefab = null;

        protected override void Activate()
        {
            var projectilizedCamoPerseInstance = Instantiate(projectilizedCamoPersePrefab);
        }
    }
}
