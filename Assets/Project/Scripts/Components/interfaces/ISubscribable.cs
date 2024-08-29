/**
 * Components implementing this interface can listen for orders given by their owner player
 */
public interface ISubscribable {

	void subscribe();
	void unsubscribe();
}
