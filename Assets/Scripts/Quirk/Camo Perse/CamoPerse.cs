using UnityEngine;

namespace MD.Quirk
{
    public class CamoPerse : BaseQuirk
    {
        [SerializeField]
        private ProjectilizedCamoPerse projectilizedCamoPersePrefab = null;

        public override void Activate()
        {
            Debug.Log("Activate Camo Perse");
            // var projectilizedCamoPerseInstance = Instantiate(projectilizedCamoPersePrefab);
        }
    }
}
