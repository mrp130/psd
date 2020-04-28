using System;
using System.Collections.Generic;

namespace Xyz.Game.ExpGainer
{
  public class ServiceLocator
  {
    private static Dictionary<string, IExpGainer> _services;
    private ServiceLocator()
    {
      _services = new Dictionary<string, IExpGainer>();
    }

    private static ServiceLocator _instance;
    public static ServiceLocator Instance()
    {
      if (_instance == null)
      {
        _instance = new ServiceLocator();
      }

      return _instance;
    }

    public void Register(string key, IExpGainer gainer)
    {
      _services[key] = gainer;
    }

    public IExpGainer Get(string key)
    {
      return _services[key];
    }
  }
}