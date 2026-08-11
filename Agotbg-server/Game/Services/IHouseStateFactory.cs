using Agotbg.Server.Game.Model;

namespace Agotbg.Server.Game.Services
{
  /// <summary>
  /// Defines the interface for a factory that creates HouseState objects based on the
  /// specified HouseType.
  /// </summary>
  public interface IHouseStateFactory
  {
    /// <summary>
    /// Creates a HouseState based on the specified HouseType. Each house has its own
    /// starting attributes. However, the starting properties are based on an eight
    /// player game, and may need to be adjusted for games with fewer players.
    /// </summary>
    ///
    /// <param name="houseType">The type of house for which to create a
    /// HouseState.</param>
    ///
    /// <returns>A HouseState object initialized with the starting attributes for the
    /// specified house.</returns>
    ///
    /// <exception cref="NotImplementedException">Thrown if the specified HouseType is
    /// not implemented.</exception>
    public HouseState Create(HouseType houseType);

    /// <summary>
    /// Creates a vassal HouseState based on the specified HouseType. Each vassal house
    /// has its own starting attributes.
    /// </summary>
    ///
    /// <param name="houseType">The type of house for which to create a vassal
    /// HouseState.</param>
    /// 
    /// <returns>A HouseState object initialized with the starting attributes for the
    /// specified vassal house.</returns>
    ///
    /// <exception cref="NotImplementedException">Thrown if the specified HouseType is
    /// not implemented.</exception>
    public HouseState CreateVassal(HouseType houseType);
  }
}
