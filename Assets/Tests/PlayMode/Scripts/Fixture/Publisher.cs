using UnityEngine;

namespace Zenject.Fixture
{
    public interface IPublisher
    {
        int Value { get; }
    }

    public class Publisher : MonoBehaviour, IPublisher
    {
        [SerializeField] private int value = 10;
        public int Value => value;
    }
}