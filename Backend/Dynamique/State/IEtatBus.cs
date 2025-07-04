using Backend.Dynamique.Entities;

namespace Backend.Dynamique.State;

public interface IEtatBus
{
  void Handle(Bus bus);
}