using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Common.Utils;

public static class AssemblyUtils
{
    public static Assembly LoadAssemblySafely(string assemblyName)
    {
        var loaded = AppDomain.CurrentDomain.GetAssemblies()
            .FirstOrDefault(a => a.GetName().Name == assemblyName);

        if (loaded != null)
            return loaded;

        try
        {
            return Assembly.Load(assemblyName);
        }
        catch (System.Exception ex) // Fully qualified 'Exception' to avoid ambiguity
        {
            throw new InvalidOperationException($"無法載入組件「{assemblyName}」。請確認其 DLL 是否存在於應用程式的輸出資料夾中。", ex);
        }
    }
}