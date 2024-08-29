using UnityEngine;

public class WaterSource : Water {
	public WaterSource(Vector3Int absPosition) : base(absPosition, maxLevel) {

	}


	/// <summary>
	/// Water source block cannot remove itself, so it only checks, if it can
	/// </summary>
	protected override void executeUpdateLogic() {
		doSpillage();
		unsubscribeToTicks();

	}

}
