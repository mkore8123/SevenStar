using System.ComponentModel;

namespace SevenStar.Shared.Domain.Enums;

/// <summary>
/// 投注類別大項
/// </summary>
public enum BetNoCategoryEnum
{   
    /// <summary>
    /// 一定位
    /// </summary>
    [Description("一定位")]
    FirstPositionBet = 15,

    /// <summary>
    /// 二字現
    /// </summary>
    [Description("二字现")]
    TwoNumberCombo = 20,

    /// <summary>
    /// 二定位
    /// </summary>
    [Description("二定位")]
    DoublePositionsBet = 24,

    /// <summary>
    /// 五位二定
    /// </summary>
    [Description("五位二定")]
    TwoFixedFiveFree = 25,

    /// <summary>
    /// 三字現
    /// </summary>
    [Description("三字现")]
    ThreeNumberCombo = 30,

    /// <summary>
    /// 三定位
    /// </summary>
    [Description("三定位")]
    FourNumberCombo = 34,

    /// <summary>
    /// 四字現
    /// </summary>
    [Description("四字现")]
    FourDigitCombo = 40,

    /// <summary>
    /// 四定位
    /// </summary>
    [Description("四定位")]
    QuadruplePositionBet = 44,
}
