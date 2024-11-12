using Godot;
using System;

public partial class PlayerCharacter : CharacterBody2D, IDamageable {
    private static PlayerCharacter _instance;
    private Health _health;
    private int _baseHealth = 10;

    public static PlayerCharacter GetInstance() {
        return _instance;
    }

    public override void _Ready() {
        _instance = this;
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
    }

    private void InitiateDeath(object sender, EventArgs e) {
        // Start death animation
        // Start death SFX


    }

    private void FinishDeath() {
        // After callback from death animation, display menu to retry level or whatever
    }
}
