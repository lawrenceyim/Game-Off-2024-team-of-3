using Godot;
using System;

public partial class PlayerCharacter : CharacterBody2D, IDamageable {
    private Health _health;
    private int _baseHealth = 10;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        _health = new Health(_baseHealth);
        _health.ZeroHealthEvent += Die;

    }

    public void TakeDamage(int damage) {
        _health.DecreaseHealth(damage);
    }

    private void Die(object sender, EventArgs e) {
        // Start death animation
        // Start death SFX

        // After callback from death animation, display menu to retry level or whatever
    }
}
