/// <summary>
/// Interface for components, that have information to show on some channel of some player
/// </summary>
public interface IShowable {
	public void show(Player player);
	public void hide(Player player);
}
