/**
 * Entity with component implementing this interface can be selected by players
 */
public interface ISelectable {

	public void select(Player player);
	public void deselect(Player player);

	public PlayerColor getOwnerColor();
}
