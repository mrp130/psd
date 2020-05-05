using System;

namespace Xyz.Game
{
  public interface IObserver<T>
  {
    void Update(T e);
  }

  public interface IObservable<T>
  {
    void Attach(IObserver<T> obs);
    void Broadcast(T e);
  }
}