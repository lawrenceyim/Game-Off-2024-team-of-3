using Godot;
using System;
using System.Collections.Generic;

public partial class PlayerCharacter : CharacterBody2D, IDamageable {
	private const string OnHitAnimation = "hit";
	[Export] private AnimationPlayer _onHitAnimationPlayer;
	[Export] private PlayerCamera _playerCamera;
	private static PlayerCharacter _instance;
	private static Queue<Action<PlayerCharacter>> _waitingForInstance = new Queue<Action<PlayerCharacter>>();
	private Health _health;
	private int _baseHealth = 10;

	public static void GetInstanceWithCallback(Action<PlayerCharacter> callback) {
		if (_instance == null) {
			_waitingForInstance.Enqueue(callback);
		} else {
			callback(_instance);
		}
	}

	public override void _Ready() {
		_instance = this;
		while (_waitingForInstance.Count > 0) {
			Action<PlayerCharacter> callback = _waitingForInstance.Dequeue();
			callback(this);
		}

		_health = new Health(_baseHealth);
		_health.ZeroHealthEvent += InitiateDeath;
	}

	public override void _ExitTree() {
		_instance = null;
	}

	public void TakeDamage(int damage) {
		// Play damaged sfx
		// Add damaged vfx
		_health.DecreaseHealth(damage);
		_playerCamera.AddTrauma(1);
		_onHitAnimationPlayer.Play(OnHitAnimation);
		GD.Print("Player damaged. Health is " + _health.GetCurrentHealth().ToString());
	}

	private void InitiateDeath(object sender, EventArgs e) {
		GD.Print("Player died.");

		SceneManager.GetInstance().RestartLevel();
	}

	private void FinishDeath() {
		// After callback from death animation, display menu to retry level or whatever
	}
}
