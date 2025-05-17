using System.ComponentModel;

namespace SevenStar.Shared.Domain.Enums;

/// <summary>
/// 投注類型
/// </summary>
public enum BetNoTypeEnum
{
    /// <summary>
    /// 一定位，口XXXX
    /// </summary>
    [Description("口XXXX")]
    OXXXX = 1501,

    /// <summary>
    /// 一定位，X口XXX
    /// </summary>
    [Description("X口XXX")]
    XOXXX = 1502,

    /// <summary>
    /// 一定位，XX口XX
    /// </summary>
    [Description("XX口XX")]
    XXOXX = 1503,

    /// <summary>
    /// 一定位，XXX口X
    /// </summary>
    [Description("XXX口X")]
    XXXOX = 1504,

    /// <summary>
    /// 一定位，XXXX口
    /// </summary>
    [Description("XXXX口")]
    XXXXO = 1505,

    /// <summary>
    /// 二字現
    /// </summary>
    [Description("二字现")]
    Box2 = 2001,

    /// <summary>
    /// 二定位，口口XX
    /// </summary>
    [Description("口口XX")]
    OOXX = 2401,

    /// <summary>
    /// 二定位，口X口X
    /// </summary>
    [Description("口X口X")]
    OXOX = 2402,

    /// <summary>
    /// 二定位，口XX口
    /// </summary>
    [Description("口XX口")]
    OXXO = 2403,

    /// <summary>
    /// 二定位，X口X口
    /// </summary>
    [Description("X口X口")]
    XOXO = 2404,

    /// <summary>
    /// 二定位，X口口X
    /// </summary>
    [Description("X口口X")]
    XOOX = 2405,

    /// <summary>
    /// 二定位，XX口口
    /// </summary>
    [Description("XX口口")]
    XXOO = 2406,

    /// <summary>
    /// 五位二定，口XXX口
    /// </summary>
    [Description("口XXX口")]
    OXXXO = 2501,

    /// <summary>
    /// 五位二定，X口XX口
    /// </summary>
    [Description("X口XX口")]
    XOXXO = 2502,

    /// <summary>
    /// 五位二定，XX口X口
    /// </summary>
    [Description("XX口X口")]
    XXOXO = 2503,

    /// <summary>
    /// 五位二定，XXX口口
    /// </summary>
    [Description("XXX口口")]
    XXXOO = 2504,

    /// <summary>
    /// 三字現
    /// </summary>
    [Description("三字现")]
    Box3 = 3001,

    /// <summary>
    /// 三定位，口口口X
    /// </summary>
    [Description("口口口X")]
    OOOX = 3401,

    /// <summary>
    /// 三定位，口口X口
    /// </summary>
    [Description("口口X口")]
    OOXO = 3402,

    /// <summary>
    /// 三定位，口X口口
    /// </summary>
    [Description("口X口口")]
    OXOO = 3403,

    /// <summary>
    /// 三定位，X口口口
    /// </summary>
    [Description("X口口口")]
    XOOO = 3404,

    /// <summary>
    /// 四字現
    /// </summary>
    [Description("四字现")]
    Box4 = 4001,

    /// <summary>
    /// 四定位
    /// </summary>
    [Description("四定位")]
    OOOO = 4401,
}
