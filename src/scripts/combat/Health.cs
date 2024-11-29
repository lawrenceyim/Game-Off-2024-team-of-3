using System;

public class Health {
	public event EventHandler ZeroHealthEvent;
	private int _baseHealth;
	private int _maxHealth;
	private int _currentHealth;


	public Health(int baseHealth) {
		_baseHealth = baseHealth;
		_maxHealth = baseHealth;
		_currentHealth = baseHealth;
	}

	public void IncreaseHealth(int amount) {
		_currentHealth = Math.Min(_currentHealth + amount, _maxHealth);
	}

	public void DecreaseHealth(int amount) {
		_currentHealth = Math.Max(_currentHealth - amount, 0);
		if (_currentHealth == 0) {
			ZeroHealthEvent?.Invoke(this, EventArgs.Empty);
		}
	}

	public int GetCurrentHealth() {
		return _currentHealth;
	}

	public bool IsHealthZero() {
		return _currentHealth == 0;
	}
}