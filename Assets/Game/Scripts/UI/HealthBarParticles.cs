using System.Collections;
using System.Collections.Generic;
using Game.Scripts;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class HealthBarParticles : MonoBehaviour {
    private ParticleSystem _particleSystem;
    
    [SerializeField] private HealthBar healthBar;
    private HealthComponent _healthComponent;
    [SerializeField] private AnimationCurve healthCurve;
    [SerializeField] private AnimationCurve damageMultiplierCurve;
    
    private void Start() {
        healthBar.onBindHealthComponent.AddListener(OnBindHealthComponent);
    }

    private void OnBindHealthComponent(HealthComponent healthComponent) {
        _healthComponent = healthComponent;
        _healthComponent.onTakeDamageFloat.AddListener(Play);
    }

    private void Play(float damage) {
        if (damage > 0) {
            int burstCount = (int)healthCurve.Evaluate(_healthComponent.health / _healthComponent.maxHealth);
            burstCount = (int)(damageMultiplierCurve.Evaluate(damage/_healthComponent.maxHealth) * burstCount);
            _particleSystem.emission.SetBursts(new [] { new ParticleSystem.Burst(0, (short)burstCount) });
            _particleSystem.Play();
        }
    }

    public void Play() {
        _particleSystem.Play();
    }
    
    void Awake() {
        _particleSystem = GetComponent<ParticleSystem>();
    }
}
