using UnityEngine;
using System.Collections;

public interface IHealth {
    void Damage(int damage);
    void Heal(int damage);
    void Regenerate(int damage, float rate, float duration);
    void Regenerate();
}
