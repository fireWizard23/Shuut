using Godot;

namespace Shuut.Scripts.Poise;

public struct Poise 
{
	private int poise;
	private int maxPoise;

	public int Current => poise;

	public void Setup(int poise)
	{
		this.poise = poise;
		this.maxPoise = poise;
	}

	public bool Reduce(int p)
	{
		poise -= p;
		
		if (poise > 0) return false;
		
		poise = maxPoise;
		return true;
	}
	
	
	
	
	
}