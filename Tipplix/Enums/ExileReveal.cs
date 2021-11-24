namespace Tipplix.Enums;

public enum ExileReveal
{
    /// <summary>
    /// Never reveal the role, this overrides the game options
    /// </summary>
    Never,
    
    /// <summary>
    /// Reveal depends on game options
    /// </summary>
    Default,
    
    /// <summary>
    /// Always reveal, this overrides the game options
    /// </summary>
    Always
}