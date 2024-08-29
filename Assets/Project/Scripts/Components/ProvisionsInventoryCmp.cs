public class ProvisionsInventoryCmp : InventoryCmp {

	protected new void Awake() {
		base.Awake();
		_resourceType = ResourceType.Provision;
	}

}
