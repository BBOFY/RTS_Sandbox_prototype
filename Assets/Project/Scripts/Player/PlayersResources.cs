using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public readonly struct PlayersResources {

	public readonly int population;
	public readonly int popcap;
	public readonly int food;
	public readonly int wood;
	public readonly int iron;
	public readonly int gold;
	public readonly int provisions;
	public readonly int ammunition;

	public PlayersResources(int population, int popcap, int food, int wood, int iron, int gold, int provisions, int ammunition) {
		this.population = population;
		this.popcap = popcap;
		this.food = food;
		this.wood = wood;
		this.iron = iron;
		this.gold = gold;
		this.provisions = provisions;
		this.ammunition = ammunition;
	}
}
