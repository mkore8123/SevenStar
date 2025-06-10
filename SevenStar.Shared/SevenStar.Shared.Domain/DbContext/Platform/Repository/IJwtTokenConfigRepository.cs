using SevenStar.Shared.Domain.DbContext.Platform.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace SevenStar.Shared.Domain.DbContext.Platform.Repository;

/// <summary>
/// JWT 設定 Repository 介面，提供 JWT Token 配置資料的非同步 CRUD 操作
/// </summary>
public interface IJwtTokenConfigRepository
{
    /// <summary>
    /// 新增一筆 JWT 設定（非同步）
    /// </summary>
    /// <param name="config">JWT 設定物件</param>
    /// <returns>資料庫產生的設定主鍵 id</returns>
    Task<int> CreateAsync(JwtTokenConfigEntity config);

    /// <summary>
    /// 依主鍵查詢 JWT 設定（非同步）
    /// </summary>
    /// <param name="id">JWT 設定主鍵 id</param>
    /// <returns>JWT 設定物件，查無則回傳 null</returns>
    Task<JwtTokenConfigEntity?> GetByIdAsync(int id);

    /// <summary>
    /// 查詢指定公司下所有 JWT 設定（非同步）
    /// </summary>
    /// <param name="companyId">公司主鍵 id</param>
    /// <returns>JWT 設定物件清單</returns>
    Task<List<JwtTokenConfigEntity>> GetByCompanyIdAsync(int companyId);

    /// <summary>
    /// 依 issuer + audience 查詢 JWT 設定（同公司可多組 audience）（非同步）
    /// </summary>
    /// <param name="issuer">JWT Token 發行者（iss）</param>
    /// <param name="audience">JWT Audience</param>
    /// <returns>JWT 設定物件清單</returns>
    Task<List<JwtTokenConfigEntity>> GetByIssuerAudienceAsync(string issuer, string audience);

    /// <summary>
    /// 更新 JWT 設定（依 id 覆寫，非同步）
    /// </summary>
    /// <param name="config">JWT 設定物件（需帶主鍵 id）</param>
    Task UpdateAsync(JwtTokenConfigEntity config);

    /// <summary>
    /// 刪除 JWT 設定（依主鍵 id，非同步）
    /// </summary>
    /// <param name="id">JWT 設定主鍵 id</param>
    Task DeleteAsync(int id);
}
