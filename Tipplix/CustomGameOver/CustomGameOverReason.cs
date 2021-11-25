using Tipplix.Enums;
using UnityEngine;

namespace Tipplix.CustomGameOver;

public abstract class CustomGameOverReason
{
    public abstract string WinText { get; }
    public virtual string DescriptionText => string.Empty;
    public abstract Color Color { get; }
    public virtual Color DescColor => Color.white;
    public virtual Color BackgroundColor => Color;
    public abstract bool ShowName { get; }
    
    /// <summary>
    /// Custom stinger for outro, this will get ignored if <see cref="Stinger"/> is other than <see cref="EndGameStingers.Default"/>
    /// </summary>
    public virtual AudioClip? CustomStinger { get; set; }
    
    public virtual EndGameStingers Stinger { get; set; }

    public int Id { get; internal set; }
    public bool YouWon { get; internal set; }
}