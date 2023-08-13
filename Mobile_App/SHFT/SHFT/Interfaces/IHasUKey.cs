// SHFT - H
// Winter 2023
// April 26th 2023
// Application Development III
// This interface represents entities that have a unique key property.

namespace SHFT.Interfaces
{
    /// <summary>
    /// Interface for entities that have a unique key property.
    /// </summary>
    public interface IHasUKey
    {
        /// <summary>
        /// Gets or sets the unique key for the entity.
        /// </summary>
        public string Key { get; set; }
    }
}
