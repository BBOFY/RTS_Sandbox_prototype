public struct ResourceData {

	public readonly ResourceType type;
	public readonly int amount;

	public ResourceData(ResourceType type, int amount) {
		this.type = type;
		this.amount = amount;
	}

	public override string ToString() {
		return $"{amount} units of {type} resource";
	}

	public static ResourceData empty() {
		return new ResourceData(ResourceType.Nothing, 0);
	}



}
