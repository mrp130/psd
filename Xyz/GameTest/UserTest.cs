using System;
using Xunit;

namespace Xyz.Game.Test
{
  public class UserTest
  {
    [Fact]
    public void UserScore()
    {
      User u = User.NewUser("Amir");
      Assert.Equal(0, u.Exp);

      u.AddExp(5);
      Assert.Equal(5, u.Exp);

      u.AddExp(13);
      Assert.Equal(18, u.Exp);
    }
  }
}
