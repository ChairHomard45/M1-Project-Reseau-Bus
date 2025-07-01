using Backend.Dynamique.StateBus;
using Backend.Statique;

namespace Backend.Dynamique;

public class Bus
{
  private IEtatBus _busState;
  private readonly string _immatriculationBus;
  private int _sensCirculation;
  private ElementLigne _previousArret; // NEW
  private ElementLigne _surArret;
  private ElementLigne _nextArret;
  private Lignes _surligne;
  private ElementLigne _deArret; // Terminus
  private ElementLigne _versArret; // Terminus


  public Bus(string immatricul, Lignes ligne, int sens, ElementLigne deArret, ElementLigne verArret,
    ElementLigne surArret)
  {
    _immatriculationBus = immatricul;
    _surligne = ligne;
    _sensCirculation = sens;
    _surArret = surArret;
    _deArret = deArret;
    _versArret = verArret;

    _previousArret = null; // Initially, no previous stop
    _nextArret = ReseauBus.Instance.GetNextElementDeLigne(_surligne, _surArret, _sensCirculation);

    _busState = new Arreter(this);
  }


// ******************************************************************************************************
// ***                                                                                                ***
// ***                                      Getter & Setter                                           ***
// ***                                                                                                ***
// ******************************************************************************************************

  public string GetImmatriculation()
  {
    return _immatriculationBus;
  }

  public void HandleStateChange(IEtatBus newState)
  {
    _busState = newState;
  }

  public Lignes GetLignes()
  {
    return _surligne;
  }

  public ElementLigne GetPreviousArret() // NEW getter
  {
    return _previousArret;
  }

  public ElementLigne GetSurArret()
  {
    return _surArret;
  }

  public ElementLigne GetNextArret() // NEW getter
  {
    return _nextArret;
  }

  public ElementLigne GetDeArret()
  {
    return _deArret;
  }

  public ElementLigne GetVersArret()
  {
    return _versArret;
  }

// ******************************************************************************************************
// ***                                                                                                ***
// ***                                      Bus Movement                                            ***
// ***                                                                                                ***
// ******************************************************************************************************

  public void ChangeElements()
  {
    _previousArret = _surArret;
    _surArret = _nextArret;

// Check if the stop we've just moved to is a terminus
    if (_surArret.IsTerminus())
    {
      // Flip direction BEFORE getting next arrêt
      _sensCirculation = -_sensCirculation;
      (_deArret, _versArret) = (_versArret, _deArret);
    }

// Now compute the next arrêt using the updated direction
    _nextArret = ReseauBus.Instance.GetNextElementDeLigne(_surligne, _surArret, _sensCirculation);
  }

// ******************************************************************************************************
// ***                                                                                                ***
// ***                                 Implementation State Pattern                                   ***
// ***                                                                                                ***
// ******************************************************************************************************

  public void ChronoEnd()
  {
    _busState.OnChronoEnd(this);
  }
}