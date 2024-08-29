public class AmmunitionInventoryCmp : InventoryCmp {
	protected new void Awake() {
    	base.Awake();
    	_resourceType = ResourceType.Ammunition;
	}
}
