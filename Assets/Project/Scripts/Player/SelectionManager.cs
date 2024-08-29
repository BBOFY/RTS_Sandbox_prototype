/// <summary>
/// Class responsible for holding selected entities. Currently supported selection is for one entity only.
/// </summary>
public class SelectionManager {

	private ISelectable _selectedObject;
	public ISelectable selectedObject => _selectedObject;
	public readonly Player owner;

	public SelectionManager(Player owner) {
		this.owner = owner;
	}

	public void selectEntity(ISelectable selectable) {
		_selectedObject?.deselect(owner);
		_selectedObject = selectable;
		_selectedObject?.select(owner);
	}

	public void deselectEntity() {
		_selectedObject?.deselect(owner);
		_selectedObject = default;
	}
}
