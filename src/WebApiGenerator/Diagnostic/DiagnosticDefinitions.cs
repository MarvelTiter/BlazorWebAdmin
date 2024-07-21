using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebApiGenerator
{
    internal class DiagnosticDefinitions
    {
        /// <summary>
        /// 接口需要标注<c>[WebControllerAttribute]</c>
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public static Diagnostic WAG00001(Location? location) => Diagnostic.Create(new DiagnosticDescriptor(
                        id: "WAG00001",
                        title: "接口未标注[WebControllerAttribute]",
                        messageFormat: "接口未标注[WebControllerAttribute]",
                        category: typeof(ControllerGenerator).FullName,
                        defaultSeverity: DiagnosticSeverity.Error,
                        isEnabledByDefault: true), location);
    }
}
