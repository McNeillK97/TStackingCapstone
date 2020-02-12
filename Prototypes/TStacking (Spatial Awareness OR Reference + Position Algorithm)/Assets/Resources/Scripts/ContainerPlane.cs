using HoloToolkit.Unity.InputModule.Utilities.Interactions;
using UnityEngine;

public class ContainerPlane : MonoBehaviour
{
    public HoloToolkit.Unity.UX.BoundingBox boundingBoxBasic;

	private void Start ()
    {
        GameObject containerPlaneAnchor = new GameObject("ContainerPlaneAnchor");
        containerPlaneAnchor.tag = "ContainerPlane";
        containerPlaneAnchor.transform.position = new Vector3(this.transform.position.x - this.transform.localScale.x / 2,
                                                                                                    this.transform.position.y + this.transform.localScale.y / 2,
                                                                                                    this.transform.position.z + this.transform.localScale.z / 2);
        this.transform.parent = containerPlaneAnchor.transform;
        containerPlaneAnchor.AddComponent<TwoHandManipulatable>();
        containerPlaneAnchor.GetComponent<TwoHandManipulatable>().BoundingBoxPrefab = boundingBoxBasic;
        containerPlaneAnchor.GetComponent<TwoHandManipulatable>().ManipulationMode = ManipulationMode.MoveAndRotate;
        containerPlaneAnchor.GetComponent<TwoHandManipulatable>().RotationConstraint = AxisConstraint.YAxisOnly;
    }
}
