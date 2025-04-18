using UnityEngine;

public class GameLayers : MonoBehaviour
{
    [SerializeField] LayerMask solidObjectsLayer;
    [SerializeField] LayerMask interactableLayer;
    [SerializeField] LayerMask grassLayer;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] LayerMask fovLayer;
    [SerializeField] LayerMask portalLayer;
    [SerializeField] LayerMask triggerLayer;
    [SerializeField] LayerMask ledgeLayer;

    public static GameLayers i { get; set; }

    private void Awake()
    {
        i = this;
    }

    public LayerMask SolidLayer => solidObjectsLayer;
    public LayerMask InteractableLayer => interactableLayer;
    public LayerMask GrassLayer => grassLayer; 
    public LayerMask PlayerLayer => playerLayer; 
    public LayerMask LedgeLayer => ledgeLayer;
    public LayerMask TriggerableLayers => grassLayer | fovLayer | portalLayer | triggerLayer;
}
